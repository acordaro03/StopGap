using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adding this component to an object flags it to be saved, spawned or deactivated on level load. This is to mimic an Inventory system, you can attach this to 'Collectible' Prefabs
/// if they don't already have it attached.
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class SaveState_Collectible : MonoBehaviour {

    [HideInInspector]
    public float uID;   //Unique ID for identifying this particular GameObject Instance, calculated from position in world

    PlayerManager playerManager;        //We need access to the Player Manager so that we can add this to our list of collectibles stored there

    void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        //Calculate this Object's Unique ID so that we have a number to access for this particular scene object
        uID = transform.position.sqrMagnitude;
    }

    /// <summary>
    /// Collect the thing, creating a unique identification (calculated in Awake). When the scene is loaded, look at uID's we've collected and add it to our list
    /// to not spawn, as we've already collected it.
    /// </summary>
    public void Collect()
    {
        //Collect a dummy collectible item into local inventory, to hold until saved
        CollectiblePlaced collectible = new CollectiblePlaced();
        //Attach this object's uID to the collectible we would like to add
        collectible.uID = uID;
        //Add the collectible to our unsaved list
        playerManager.GetUnsavedListForScene().placedCollectibles.Add(collectible);
    }
    
}
