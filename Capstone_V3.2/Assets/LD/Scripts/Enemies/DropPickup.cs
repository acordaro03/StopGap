using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This scripts primary function is to Roll a drop chance, and if drop successful, spawn a random object of the itemsToDrop array.
/// Make sure you're only dropping Pickups with this, DONT drop Collectibles. Dropping other things might be ok, just NOT collectibles please for the love of god.
/// </summary>

//TODO convert this into a more robust system, where we can specify drop % chance of each item
public class DropPickup : MonoBehaviour {

    [Header("General Settings")]
    [Tooltip("% chance out of 100 to drop anything")]
    public float dropChance = 30f;          //% chance out of 100 to trigger a 'Drop' state
    [Tooltip("List of objects to be dropped. One will be spawned at random, with equal probability")]
    public GameObject[] itemsToDrop;        //If drop state is triggered on roll, drop one of these objects with equal probability

    //TODO add a sound effect on drop

    /// <summary>
    /// This event gets called in the Receives Damage script. When called it will roll a number to see if there's a drop (% chance out of a hundred)
    /// If the roll is successful, it will have an equal chance to drop any of the specified items
    /// </summary>
    public void RollAndDrop()
    {
        //If there's nothing in our array, skip this function
        if (itemsToDrop.Length != 0)
        {
            //Decide whether an item will drop
            float dropRoll = Random.Range(0, 100);
            //Drop the item, if we rolled well
            if (dropRoll <= dropChance)
            {
                //Decide which item will drop
                int dropPicker = Random.Range(0, itemsToDrop.Length);
                //Create the item at this location
                Instantiate(itemsToDrop[dropPicker], transform.position, Quaternion.identity);
            }
        }  
    }
}
