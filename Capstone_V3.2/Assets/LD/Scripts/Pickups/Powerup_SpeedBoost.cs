using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup_SpeedBoost : MonoBehaviour {

    [Header("General Settings")]
    [Tooltip("Amount to add to player max speed")]
    public float speedIncrease = 3f;
    [Tooltip("Duration of speed increase, in seconds")]
    public float speedDuration = 5f;
    [Tooltip("Line Trail ParticleObject")]
    public GameObject poSpeedTrail;

    [Header("SFX")]
    [Tooltip("Sound Effect to play when this powerup is triggered")]
    public AudioClip sfx_Pickup;   //Sound to play when Collected

    GameObject player;          //Reference to player object
    CharacterMotor charMotor;     //Reference to player Motor script

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        charMotor = player.GetComponent<CharacterMotor>();
    }

    void OnTriggerEnter(Collider other)
    {
        //Make sure it's the PLAYER who has entered our trigger
        if (other.CompareTag("Player"))
        {
            charMotor.SpeedMaxChangeTemporary(speedIncrease, speedDuration);   //Add the Health
            //Add visual
            if (poSpeedTrail != null)
            {
                //Spawn a new particle object, parent it and resize
                GameObject po;
                po = Instantiate(poSpeedTrail,player.transform.position, Quaternion.identity, player.transform) as GameObject;
                Destroy(po, speedDuration);
            }
            //If we have an audio clip assigned, play it
            if (sfx_Pickup != null)
            {
                SoundManager.instance.PlaySound2DOneShot(sfx_Pickup, 1f, true);    //Play 2D Sound Effect
            }
            gameObject.SetActive(false);    //Destroy this gameObject because it has been collected
        }
    }
}
