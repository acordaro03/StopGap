using UnityEngine;
using System.Collections;

/// <summary>
/// This script gives the Bouncer movement pattern to this GameObject. Bouncer enemy will move between given destination points in a circular pattern.
/// </summary>
[RequireComponent(typeof (Rigidbody))]              //This object is moving, it needs a Rigidbody
[RequireComponent(typeof (AudioSource))]            //AudioSource to play from, when we start a new direction
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject
//NOTE: You MUST fill the array with move points, otherwise the script will not work and give you errors

public class Enemy_Bouncer : MonoBehaviour {

    [Header("Movement Settings")]
    [Tooltip("Transform positions to move to. These will be cycled through in order. DO NOT leave an empty array slot, or things will get weird.")]
    public Transform[] destinationPoints;       //End points of travel path
    [Tooltip("Movement speed toward the next destination point")]
    public float moveSpeed = 10f;        //How fast does this enemy move towards the destination
    [Tooltip("Time duration this object will pause after hitting a destination, before moving to the next destination point")]
    public float pauseDuration = 1f;       //How long will the monster wait before moving again
    [Tooltip("Time duration of our check to see if we're stuck, before returning to the previous destination point")]
    public float timeUntilReverse = 2f;      //If this amount of time passes and it hasn't reached destination, switch anyways

    [Header("General Settings")]
    [Tooltip("Do we rotate this object towards the next destination point it's moving to? (Mostly a visual thing)")]
    public bool rotateTowardsDestination = false;     //Should the object rotate towards destination, or retain orientation

    [Header("SFX")]
    [Tooltip("Audio played when starting to move towards a new destination point")]
    public AudioClip sfx_switchDirection;   //Sound made when starting a new direction

    private bool isMoving = false;     //Are we moving towards something?
    private float speedDampener = .001f;      //Multiply the speed by this so that we can work with larger numbers
    private int currentDestinationIndex = 0;     //Where are we currently headed? Initialize towrads first destination
    private float timeUntilReverseTimer;     //Use this to make sure we haven't gotten stuck, return to a previous point
    private Vector3 moveDirection;                      //Direction we want to move

    AudioSource audioSource;
    Rigidbody rb;

    void Awake(){
        //Get References
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        //if an AudioClip exists, assign it to the AudioSource
        if(sfx_switchDirection != null)
        {
            audioSource.clip = sfx_switchDirection;  //Play the attack sound effect into the AudioSource
        }
    }

    void Start()
    {
        //Unparent our destination points, so that they don't move with the enemy
        UnparentDestinationPoints();
        //Start moving towards the current index
        StartMoving();
    }

    private void Update()
    {
        UpdateTimers();
        //Check to see if we're not currently moving and we've come close to our destination
        if(isMoving == true && Vector3.Distance(transform.position, destinationPoints[currentDestinationIndex].position) < .5f)
        {
            StartCoroutine(PauseMovement());
        }

        //If we have, reverse
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            //Move in our precalculated direction towards new point
            rb.MovePosition(transform.position + moveDirection * moveSpeed * speedDampener);
        }
    }

    void UpdateTimers()
    {
        if (timeUntilReverseTimer > 0)
        {
            //still moving, count down the timer
            timeUntilReverseTimer -= Time.deltaTime;
            //Check to see if we just now hit 0
            if (timeUntilReverseTimer == 0)
            {
                GetPreviousDestination();
                StartMoving();
            }
        }
        //if our timer has fallen below 0, deactivate and set to 0
        if (timeUntilReverseTimer < 0)
        {
            GetPreviousDestination();
            StartMoving();
        }
    }

    //Pause movement, then decide where to go next
    IEnumerator PauseMovement()
    {
        StopMoving();
        //Pause for a bit
        yield return new WaitForSeconds(pauseDuration);
        //Find a new destination and start moving again
        GetNextDestination();
        StartMoving();
    }

    //Unparent destination points, otherwise they will move with the enemy due to parenting hierarchy
    void UnparentDestinationPoints()
    {
        //Unparent all of the destination points in our array
        foreach(Transform destination in destinationPoints)
        {
            destination.parent = null;
        }
    }

    //Choose a new destination to move towrads
    void GetNextDestination()
    {
        //Store new destination point
        currentDestinationIndex++;
        //if our array goes out of bounds, return to 0
        if (currentDestinationIndex > destinationPoints.Length - 1)
        {
            currentDestinationIndex = 0;
        }
    }

    //Orient Character and start moving
    void StartMoving()
    {
        //Make sure we haven't flipped upside down
        transform.Rotate(0, transform.rotation.y, 0);
        //Get a new movement direction
        moveDirection = destinationPoints[currentDestinationIndex].position - transform.position;

        //Rotate towards new direction, if we've flagged it
        if (rotateTowardsDestination)
        {
            transform.LookAt(destinationPoints[currentDestinationIndex]);
        }

        //Start Moving
        isMoving = true;

        //If we have an audioSource, play the sound
        if (audioSource != null)
        {
            if (sfx_switchDirection != null)
            {
                audioSource.clip = sfx_switchDirection;     //Assign the sound effect to the AudioSource
                float randomPitch = Random.Range(.90f, 1.1f);
                audioSource.pitch = randomPitch;
                audioSource.Play();		//Play the sound effect
            }
        }

        //Start our Reverse timer, to make sure we don't get caught
        timeUntilReverseTimer = timeUntilReverse;
    }

    void StopMoving()
    {
        //We are no longer moving
        isMoving = false;
        //Stop counting down the reverse timer, we know we're safe now
        timeUntilReverseTimer = 0f;
    }

    //Return to previous location
    void GetPreviousDestination()
    {
        //Store new destination point
        currentDestinationIndex--;
        //if our array goes out of bounds, return to 0
        if (currentDestinationIndex < 0)
        {
            currentDestinationIndex = destinationPoints.Length - 1;
        }
    }
}
