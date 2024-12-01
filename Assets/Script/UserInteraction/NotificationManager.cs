using ExitGames.Client.Photon;
using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    [System.Serializable]
    public class PressableButtons
    {
        public PressableButton acceptButton;
        public PressableButton declineButton;
        public PressableButton holdButton;
    }
    public PressableButtons buttons;
    [SerializeField] GameObject MatchingUI;
    [SerializeField] GameObject afMatchingUI;
    [SerializeField] GameObject profileUI;

    private string processingUserId = "";

    public void ShowMatchingNotification(string fromUserId)
    {
        MatchingUI.SetActive(true);
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
        MatchingUI.SetActive(false);
        UserMatchingManager.Instance.isUserMatchingSucceed = true;
    }

    public void SendDeclineMessage()
    {
        Debug.Log($"Decline User: {processingUserId}");
        MatchingUI.SetActive(false);
    }

    public void SendHoldMessage()
    {
        Debug.Log($"Hold User: {processingUserId}");
        MatchingUI.SetActive(false);
    }

    public void OpenFile()
    {
        Debug.Log($"Open File from {processingUserId}");
        profileUI.SetActive(true);
        afMatchingUI.SetActive(false);
    }

    public void CloseFile()
    {
        Debug.Log($"Open File from {processingUserId}");
        profileUI.SetActive(false);
    }
}
