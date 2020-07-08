using UnityEngine;
using System.Collections;

/// <summary>
/// This class is a singleton that travels between scenes as a persistent GameObject. This allows us to continue playing audio without cutting off on scene load.
/// Audio can be played using functions on this class. You should play music from this object as well as any generic 2D One Shots that don't require a positional component.
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class SoundManager : MonoBehaviour {

    public static SoundManager instance = null;		//Allows other scripts to access SoundManager functions

    [Header("Sound Settings")]
    [Tooltip("AudioSource that plays the 2D sound effects")]
    public AudioSource efxSource;		//Reference to the sound effects audio source
    [Tooltip("AudioSource that plays the music")]
    public AudioSource musicSource;		//Reference to the music audio source
    [Tooltip("Lower bounds of randomized pitch range")]
    public float lowPitchRange = .95f;		//The lowest a sound effect will be randomly pitched
    [Tooltip("Upper bounds of randomized pitch range")]
    public float highPitchRange = 1.05f;    //The highest a sound effect will be randomly pitched.

    [Header("General Settings")]
    [Tooltip("ParentObject that holds the list of 2D Audio Pool objects")]
    public ObjectPooler audio2DPool;
    [Tooltip("ParentObject that holds the list of 3D Audio Pool objects")]
    public ObjectPooler audio3DPool;

	// Use this for initialization
	void Awake () {
		//SingleTon Pattern!
		//Check if there is already an instance of SoundManager
		if(instance == null){
			//if not, set it to this
			instance = this;
		}
		//If instance already exists:
		else if (instance != this){
			//Destroy this, to enforce our singleton pattern and make sure this is the only instance of SoundManager
			Destroy(gameObject);
		}
		//Set SoundManager to DontDestroyOnLoad, so that it wont be destroyed
		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// Plays the music.
	/// </summary>
	/// <param name="clip">Clip.</param>
	public void PlayMusic(AudioClip clip){
		//Set the clip of our Music audio source to the clip passed in as a parameter
		musicSource.clip = clip;
		//Play the clip
		musicSource.Play();
	}

	/// <summary>
	/// Stops the music
	/// </summary>
	/// <param name="clip">Clip.</param>
	public void StopMusic(AudioClip clip){
		//Stop playing the clip
		musicSource.Stop();
	}

	/// <summary>
	/// Plays a random sound effect from an array, at a variable range
	/// </summary>
	/// <param name="clips">Clips.</param>
	public void RandomizeSFX (params AudioClip[] clips)
	{
		//Generate a random number between 0 and the length of our array of clips passed in.
		int randomIndex = Random.Range(0,clips.Length);
		//Choose a random pitch to play back our clip, between our high and low pitch ranges.
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		//Set the pitch of the audio source to the randomly chosen pitch
		efxSource.pitch = randomPitch;
		//Set the clip to the clip at our randomly chosen index
		efxSource.clip = clips[randomIndex];
		//Play the clip
		efxSource.Play();
	}

	/// <summary>
	/// Places an audio source from the pool at the location, plays sound, then disables
	/// </summary>
	/// <param name="newClip">New clip.</param>
	/// <param name="newVolume">New volume.</param>
	public void PlaySound2DOneShot(AudioClip newClip, float newVolume, bool randomizePitch){
		//Spawn the AudioSource Object
		GameObject audioObject = audio2DPool.GetPooledObject();
		//Grab the AudioSource component reference
		AudioSource audioSource = audioObject.GetComponent<AudioSource>();
		//Enable the audioObject
		audioObject.SetActive(true);
		//load clip into audiosource clip parameter
		audioSource.clip = newClip;
		//Set the new volume
		audioSource.volume = newVolume;

		//If we've set to randomize pitch, do it
		if(randomizePitch){
			//Choose a random pitch to play back our clip, between our high and low pitch ranges.
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);
			//Set the pitch of the audio source to the randomly chosen pitch
			audioSource.pitch = randomPitch;
		}

		//Play the sound One Shot
		audioSource.Play();
		//start Coroutine, to disable the sound when it is finished playing
		StartCoroutine(DisableSoundOnEnd(audioObject, audioSource));
	}

	/// <summary>
	/// Places an audio source from the pool at the location, plays sound, then disables
	/// </summary>
	/// <param name="newClip">New clip.</param>
	/// <param name="newVolume">New volume.</param>
	public void PlaySound3DOneShot(AudioClip newClip, float newVolume, bool randomizePitch, Vector3 pointToPlay){
		//Spawn the AudioSource Object
		GameObject audioObject = audio3DPool.GetPooledObject();
		//Grab the AudioSource component reference
		AudioSource audioSource = audioObject.GetComponent<AudioSource>();
		//Move object to the designated world point
		audioObject.transform.position = pointToPlay;
		//Enable the audioObject
		audioObject.SetActive(true);
		//load clip into audiosource clip parameter
		audioSource.clip = newClip;
		//Set the new volume
		audioSource.volume = newVolume;

		//If we've set to randomize pitch, do it
		if(randomizePitch){
			//Choose a random pitch to play back our clip, between our high and low pitch ranges.
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);
			//Set the pitch of the audio source to the randomly chosen pitch
			audioSource.pitch = randomPitch;
		}

		//Play the sound One Shot
		audioSource.Play();
		//start Coroutine, to disable the sound when it is finished playing
		StartCoroutine(DisableSoundOnEnd(audioObject, audioSource));
	}

    /// <summary>
    /// Waits for the sound to finish before disabling the object
    /// </summary>
    /// <param name="aObject"></param>
    /// <param name="aSource"></param>
    /// <returns></returns>
	IEnumerator DisableSoundOnEnd(GameObject aObject, AudioSource aSource){
		//wait for the length of the audiosource
        if(aSource.clip != null)
        {
            yield return new WaitForSeconds(aSource.clip.length);
            //Disable object when it is finished playing
            aObject.SetActive(false);
        }
        //AudioSource is null, disable the GameOBject automatically
        else
        {
            aObject.SetActive(false);
        }
	}
}
