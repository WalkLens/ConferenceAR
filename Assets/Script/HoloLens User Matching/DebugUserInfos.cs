using System;
using System.Collections;
using System.Collections.Generic;
using CustomLogger;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUserInfos : MonoBehaviour
{
    public TextMeshProUGUI myUserInfoText;
    public TextMeshProUGUI userInfosText;

    public Transform[] buttonTransforms;
    public TextMeshProUGUI[] userButtonTexts;
    public Button[] userButtons;
    public static DebugUserInfos Instance;

    public MatchInfo matchInfo;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        SetDebugButtons();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            DebugAllUsersInfo();
    }

    private void SetDebugButtons()
    {
        // 버튼, 텍스트 할당
        userButtonTexts = new TextMeshProUGUI[buttonTransforms.Length];
        userButtons = new Button[buttonTransforms.Length];
        for (int i = 0; i < userButtonTexts.Length; i++)
        {
            TextMeshProUGUI buttonText = buttonTransforms[i].GetComponentInChildren<TextMeshProUGUI>();
            userButtonTexts[i] = buttonText;

            Button button = buttonTransforms[i].GetComponentInChildren<Button>();
            userButtons[i] = button;
        }
    }

    private void SetButtonTextsFromAllUsersInfo()
    {
        foreach (var userButton in userButtons)
        {
            if (userButton != null)
            {
                userButton.onClick.RemoveAllListeners();
            }
        }

        for (int i = 0; i < userButtonTexts.Length; i++)
        {
            userButtonTexts[i].text = "player_@";
        }


        List<UserInfo> userInfos = new List<UserInfo>();
        UserInfo myUserInfo = new UserInfo();
        foreach (var hostBehaviour in HostBehaviourManager.Instance.hostBehaviours)
        {
            if (hostBehaviour.TryGetComponent(out UserMatchingManager matchingManager))
            {
                userInfos = matchingManager.userInfos;
                myUserInfo = matchingManager.myUserInfo;
            }
        }

        for (int i = 0; i < userInfos.Count; i++)
        {
            int index = i; // 안전한 캡처를 위해 별도의 변수 사용
            userButtonTexts[i].text = userInfos[index].photonUserName;
            userButtons[i].onClick
                .AddListener(() => SendMatchRequestToAUser(userInfos[index].photonUserName, myUserInfo));
        }
    }

    public void SendMatchRequestToAUser(string targetUserName, UserInfo myUserInfo)
    {
        // player 이름에 해당하는 photon Actor Number 획득
        int targetActorNumber = UserMatchingManager.GetPlayerActorNumber(targetUserName);

        FileLogger.Log($"Send Message to {targetUserName}({targetActorNumber})", this);

        // TODO: 매칭 요청 보내는 작업 구현 

        // 유효성 검사: Actor Number 확인
        if (PhotonNetwork.CurrentRoom.GetPlayer(targetActorNumber) == null)
        {
            FileLogger.Log($"Invalid targetActorNumber: {targetActorNumber}", this);
            return;
        }

        // MatchInfo가 없다면 새로 생성
        // MatchInfo를 받았다(= 매칭 요청 받은 상태)면 그대로 전송
        if (matchInfo == null)
        {
            matchInfo = new MatchInfo
            {
                userWhoSend = myUserInfo.photonUserName,
                userWhoReceive = targetUserName,
                matchRequest = ""
            };
        }
        FileLogger.Log($"MatchInfo to send : User who send: {matchInfo.userWhoSend}, User who receive: {matchInfo.userWhoReceive}, matchRequest: {matchInfo.matchRequest}");

        // MatchInfo를 포함한 데이터 생성
        object[] data = new object[] { matchInfo };

        // RaiseEventOptions 설정
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber }
        };

        try
        {
            PhotonNetwork.RaiseEvent(4, data, options, SendOptions.SendReliable);
            FileLogger.Log($"Send Message to {targetUserName}({targetActorNumber})", this);
            
            // 전송 후에 match 정보 지움.
            matchInfo = null;
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Failed to send UserInfo: {ex.Message}", this);
        }
    }

    public void DebugMyUserInfo(UserInfo userInfo)
    {
        myUserInfoText.text = $"• Current Room Number: {userInfo.currentRoomNumber} \n" +
                              $"• Photon Role: {userInfo.photonRole} \n" +
                              $"• Photon UserName: {userInfo.photonUserName} \n" +
                              $"• Photon State: {userInfo.currentState} \n";
    }

    public void DebugAllUsersInfo()
    {
        if (HostBehaviourManager.Instance.IsCentralHost)
        {
            List<UserInfo> userInfos = new List<UserInfo>();
            foreach (var hostBehaviour in HostBehaviourManager.Instance.hostBehaviours)
            {
                if (hostBehaviour.TryGetComponent(out UserMatchingManager matchingManager))
                    userInfos = matchingManager.userInfos;
            }

            userInfosText.text = ""; // 텍스트 초기화
            foreach (var userInfo in userInfos)
            {
                userInfosText.text +=
                    $"Room: {userInfo.currentRoomNumber}, Role: {userInfo.photonRole}, UserName: {userInfo.photonUserName}, State: {userInfo.currentState}, \n";
            }
        }
        else
        {
            List<UserInfo> userInfos = new List<UserInfo>();
            foreach (var hostBehaviour in HostBehaviourManager.Instance.hostBehaviours)
            {
                if (hostBehaviour.TryGetComponent(out UserMatchingManager matchingManager))
                    userInfos = matchingManager.userInfos;
            }

            userInfosText.text = ""; // 텍스트 초기화
            foreach (var userInfo in userInfos)
            {
                userInfosText.text +=
                    $"Room: {userInfo.currentRoomNumber}, Role: {userInfo.photonRole}, UserName: {userInfo.photonUserName}, State: {userInfo.currentState}, \n";
            }
        }

        for (int i = 0; i < userButtonTexts.Length; i++)
        {
            userButtonTexts[i].text = "player_@";
        }

        SetButtonTextsFromAllUsersInfo();
    }

    void OnDestroy()
    {
        foreach (var userButton in userButtons)
        {
            if (userButton != null) userButton.onClick.RemoveAllListeners();
        }
    }
}