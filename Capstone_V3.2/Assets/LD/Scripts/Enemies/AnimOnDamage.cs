using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activates an Animator Component on a specified object when damage is received. TO receive Damage you need both a Trigger Volume and the Enemy Tag on this object
/// </summary>
public class AnimOnDamage : MonoBehaviour {

    [Header("Animation Settings")]

    [Tooltip("Object to Animate (MUST have Animator Component)")]
    public Animator objectToAnimate;  //Object to Animate (Animator MUST be on this GameObject)
    [Tooltip("Name of Activate Trigger on Controller")]
    public string activateTriggerName = "Open"; //Name of the Open Trigger in the Controller
    [Tooltip("Name of Deactivate Trigger on Controller")]
    public string deactivateTriggerName = "Close";   //Name of the Close Trigger in teh Controller

    [Header("General Settings")]
    [Tooltip("Destroys object when Animation is triggered if true. Cutting the bridge rope, blow up the bomb, etc.")]
    public bool destroyOnActivate = false;        //Does the object despawn when triggered? Aka cut the rope
    [Tooltip("Object that we want to change the material of, while activated. Use this if you want to change the color of something else besides this object to show activation (separate statue, etc.)")]
    public GameObject materialObjectToChange;       //This is the Material we will change on activation
    [Tooltip("Change the material to this color")]
    public Color activateColor = Color.blue;    //Color when object is activated
    [Header("SFX")]
    [Tooltip("Audio to play when Animation is activated")]
    public AudioClip sfx_triggerFlip;       //Trigger audio when flipping the switch

    bool isActive = false;
    Material mat;   //Material to change color when activated
    Color initialColor;     //Original color of material to swap back to

    // Use this for initialization
    void Awake()
    {
        if(materialObjectToChange != null)
        {
            mat = materialObjectToChange.GetComponent<Renderer>().material; 
        }       
    }

    void Start()
    {
        //Store our initial color so we can return to it
        initialColor = mat.color;
    }

    public void DamageEnemy(int damageTaken)
    {
        //reverse open and close
        isActive = !isActive;
        if (isActive)
        {
            //If we've specified an object to animate, play the animation
            if(objectToAnimate != null)
            {
                //Play Animation
                objectToAnimate.SetTrigger(activateTriggerName);
            }
            //If we've specified a material to change, change the material
            if(materialObjectToChange != null)
            {
                //Change color of this trigger material
                mat.color = activateColor;
            }
            //If we've specified a sound to play, play the sound
            if(sfx_triggerFlip != null)
            {
                SoundManager.instance.PlaySound2DOneShot(sfx_triggerFlip, 1f, true);
            }
        } else
        {
            //If we've specified an object to animate, play the animation
            if (objectToAnimate != null)
            {
                //Reverse the Animation
                objectToAnimate.SetTrigger(deactivateTriggerName);
            }
            //If we've specified a material to change, change the material
            if (materialObjectToChange != null)
            {
                //Return to original color
                mat.color = initialColor;
            }
            //If we've specified a sound to play, play the sound
            if (sfx_triggerFlip != null)
            {
                SoundManager.instance.PlaySound2DOneShot(sfx_triggerFlip, 1f, true);
            }
        }

        //If this is a one shot object, get rid of the object
        if (destroyOnActivate)
        {
            gameObject.SetActive(false);
        }
    }
}
