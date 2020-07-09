using UnityEngine;
using System.Collections;

/// <summary>
/// This script gives the Enemy_Crawler movement pattern to a gameObject. This object will now move towards the player in a straight line, if the player is in range
/// </summary>
[RequireComponent(typeof(Rigidbody))]               //This object is moving, it needs a Rigidbody
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class Enemy_Crawler : MonoBehaviour {

    [Header("Movement Settings")]
    [Tooltip("Movement while moving towards the player")]
    public float moveSpeed = 3f;        //How fast does this enemy move towards the player

    [Header("General Settings")]
    [Tooltip("Distance player must be to this object will start moving towards player")]
    public float rangeOfAwareness = 30f;     //How close must the player be to be considered "In Range"
    [Tooltip("How often to detect the player's distance, to determine whether to start the attack pattern. Higher values are less frequent, but more optimized. Find a balance.")]
    public float detectFrequency = 1f;      //How often will the enemy detect player distance, higher values for efficiency

    //TODO public float verticalDetectRange = 1f;      //How far vertical will the enemy detect
    private bool playerInRange;
	private Vector3 playerPos;
    private float speedDampener = .01f;   //Multiply the speed so that we can use more reasonable numbers

    GameObject player;
    Rigidbody rb;

    void Awake()
    {
        //Fill our references
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
    }

	// Use this for initialization
	void Start () {
        //Detect the player according to the detect Frequency
        InvokeRepeating("DetectPlayer", 0, detectFrequency);
	}
	
    void FixedUpdate()
    {
        //if the Player is in range, we want to track the player's movements
        if (playerInRange)
        {

            //Get the player's current location
            playerPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
            //Face the direction we want to charge, so that we can move in forward direction
            transform.LookAt(playerPos);
            //Move in the current direction
            rb.MovePosition(transform.position + transform.forward * moveSpeed * speedDampener);
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
        }
        else
        {
            playerInRange = false;
        }
        //Debug.Log("Player Range " + playerDistance);
    }

    void OnDisable()
    {
        //When this Object is disabled, make sure you also cancel the Invoke repeating method
        CancelInvoke();
    }

}
