using System;
using CustomLogger;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class UserMatchingManager : HostOnlyBehaviour
{
    public static UserMatchingManager Instance;
    public UserInfo myUserInfo; // 나의 정보
    public List<UserInfo> userInfos = new List<UserInfo>(); // 모든 유저 정보 리스트
    public DebugUserInfos debugUserInfo;
    public const byte RenameEvent = 1; // 유저 이름 변경 이벤트 코드
    public const byte SendUserInfoEvent = 2; // 유저 정보 전송 이벤트 코드
    public const byte SendUsersInfoEvent = 3; // 모든 유저 정보 전송 이벤트 코드
    public const byte SendMatchInfoEvent = 4; // 매칭 요청 이벤트 코드

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += HandleEvent; // 이벤트 핸들러 등록
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= HandleEvent; // 이벤트 핸들러 해제
    }

    #region PhotonEventOVERRIDE

    public void HandleEvent(EventData photonEvent)
    {
        // 메서드 이름 변경
        FileLogger.Log($"photon event {photonEvent.Code} received", this);
        if (photonEvent.Code == SendUserInfoEvent)
        {
            try
            {
                // 데이터 무결성 확인
                object[] data = (object[])photonEvent.CustomData;
                if (data.Length < 1 || !(data[0] is UserInfo))
                {
                    FileLogger.Log("Invalid CustomData received", this);
                    return;
                }

                // 데이터 처리
                UserInfo receivedUserInfo = (UserInfo)data[0];
                userInfos.Add(receivedUserInfo);

                debugUserInfo.LogAllUsersInfo(ref userInfos);
                DebugUserInfos.Instance.DebugAllUsersInfo();

                // List<UserInfo> 동기화 
                BroadcastUserInfos();
                FileLogger.Log($"UserInfo received for {receivedUserInfo.photonUserName}", this);
            }
            catch (Exception ex)
            {
                FileLogger.Log($"Error handling photon event: {ex.Message}", this);
            }
        }
        else if (photonEvent.Code == SendUsersInfoEvent)
        {
            try
            {
                FileLogger.Log($"photon event {photonEvent.Code} received", this);

                // 수신된 데이터를 배열로 역직렬화
                UserInfo[] receivedUserInfoArray = (UserInfo[])photonEvent.CustomData;

                // 배열을 List로 변환
                var receivedUserInfos = new List<UserInfo>(receivedUserInfoArray);

                foreach (var receivedUserInfo in receivedUserInfos)
                {
                    // 기존 리스트에서 해당 유저 정보 찾기
                    var existingUserInfo =
                        userInfos.Find(user => user.photonUserName == receivedUserInfo.photonUserName);

                    if (existingUserInfo != null)
                    {
                        // 기존 유저 정보 업데이트
                        existingUserInfo.currentRoomNumber = receivedUserInfo.currentRoomNumber;
                        existingUserInfo.photonRole = receivedUserInfo.photonRole;
                        existingUserInfo.currentState = receivedUserInfo.currentState;
                    }
                    else
                    {
                        // 새로운 유저 정보 추가
                        userInfos.Add(receivedUserInfo);
                        FileLogger.Log($"Added new UserInfo: {receivedUserInfo.photonUserName}", this);
                    }
                }

                DebugUserInfos.Instance.DebugAllUsersInfo();
                FileLogger.Log($"UserInfo list updated successfully. Total users: {userInfos.Count}", this);
            }
            catch (Exception ex)
            {
                FileLogger.Log($"Failed to handle UserInfoList event: {ex.Message}", this);
            }
        }
        else if (photonEvent.Code == RenameEvent) // 닉네임 변경 이벤트
        {
            object[] data = (object[])photonEvent.CustomData;
            int actorNumber = (int)data[0];
            string newNickName = (string)data[1];

            Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (player != null)
            {
                player.NickName = newNickName; // 닉네임 업데이트
                DebugUserInfos.Instance.DebugMyUserInfo(myUserInfo);
                FileLogger.Log($"Player {actorNumber}의 닉네임이 {newNickName}(으)로 변경되었습니다.");
            }
        }
        else if (photonEvent.Code == 254) // 포톤 유저 한 명이 접속 종료 이벤트
        {
            SyncUserListWithPhotonPlayers();
        }
        else if (photonEvent.Code == SendMatchInfoEvent) // 매칭 요청, 응답 이벤트
        {
            try
            {
                // 데이터 무결성 확인
                if (photonEvent.CustomData == null)
                {
                    FileLogger.Log("CustomData is null", this);
                    return;
                }

                object[] data = photonEvent.CustomData as object[];
                if (data == null || data.Length < 1 || !(data[0] is MatchInfo))
                {
                    FileLogger.Log("Invalid CustomData received or missing MatchInfo", this);
                    return;
                }

                // 데이터 처리
                MatchInfo receivedMatchInfo = (MatchInfo)data[0];
                if (debugUserInfo == null)
                {
                    FileLogger.Log("debugUserInfo is null", this);
                    return;
                }

                debugUserInfo.receivedMatchInfo = receivedMatchInfo;
                debugUserInfo.DebugMatchText();
                
                if (debugUserInfo.receivedMatchInfo.matchRequest == "Request...")   // 매칭 요청을 받음
                {
                    debugUserInfo.ShowMatchRequestUI(); 
                }
                else if (debugUserInfo.receivedMatchInfo.matchRequest == "Yes")     // 매칭 응답(Yes)을 받음
                {
                    debugUserInfo.ShowMatchRequestYesUI();
                }
                else if (debugUserInfo.receivedMatchInfo.matchRequest == "No")      // 매칭 응답(No)을 받음
                {
                    debugUserInfo.ShowMatchRequestNoUI();
                }
            }
            catch (Exception ex)
            {
                FileLogger.Log($"Error handling photon event: {ex.Message}", this);
            }
        }
    }

    #endregion

    #region HostBehaviourOVERRIDE

    public override void HandleOnJoinedRoom()
    {
        // 사용자가 방에 접속할 때 myUserInfo 초기화
        FileLogger.Log("UserMatchingManager 사용자 접속, 정보 입력", this);

        myUserInfo = new UserInfo
        {
            currentRoomNumber = PhotonNetwork.CurrentRoom.Name,
            photonRole = FileLogger.GetRoleString(),
            photonUserName = PhotonNetwork.NickName,
            currentState = "None"
        };
        FileLogger.Log($"{myUserInfo.photonUserName}: {myUserInfo.photonRole}", this);
        FileLogger.Log("UserMatchingManager 사용자 접속 초기 정보 생성 완료", this);
        base.HandleOnJoinedRoom();
    }

    public override void OnBecameHost()
    {
        FileLogger.Log("UserMatchingManager 초기화 시작", this);

        // TODO: 초기화 작업 구현

        FileLogger.Log("UserMatchingManager 초기화 완료", this);

        base.OnBecameHost();
    }

    public override void OnStoppedBeingHost()
    {
        FileLogger.Log("UserMatchingManager 잉여 데이터 정리 시작", this);

        // TODO: 잉여 데이터 정리 작업 구현

        FileLogger.Log("UserMatchingManager 잉여 데이터 정리 완료", this);

        base.OnStoppedBeingHost();
    }

    #endregion

    #region MessageHandlers

    public void TrySendingUserInfo()
    {
        // 중앙 호스트에게 User Info 전송
        if (!FileLogger.GetRoleString().Equals("CentralHost"))
        {
            int centralHostActorNumber = PhotonUserUtility.GetCentralHostActorNumber();
            FileLogger.Log($"UserMatchingManager GetCentralHostActorNumber {centralHostActorNumber}", this);
            if (centralHostActorNumber != -1)
                SendUserInfo(myUserInfo, centralHostActorNumber);
        }
        else
        {
            FileLogger.Log($"UserMatchingManager GetCentralHostActorNumber {-1}", this);
        }
    }

    public void SendUserInfo(UserInfo userInfo, int targetActorNumber)
    {
        FileLogger.Log($"Send User Info to {targetActorNumber}", this);

        // 유효성 검사: ActorNumber 확인
        if (PhotonNetwork.CurrentRoom.GetPlayer(targetActorNumber) == null)
        {
            FileLogger.Log($"Invalid targetActorNumber: {targetActorNumber}", this);
            return;
        }

        // UserInfo를 포함한 데이터 생성
        object[] data = new object[] { userInfo };

        // RaiseEventOptions 설정
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber } // 중앙 호스트에게만 전송
        };

        try
        {
            PhotonNetwork.RaiseEvent(SendUserInfoEvent, data, options, SendOptions.SendReliable);
            FileLogger.Log($"Successfully sent UserInfo to {targetActorNumber}", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Failed to send UserInfo: {ex.Message}", this);
        }
    }

    public void UpdateNickNameAfterJoin(string newNickName)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.NickName = newNickName;
            myUserInfo.photonUserName = newNickName;

            // 닉네임 변경 브로드캐스트
            object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, newNickName };
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(1, content, options, SendOptions.SendReliable);
        }
        else
        {
            FileLogger.Log("닉네임을 변경하려면 방에 입장해야 합니다.", this);
        }
    }

    public void SyncUserListWithPhotonPlayers()
    {
        // Photon에 접속 중인 모든 유저 이름 가져오기
        HashSet<string> connectedPlayerNames = new HashSet<string>();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            connectedPlayerNames.Add(player.NickName);
        }

        // userInfos 리스트에서 Photon에 존재하지 않는 유저 제거
        userInfos.RemoveAll(user => !connectedPlayerNames.Contains(user.photonUserName));

        FileLogger.Log($"UserInfos synced. Remaining users: {userInfos.Count}", this);
    }

    public void BroadcastUserInfos()
    {
        FileLogger.Log("Broadcasting user info list to all clients", this);

        // List<UserInfo>를 배열로 변환
        var userInfoArray = userInfos.ToArray();

        try
        {
            PhotonNetwork.RaiseEvent(
                SendUsersInfoEvent,
                userInfoArray, // 배열로 전송
                new RaiseEventOptions { Receivers = ReceiverGroup.All }, // 모든 클라이언트에게 전송
                SendOptions.SendReliable
            );
            FileLogger.Log("Successfully sent user info list to all clients", this);
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Failed to send user info list: {ex.Message}", this);
        }
    }

    #endregion
}