using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will receive an OnTrigger event, and if it is the player will add health to the player's total.
/// NOTE: We need a Trigger Collider on this gameObject to receive the OnTrigger event
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class Powerup_HealthIncrease : MonoBehaviour {

    [Header("General Settings")]
    [Tooltip("Amount of health to add to player total")]
    public int healthToAdd = 3;     //How much health to add

    [Header("SFX")]
    [Tooltip("Sound Effect to play when this powerup is triggered")]
    public AudioClip sfx_Pickup;   //Sound to play when Collected

    PlayerManager playerManager;        //We get a player reference so that we can add to the player health total (which is where health is stored)

    void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();  //Find the Game Manager
    }

    void OnTriggerEnter(Collider other)
    {
        //Make sure it's the PLAYER who has entered our trigger
        if (other.CompareTag("Player"))
        {
            playerManager.AddHealth(healthToAdd);   //Add the Health

            //If we have an audio clip assigned, play it
            if(sfx_Pickup != null)
            {
                SoundManager.instance.PlaySound2DOneShot(sfx_Pickup, 1f, true);    //Play 2D Sound Effect
            }
            gameObject.SetActive(false);    //Destroy this gameObject because it has been collected
        }
    }
}
