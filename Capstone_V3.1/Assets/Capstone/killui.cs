using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class killui : MonoBehaviour {

    public GameObject stupid;

    void OnMouseDown()
    {
        Debug.Log("ffsavad");
     
        stupid.SetActive(false);
    }

}
