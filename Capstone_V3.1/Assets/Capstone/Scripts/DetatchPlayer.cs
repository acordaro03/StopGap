using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetatchPlayer : MonoBehaviour {



    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Test1");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Test");
            other.transform.parent =null;
        }
    }

   
}
