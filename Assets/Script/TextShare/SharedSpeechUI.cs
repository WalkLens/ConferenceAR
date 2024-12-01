using Photon.Pun;
using TMPro;
using UnityEngine;

public class SharedSpeechUI : MonoBehaviourPun
{
    public TextMeshProUGUI sharedText; // 공유할 텍스트 UI
    private string lastSyncedText = ""; // 마지막으로 동기화된 텍스트

    void Update()
    {
        if (photonView.IsMine) // 로컬 플레이어만 텍스트를 동기화
        {
            if (sharedText.text != lastSyncedText)
            {
                lastSyncedText = sharedText.text; // 텍스트 업데이트
                photonView.RPC("SyncText", RpcTarget.All, lastSyncedText);
            }
        }
    }

    [PunRPC]
    public void SyncText(string syncedText)
    {
        // 네트워크를 통해 받은 텍스트를 UI에 반영
        sharedText.text = syncedText;
    }
}
