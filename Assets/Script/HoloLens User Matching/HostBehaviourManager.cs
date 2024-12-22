using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;

public class HostBehaviourManager : MonoBehaviourPunCallbacks
{
    private static HostBehaviourManager instance;
    private List<HostOnlyBehaviour> hostBehaviours = new List<HostOnlyBehaviour>();
    public static HostBehaviourManager Instance => instance;
    public bool IsCentralHost => 
        PhotonNetwork.IsMasterClient && 
        PhotonNetwork.InRoom &&
        PhotonNetwork.CurrentRoom.Name == "DefaultRoom";

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

    public void RegisterHostBehaviour(HostOnlyBehaviour behaviour)
    {
        if (!hostBehaviours.Contains(behaviour))
            hostBehaviours.Add(behaviour);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        
        if (PhotonNetwork.CurrentRoom.Name != "DefaultRoom") 
            return;

        bool wasCentralHost = PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.Name == "DefaultRoom";
        bool willBeCentralHost = PhotonNetwork.LocalPlayer == newMasterClient;

        foreach (var behaviour in hostBehaviours)
        {
            if (wasCentralHost && behaviour.isActiveAsHost)
            {
                behaviour.OnStoppedBeingHost();
                behaviour.isActiveAsHost = false;
            }

            if (willBeCentralHost)
            {
                behaviour.isActiveAsHost = true;
                behaviour.OnBecameHost();
                PhotonNetwork.NickName = "CentralHost";
            }
        }
    }
}