using UnityEngine;
using System.Collections;

/// <summary>
/// This Script will spawn the chosen GameObject at the chosen rate. The Objects spawned must contain their own movement properties
/// </summary>

//TODO Make this work with Object Pooling
public class Hazard_Spawner : MonoBehaviour {

    [Header("Spawn Settings")]
    [Tooltip("GameObject prefab to spawn")]
    public GameObject objectToSpawn;		//What to spawn
    [Tooltip("Where to spawn. If not specified, object will spawn at this GameObject's position")]
    public Transform spawnLocation;		//Where to spawn
    [Tooltip("How often objects are spawned, in seconds")]
    public float spawnRate = 3f;		//How often to spawn
    [Tooltip("Spawned object's lifetime before being destroyed, in seconds")]
    public float spawnLife = 1.5f;      //How long should the spawn live

	void Start(){
		//Error checking, if there is no given spawn location
		if(spawnLocation == null){
			spawnLocation = gameObject.transform;
		}

		//Begin firing on level Load, if we have a spawn object assigned
        if(objectToSpawn != null)
        {
            InvokeRepeating("SpawnObject", spawnRate, spawnRate);
        }
        //If we dont have an object assigned, warn the user
        if(objectToSpawn == null)
        {
            Debug.LogWarning("Hazard Spawner does not have Spawn object assigned");
        }
	}

    /// <summary>
    /// Spawn a new object at the spawn location and flag it to be destroyed in the designated timeframe
    /// </summary>
	void SpawnObject(){
        //Create the instance in which to store our new Spawner
        GameObject newSpawn;
        //Instantiate a new object, at the given transform, and store in a GameObject Instance
        newSpawn = Instantiate(objectToSpawn, spawnLocation.position, transform.rotation) as GameObject;
        //Flag this object to destroy after a certain number of seconds
        Destroy(newSpawn, spawnLife);
	}
}
