using ExitGames.Client.Photon;
using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PressableButtons
{
    public PressableButton acceptButton;
    public PressableButton declineButton;
    public PressableButton holdButton;
}

public class NotificationManager : MonoBehaviour
{
    
    public PressableButtons buttons;
    [SerializeField] GameObject matchingUI;
    [SerializeField] GameObject profileUI;
    [SerializeField] GameObject timeSetUI;
    [SerializeField] GameObject afMatchingUI;

    private string processingUserId = "";
    private int time = 0;

    public void ShowMatchingNotification(string fromUserId)
    {
        OpenMatchingUI();
        // !!!! 프로필 UI에 데이터 띄우는 부분 구현 필요
    }

    public void OnMatchRequestReceived(string fromUserId)   // if someone choose me, it works
    {
        processingUserId = fromUserId;
        ShowMatchingNotification(processingUserId);
    }

    public void ShowFileNotification(string fromUserId)
    {
        afMatchingUI.SetActive(true);
    }

    public void OnFileReceived(string fromUserId)   // if someone choose me, it works
    {
        ShowFileNotification(processingUserId);
    }


    public void OnEvent(EventData photonEvent)
    {

    }

    public void AddListenerToPanel()
    {

    }

    public void SendAcceptMessage()
    {
        Debug.Log($"Accept User: {processingUserId}");
        UserMatchingManagerSM.Instance.isUserMatchingSucceed = true;
    }

    public void SendDeclineMessage()
    {
        Debug.Log($"Decline User: {processingUserId}");
    }

    public void SendHoldMessage()
    {
        Debug.Log($"Hold User: {processingUserId}");
    }

    /////////////// ON/OFF BUTTON Callbacks ///////////////
    public void OpenMatchingUI()
    {
        matchingUI.SetActive(true);
    }

    public void CloseMatchingUI()
    {
        matchingUI.SetActive(false);
    }

    public void OpenProfileUI()
    {
        profileUI.SetActive(true);
        Debug.Log($"Open Profile about {processingUserId}");

        // !!! 데이터베이스 데이터 가져오는 함수 작동할 것
        //afMatchingUI.SetActive(false);
    }

    public void CloseProfileUI()
    {
        profileUI.SetActive(false);
        Debug.Log($"Close Profile about {processingUserId}");
    }

    public void OpenTimeSetUI()
    {
        timeSetUI.SetActive(true);
    }

    public void CloseTimeSetUI()
    {
        timeSetUI.SetActive(false);
    }

    public void OpenAfMatchingUI()
    {
        afMatchingUI.SetActive(true);
    }

    public void CloseAfMatchingUI()
    {
        afMatchingUI.SetActive(false);
    }
    //////////////////////////////////////////////


    //////////////// TIME BUTTON Callbacks ///////////////
    public void MeetTimePlus()
    {
        time += 10;
        Debug.Log(time);
    }

    public void MeetTimeMinus()
    {
        if (time >= 10)
        {
            time -= 10;
        }
        Debug.Log(time);
    }

    public void MeetTimeZero()
    {
        time = 0;
    }

    public void MeetTimeUpdate()
    {
        if (time > 0)
        {
            // !!!! DB 업데이트
        }
        else
        {
            SendAcceptMessage();
        }
    }
    //////////////////////////////////////////////
}
