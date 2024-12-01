using Photon.Pun;
using UnityEngine;
using CustomLogger;
public class HostOnlyBehaviour : MonoBehaviourPunCallbacks
{
    protected virtual void Awake(){
        // 호스트가 아닌 경우 자동으로 비활성화
        if(PhotonNetwork.IsMasterClient){
            enabled = false;
            Debug.Log($"[{GetType().Name}] 호스트가 아니므로 비활성화됨");
            return;
        }

        Debug.Log($"[{GetType().Name}] 호스트이므로 활성화됨");
        OnBecameHost(); // 1등으로 방에 들어온 호스트도 초기화 작업을 실행해야 함.
    }

    // 호스트 권한이 변경될 때 호출되는 콜백 메서드 (호스트가 나가서 다른 클라이언트가 호스트가 될 때)
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient){
        // 새로운 마스터 클라이언트인 경우에만 활성화
        if (PhotonNetwork.LocalPlayer == newMasterClient)
        {
            enabled = true;
            OnBecameHost();
            Debug.Log($"[{GetType().Name}] 새로운 호스트가 되어 활성화됨");            
        }
    }

    // 호스트가 되었을 때 실행할 가상 메서드
    protected virtual void OnBecameHost(){        
        // TODO: 파생 클래스에서 필요한 초기화 구현

        FileLogger.Log("호스트 등록: 호스트 매니저 초기화 완료", this);
    }

    // 호스트 권한을 잃었을 때 실행할 가상 메서드
    protected virtual void OnStoppedBeingHost(){
        // TODO: 파생 클래스에서 필요한 잉여 데이터 정리 작업 구현

        FileLogger.Log("호스트 해제: 기존 호스트의 잉여 데이터 정리 완료", this);
    }
}
