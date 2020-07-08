using UnityEngine;
using System.Collections;

/// <summary>
/// This script gives the Enemy_Charger movement pattern to this GameObject. The gameObject will periodically charge in a straight line towards the player's current position
/// (does not update player position after start of charge).
/// </summary>

[RequireComponent(typeof(Rigidbody))]               //This object is moving, it needs a Rigidbody
[RequireComponent(typeof(AudioSource))]            //AudioSource to play from, when we start a charge attack
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class Enemy_Charger : MonoBehaviour {

    [Header("Movement Settings")]
    [Tooltip("Movement speed during the charge attack")]
    public float moveSpeed = 20f;            //How fast does this enemy move towards the player
    [Tooltip("Duration of time the charge attack continues before stopping")]
    public float chargeDuration = 1.5f;       //Duration of charge attack
    [Tooltip("Duration of time to wait after a charge attack, before starting the next charge attack")]
    public float waitAfterCharge = 1f;      //Time spent waiting after charge is over

    [Header("General Settings")]
    [Tooltip("Distance player must be to this object before starting the attack pattern")]
    public float rangeOfAwareness = 20f;     //How close must the player be to be considered "In Range"
    [Tooltip("How often to detect the player's distance, to determine whether to start the attack pattern. Higher values are less frequent, but more optimized. Find a balance.")]
    public float detectFrequency = 1f;      //How often will the enemy detect player distance, higher values for efficiency

    [Header("SFX")]
    [Tooltip("Audio played when starting to move towards a new destination point")]
    public AudioClip sfx_Attack;    //Sound effect to play when enemy charges

    private bool playerInRange;             //Use to detect if the player is currently in range
    private Vector3 playerPos;              //Keep track of player position
    private bool isAttacking = false;       //Is the enemy in its attack sequence?
    private bool isCharging = false;        //Is the enemy currently in a charging state
    private float speedDampener = .01f;     //Multiply the speed so that we can use more reasonable numbers

    AudioSource audioSource;
    Rigidbody rb;
    GameObject player;

    void Awake(){
		//Fill our references
        player = GameObject.FindWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        //if an AudioClip exists, assign it to the AudioSource
        if(sfx_Attack != null)
        {
            audioSource.clip = sfx_Attack;  //Play the attack sound effect into the AudioSource
        }
        rb = GetComponent<Rigidbody>();
    }

    // Use this for initialization
    void Start()
    {
        //Adjust Object's Trigger collider to reflect inspector properties
        InvokeRepeating("DetectPlayer", .01f, detectFrequency);
    }
	
    void FixedUpdate()
    {
        //If we're charging, keep charging!
        if (isCharging)
        {
            //Move in the current direction
            rb.MovePosition(transform.position + transform.forward * moveSpeed * speedDampener);
        }
    }

	void StartAttack ()
	{
		//Get the player's current location
		playerPos = new Vector3 (player.transform.position.x, player.transform.position.y, player.transform.position.z);
        //Auto correct our rotation, if we got screwed up along the way
        transform.Rotate(0, transform.rotation.y, 0);
        //Face the direction we want to charge, so that we can move in forward direction
        transform.LookAt(playerPos);

        //If we have an audioSource, play the sound
        if(audioSource != null)
        {
            if(sfx_Attack != null)
            {
                audioSource.clip = sfx_Attack;     //Assign the sound effect to the AudioSource
                float randomPitch = Random.Range(.90f, 1.1f);
                audioSource.pitch = randomPitch;
                audioSource.Play();		//Play the sound effect
            }
        }
	}

    void DetectPlayer()
    {
        //Store the distance between the player and this enemy
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);
        //If the player is close enough, flag the boolean to communicate this
        if(playerDistance <= rangeOfAwareness)
        {
            playerInRange = true;
            //Start our attack pattern if we're not already attacking
            if (!isAttacking)
            {
                isAttacking = true;
                //Start the Coroutine that switches the direction at specified lengths of time
                StartCoroutine("AttackPattern");
            }
        }
        else
        {
            //Player is not in range, cancel our attackpattern coroutine()
            playerInRange = false;
            if (isAttacking)
            {
                isAttacking = false;
                //Debug.Log("Stop the attack Pattern");
                //Stop our attack pattern coroutine
                StopCoroutine("AttackPattern");
                //Make sure we also stop our charge, if we're mid charge
                isCharging = false;
            }
        }
    }

    void OnDisable()
    {
        //When this Object is disabled, make sure you also cancel the Invoke repeating method
        CancelInvoke();
    }

    IEnumerator AttackPattern()
    {
        //Debug.Log("Start an attack Pattern");
        //Switch according to the frequency as long as this GameObject is active
        while (true)
        {
            //If the player is in range, CHARGE
            if (playerInRange)
            {
                StartAttack();      //Locate player and start attack!
                isCharging = true;   //We're attacking again
            }
            //We're charging for the next few seconds
            yield return new WaitForSeconds(chargeDuration);
            isCharging = false;  //We're no longer attacking
            yield return new WaitForSeconds(waitAfterCharge);
        }
    }

}
