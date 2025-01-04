using System;
using System.Collections;
using System.Collections.Generic;
using CustomLogger;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugUserInfos : MonoBehaviour
{
    public TextMeshProUGUI myUserInfoText;
    public TextMeshProUGUI userInfosText;

    public Transform[] buttonTransforms;
    public TextMeshProUGUI[] userButtonTexts;
    public Button[] userButtons;
    public static DebugUserInfos Instance;

    public MatchInfo matchInfo; // 보낼 Match Info
    public MatchInfo receivedMatchInfo; // 받을 Match Info
    public GameObject matchButtonGameObject;
    public Button[] matchButtons;
    public TextMeshProUGUI receivedMatchInfoText;
    public TextMeshProUGUI matchInfoText;

    public const byte SendMatchInfoEvent = 4; // 매칭 요청 이벤트 코드

    private void Awake()
    {
        if (Instance == null) Instance = this;
        CacheDebugUserButtons();
        // Debug.Log(Application.persistentDataPath);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            DebugAllUsersInfo();
    }

    #region ButtonSetup

    // 각 버튼에 필요한 기능을 할당한다.
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

        for (int i = 0; i < UserMatchingManager.Instance.userInfos.Count; i++)
        {
            int index = i; // 안전한 캡처를 위해 별도의 변수 사용
            userButtonTexts[i].text = UserMatchingManager.Instance.userInfos[index].photonUserName;
            userButtons[i].onClick
                .AddListener(() =>
                {
                    matchInfo.matchRequest = "Request...";
                    SendMatchRequestToAUser(UserMatchingManager.Instance.userInfos[index].photonUserName,
                        UserMatchingManager.Instance.myUserInfo);
                });
        }
    }

    public void SetMatchButtonStatus(bool status)
    {
        if (status)
        {
            // true이면 Match 응답 버튼과 관련된 부분에 기능 할당
            // 보낼 Match Info 생성 
            matchInfo = new MatchInfo
            {
                userWhoSend = UserMatchingManager.Instance.myUserInfo.photonUserName,
                userWhoReceive = "",
                matchRequest = ""
            };
            DebugMatchText();


            // 버튼에 기능 추가
            matchButtons[0].onClick.AddListener(() =>
            {
                matchInfo.matchRequest = "Yes";

                // 실제 메서드 실행
                SendMatchRequestToAUser(receivedMatchInfo.userWhoSend, UserMatchingManager.Instance.myUserInfo);

                matchButtonGameObject.SetActive(false);
            });
            matchButtons[1].onClick.AddListener(() =>
            {
                matchInfo.matchRequest = "No";

                // 실제 메서드 실행
                SendMatchRequestToAUser(receivedMatchInfo.userWhoSend, UserMatchingManager.Instance.myUserInfo);

                matchButtonGameObject.SetActive(false);
            });
        }
        else
        {
            // false이면 Match 응답 버튼과 관련된 부분에 기능 지우기
            // 버튼 기능 지우기
            for (int i = 0; i < matchButtons.Length; i++)
            {
                matchButtons[i].onClick.RemoveAllListeners();
            }

            matchButtonGameObject.SetActive(false);
        }
    }

    public void SendMatchRequestToAUser(string targetUserName, UserInfo myUserInfo)
    {
        // player 이름에 해당하는 photon Actor Number 획득
        int targetActorNumber = PhotonUserUtility.GetPlayerActorNumber(targetUserName);

        FileLogger.Log($"Send Message to {targetUserName}({targetActorNumber})", this);

        // TODO: 매칭 요청 보내는 작업 구현 

        // 유효성 검사: Actor Number 확인
        if (PhotonNetwork.CurrentRoom.GetPlayer(targetActorNumber) == null)
        {
            FileLogger.Log($"Invalid targetActorNumber: {targetActorNumber}", this);
            return;
        }

        matchInfo.userWhoSend = myUserInfo.photonUserName;
        matchInfo.userWhoReceive = targetUserName;
        DebugMatchText();

        FileLogger.Log($"MatchInfo to send : User who send: {matchInfo.userWhoSend}, " +
                       $"User who receive: {matchInfo.userWhoReceive}, matchRequest: {matchInfo.matchRequest}");

        // MatchInfo 포함한 데이터 생성
        object[] data = new object[] { matchInfo };

        // RaiseEventOptions 설정
        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { targetActorNumber }
        };

        try
        {
            PhotonNetwork.RaiseEvent(SendMatchInfoEvent, data, options, SendOptions.SendReliable);
            FileLogger.Log($"Send Message to {targetUserName}({targetActorNumber})", this);

            // 전송 후에 match 정보 지움.
            matchInfo = null;
        }
        catch (Exception ex)
        {
            FileLogger.Log($"Failed to send UserInfo: {ex.Message}", this);
        }
    }

    #endregion

    #region DEBUG

    private void CacheDebugUserButtons()
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

    public void DebugMatchText()
    {
        receivedMatchInfoText.text = $"• userWhoSend: {receivedMatchInfo.userWhoSend} \n" +
                                     $"• userWhoReceived: {receivedMatchInfo.userWhoReceive} \n" +
                                     $"• matchRequest: {receivedMatchInfo.matchRequest} ";

        matchInfoText.text = $"• userWhoSend: {matchInfo.userWhoSend} \n" +
                             $"• userWhoReceived: {matchInfo.userWhoReceive} \n" +
                             $"• matchRequest: {matchInfo.matchRequest} ";
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
            userInfosText.text = ""; // 텍스트 초기화
            foreach (var userInfo in UserMatchingManager.Instance.userInfos)
            {
                userInfosText.text +=
                    $"Room: {userInfo.currentRoomNumber}, Role: {userInfo.photonRole}, UserName: {userInfo.photonUserName}, State: {userInfo.currentState}, \n";
            }
        }
        else
        {
            userInfosText.text = ""; // 텍스트 초기화
            foreach (var userInfo in UserMatchingManager.Instance.userInfos)
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
    
    public void LogAllUsersInfo(ref List<UserInfo> allUsersInfo)
    {
        foreach (UserInfo userInfo in allUsersInfo)
        {
            FileLogger.Log(
                $"{userInfo.currentRoomNumber} || {userInfo.photonRole} || {userInfo.photonUserName} || {userInfo.currentState}");
        }
    }

    #endregion

    public void ShowMatchRequestUI()
    {
        // TODO: 매칭 요청을 받고 그에 해당하는 UI를 띄운다.
        matchButtonGameObject.SetActive(true);        

    }
    public void ShowMatchRequestYesUI()
    {
        // TODO: 매칭 응답(Yes)을 받고 그에 해당하는 UI를 띄운다.

    }
    public void ShowMatchRequestNoUI()
    {
        // TODO: 매칭 응답(No)을 받고 그에 해당하는 UI를 띄운다.

    }
    void OnDestroy()
    {
        foreach (var userButton in userButtons)
        {
            if (userButton != null) userButton.onClick.RemoveAllListeners();
        }
    }
}