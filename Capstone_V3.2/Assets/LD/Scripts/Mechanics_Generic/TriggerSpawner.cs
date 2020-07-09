using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will send an OnTrigger Event that when triggered will spawn the designated objects at the designated positions.
/// NOTE: A trigger collider must also be on this gameObject in order for it to receive the OnTrigger event
/// NOTE: DO NOT leave any of the array indices empty, or you will get an error
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class TriggerSpawner : MonoBehaviour {

    [Header("Spawn Settings")]
    [Tooltip("GameObject Prefabs to be spawned when player enters trigger")]
    public GameObject[] objectsToSpawn;
    [Tooltip("Positions to spawn the above listed gameObjects. Note: Each spawn object will be spawned at the corresponding spawn point index.")]
    public Transform[] spawnLocations;

    [Header("SFX")]
    [Tooltip("Sound effect to play when the objects are spawned")]
    public AudioClip sfx_Spawn;

    void OnTriggerEnter(Collider other)
    {
        //Check to make sure the player was the one who entered
        if (other.CompareTag("Player"))
        {
            //Spawn the things at the given locations
            SpawnObjects();
            //Disable this object, so we don't have repeat spawns
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Go through our 2 arrays, spawning each index object at the index spawn point position. Play the sound effect when done
    /// </summary>
    void SpawnObjects()
    {
        //If either of our arrays are empty, don't try to Instantiate
        if(objectsToSpawn.Length == 0 || spawnLocations.Length == 0)
        {
            Debug.LogWarning("Spawner arrays are empty");
            return;
        }
        //Check to make sure our spawner arrays match
        if(objectsToSpawn.Length != spawnLocations.Length)
        {
            Debug.LogWarning("Our Spawner arrays do not match!");
            return;
        }
        //Our spawner arrays match! Time to spawn some objects
        if(objectsToSpawn.Length == spawnLocations.Length)
        {
            //Go through all the objects, and spawn an object at the current position index
            for(int i=0; i < objectsToSpawn.Length; i++)
            {
                if(objectsToSpawn[i] != null && spawnLocations[i] != null)
                {
                    Instantiate(objectsToSpawn[i], spawnLocations[i].position, spawnLocations[i].rotation);
                }
            }
        }

        //Play Audio for spawning, if it's designated
        if(sfx_Spawn != null)
        {
            SoundManager.instance.PlaySound2DOneShot(sfx_Spawn, 1f, false);
        }
    }


}
