using System;
using System.Collections;
using System.Collections.Generic;
using CustomLogger;
using ExitGames.Client.Photon;
using MixedReality.Toolkit.UX;
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
    public GameObject newMatchGameObject;
    public PressableButton[] matchButtons;          //============ SM MODI ============//
    public TextMeshProUGUI receivedMatchInfoText;
    public TextMeshProUGUI matchInfoText;

    public const byte SendMatchInfoEvent = 4; // 매칭 요청 이벤트 코드

    //============ SM ADD ============//
    public NotificationManager notificationManager;
    public int selectedUserIdx;
    //============ SM ADD ============//

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
        if (Input.GetKeyDown(KeyCode.Q))
            DebugMyMatchInfo();
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
                    //==== SM MODI ====//
                    // 보내려는 사용자를 클릭할 시, matchRequest에 Request... 이라는 데이터를 채움
                    matchInfo.matchRequest = "Request...";
                    // 버튼을 누르면 바로 데이터 보내는 것이 아닌, 프로필 확인으로 변경
                    //SendMatchRequestToAUser(UserMatchingManager.Instance.userInfos[index].photonUserName,
                    //    UserMatchingManager.Instance.myUserInfo);
                    // 대신 이 작동을 새로운 버튼을 눌렀을 때, NotificationManager.cs의 SendRequestMessage()에 넣음
                    
                    selectedUserIdx = index;
                    //Debug.Log(selectedUserIdx);
                    Debug.Log($"선택한 유저의 이름: {UserMatchingManager.Instance.userInfos[selectedUserIdx].photonUserName}");
                    //==== SM MODI ====//

                    //==== SM ADD ====//
                    notificationManager.OpenMatchingSendUI();
                    // TODO : 나타나는 UI에 DB 연결되어 데이터 작성돼야 함
                    //==== SM ADD ====//
                });
        }
    }

    // 상대방에게 매칭을 요청할 때, 요청을 보내는 곳에서의 버튼에서 작동하는 함수
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
            matchButtons[0].OnClicked.AddListener(() =>
            {
                if (matchInfo == null)
                {
                    matchInfo = new MatchInfo
                    {
                        userWhoSend = "",
                        userWhoReceive = "",
                        matchRequest = ""
                    };
                }

                // !! 이후에 요청에 대한 응답을 받는 부분이 이 결과를 못 받고 있음... !!
                matchInfo.matchRequest = "Accept";
                //Debug.Log("버튼에 Accept 기능 더해짐");

                // 실제 메서드 실행
                SendMatchRequestToAUser(receivedMatchInfo.userWhoSend, UserMatchingManager.Instance.myUserInfo);
                //SendMatchRequestToAUser(UserMatchingManager.Instance.userInfos[selectedUserIdx].photonUserName,
                //        UserMatchingManager.Instance.myUserInfo);
                //Debug.Log($"Accept 신호를 {receivedMatchInfo.userWhoSend}에게  보냄");

                matchButtonGameObject.SetActive(false);
            });
            matchButtons[1].OnClicked.AddListener(() =>
            {
                if (matchInfo == null)
                {
                    matchInfo = new MatchInfo
                    {
                        userWhoSend = "",
                        userWhoReceive = "",
                        matchRequest = ""
                    };
                }

                matchInfo.matchRequest = "Decline";
                //Debug.Log("버튼에 Decline 기능 더해짐");

                // 실제 메서드 실행
                SendMatchRequestToAUser(receivedMatchInfo.userWhoSend, UserMatchingManager.Instance.myUserInfo);
                //Debug.Log($"Decline 신호를 {receivedMatchInfo.userWhoSend}에게  보냄");

                matchButtonGameObject.SetActive(false);
            });
            matchButtons[2].OnClicked.AddListener(() =>
            {
                if (matchInfo == null)
                {
                    matchInfo = new MatchInfo
                    {
                        userWhoSend = "",
                        userWhoReceive = "",
                        matchRequest = ""
                    };
                }

                matchInfo.matchRequest = "Decline";
                Debug.Log("버튼에 Decline 기능 더해짐");

                // 실제 메서드 실행
                SendMatchRequestToAUser(receivedMatchInfo.userWhoSend, UserMatchingManager.Instance.myUserInfo);
                //Debug.Log($"Decline 신호를 {receivedMatchInfo.userWhoSend}에게  보냄");

                matchButtonGameObject.SetActive(false);
            });
            matchButtons[3].OnClicked.AddListener(() =>
            {
                if (matchInfo == null)
                {
                    matchInfo = new MatchInfo
                    {
                        userWhoSend = "",
                        userWhoReceive = "",
                        matchRequest = ""
                    };
                }

                matchInfo.matchRequest = "Decline";
                Debug.Log("버튼에 Decline 기능 더해짐");

                // 실제 메서드 실행
                SendMatchRequestToAUser(receivedMatchInfo.userWhoSend, UserMatchingManager.Instance.myUserInfo);
                //Debug.Log($"Decline 신호를 {receivedMatchInfo.userWhoSend}에게  보냄");

                matchButtonGameObject.SetActive(false);
            });

        }
        else
        {
            // false이면 Match 응답 버튼과 관련된 부분에 기능 지우기
            // 버튼 기능 지우기
            for (int i = 0; i < matchButtons.Length; i++)
            {
                matchButtons[i].OnClicked.RemoveAllListeners();
            }

            matchButtonGameObject.SetActive(false);
        }
    }

    // 다른 사람에게 요청을 보낼 시 작동
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
            // 실제 Photon에서 데이터를 보내는 부분
            PhotonNetwork.RaiseEvent(SendMatchInfoEvent, data, options, SendOptions.SendReliable);
            FileLogger.Log($"Send Message to {targetUserName}({targetActorNumber})", this);

            // 전송 후에 match 정보 지움.
            //matchInfo = null;
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
        //Debug.Log("XYZ1");
        receivedMatchInfoText.text = $"• userWhoSend: {receivedMatchInfo.userWhoSend} \n" +
                                     $"• userWhoReceived: {receivedMatchInfo.userWhoReceive} \n" +
                                     $"• matchRequest: {receivedMatchInfo.matchRequest} ";
        //Debug.Log("XYZ2");
        matchInfoText.text = $"• userWhoSend: {matchInfo.userWhoSend} \n" +
                             $"• userWhoReceived: {matchInfo.userWhoReceive} \n" +
                             $"• matchRequest: {matchInfo.matchRequest} ";
        //Debug.Log("XYZ3");
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

    public void DebugMyMatchInfo()
    {
        if(receivedMatchInfo.userWhoSend!=null)
            Debug.Log($"• receivedMatchInfo.userWhoSend: {receivedMatchInfo.userWhoSend}");
        if (receivedMatchInfo.userWhoReceive != null)
            Debug.Log($"• receivedMatchInfo.userWhoReceive: {receivedMatchInfo.userWhoReceive}");
        if (receivedMatchInfo.matchRequest != null)
            Debug.Log($"• receivedMatchInfo.matchRequest: {receivedMatchInfo.matchRequest}");
        
        // 받는 쪽 기준, matchInfo 부터는 안 뜸
        if(matchInfo == null)
            matchInfo = new MatchInfo();
        if (matchInfo.userWhoSend != null)
            Debug.Log($"• matchInfo.userWhoSend: {matchInfo.userWhoSend}");
        if (matchInfo.userWhoReceive != null)
            Debug.Log($"• matchInfo.userWhoReceive: {matchInfo.userWhoReceive}");
        if (matchInfo.matchRequest != null)
            Debug.Log($"• matchInfo.matchRequest: {matchInfo.matchRequest} ");
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
        matchButtonGameObject.SetActive(true);        
    }
    public void ShowNewMatchRequestUI()
    {
        newMatchGameObject.SetActive(true);
    }
    public void ShowReceiveAcceptUI()
    {
        notificationManager.OpenReceiveAcceptPopupUI();
    }
    public void ShowReceiveDeclineUI()
    {
        notificationManager.OpenReceiveDeclinePopupUI();
    }
    public void ShowRouteUI(Vector3 direction)
    {
        notificationManager.OpenRouteVisualizationUI(direction);
    }
    public void UpdateRouteUI(Vector3 direction, float myRotY)
    {
        notificationManager.UpdateRouteVisualizationUI(direction, myRotY);
    }
    public void HideRouteUI()
    {
        notificationManager.CloseRouteVisualizationUI();
    }

    void OnDestroy()
    {
        foreach (var userButton in userButtons)
        {
            if (userButton != null) userButton.onClick.RemoveAllListeners();
        }
    }
}