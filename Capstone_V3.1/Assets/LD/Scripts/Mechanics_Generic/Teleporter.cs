using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Teleporter that warps player who enters to a new position. Does NOT currently support 2 way warping, meaning, don't set a 2nd Teleporter as the destination or else a
/// recursive black hole will result. Set the destination 'near' the next teleporter, just not in it.
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class Teleporter : MonoBehaviour {

    [Header("Teleport Settings")]
    [Tooltip("Destination position to be teleported when entering the teleport volume")]
    public Transform destination;

    [Header("SFX")]
    [Tooltip("Sound clip to play when teleport is activated")]
    public AudioClip sfx_teleport;

    GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Warp();
        }
    }

    /// <summary>
    /// Move the player to the new destination and play a warp sound
    /// </summary>
    void Warp()
    {
        //Move the player to the new destination
        player.transform.position = destination.position;
        //Play a Warp Sound
        if(sfx_teleport != null)
        {
            SoundManager.instance.PlaySound2DOneShot(sfx_teleport, 1f, false);
        }
    }
}
