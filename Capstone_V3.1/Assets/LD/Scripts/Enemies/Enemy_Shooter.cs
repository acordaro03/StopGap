using UnityEngine;
using System.Collections;
/// <summary>
/// This script gives a GameObject the Enemy_Turret attack pattern. A turret will face the player (when they are close enough) and fire bullets
/// repeatedly every incrememnt of time.
/// </summary>
[RequireComponent(typeof(Rigidbody))]              //This object is moving, it needs a Rigidbody
[RequireComponent(typeof(AudioSource))]            //AudioSource to play from, when we start a charge attack
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class Enemy_Shooter : MonoBehaviour {

    //TODO make enemy RotateTowards function work with Rigidbody physics, using rb.Rotate()
    [Header("Fire Settings")]
    [Tooltip("Projectile to spawn when 'firing'")]
    public GameObject projectileToFire;        //What GameObject is the enemy shooting
    [Tooltip("Where to spawn projectile. NOTE: make sure spawn position is outside of this object's collider range. Projectile will not get instantiated without a spawn position")]
    public Transform projectileSpawnPosition;   //Missile spawn location
    [Tooltip("How often to fire a bullet")]
    public float fireRate = 1f;         //How often will the enemy fire

    [Header("Movement Settings")]
    [Tooltip("True will face towards enemy player, tracking movement. False will retain original set direction")]
    public bool tracksPlayer = true;        //Will the shooter track the player
    [Tooltip("True will track player vertically, false will only track horizontal. Without vertical tracking, turret will only fire forward around Y axis, and not rotate upwards")]
    public bool trackVertical = false;	    //Whether or not to Track players vertically

    [Header("General Settings")]
    [Tooltip("Distance player must be to this object before starting the attack pattern")]
    public float rangeOfAwareness = 20f;    //How far away will enemy detect the player
    [Tooltip("How often to detect the player's distance, to determine whether to start the attack pattern. Higher values are less frequent, but more optimized. Find a balance.")]
    public float detectFrequency = 1f;      //HOw often will the enemy detect player distance, higher values for efficiency
	
    [Header("SFX")]
    [Tooltip("Sound to play when projectile is fired")]
    public AudioClip sfx_fireMissile;		//Sound effect for firing the missiles

	private bool playerInRange = false;     //Is the player in range?
	private GameObject player;      //Reference to the Player
	private Vector3 playerPos;      //Player's position
    private bool isCooldown;        //Are we already shooting
    private bool firstShot;     //First shot when player enters range
    private float shootTimer = 0;       //Timer for shooting

	AudioSource audioSource;

	void Awake(){
		//Fill our references
		player = GameObject.FindWithTag("Player");
		audioSource = GetComponent<AudioSource>();
        //If an AudioClip exists, assign it to the AudioSource
        if(sfx_fireMissile != null)
        {
            audioSource.clip = sfx_fireMissile;
        }
        if(projectileSpawnPosition == null)
        {
            Debug.LogWarning("There is no projectile spawn position designated on the Turret");
        }
	}

	// Use this for initialization
	void Start () {
        //Start detecting the player, every specified amount
        InvokeRepeating("DetectPlayer", 0, detectFrequency);
	}

    // Update is called once per frame
    void Update() {
        //Countdown our timers
        UpdateTimers();
        //Track the Player, facing them while in range
        TrackPlayer();

        //Is the player in range and we're currently not shooting?
        if(playerInRange && !isCooldown)
        {
            //Shoot!
            LaunchProjectile();
        }
    }

    /// <summary>
    /// If the player is in range, make sure we are facing the player
    /// </summary>
    void TrackPlayer()
    {
        //if the Player is in range, we want to track the player's movements
        if (playerInRange && tracksPlayer)
        {
            //if we track vertical we need precise location
            if (trackVertical)
            {
                //Get the player's current location
                playerPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            }
            else if (!trackVertical)
            {
                //Get the player's current location without Y value
                playerPos = new Vector3(player.transform.position.x, 1, player.transform.position.z);
            }
            //Track the player
            transform.LookAt(playerPos, Vector3.up);
        }
    }

    /// <summary>
    /// Handle the cooldown timers, counting down where appropriate and resetting if below 0
    /// </summary>
    void UpdateTimers()
    {
        //If we're still cooling down, keep counting
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
            //If we got exactly 0, handle fringe case
            if (shootTimer == 0)
            {
                //Shoot!
                LaunchProjectile();
            }
        }
        //If our timer goes below 0, we're ready to shoot again
        if (shootTimer < 0)
        {
            isCooldown = false; //We are no longer shooting
            shootTimer = 0;     //Set shoot timer to 0
        }
    }

    /// <summary>
    /// Spawn the designated projectile, play a sound and reset the fire rate
    /// </summary>
	void LaunchProjectile(){
		//Instantiate Projectile here
        if(projectileSpawnPosition != null)
        {
            Instantiate(projectileToFire, projectileSpawnPosition.position, transform.rotation);
        }
        else
        {
            return;
        }

        //Play our audio shot
        if(sfx_fireMissile != null)
        {
            audioSource.clip = sfx_fireMissile;     //Assign the sound effect to the AudioSource
            float randomPitch = Random.Range(.90f, 1.1f);
            audioSource.pitch = randomPitch;
            audioSource.Play();		//Play the AudioSource
        }

        shootTimer = fireRate;  //Set the cooldown timer to our fireRate
        isCooldown = true;  //Start shooting the player
	}

    /// <summary>
    /// Detect if the player is within range. We are detecting by distance instead of trigger enter, so that we don't have to deal
    /// with large trigger volumes blocking raycasts
    /// </summary>
    void DetectPlayer()
    {
        //Store the distance between the player this enemy
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);
        //If the player is close enough, flag the boolean to communicate this
        if(playerDistance <= rangeOfAwareness)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
        //Debug.Log("Player Range " + playerDistance);
    }

    void OnDisable()
    {
        //When this object is disabled, make sure you also cancel the Invoke repeating method
        CancelInvoke();
    }
}
