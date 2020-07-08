using UnityEngine;
using System.Collections;

/// <summary>
/// Generic door that opens if you have a generic key. Note: Collectible means that it is considered in the Save/Spawn system, and will be propogated if saved on level load
/// Technically a door doesn't fit in your inventory, but for save states and functionality you can think of the door as being 'collected'. SCIENCE.
/// </summary>
[RequireComponent(typeof(SaveState_Collectible))]   //For now this is required. Remove this whenever you decouple savestate functionality
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class LockedDoorGeneric : MonoBehaviour {

    [Header("SFX")]
    [Tooltip("Sound to play when the door is unlocked")]
    public AudioClip sfx_unlock;    //Sound Effect for unlocking the door

    PlayerManager playerManager;
	UIManager uiManager;
    //SaveState_Collectible saveCollect;

    void Awake(){
		playerManager = FindObjectOfType<PlayerManager>();
		uiManager = FindObjectOfType<UIManager>();
        //saveCollect = GetComponent<SaveState_Collectible>();    //Attempt to find SaveState script
    }

	void OnTriggerEnter(Collider other){

		if(other.gameObject.tag == "Player"){		//make sure we're only detecting the player
			//If we dont have enough keys, the door remains locked
			if (playerManager.localPlayerData.keys == 0){
				//Display Message that the door is locked
				//Display Sound Effect that the door is locked
			}
			//If we have enough keys, disable door, subtract a key, and update UI
			else if(playerManager.localPlayerData.keys > 0){
                Collect();  //Detect whether or not to save this object
                Pickup();   //Apply pickup properties and disable
            }
        }
	}

    /// <summary>
    /// Unlock the door ('pick it up'), play the sound effect, subtract a key, update GUI and deactivate this object
    /// </summary>
    void Pickup()
    {
        SoundManager.instance.PlaySound2DOneShot(sfx_unlock, 1f, true);  //Play 2D Sound Effect
        playerManager.localPlayerData.keys--;     //Subtract from our keys, we used it on a door
        uiManager.UpdateScore();        //Update the score in our UI Manager
        gameObject.SetActive(false);        //Destroy this gameobject because it has been collected
    }

    //TODO change to event/listener
    /// <summary>
    /// Collect the door, saving it into the player's inventory. It seems weird, but we can treat this as a thing the player collects for the purpose of level spawning/despawning
    /// in the save system
    /// </summary>
    void Collect()
    {
        //If there is a SaveState_Collectible script, run it
        SaveState_Collectible saveState = GetComponent<SaveState_Collectible>();
        if (saveState != null)
        {
            saveState.Collect();    //Save the item to the player
        }
    }
}
