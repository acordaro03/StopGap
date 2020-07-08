using UnityEngine;
using System.Collections;

/// <summary>
/// Generic key that opens a generic locked door. Note: Collectible means that it is considered in the Save/Spawn system, and will be propogated if saved on level load
/// </summary>
[RequireComponent(typeof(SaveState_Collectible))]   //For now this is required. Remove this whenever you decouple savestate functionality
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class Collectible_KeyGeneric : MonoBehaviour
{
    [Header("SFX")]
    [Tooltip("Sound to play when the key is collected")]
	public AudioClip sfx_Collect;   //Sound to play when collected

    UIManager uiManager;
    PlayerManager playerManager;
    //SaveState_Collectible saveCollect;

    void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        uiManager = FindObjectOfType<UIManager>();
        //saveCollect = GetComponent<SaveState_Collectible>();    //Attempt to find SaveState script
    }

    void OnTriggerEnter(Collider other)
    {
        //detect to see if the player has entered the object trigger
        if (other.gameObject.tag == "Player")
        {
            Collect();  //Detect whether or not to save this object
            Pickup();   //Apply pickup properties and disable
        }
    }

    /// <summary>
    /// Pickup the key, play the sound effect, update GUI and deactivate it
    /// </summary>
    void Pickup()
    {
        SoundManager.instance.PlaySound2DOneShot(sfx_Collect, 1f, true);  //Play 2D Sound Effect
        playerManager.localPlayerData.keys++;     //Add 1 to our Collectible static variable
        uiManager.UpdateScore();        //Update the score in our UI Manager
        gameObject.SetActive(false);        //Destroy this gameobject because it has been collected
    }

    //TODO change to event/listener
    /// <summary>
    /// Collect the key, saving it into the player's inventory
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
