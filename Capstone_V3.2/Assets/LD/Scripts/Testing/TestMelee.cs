using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMelee : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.SendMessage("DamageEnemy",1,SendMessageOptions.DontRequireReceiver);
        }
    }
}
