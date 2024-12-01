using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserBehaviourManager : MonoBehaviour
{
    public void CheckMetState(Vector3 me, Vector3 partner)
    {
        if ((me - partner).magnitude < 2f) { UserMatchingManager.Instance.isUserMet = true; }
        else { UserMatchingManager.Instance.isUserMet = false; }

        Debug.Log((me - partner).magnitude);
    }
}
