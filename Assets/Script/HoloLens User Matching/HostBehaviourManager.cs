using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using CustomLogger;

public class HostBehaviourManager : MonoBehaviourPunCallbacks
{
    private static HostBehaviourManager instance;
    public List<HostOnlyBehaviour> hostBehaviours = new List<HostOnlyBehaviour>();
    public static HostBehaviourManager Instance => instance;

    public bool IsCentralHost
    {
        get
        {
            if (PhotonNetwork.IsMasterClient &&
                PhotonNetwork.InRoom &&
                PhotonNetwork.CurrentRoom.Name == "DefaultRoom" &&
                PhotonNetwork.NickName != "CentralHost"
                )
            {
                PhotonNetwork.NickName = "CentralHost";
                foreach (var hostBehaviour in hostBehaviours)
                {
                    // 최초 1회 Central Host로 변경 시에만 호출(반복 호출 방지)
                    if (hostBehaviour.TryGetComponent(out UserMatchingManager hb))
                    {
                        FileLogger.Log($"hostBehaviour: {hb}");
                        hb.UpdateNickNameAfterJoin(PhotonNetwork.NickName);
                        hb.myUserInfo.photonUserName = PhotonNetwork.NickName;
                        
                        // myUserInfo와 동일한 photonUserName이 없고, photonRole이 존재하는 경우 추가
                        if (!hb.userInfos.Exists(user => user.photonUserName == hb.myUserInfo.photonUserName )
                             &&  !string.IsNullOrEmpty(hb.myUserInfo.photonRole))
                        {
                            hb.userInfos.Add(hb.myUserInfo);
                            FileLogger.Log($"UserInfo added: {hb.myUserInfo.photonUserName}");
                        }
                    }
                }
                return true;
            }
            else if (PhotonNetwork.NickName == "CentralHost")
                return true;
            else
                return false;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    #region DEBUG
    [ContextMenu("Log All Users Info")]
    public void LogAllUsersInfo(ref List<UserInfo> allUsersInfo)
    {
        foreach (UserInfo userInfo in allUsersInfo)
        {
            FileLogger.Log($"{userInfo.currentRoomNumber} || {userInfo.photonRole} || {userInfo.photonUserName} || {userInfo.currentState}");
        }
    }
    #endregion

    public void RegisterHostBehaviour(HostOnlyBehaviour behaviour)
    {
        if (!hostBehaviours.Contains(behaviour))
        {
            FileLogger.Log($"Registered HostBehaviour: {behaviour}", this);
            hostBehaviours.Add(behaviour);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        FileLogger.Log("OnMasterClientSwitched", this);
        base.OnMasterClientSwitched(newMasterClient);

        if (PhotonNetwork.CurrentRoom.Name != "DefaultRoom")
            return;

        UpdateCentralHostStatus(newMasterClient);
    }
    
    // 중앙 호스트 상태 업데이트
    public void UpdateCentralHostStatus(Player newMasterClient)
    {
        bool wasCentralHost = PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Name == "DefaultRoom";
        bool willBeCentralHost = PhotonNetwork.LocalPlayer == newMasterClient;

        foreach (var behaviour in hostBehaviours)
        {
            if (wasCentralHost && behaviour.isActiveAsHost)
            {
                behaviour.OnStoppedBeingHost();
                behaviour.isActiveAsHost = false;
                FileLogger.Log($"[{behaviour.GetType().Name}] 호스트 비활성화", this);
            }

            if (willBeCentralHost)
            {
                behaviour.isActiveAsHost = true;
                behaviour.OnBecameHost();
                PhotonNetwork.NickName = "CentralHost";
                FileLogger.Log($"[{behaviour.GetType().Name}] 호스트 활성화", this);
            }
        }
    }
    public void UpdateCentralHostStatus()
    {
        foreach (var behaviour in hostBehaviours)
        {
            if (!IsCentralHost)
            {
                if (behaviour.isActiveAsHost)
                {
                    behaviour.isActiveAsHost = false;
                    behaviour.OnStoppedBeingHost();
                    FileLogger.Log($"[{behaviour.GetType().Name}] 호스트 비활성화", this);
                }
            }
            else
            {
                if (!behaviour.isActiveAsHost)
                {
                    behaviour.isActiveAsHost = true;
                    behaviour.OnBecameHost();
                    FileLogger.Log($"[{behaviour.GetType().Name}] 호스트 활성화", this);
                }
            }
        }
    }

    public void HandleOnJoinedRoom()
    {
        foreach (var behaviour in hostBehaviours)
        {
            behaviour.HandleOnJoinedRoom();
        }
    }
}