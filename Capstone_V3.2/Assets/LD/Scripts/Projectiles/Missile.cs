using UnityEngine;
using System.Collections;

/// <summary>
/// This script gives this GameObject missile projectile functionality. This object will move forward in a straight line until it hits a collider.
/// If the collider hit is a player, it will deal damage and destroy itself. If it hits either a trigger or enemy gameObject it will continue going. If it hits anything else it will destroy itself.
/// NOTE: You need both a trigger collider and a Rigidbody in order for this script to function properly
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject
[RequireComponent(typeof(Rigidbody))]               //This object is moving, it needs a Rigidbody

public class Missile : MonoBehaviour {

    [Header("Movement Settings")]
    [Tooltip("Speed this object will move in its forward direction")]
    public float travelSpeed = 5f;	//How fast is the Missile traveling

    [Header("General Settings")]
    [Tooltip("If true a player attack will preemptively destroy this object. Useful if you wish to give player a way to destroy bullets with good reflexes")]
    public bool attackDestructible = false;     //Can the player destroy this projectile with an attack?

	PlayerManager playerManager;
	Rigidbody rigidBody;

	void Awake(){
		rigidBody = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void FixedUpdate () {
		//Move bullets forward every frame
		rigidBody.MovePosition(transform.position + transform.forward * travelSpeed * Time.deltaTime);
	}

	void OnTriggerEnter(Collider other){

		if(other.tag == "Enemy" || other.tag == "Trigger"){
			//Another enemy was hit, ignore them
			return;
		}
        if(other.tag == "PlayerWeapon")
        {
            //encountered player weapon, determine whether or not projectile should be destroyed
            if (attackDestructible)
            {
                Destroy(gameObject);
            } else
            {
                //Cannot be destroyed with attacks, keep traveling
                return;
            }
        }
        //Test to see if we find the player
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
        //If we've collided with anything else in the level, destroy the missile
        else {
			Destroy(gameObject);
		}
	}
}
