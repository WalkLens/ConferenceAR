using System;
using System.Collections;
using System.Collections.Generic;
using CustomLogger;
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
        foreach (var hostBehaviour in HostBehaviourManager.Instance.hostBehaviours)
        {
            if (hostBehaviour.TryGetComponent(out UserMatchingManager matchingManager))
                userInfos = matchingManager.userInfos;
        }

        for (int i = 0; i < userInfos.Count; i++)
        {
            int index = i; // 안전한 캡처를 위해 별도의 변수 사용
            userButtonTexts[i].text = userInfos[index].photonUserName;
            userButtons[i].onClick.AddListener(() => SendMessageToAUser(userInfos[index].photonUserName));
        }
    }

    public void SendMessageToAUser(string userName)
    {
        // player 이름에 해당하는 photon Actor Number 획득
        int actorNumber = UserMatchingManager.GetPlayerActorNumber(userName);
        
        FileLogger.Log($"Send Message to {userName}({actorNumber})", this);
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

        SetButtonTextsFromAllUsersInfo();
    }

    void OnDestroy()
    {
        foreach (var userButton in userButtons)
        {
            if(userButton != null) userButton.onClick.RemoveAllListeners();
        }
    }
}