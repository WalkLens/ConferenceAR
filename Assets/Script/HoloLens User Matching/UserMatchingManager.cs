using CustomLogger;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
public class UserMatchingManager : HostOnlyBehaviour
{    
    private UserInfo myUserInfo;        // 나의 정보
    private List<UserInfo> userInfos;   // 모든 유저 정보 리스트

    public override void OnJoinedRoom(){
        // 사용자가 방에 접속할 때 myUserInfo 초기화
        FileLogger.Log("UserMatchingManager 사용자 접속, 정보 입력", this);
        
        myUserInfo = new UserInfo{
            currentRoomNumber = PhotonNetwork.CurrentRoom.Name,
            photonRole = FileLogger.GetRoleString(),
            photonUserName = PhotonNetwork.NickName,
            currentState = MatchingState.None
        };        
        Debug.Log(myUserInfo.currentRoomNumber);
        Debug.Log(myUserInfo.photonRole);
        Debug.Log(myUserInfo.photonUserName);
        Debug.Log(myUserInfo.currentState);
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