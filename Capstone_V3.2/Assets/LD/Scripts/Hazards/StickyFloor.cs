using UnityEngine;
using System.Collections;

/// <summary>
/// This volume will apply a slow (and/or disable jump) when the player enters it. Upon exit, initial speed and jump values will be returned.
/// NOTE: You MUST apply a trigger collider on this object in order for it to function properly
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class StickyFloor : MonoBehaviour {

    [Header("Sticky Settings")]
    [Tooltip("Amount to slow the player")]
    public float slowAmount = 2f;     //How much we slow the player
    [Tooltip("If false, player is unable to jump while in sticky volume. If true jump is unaffected by sticky volume.")]
    public bool jumpWhileSticky = false;    //Can the player jump while in sticky volume?

    private bool jumpDefault;           //Player's default jump state, to track for unsticky
    private float slowAmountAdjusted;   //Use this to store the adjusted speed amount (in case of multiple slows stacking)

	CharacterMotor playerMotor;     //Reference to player Motor script

	void Awake(){
        playerMotor = FindObjectOfType<CharacterMotor>();
    }

	void Start(){
        
		//If the sticky amount is to high, we need to error check and make sure that there's SOME movement left
		if(slowAmount >= playerMotor.movement.maxForwardSpeed){
			slowAmount = playerMotor.movement.maxForwardSpeed - 1;
		}
        
        //Convert to positive number, to make calculations easier
        slowAmount = Mathf.Abs(slowAmount);

        //Store jump default state, so that we're not enabling jump on a controller that is not supposed to jump
        jumpDefault = playerMotor.jumping.enabled;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MakeSticky();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UnSticky();
        }
    }

    /// <summary>
    /// Slow the player by the designated slow amount. Disable jump if specified.
    /// </summary>
    void MakeSticky()
    {
        //Convert speed to an amount that won't take the player below 1; Return positive value so future calculations are straight-forward
        slowAmountAdjusted = Mathf.Abs(playerMotor.NormalizeSpeedChange(-slowAmount));

        //Subtract from the player max Speed;
        playerMotor.SpeedMaxChange(-slowAmountAdjusted);
        Debug.Log(slowAmountAdjusted);

        //If sticky jump disabled AND our character can originally jump, deactivate
        if (!jumpWhileSticky && jumpDefault)
        {
            playerMotor.jumping.enabled = false;
        }
    }

    /// <summary>
    /// Return player to the original amounts. ReEnable jump if previously disabled
    /// </summary>
    void UnSticky()
    {
        playerMotor.movement.isSticky = false;      //Keep track of sticky variable on player
        //Add previously subtracted speed back to the player -> Positive values add
        playerMotor.SpeedMaxChange(slowAmountAdjusted);
        //If sticky jump disabled AND our character can originally jump, reactivate
        if (!jumpWhileSticky && jumpDefault)
        {
            playerMotor.jumping.enabled = true;
        }

    }
}
