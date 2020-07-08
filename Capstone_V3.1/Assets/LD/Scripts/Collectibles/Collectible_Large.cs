using UnityEngine;
using System.Collections;

/// <summary>
/// Large collectible that when collected adds to the player's large collectible count. Note: Collectible means that it is considered in the Save/Spawn system, and will be propogated if saved on level load
/// NOTE: If you dont want this collectible to respawn after it is collected, make sure you add a SaveState_Collectible script to this gameObject as well
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class Collectible_Large : MonoBehaviour {

    [Header("SFX")]
    [Tooltip("Sound to play when the key is collected")]
    public AudioClip sfx_Collect;   //Sound to play when the collectible is touched

    UIManager uiManager;        //Reference to the UI Manager
	PlayerManager playerManager;    //Reference to the Player Manager
    //SaveState_Collectible saveCollect;

    void Awake(){
		uiManager = FindObjectOfType<UIManager>();		//find the UIManager in the scene
		playerManager = FindObjectOfType<PlayerManager>();  //find the LevelManager in the scene
        //saveCollect = GetComponent<SaveState_Collectible>();    //Attempt to find SaveState script
    }

    void OnTriggerEnter(Collider other){

		//detect to see if the player has entered the object trigger
		if(other.gameObject.tag == "Player"){
            Collect();  //Detect whether or not to save this object
            Pickup();   //Apply pickup properties and disable
        }
    }

    /// <summary>
    /// Pickup the large collectible, play the sound effect, update GUI and deactivate it
    /// </summary>
    void Pickup()
    {
        SoundManager.instance.PlaySound2DOneShot(sfx_Collect, 1f, true);  //Play 2D Sound Effect
        playerManager.localPlayerData.largeCollectibles++;     //Add 1 to our Collectible static variable
        uiManager.UpdateScore();        //Update the score in our UI Manager
        gameObject.SetActive(false);        //Destroy this gameobject because it has been collected
    }

    //TODO change to event/listener
    /// <summary>
    /// Collect the large collectible, saving it into the player's inventory
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
