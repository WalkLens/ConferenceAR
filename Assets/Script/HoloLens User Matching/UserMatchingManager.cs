using CustomLogger;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class UserMatchingManager : HostOnlyBehaviour
{    
    public UserInfo myUserInfo;        // 나의 정보
    private List<UserInfo> userInfos = new List<UserInfo>();   // 모든 유저 정보 리스트

    public const byte SendUserInfoEvent = 2; // 유저 정보 전송 이벤트 코드

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += HandleUserInfoEvent; // 이벤트 핸들러 등록
    }

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= HandleUserInfoEvent; // 이벤트 핸들러 해제
    }

    public void HandleUserInfoEvent(EventData photonEvent) { // 메서드 이름 변경
        if (photonEvent.Code == SendUserInfoEvent) {
            object[] data = (object[])photonEvent.CustomData;
            UserInfo receivedUserInfo = (UserInfo)data[0];

            // 수신한 UserInfo를 리스트에 추가
            userInfos.Add(receivedUserInfo);
            Debug.Log($"UserInfo received for {receivedUserInfo.photonUserName}");
        }
    }

    public override void OnJoinedRoom(){
        // 사용자가 방에 접속할 때 myUserInfo 초기화
        FileLogger.Log("UserMatchingManager 사용자 접속, 정보 입력", this);
        
        myUserInfo = new UserInfo{
            currentRoomNumber = PhotonNetwork.CurrentRoom.Name,
            photonRole = FileLogger.GetRoleString(),
            photonUserName = PhotonNetwork.NickName,
            currentState = MatchingState.None
        };        
        userInfos.Add(myUserInfo);        

        // 중앙 호스트에게 User Info 전송
        if(!FileLogger.GetRoleString().Equals("CentralHost")){
            int centralHostActorNumber = GetCentralHostActorNumber();
            FileLogger.Log($"UserMatchingManager GetCentralHostActorNumber {centralHostActorNumber}", this);
            if(centralHostActorNumber != -1)
                SendUserInfo(myUserInfo, centralHostActorNumber);
        }
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
    public void SendUserInfo(UserInfo userInfo, int targetActorNumber) {
        // UserInfo를 포함한 데이터 생성
        object[] data = new object[] { userInfo };

        // RaiseEventOptions 설정
        RaiseEventOptions options = new RaiseEventOptions {
            TargetActors = new int[] { targetActorNumber } // 중앙 호스트에게만 전송
        };

        PhotonNetwork.RaiseEvent(SendUserInfoEvent, data, options, SendOptions.SendReliable);
    }
}

public class UserInfo{
    public string currentRoomNumber {get;set;}
    public string photonRole {get;set;}
    public string photonUserName {get;set;}
    public MatchingState currentState {get;set;}
}

public enum MatchingState{
    None,
    Waiting,
    InMathcing,
    Finished
}