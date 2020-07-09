using UnityEngine;
using System.Collections;

/// <summary>
/// Activates an animation OnTriggerEnter(). Note that you will need a Trigger on this script, and a Rigidbody on the object entering
/// in order to run this properly. Make sure that the activateTriggerName fields match the Trigger names inside of the corresponding Animator Components.
/// </summary>
public class AnimTrigger : MonoBehaviour {

    [Header("Animation Settings")]

    [Tooltip("Object to Animate (MUST have Animator Component)")]
    public Animator objectToAnimate;	//Object to Animate (Animator MUST be on this GameObject)
    [Tooltip("Name of Activate Trigger on Controller")]
    public string activateTriggerName = "Open";	//Name of the Open Trigger in the Controller
    [Tooltip("Name of Deactivate Trigger on Controller")]
    public string deactivateTriggerName = "Close";	//Name of the Close Trigger in teh Controller

    [Header("General Settings")]
    [Tooltip("True will play a reverse animation, false will just trigger once and then stay (bridge permanently activated, etc.)")]
    public bool reverseOnExit = true;	//Will the animation play in reverse? if not, only triggers once
    [Tooltip("Will other Rigidbodies activate on Trigger Enter?")]
    public bool allowNonPlayerActivate;	//Can non-player Rigidbodies activate the trigger?

    [Header("SFX")]
    [Tooltip("Audio to play when Animation is activated")]
    public AudioClip sfx_activate;       //Trigger audio when flipping the switch
    [Tooltip("Audio to play when Animation is deactivated")]
    public AudioClip sfx_deactivate;       //Trigger audio when flipping the switch

    int objectsPresent = 0;	//Keeps track of the number of objects in the Trigger
	
	void OnTriggerEnter(Collider other){
		//We must make sure that the player has entered, or an entity has entered if we allow it with our boolean
		if(other.gameObject.tag == "Player" || allowNonPlayerActivate == true){
			//If there's nothing in the trigger and something enters, play the animation
			if(objectsPresent == 0){
				//Play Animation
                if(objectToAnimate != null)
                {
                    objectToAnimate.SetTrigger(activateTriggerName);
                }
                //If we've specified a sound to play, play the sound
                if (sfx_activate != null)
                {
                    SoundManager.instance.PlaySound2DOneShot(sfx_activate, 1f, true);
                }
            }
			objectsPresent ++;	//Keep track of the number of objects in the trigger
		}
	}

	void OnTriggerExit(Collider other){
		//If the player exits, or an object exits (if we've allowed it)
		if(other.gameObject.tag == "Player" || allowNonPlayerActivate == true ){
			objectsPresent --;	//An object has left
			//now that we've left, if there's nothing else in the trigger, reverse the animation
			if(objectsPresent == 0 && reverseOnExit){
				//Reverse the animation
                if(objectToAnimate != null)
                {
                    objectToAnimate.SetTrigger(deactivateTriggerName);
                }
                //If we've specified a sound to play, play the sound
                if (sfx_deactivate != null)
                {
                    SoundManager.instance.PlaySound2DOneShot(sfx_deactivate, 1f, true);
                }
            }
		}
	}
}
