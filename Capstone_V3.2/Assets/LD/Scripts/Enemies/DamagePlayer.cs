using UnityEngine;
using System.Collections;

/// <summary>
/// This Script handles Damage events sent to the player, including damage over time. 
/// In order to damage the player, this GameObject will also need a Trigger Volume (which calls on player enter)
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class DamagePlayer : MonoBehaviour {

    [Header("Damage Settings")]
    [Tooltip("Amount of damage to subtract from player health")]
    public int damageAmount = 1;    //Amount to damage the player
    [Tooltip("Force to add to player rigidbody, pushing back in a straight line from this gameObjects position")]
    public float pushBack = 0;      //Amount to push back the player
    [Tooltip("How long to stun the player, disabling input")]
    public float stunTime = 0;      //Length of player stun on damage
    [Tooltip("How long between damage intervals, continuously damaging the player.")]
    public float damageFrequency = 0.5f;    //How often to keep applying damage while in trigger

    float damageCounter = 0;        //Use this for setting time since last damage event
    bool isDamaging = false;          //Keep track of if damage will continue to be applied

	PlayerManager playerManager;

	// Use this for initialization
	void Awake () {
		playerManager = FindObjectOfType<PlayerManager>();
	}

    void Update()
    {
        //If recurring damage is enabled, countdown the timers
        if (isDamaging)
        {
            UpdateCounters();
        }
    }

	void OnTriggerEnter(Collider other){
        //If it's the player, Send initial damage, start the counter, and enable recurring damage
		if(other.tag == "Player"){
            SendDamage();
            damageCounter = damageFrequency;
            DamageRecurring(true);
		}
	}

    void OnTriggerExit(Collider other)
    {
        //If it's the player, turn on our constant damage bool and send an initial damage hit
        if (other.tag == "Player")
        {
            DamageRecurring(false);
        }
    }

    //Countdown and handle our timers
    void UpdateCounters()
    {
        //Countdown our timer
        damageCounter -= Time.deltaTime;
        //if damageCounter is less than 0, it's time to reapply damage
        if(damageCounter <= 0)
        {
            //Damage the player and reset the counter
            SendDamage();
            //If the player's health is now below zero, stop damage over time
            if (playerManager.IsHealthZero())
            {
                DamageRecurring(false);
            } else //Otherwise reset the counter and count down again
            {
                damageCounter = damageFrequency;
            }
        }
    }

    //Send the damage event to the player, applying damage, pushback and stun
    void SendDamage()
    {
        playerManager.PlayerHit(damageAmount, pushBack, stunTime, transform.position);		//Attempt a hit on the player
    }

    //Enable or disable our recurring damage flag
    void DamageRecurring(bool isRecurring)
    {
        isDamaging = isRecurring;
    }
}
