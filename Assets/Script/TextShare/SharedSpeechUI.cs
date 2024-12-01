using Photon.Pun;
using TMPro;
using UnityEngine;

public class SharedSpeechUI : MonoBehaviourPun
{
    public TextMeshProUGUI sharedText; // ������ �ؽ�Ʈ UI
    private string lastSyncedText = ""; // ���������� ����ȭ�� �ؽ�Ʈ

    void Update()
    {
        if (photonView.IsMine) // ���� �÷��̾ �ؽ�Ʈ�� ����ȭ
        {
            if (sharedText.text != lastSyncedText)
            {
                lastSyncedText = sharedText.text; // �ؽ�Ʈ ������Ʈ
                photonView.RPC("SyncText", RpcTarget.All, lastSyncedText);
            }
        }
    }

    [PunRPC]
    public void SyncText(string syncedText)
    {
        // ��Ʈ��ũ�� ���� ���� �ؽ�Ʈ�� UI�� �ݿ�
        sharedText.text = syncedText;
    }
}
