using UnityEngine;
using System.Collections;

/// <summary>
/// Move Volume pushes player upwards by applying a force and reducing gravity (if desired). Other Rigidbodies can be pushed as well, though the force must be far greater.
/// NOTE: You MUST apply a trigger collider on this object in order for it to function properly
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class MoveVolume : MonoBehaviour {

    [Header("Move Settings")]
    [Tooltip("Direction we will apply the force (up, down, forward, etc.)")]
    public Vector3 moveDirection = new Vector3(1,0,0);	//direction of force
    [Tooltip("Force strength applied to player")]
    public float playerMoveSpeed = 3f;	//Strength of force on the player
    [Tooltip("Force strength applied to object, if allowNonPlayerObjects is flagged true. NOTE: This value must be significantly higher than playerMoveSpeed values")]
    public float objectMoveSpeed = 700f;   //Strength of force on the object
    [Tooltip("Use this to alter gravity while inside the volume. When applying upward force reducing gravity helps keep the object afloat")]
    public float gravityReduceAmount = 20f;

    [Header("General Settings")]
    [Tooltip("True will apply movement forces to any rigidbody that enters the volume, not just the player. False will only affect the player")]
    public bool allowNonPlayerObjects = false;	//allow non-player rigidbodies
    [Tooltip("True applies an initial force on Trigger Enter. This is useful as it prevents players from entering from the end direction, as it immediately pushes them out (think river rapids)")]
    public bool forceOnEnter = false;   //apply force on enter, this prevents 2-Way entering

    private float initPlayerGravity;    //Player's initial gravity
    private float newPlayerGravity;     //New gravity after we apply our reduction
    private bool initObjectIsGravity;     //Store whether the object is using gravity

    CharacterMotor playerMotor; // Player motor
    Rigidbody rb;

	void Awake(){
        playerMotor = FindObjectOfType<CharacterMotor>();
    }

    void Start()
    {
        //Store our initial gravity, so that we can return to it later
        initPlayerGravity = playerMotor.movement.gravity;
        //If our gravity reduce factor is greater than our initial gravity, match it. We don't want -gravity
        if (gravityReduceAmount > initPlayerGravity)
        {
            gravityReduceAmount = initPlayerGravity;
        }
        //Calculate our new Gravity to change to. We don't want to add/subtract in case of volume overlap
        newPlayerGravity = initPlayerGravity - gravityReduceAmount;
    }

	void OnTriggerEnter(Collider other)
	{
        //If the player enters the trigger, apply our movement
		if(other.tag == "Player")
		{
            //Apply an initial force to the player, if we've flagged it
			if(forceOnEnter){
				playerMotor.movement.velocity = moveDirection * playerMoveSpeed;
			}
            //Change our gravity, if necessary
            playerMotor.movement.gravity = newPlayerGravity;
            playerMotor.SetVelocity(Vector3.zero);
		}		
	}

	void OnTriggerStay(Collider other){

		if(other.tag == "Player"){
			//playerMotor.grounded = false;       //tell char controller not to ground player while in volume
			//Apply force to player
			other.transform.Translate(moveDirection * Time.deltaTime * playerMoveSpeed, Space.World);
		}

		if(other.tag != "Player" && allowNonPlayerObjects == true){
            //if it has a rigidbody, apply the force
            rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(moveDirection * Time.deltaTime * objectMoveSpeed);
            }
		}

	}

	void OnTriggerExit(Collider other)
	{
        //If the player enters the trigger, retore our movement
        if (other.tag == "Player")
        {
            //Change our gravity, if necessary
            playerMotor.movement.gravity = initPlayerGravity;
        }
	}
}
