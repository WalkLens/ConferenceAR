using Photon.Pun;
using UnityEngine;
using CustomLogger;
public class HostOnlyBehaviour : MonoBehaviourPunCallbacks
{
    public bool isActiveAsHost = false;
    protected virtual void Awake()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            isActiveAsHost = false;
            FileLogger.Log($"[{GetType().Name}] 호스트가 아니므로 비활성화됨", this);
            return;
        }

        isActiveAsHost = true;
        FileLogger.Log($"[{GetType().Name}] 호스트이므로 활성화됨", this);
        OnBecameHost(); // 1등으로 방에 들어온 호스트도 초기화 작업을 실행해야 함.
    }

    void Start()
    {
        FileLogger.Log($"HostOnlyBehaviour Start - GameObject Active: {gameObject.activeInHierarchy}, Component Enabled: {enabled}", this);
    }
    


    // 호스트가 되었을 때 실행할 가상 메서드
    public virtual void OnBecameHost(){
        // TODO: 파생 클래스에서 필요한 초기화 구현

        FileLogger.Log("호스트 등록: 호스트 매니저 초기화 완료", this);
    }

    // 호스트 권한을 잃었을 때 실행할 가상 메서드
    public virtual void OnStoppedBeingHost(){
        // TODO: 파생 클래스에서 필요한 잉여 데이터 정리 작업 구현

        FileLogger.Log("호스트 해제: 기존 호스트의 잉여 데이터 정리 완료", this);
    }

}
