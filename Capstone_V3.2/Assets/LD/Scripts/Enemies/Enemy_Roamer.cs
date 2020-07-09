using UnityEngine;
using System.Collections;

/// <summary>
/// This script gives the Enemy_Roamer movement pattern to a GameObject. This object will pick a random direction between 4 90* angles. It will then move
/// that direction for the specified amount of time. After moving, it will pause briefly before choosing a new direction and moving again. Distance from
/// start position can be specified so that it does not stray too far (and potentially off of ledges).
/// </summary>

[RequireComponent(typeof (Rigidbody))]              //This object is moving, it needs a Rigidbody
[RequireComponent(typeof(AudioSource))]            //AudioSource to play from, when we start a charge attack
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class Enemy_Roamer : MonoBehaviour {

    [Header("Movement Settings")]
    [Tooltip("Movement speed while moving")]
    public float moveSpeed = 10f;        //How fast does this enemy move towards the player
    [Tooltip("How long to continue movement in direction before stopping")]
    public float moveDuration = 2f;      //How often will the enemy detect player distance, higher values for efficiency
    [Tooltip("How long to pause after moving, before moving again")]
    public float pauseAfterMove = 1f;       //How long will the monster wait before moving again
    [Tooltip("Distance to travel away from starting position. If traveled more than this specified amount, direction will reverse back towards start")]
    public float maxRoamDistance = 30f;        //How far will the enemy roam from its initial location

    [Header("SFX")]
    [Tooltip("Sound to play when beginning to move in a new direction")]
    public AudioClip sfx_switchDirection;   //Sound made when starting a new direction

    private bool isWalking = false;     //Are we moving a direction?
    private Vector3 startPosition;      //Keep track of our start position, so we know if we've roamed too far
    private float speedDampener = .01f;      //Multiply the speed by this so that we can work with larger numbers

    AudioSource audioSource;
    Rigidbody rb;
    GameObject player;

    void Awake(){
        audioSource = GetComponent<AudioSource>();
        //if an AudioClip exists, assign it to the AudioSource
        if(sfx_switchDirection != null)
        {
            audioSource.clip = sfx_switchDirection;  //Play the attack sound effect into the AudioSource
        }


        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //Store our start position to compare our current distance later
        startPosition = transform.position;

        //Start the Coroutine that switches the direction at specified lengths of time
        StartCoroutine("WalkPattern");
    }
	
    void FixedUpdate()
    {
        if (isWalking)
        {
            //Move in the current direction
            rb.MovePosition(transform.position + transform.forward * moveSpeed * speedDampener);
        }
    }

    /// <summary>
    /// Our movement cycle to handle our movement states. States are set here and then handled in Update/FixedUpdate when true
    /// </summary>
    /// <returns></returns>
    IEnumerator WalkPattern()
    {
        //Switch according to the frequency as long as this GameObject is active
        while (true)
        {
            NewDirection();     //Pick a random direction to move in
            isWalking = true;   //We're now walking again
            yield return new WaitForSeconds(moveDuration);
            isWalking = false;  //We're no longer walking
            yield return new WaitForSeconds(pauseAfterMove);
        }
    }

    /// <summary>
    /// //If we haven't roamed too far, choose a new random direction and play a sound. If we have roamed too far, 
    /// reverse direction instead of choosing a new one
    /// </summary>
	void NewDirection ()
	{
        //If we've roamed to far, reverse our direction to move closer to the origin
        float roamDistance = Vector3.Distance(startPosition, transform.position);
        //Debug.Log(roamDistance);
        if(roamDistance > maxRoamDistance)
        {
            //Debug.Log("We've roamed too far!");
            //Reverse our local direction
            Vector3 rot = transform.rotation.eulerAngles;
            rot = new Vector3(rot.x, rot.y + 180, rot.z);
            transform.rotation = Quaternion.Euler(rot);
        }
        else
        {
            RotateRandom();     //Rotate this GameObject a random direction from Y Axis
        }

        //If we have an audioSource, play the sound
        if (audioSource != null)
        {
            if(sfx_switchDirection != null)
            {
                audioSource.clip = sfx_switchDirection;     //Assign the sound effect to the AudioSource
                float randomPitch = Random.Range(.90f, 1.1f);
                audioSource.pitch = randomPitch;
                audioSource.Play();		//Play the sound effect
            }
        }
	}

    /// <summary>
    /// Choose a random direction between north, east, west, south, and rotate that direction
    /// </summary>
    void RotateRandom()
    {
        //our rotation will be applied to our Y Rotate, enemy will move forward
        float randomRotate = 0;
        //Choose a random direction from N,E,S,W
        int directionPicker = Random.Range(0, 4);
        //Choose a random direction from 4 directions
        switch (directionPicker)
        {
            case 0:
                randomRotate = 0;
                break;
            case 1:
                randomRotate = 90;
                break;
            case 2:
                randomRotate = 180;
                break;
            case 3:
                randomRotate = 270;
                break;
        }
        //Rotate the new direction
        transform.Rotate(0, randomRotate, 0);
    }
}
