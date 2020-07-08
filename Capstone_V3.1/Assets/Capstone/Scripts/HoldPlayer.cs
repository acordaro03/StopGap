using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldPlayer : MonoBehaviour
{

    public GameObject Player;
    public GameObject movingPlatform;

    private void OnTriggerEnter(Collider other)
    {
        Player.transform.parent = movingPlatform.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        Player.transform.parent = null;
    }
}
