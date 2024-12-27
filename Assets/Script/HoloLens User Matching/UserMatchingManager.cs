using System;
using CustomLogger;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class UserMatchingManager : HostOnlyBehaviour
{    
    public UserInfo myUserInfo;        // 나의 정보
    public List<UserInfo> userInfos = new List<UserInfo>();   // 모든 유저 정보 리스트

    public const byte SendUserInfoEvent = 2; // 유저 정보 전송 이벤트 코드

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += HandleEvent; // 이벤트 핸들러 등록
    }

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= HandleEvent; // 이벤트 핸들러 해제
    }

    public void HandleEvent(EventData photonEvent) { // 메서드 이름 변경
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
                HostBehaviourManager.Instance.LogAllUsersInfo(ref userInfos);
                FileLogger.Log($"UserInfo received for {receivedUserInfo.photonUserName}", this);
            }
            catch (Exception ex)
            {
                FileLogger.Log($"Error handling photon event: {ex.Message}", this);
            }
        }
        else if (photonEvent.Code == 1) // 닉네임 변경 이벤트
        {
            object[] data = (object[])photonEvent.CustomData;
            int actorNumber = (int)data[0];
            string newNickName = (string)data[1];

            Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (player != null)
            {
                player.NickName = newNickName; // 닉네임 업데이트
                FileLogger.Log($"Player {actorNumber}의 닉네임이 {newNickName}(으)로 변경되었습니다.");
            }
        }
    }
    
    public override void HandleOnJoinedRoom()
    {
        // 사용자가 방에 접속할 때 myUserInfo 초기화
        FileLogger.Log("UserMatchingManager 사용자 접속, 정보 입력", this);
        
        myUserInfo = new UserInfo{
            currentRoomNumber = PhotonNetwork.CurrentRoom.Name,
            photonRole = FileLogger.GetRoleString(),
            photonUserName = PhotonNetwork.NickName,
            currentState = "None"
        };        
        userInfos.Add(myUserInfo);        
        FileLogger.Log($"{myUserInfo.photonUserName}: {myUserInfo.photonRole}", this);
        
        FileLogger.Log("UserMatchingManager 사용자 접속 처리 완료, 정보 입력", this);
        base.HandleOnJoinedRoom();
    }
    
    public override void OnBecameHost(){
        FileLogger.Log("UserMatchingManager 초기화 시작", this);

        // TODO: 초기화 작업 구현

        FileLogger.Log("UserMatchingManager 초기화 완료", this);

        base.OnBecameHost();
    }

    public override void OnStoppedBeingHost(){
        FileLogger.Log("UserMatchingManager 잉여 데이터 정리 시작", this);

        // TODO: 잉여 데이터 정리 작업 구현

        FileLogger.Log("UserMatchingManager 잉여 데이터 정리 완료", this);

        base.OnStoppedBeingHost();
    }

    private void LogCurrentPlayersInfo()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            FileLogger.Log($"{myUserInfo.photonUserName}: {myUserInfo.photonRole}", this);
        } 
    }
    private int GetCentralHostActorNumber() {
    // 모든 플레이어를 순회하여 중앙 호스트의 ActorNumber를 반환
        foreach (var player in PhotonNetwork.PlayerList) {
            FileLogger.Log($"UserMatchingManager {player.NickName}", this);
            if (player.IsMasterClient && player.NickName == "CentralHost") {
                return player.ActorNumber; // 중앙 호스트의 ActorNumber 반환
            }
        }
    return -1; // 중앙 호스트가 아닐 경우
    }

    public void TrySendingUserInfo()
    {
        // 중앙 호스트에게 User Info 전송
        if(!FileLogger.GetRoleString().Equals("CentralHost")){
            int centralHostActorNumber = GetCentralHostActorNumber();
            FileLogger.Log($"UserMatchingManager GetCentralHostActorNumber {centralHostActorNumber}", this);
            if(centralHostActorNumber != -1)
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
    public void SyncNickName(string nickName)
    {
        object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, nickName };
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(1, content, options, SendOptions.SendReliable);
    }
}