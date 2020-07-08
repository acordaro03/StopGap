using UnityEngine;
using System.Collections;

/// <summary>
/// This script will add a force to the player when they enter a volume attached to this object. NOTE: You need a trigger volume on this gameObject
/// in order to receive the OnTriggerEnter() event that adds the force
/// NOTE: You MUST apply a trigger collider on this object in order for it to function properly
/// </summary>

[RequireComponent(typeof(AudioSource))]     //We need an audiosource to play the jump sound effect
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class JumpPad1 : MonoBehaviour {

    [Header("Bounce Settings")]
    [Tooltip("Direction to apply the force. NOTE: This is a global direction, not local. If you rotate this object, make sure the proper direction is set on this instance")]
    public Vector3 forceDirection = new Vector3(0, 15, 0);
    [Tooltip("Strength of the force. This gives us more control once we have the direction defined")]
    public float forceStrength = 1f;

    [Header("SFX")]
    [Tooltip("Sound to play during the bounce")]
    public AudioClip sfx_bounce;

	private CharacterMotor playerMotor; // Player motor

    AudioSource audioSource;

    void Awake(){
		playerMotor = FindObjectOfType<CharacterMotor>();
		audioSource = GetComponent<AudioSource>();	//Find the AudioSource

        if(sfx_bounce != null)
        {
            audioSource.clip = sfx_bounce;		//Add the bounce effect to the clip
        }
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Player"){
			playerMotor.SetVelocity(forceDirection * forceStrength);		//Add a force the player

            if (sfx_bounce != null)
            {
                audioSource.clip = sfx_bounce;     //Assign the sound effect to the AudioSource
                float randomPitch = Random.Range(.90f, 1.1f);
                audioSource.pitch = randomPitch;
                audioSource.Play();		//Play the sound effect
            }
            
		}
	}

}
