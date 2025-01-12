using System;
using CustomLogger;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UIElements;

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

    //============ SM ADD ============//
    public bool _isMatchingSucceed = false;
    public bool isUserMet = false;
    public bool isUserRibbonSelected = false;
    public GameObject myGameObject;
    public GameObject partnerGameObject;
    //============ SM ADD ============//

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
        //FileLogger.Log($"photon event {photonEvent.Code} received", this);
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
                userInfos.Add(receivedUserInfo); // 연결된 유저 데이터가 리스트에 추가됨

                // 모든 유저 정보 시각화
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
                //Debug.Log("AAA");
                // 데이터 무결성 확인
                if (photonEvent.CustomData == null)
                {
                    FileLogger.Log("CustomData is null", this);
                    return;
                }
                //Debug.Log("BBB");

                object[] data = photonEvent.CustomData as object[];
                if (data == null || data.Length < 1 || !(data[0] is MatchInfo))
                {
                    FileLogger.Log("Invalid CustomData received or missing MatchInfo", this);
                    return;
                }
                //Debug.Log("CCC");

                // 데이터 처리
                MatchInfo receivedMatchInfo = (MatchInfo)data[0];

                if (debugUserInfo == null)
                {
                    FileLogger.Log("debugUserInfo is null", this);
                    return;
                }
                //Debug.Log("DDD");

                debugUserInfo.receivedMatchInfo = receivedMatchInfo;
                //debugUserInfo.DebugMatchText();

                // 요청을 받았을 때, 받은 곳에서 작동
                //Debug.Log($"요청 받음! : {debugUserInfo.receivedMatchInfo.matchRequest}");
                
                // 매칭을 보냈을 때 - Send
                if (debugUserInfo.receivedMatchInfo.matchRequest == "Request...")   // 매칭 요청을 받음
                {
                    //Debug.Log("난 분명 Request... 를 받았다");
                    if (!isMatchingSucceed)      // 매칭이 이루어지지 않았다면
                    {
                        // Request... 라는 matchRequest를 받았을 때, 나오는 UI에서의 버튼에 
                        debugUserInfo.SetMatchButtonStatus(true);
                        debugUserInfo.ShowMatchRequestUI();
                    }
                    else                        // 매칭이 되어있었다면 
                    {
                        // !! 현재는 반복되는 Request 요청 수신이 안 되고 있음..
                        debugUserInfo.ShowNewMatchRequestUI();
                    }
                    
                    //Debug.Log($"matchInfo.whoSend = {receivedMatchInfo.userWhoSend}"); -> Player2라고 제대로 나오고 있음. 근데 답장으로는 여기로 안 가고 있음..
                }
                // 매칭을 받았을 때 - Receive
                else if (debugUserInfo.receivedMatchInfo.matchRequest == "Accept")     // 매칭 응답(Yes)을 받음
                {
                    debugUserInfo.ShowReceiveAcceptUI();
                    MatchingStateUpdateAsTrue();
                    //Debug.Log("난 분명 Accept 응답을 받았다");
                    //notificationManager.SendAcceptMessage();
                    //Debug.Log("보낸 요청에 대해 Accept 응답을 받음!");
                }
                else if (debugUserInfo.receivedMatchInfo.matchRequest == "Decline")      // 매칭 응답(No)을 받음
                {
                    debugUserInfo.ShowReceiveDeclineUI();
                    //Debug.Log("난 분명 Decline 응답을 받았다");
                    //notificationManager.SendDeclineMessage();
                    //Debug.Log("보낸 요청에 대해 Decline 응답을 받음!");
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


    //============ SM ADD ============//
    public void MatchingStateUpdateAsTrue()
    {
        isMatchingSucceed = true;
    }

    // 프로퍼티로 추가
    public bool isMatchingSucceed
    {
        get => _isMatchingSucceed;
        set
        {
            if (_isMatchingSucceed != value) // 값이 변경되었는지 확인
            {
                _isMatchingSucceed = value;

                // 값이 true로 바뀌었을 때만 UI를 띄우는 로직 실행
                if (_isMatchingSucceed)
                {
                    //--------------------------
                    // 여기 정확하게 파악 필요
                    //--------------------------
                    Debug.Log($"debugUserInfo.receivedMatchInfo.userWhoSend: {debugUserInfo.receivedMatchInfo.userWhoSend}");
                    Debug.Log($"debugUserInfo.receivedMatchInfo.userWhoReceive: {debugUserInfo.receivedMatchInfo.userWhoReceive}");
                    //Debug.Log($"이건가 : User{debugUserInfo.selectedUserIdx}");

                    myGameObject = GameObject.Find($"User{debugUserInfo.receivedMatchInfo.userWhoReceive}");
                    partnerGameObject = GameObject.Find($"User{debugUserInfo.receivedMatchInfo.userWhoSend[debugUserInfo.receivedMatchInfo.userWhoSend.Length - 1]}");

                    ShowMeetingUI();
                }
            }
        }
    }

    private void ShowMeetingUI()
    {
        Debug.Log("매칭 이후 UI 뜰 화면1");
        Vector3 temp = myGameObject.transform.position - partnerGameObject.transform.position;
        Debug.Log(temp);
        Debug.Log("매칭 이후 UI 뜰 화면2");
    }
    //============ SM ADD ============//

    #endregion
}