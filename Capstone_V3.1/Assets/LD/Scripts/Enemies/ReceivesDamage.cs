using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This Script will handle Health and damage events on this object. In order to receive damage, this gameObject will need an Enemy Tag and a Trigger Collider.
/// If you want the object to receive pushback, this GameObject will also need a Rigidbody
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class ReceivesDamage : MonoBehaviour {

    [Header("Health Settings")]
    [Tooltip("How much health this gameObject has before being disabled")]
    public int objectHealth = 2;            //How much health does the object have
    [Tooltip("If this object is invincible it will receive pushback/stun but will not take damage to health (and thus will never die)")]
    public bool isInvincible = false;
    [Tooltip("How long this object will be immune to hit events after being damaged")]
    public float hitInvulTime = .5f;     //How long is the object invincible after being damaged

    [Header("General Settings")]
    [Tooltip("How far to push the Enemy. Note, this GameObject will not be pushed unless it has a Rigidbody Component attached that has isKinematic disabled")]
    public float pushAmount = 0;            //How far back to push the object
    [Tooltip("This is the Particle Object to create when receiving damage")]
    public GameObject particalObjectWhenDamaged;    //Turn on and off particle object
    [Tooltip("Size of particle Object when it is spawned")]
    public float poDamageSize = 1;            //Size of particle object when spawned
    [Tooltip("This is the Particle Object to create when receiving damage")]
    public GameObject particalObjectWhenKilled;    //Turn on and off death particle object
    [Tooltip("Size of particle Object when it is spawned")]
    public float poKillSize = 1;            //Size of particle object when spawned


    [Header("SFX")]
    [Tooltip("Audio that plays when damage is successfully received")]
    public AudioClip sfx_damaged;           //Sound to play when damaged
    [Tooltip("Audio that plays when damage is successfully received")]
    public AudioClip sfx_death;             //Sound to play when killed

    private float invulTimer = 0f;      //Timer conuts down when activated, and deactivates when it hits 0. Use this to track our invul Time duration
    private bool isHitInvl = false;     //Refers to temporarily not receiving damage hit events. This is different than isInvul, as in this case pushback will not affect this object

    Color originalColor;        //Store the color our Material was originally using
    Rigidbody rb;                           
    GameObject player;

    void Awake()
    {
        //Grab our references
        rb = GetComponent<Rigidbody>();

        //Find the player, for pushback calculations
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        InvulTimer();       //Handle our timer countdowns
    }

    void InvulTimer()
    {
        //If we're not at 0, keep counting down the timer
        if(invulTimer > 0)
        {
            invulTimer -= Time.deltaTime;
            //If we just reached 0, lock it out
            if(invulTimer == 0)
            {
                //No longer invulerable
                InvulDeactivate();
            }
        }
        //If our timer has gone past 0, deactivate
        if(invulTimer < 0)
        {
            InvulDeactivate();
        }
    }

    void InvulActivate()
    {
        //We are temporarily invulnerable
        isHitInvl = true;
        //Assign the timer to count down
        invulTimer = hitInvulTime;
        //Push back the enemy, if there's a Rigidbody available
        if(rb != null)
        {
            PushBack();
        }
    }

    void InvulDeactivate()
    {
        //We have waited the invulnerable time, we are now vulnerable again
        isHitInvl = false;
    }

    public void DamageEnemy(int damageAmount)
    {
        if (!isHitInvl)
        {
            //Damage the object, if they're not invincible
            if (!isInvincible)
            {
                objectHealth -= damageAmount;
            }
            //Display visual, if a visual exists
            if (!isInvincible && particalObjectWhenDamaged != null)
            {
                //Spawn a new particle object, parent it and resize
                GameObject po;
                po = Instantiate(particalObjectWhenDamaged, transform.position, Quaternion.identity, gameObject.transform) as GameObject;
                po.transform.localScale = new Vector3(poDamageSize, poDamageSize, poDamageSize);
            }
            //Play a sound, if a sound clip exists
            if(sfx_damaged != null)
            {
                SoundManager.instance.PlaySound2DOneShot(sfx_damaged, 1f, true);
            }
            //If we go below 0 health, Disable the object
            if (objectHealth <= 0)
            {
                KillObject();
            }
            else
            {
                InvulActivate();    //Start Invul sequence
            }
        }
    }

    void PushBack()
    {
        Vector3 playerPos, pushDirection;
        playerPos = player.transform.position;
        //Calculate the vector
        pushDirection = transform.position - playerPos;
        //Push back the enemy, in the force we calculated
        rb.AddForce(pushDirection * pushAmount);
    }

    void KillObject()
    {
        //Play a sound, if a sound clip exists
        if (sfx_damaged != null)
        {
            SoundManager.instance.PlaySound2DOneShot(sfx_death, 1f, true);
        }
        //Play Visual
        //Display visual, if a visual exists
        if (particalObjectWhenDamaged != null)
        {
            //Spawn a new particle object, parent it and resize
            GameObject po;
            po = Instantiate(particalObjectWhenKilled, transform.position, Quaternion.identity) as GameObject;
            po.transform.localScale = new Vector3(poKillSize, poKillSize, poKillSize);
        }

        //TODO convert this to an Event later, to decouple our dropScript
        //If the Drop Object script is attached, Drop an Object
        DropPickup dropScript = GetComponent<DropPickup>();
        if(dropScript != null)
        {
            dropScript.RollAndDrop();
        }
        //Disable the object, it ran out of health
        gameObject.SetActive(false);
    }
}
