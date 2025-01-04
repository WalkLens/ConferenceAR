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
    [SerializeField] GameObject newMatchingUI;
    [SerializeField] GameObject profileUI;
    [SerializeField] GameObject timeSetUI;
    [SerializeField] GameObject afMatchingUI;
    [SerializeField] GameObject acceptPopupUI;
    [SerializeField] GameObject declinePopupUI;
    [SerializeField] GameObject matchingFailPopupUI;

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
        UserMatchingManagerSM.Instance.isUserMatchingFailed = true;
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

    public void OpenMatchingFailUI()
    {
        matchingFailPopupUI.SetActive(true);
    }

    public void CloseMatchingFailUI()
    {
        matchingFailPopupUI.SetActive(false);
    }

    public void OpenNewMatchingUI()
    {
        newMatchingUI.SetActive(true);
    }

    public void CloseNewMatchingUI()
    {
        newMatchingUI.SetActive(false);
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

    public void OpenAcceptPopupUI()
    {
        acceptPopupUI.SetActive(true);
    }

    public void CloseAcceptPopupUI()
    {
        acceptPopupUI.SetActive(false);
    }

    public void OpenDeclinePopupUI()
    {
        declinePopupUI.SetActive(true);
    }

    public void CloseDeclinePopupUI()
    {
        declinePopupUI.SetActive(false);
    }
    //////////////////////////////////////////////


    //////////////// TIME BUTTON Callbacks ///////////////
    public void MeetTimePlus()
    {
        time += 1;
        Debug.Log(time);
    }

    public void MeetTimeMinus()
    {
        if (time >= 1)
        {
            time -= 1;
        }
        Debug.Log(time);
    }

    public void MeetTimeZero()
    {
        time = 0;
        Debug.Log(time);
    }

    public void MeetTimeUpdate()
    {

        Debug.Log($"time is {time}");

        //if (time == 0)
        //{
        //    // !!!!바로 만남
        //    SendAcceptMessage();
        //}
        //else
        //{
        //    Debug.Log("dasdsdadasdadsd");
        //    // !!!time 값을 DB에 업데이트
        //}
    }

    public float GetTime()
    {
        return (float)time * 60;
    }
    //////////////////////////////////////////////
}
