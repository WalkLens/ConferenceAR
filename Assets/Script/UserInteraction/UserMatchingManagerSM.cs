using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class UserMatchingManagerSM : MonoBehaviour
{
    [SerializeField] NotificationManager notificationManager;           // �˸��� �����ϴ� �κ�
    [SerializeField] InteractionUIManager interactionUIManager;         // HMD UI�� �����ϴ� �κ�
    [SerializeField] UserBehaviourManager userBehaviourManager;         // ������� �ൿ(������ ���� �̵�,)�� �����ϴ� �κ�

    public static UserMatchingManagerSM Instance { get; private set; }

    // Flags
    public bool isUserMatchingSended = false;
    public bool isUserMatchingSucceed = false;
    public bool isUserMet = false;
    public bool isUserRibbonSelected = false;
    public bool isUserFileSended = false;

    // User Data
    public string imsiId = "241201";
    public Transform myPosition;
    public Transform partnerPosition;

    // Phases
    public enum Phase
    {
        Matching,
        RouteVisualizing,
        AfterMatching,
    }
    private Phase currentPhase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    ///////////// Corutine just for Test /////////////////
    private IEnumerator SendUserMatchingRoutine()
    {
        if (!isUserMatchingSucceed)
        {
            yield return new WaitForSeconds(5f);
            isUserMatchingSended = true;
        }
    }

    private IEnumerator SendUserFileRoutine()
    {
        if (isUserMet)
        {
            yield return new WaitForSeconds(20f);
            isUserFileSended = true;
        }
    }
    //////////////////////////////////////////////////////

    private void Start()
    {
        StartCoroutine(SendUserMatchingRoutine());
        StartCoroutine(SendUserFileRoutine());
    }

    void Update()
    {
        // SWITCH�� �����ϸ� ���� ��
        switch (currentPhase)
        {
            // Phase 1. Matching - Accept�� ���ȴ��� �ֱ������� ����
            case Phase.Matching:
                if (isUserMatchingSended)
                {
                    notificationManager.OnMatchRequestReceived(imsiId);
                    //isUserMatchingSended = false;
                    currentPhase++;
                }
                break;

            // Phase 2. Route Visualizing - ��Ī�� �Ϸ�Ǹ� ��� �ð�ȭ
            case Phase.RouteVisualizing:
                if (isUserMatchingSucceed && !isUserMet)
                {
                    interactionUIManager.ShowRoute(myPosition.position, partnerPosition.position);
                    userBehaviourManager.CheckMetState(myPosition.position, partnerPosition.position);

                    // IMSI MOVER - ����ڰ� �����̴� ���̶�� �ӽ� ����
                    myPosition.transform.position = Vector3.MoveTowards(myPosition.position, partnerPosition.position, 1f * Time.deltaTime);
                }
                else if (isUserMet)
                {
                    interactionUIManager.HideRoute();
                    interactionUIManager.ShowBox();
                    currentPhase++;
                }
                break;

            // Phase 3. After Matching Service On
            case Phase.AfterMatching:
                if (isUserRibbonSelected)       // 1) ������ �� ������ ���õǾ����� �ֱ������� Ȯ��
                {
                    interactionUIManager.OpenBox();
                }
                if (isUserFileSended)           // 2) ������ �Դ��� Ȯ��
                {
                    notificationManager.OnFileReceived(imsiId);
                    isUserFileSended = false;
                }
                break;
        }
    }
}
