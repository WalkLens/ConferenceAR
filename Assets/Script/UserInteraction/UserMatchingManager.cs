using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class UserMatchingManager : MonoBehaviour
{
    [SerializeField] NotificationManager notificationManager;
    [SerializeField] InteractionUIManager interactionUIManager;
    [SerializeField] UserBehaviourManager userBehaviourManager;

    public static UserMatchingManager Instance { get; private set; }

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
        // Phase 1. Matching
        if (isUserMatchingSended)
        {
            notificationManager.OnMatchRequestReceived(imsiId);
            isUserMatchingSended = false;
        }

        // Phase 2. Route Visualizing
        if (isUserMatchingSucceed && !isUserMet)
        {
            interactionUIManager.ShowRoute(myPosition.position, partnerPosition.position);
            userBehaviourManager.CheckMetState(myPosition.position, partnerPosition.position);
            myPosition.transform.position = Vector3.MoveTowards(myPosition.position, partnerPosition.position, 1f * Time.deltaTime);      // IMSI MOVER
        }
        else if (isUserMet)
        {
            interactionUIManager.HideRoute();
            interactionUIManager.ShowBox();
        }

        // Phase 3. After Matching Service On
        if (isUserRibbonSelected)
        {
            interactionUIManager.OpenBox();
        }
        if (isUserFileSended)
        {
            notificationManager.OnFileReceived(imsiId);
            isUserFileSended = false;
        }
    }
}
