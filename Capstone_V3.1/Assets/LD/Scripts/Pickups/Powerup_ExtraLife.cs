using UnityEngine;
using System.Collections;

/// <summary>
/// This script will give this gameObject the Extra Life functionality. On an OnTrigger event, if the player is detected the specified number of lives will be added to the players current lives.
/// NOTE: A trigger collider must be present on this object to receive this event
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class Powerup_ExtraLife : MonoBehaviour {

    [Header("General Settings")]
    [Tooltip("Number of lives to add")]
    public int livesToAdd = 1;		//How many lives do we want to add?

    [Header("SFX")]
    [Tooltip("Sound to play when this powerup is triggered")]
    public AudioClip sfx_Pickup;	//Sound to play when Collected

	PlayerManager playerManager;        //We get the player reference so that we can add to the player life total (which is where lives are stored)

	void Awake(){
		playerManager = FindObjectOfType<PlayerManager>();		//Find the Game Manager
	}

	void OnTriggerEnter(Collider other){

		//detect to see if the player has entered the object trigger
		if(other.gameObject.tag == "Player"){
			playerManager.AddLife(livesToAdd);  //Add the lives
            //TODO Display Visual
            //If we have an audio clip assigned, play it
            if (sfx_Pickup != null)
            {
                SoundManager.instance.PlaySound2DOneShot(sfx_Pickup, 1f, true);    //Play 2D Sound Effect
            }
            gameObject.SetActive(false);	//Destroy this gameobject because it has been collected
		}
	}
}
