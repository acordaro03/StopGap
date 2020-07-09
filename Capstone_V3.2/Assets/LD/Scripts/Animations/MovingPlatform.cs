using UnityEngine;
using System.Collections;

/// <summary>
/// This script will animate a platform to move between 2 positions, using movement functions. You may set a destination, as well as specify speed and pause amounts
/// </summary>
[RequireComponent(typeof(Rigidbody))]               //This object is moving, it needs a Rigidbody

public class MovingPlatform : MonoBehaviour {

    [Header("Movement Settings")]
    [Tooltip("Destination transform to move towards")]
    public Transform destinationPoint;
    [Tooltip("Seconds to take to reach destination. Lower numbers means faster movement")]
    public float secondsForOneTrip = 5f;
    [Tooltip("Duration in seconds to pause after reaching destination")]
    public float pauseLength = 1f;

    [Header("General Settings")]
    [Tooltip("True will auto start movement on scene start. Disaable this if you'd like to control this on triggers, through code, etc.")]
    public bool playOnAwake = true;
    [Tooltip("Delay before starting movement, in seconds. Use this to stagger platform timing while keeping the same speed, or to create specific synchronizations")]
    public float startOffset = 0f;

	private bool platformActive = false;        //Activate the platform to start move cycle
    private bool platformMoving = false;        //Used to move platform between pauses
	private float delayTimer = 0f;
	private float platformTimer = 0f;
	private float nextPauseTime = 0f;

    Vector3 startPoint;
    Vector3 endPoint;

    private void Awake()
    {
        startPoint = transform.position;        //Initialize destination transform 1
        //Error check to make sure we have a destination point. If we do fill it, if not make it equal to initial position
        if(destinationPoint != null)
        {
            endPoint = destinationPoint.position;		//Initialize destination transform 2
            
        } else
        {
            Debug.LogWarning("No destination point set on moving platform.");
            endPoint = transform.position;
        }
        
    }

    void Start(){


		nextPauseTime = secondsForOneTrip;		//Make sure that our first pause isn't until the end of 1 trip

		//If playOnAwake == true, Pause for the startOffset then activate
		if(playOnAwake){
			Invoke("ActivatePlatform", startOffset);
		}
	}

	void Update(){
        //if our platform is Active, continue
        if (platformActive)
        {
            //If the platform is moving, make sure we're counting it to the timer
            if (platformMoving)
            {
                platformTimer += Time.deltaTime;        //Increment the platform timer, so that our loop calculates appropriately
            }
            //If it's not moving, make sure we're counting down the delay/pause timer
            if (!platformMoving)
            {
                UpdateDelayTimers();
            }
        }
	}

	void FixedUpdate(){
		//This controls platform movement
		if(platformActive){
			//Bounce the platform back and forth between the 2 points
			GetComponent<Rigidbody>().position = Vector3.Lerp(startPoint, endPoint, Mathf.SmoothStep(0f, 1f, Mathf.PingPong(platformTimer/secondsForOneTrip, 1f)));
			//Check to see if we've reached our next pause time, if so, pause the platform
			if(platformTimer > nextPauseTime){
				nextPauseTime += secondsForOneTrip;		//Pause after one trips time has passed
				PausePlatform(pauseLength);		//Delay the platform for the above specified amount (public variable)
			}
		}
	}

    /// <summary>
    /// Update timers that calculate the platform pause/delay between movements. If timer runs out, initiate movement cycle
    /// </summary>
    void UpdateDelayTimers()
    {
        //If the platform is not moving Count down the delay timer, so that the platform pauses before restarting its journey
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        }
        //If the pause is finished, reset the the delay and prevent unnecessary calculations when below 0
        if (delayTimer <= 0)
        {
            delayTimer = 0;         //Reset delay timer to 0 to maintain clean code
            MovePlatform();     //Reactivate the platform
        }
    }

    /// <summary>
    /// Activate the platform. This activates the entire move/pause cycle, and is used to entirely shut off the platform cycle (inactive, no power, etc.)
    /// </summary>
	void ActivatePlatform(){
		//Change the platform
		platformActive = true;
	}

    /// <summary>
    /// Move the platform between estinations. This tracks whether it's moving during it's move/pause cycle
    /// </summary>
    void MovePlatform()
    {
        //Move platform between destinations
        platformMoving = true;
    }

    /// <summary>
    /// Temporarily pause the platform at the destination, before continuing the movement cycle
    /// </summary>
    /// <param name="secondsPaused"></param>
	void PausePlatform(float secondsPaused){
		//Delay the platform
		delayTimer = secondsPaused;
		platformMoving = false;
	}

}
