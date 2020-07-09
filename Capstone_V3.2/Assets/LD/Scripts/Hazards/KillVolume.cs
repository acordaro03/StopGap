using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// This script will add Kill functionality to this object. If the player touches a Trigger Volume on this object, the player will lose a life.
/// This is different than a damage script in that the player will always lose a full life from this script rather than take damage. Apply this to 
/// lava pits/endless holes/or dangerous hazards that always kill player regardless of life.
/// NOTE: This script will only work if you have a Trigger volume attached to this GameObject, as KillLife happens on TriggerEnter()
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class KillVolume : MonoBehaviour {

    [Header("Kill Settings")]
    [Tooltip("True will kill enemies in addition to the player. False will only kill the player, leaving enemies immune")]
    public bool killEnemy = true;           //Kill enemies in addition to the player

    PlayerManager playerManager;

    void Awake(){
		playerManager = FindObjectOfType<PlayerManager>();
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

	//Kill the player if they touch the collider on this object
	void OnTriggerEnter(Collider other){
		//Detect for the player
		if(other.CompareTag("Player")){
			//Kill the Player
			playerManager.LoseLife(1);
		}
        //If the object that entered is an enemy, and the killEnemy bool is true, disable it
        if (other.CompareTag("Enemy") && killEnemy)
        {
            //Kill the enemy
            other.gameObject.SetActive(false);
        }
	}
}
