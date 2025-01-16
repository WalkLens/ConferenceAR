using ExitGames.Client.Photon;
using MixedReality.Toolkit.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//[System.Serializable]
//public class PressableButtons
//{
//    public PressableButton acceptButton;
//    public PressableButton declineButton;
//    public PressableButton holdButton;
//}

public class NotificationManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] DebugUserInfos userInfos;

    //public PressableButtons buttons;
    [Header("Matching UI")]
    [SerializeField] GameObject matchingSendUI;
    [SerializeField] GameObject matchingReceiveUI;
    [SerializeField] GameObject newMatchingUI;
    [SerializeField] GameObject profileUI;
    [SerializeField] GameObject timeSetUI;
    [SerializeField] GameObject routeVisualizationUI;
    [SerializeField] TextMeshProUGUI uiText;
    [SerializeField] RectTransform arrowRot;

    [Header("Pop UP UI")]
    [SerializeField] GameObject receiveAcceptPopupUI;
    [SerializeField] GameObject receiveDeclinePopupUI;
    [SerializeField] GameObject sendAcceptPopupUI;
    [SerializeField] GameObject sendDeclinePopupUI;
    [SerializeField] GameObject matchingFailPopupUI;

    private string processingUserId = "";
    private int time = 0;


    public void ShowMatchingNotification(string fromUserId)
    {
        OpenMatchingSendUI();
        // !!!! ������ UI�� ������ ���� �κ� ���� �ʿ�
    }

    public void OnMatchRequestReceived(string fromUserId)   // if someone choose me, it works
    {
        processingUserId = fromUserId;
        ShowMatchingNotification(processingUserId);
    }

    //public void ShowFileNotification(string fromUserId)
    //{
    //    afMatchingUI.SetActive(true);
    //}

    //public void OnFileReceived(string fromUserId)   // if someone choose me, it works
    //{
    //    ShowFileNotification(processingUserId);
    //}


    public void OnEvent(EventData photonEvent)
    {

    }

    public void AddListenerToPanel()
    {

    }

    // ��Ī ��û �޼��� �̺�Ʈ �Լ�
    public void SendRequestMessage()
    {
        userInfos.SendMatchRequestToAUser(UserMatchingManager.Instance.userInfos[userInfos.selectedUserIdx].photonUserName,
                        UserMatchingManager.Instance.myUserInfo);
        //Debug.Log($"selected Idx : {userInfos.selectedUserIdx}");
        //Debug.Log($"Accept User: {processingUserId}");
        //UserMatchingManagerSM.Instance.isUserMatchingSucceed = true;
    }

    // ��Ī ��û ���� �޼��� �̺�Ʈ �Լ� - Y
    //public void SendAcceptMessage()
    //{
    //    // �̰� �۵��ɰ� �ƴ϶�
    //    //userInfos.SendMatchRequestToAUser(UserMatchingManager.Instance.userInfos[userInfos.selectedUserIdx].photonUserName,
    //    //                UserMatchingManager.Instance.myUserInfo);
    //    // �̰� �۵��ž��� ��
    //    userInfos.SendMatchRequestToAUser(userInfos.receivedMatchInfo.userWhoSend, UserMatchingManager.Instance.myUserInfo);
    //    OpenSendAcceptPopupUI();
    //    //Debug.Log("DDDDD");

    //    //Debug.Log($"selected Idx : {userInfos.selectedUserIdx}");
    //    //Debug.Log($"Accept User: {processingUserId}");
    //    //UserMatchingManagerSM.Instance.isUserMatchingSucceed = true;
    //    //Debug.Log("Accept!");
    //}

    // ��Ī ��û ���� �޼��� �̺�Ʈ �Լ� - N
    //public void SendDeclineMessage()
    //{
    //    userInfos.SendMatchRequestToAUser(userInfos.receivedMatchInfo.userWhoSend, UserMatchingManager.Instance.myUserInfo);
    //    OpenSendDeclinePopupUI();
    //    Debug.Log($"�� �и� Decline ������ {userInfos.receivedMatchInfo.userWhoSend}���� ���´�");
    //    //Debug.Log("EEEEE");

    //    //Debug.Log($"Decline User: {processingUserId}");
    //    //UserMatchingManagerSM.Instance.isUserMatchingFailed = true;
    //    //Debug.Log("Decline!");
    //}

    //public void SendHoldMessage()
    //{
    //    Debug.Log($"Hold User: {processingUserId}");
    //}

    /////////////// ON/OFF BUTTON Callbacks ///////////////
    public void OpenMatchingSendUI()
    {
        matchingSendUI.SetActive(true);
    }
    public void CloseMatchingSendUI()
    {
        matchingSendUI.SetActive(false);
    }


    public void OpenMatchingReceiveUI()
    {
        matchingReceiveUI.SetActive(true);
    }
    public void CloseMatchingReceiveUI()
    {
        matchingReceiveUI.SetActive(false);
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

        // !!! �����ͺ��̽� ������ �������� �Լ� �۵��� ��
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

    public void OpenRouteVisualizationUI(Vector3 direction)
    {
        routeVisualizationUI.SetActive(true);
        //uiText.text = direction.magnitude.ToString() + "m left..";
        //SetRouteVisualizationUI(direction);
    }
    public void CloseRouteVisualizationUI()
    {
        routeVisualizationUI.SetActive(false);
    }
    public void UpdateRouteVisualizationUI(Vector3 direction, float myRotY)
    {
        uiText.text = direction.magnitude.ToString() + " m left..";

        //Debug.Log(direction.x / direction.z);
        float rotY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + 180 - myRotY;

        arrowRot.localEulerAngles = new Vector3(60, rotY, 0);
    }


    //---- PopUps ----//
    public void OpenReceiveAcceptPopupUI()
    {
        receiveAcceptPopupUI.SetActive(true);
        StartCoroutine(ActivateReceiveAcceptPopupUI());
    }
    private IEnumerator ActivateReceiveAcceptPopupUI()
    {
        receiveAcceptPopupUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        receiveAcceptPopupUI.SetActive(false);
    }

    public void OpenReceiveDeclinePopupUI()
    {
        receiveDeclinePopupUI.SetActive(true);
        StartCoroutine(ActivateReceiveDeclinePopupUI());
    }
    private IEnumerator ActivateReceiveDeclinePopupUI()
    {
        receiveDeclinePopupUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        receiveDeclinePopupUI.SetActive(false);
    }

    public void OpenSendAcceptPopupUI()
    {
        sendAcceptPopupUI.SetActive(true);
        StartCoroutine(ActivateSendAcceptPopupUI());
    }
    private IEnumerator ActivateSendAcceptPopupUI()
    {
        sendAcceptPopupUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        sendAcceptPopupUI.SetActive(false);
    }

    public void OpenSendDeclinePopupUI()
    {
        sendDeclinePopupUI.SetActive(true);
        StartCoroutine(ActivateSendDeclinePopupUI());
    }
    private IEnumerator ActivateSendDeclinePopupUI()
    {
        sendDeclinePopupUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        sendDeclinePopupUI.SetActive(false);
    }

    // !!! ���� �� 1
    //public void CloseAcceptPopupUI()
    //{
    //    receiveAcceptPopupUI.SetActive(false);
    //}
    //// !!!! ���� �� 2
    //public void CloseDeclinePopupUI()
    //{
    //    receiveDeclinePopupUI.SetActive(false);
    //}

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
        //Debug.Log($"time is {time}");

        //if (time == 0)
        //{
        //    // !!!!�ٷ� ����
        //    SendAcceptMessage();
        //}
        //else
        //{
        //    Debug.Log("dasdsdadasdadsd");
        //    // !!!time ���� DB�� ������Ʈ
        //}
    }

    public float GetTime()
    {
        return (float)time * 60;
    }
    //////////////////////////////////////////////
}
