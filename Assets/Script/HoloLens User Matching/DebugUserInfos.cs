using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUserInfos : MonoBehaviour
{
    public TextMeshProUGUI myUserInfoText;
    public TextMeshProUGUI userInfosText;
    
    public static DebugUserInfos Instance;
    private void Awake()
    {
        if(Instance == null) Instance = this;
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
        List<UserInfo> userInfos = new List<UserInfo>();
        foreach (var hostBehaviour in HostBehaviourManager.Instance.hostBehaviours)
        {
            if (hostBehaviour.TryGetComponent(out UserMatchingManager matchingManager))
                userInfos = matchingManager.userInfos;
        }

        userInfosText.text = ""; // 텍스트 초기화
        foreach (var userInfo in userInfos)
        {
            userInfosText.text += $"Room: {userInfo.currentRoomNumber}, Role: {userInfo.photonRole}, UserName: {userInfo.photonUserName}, State: {userInfo.currentState}, \n";
        }
        
    }
}
