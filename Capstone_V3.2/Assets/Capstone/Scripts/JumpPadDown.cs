using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadDown : MonoBehaviour
{

    public float speedDown;
    // public GameObject player;
    // public float buffer;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Shootable"))
        {

            other.gameObject.GetComponent<Rigidbody>
                    ().AddForce(Vector3.down* speedDown);
            //,ForceMode.Impulse
            /* if (other.gameObject.transform.position.y >= player.gameObject.transform.position.y + buffer) {
                other.gameObject.GetComponent<Rigidbody>
            ().AddForce(Vector3.down * speed); // Debug.Log("yes"); */
        }
    }

}
