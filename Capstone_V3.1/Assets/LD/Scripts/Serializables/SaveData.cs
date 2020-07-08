using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


[Serializable]
public class PlayerData
{
    public int sceneID;     //What scene index we are in

    public int smallCollectibles;   //Number of small Collectibles we've collected
    public int largeCollectibles;   //Number of larger Collectibles we've collected
    public int keys;                //Number of keys we've collected
    public int lives;               //Number of lives we have

    //Store positions as x, y, z floats because Vector3's are not serializable
    public float playerStartX, playerStartY, playerStartZ;
    public float cameraStartX, cameraStartY, cameraStartZ;
}

[Serializable]
public class CollectiblePlaced
{
    public float uID;       //unique ID associated with this Placed Collectible
}

//Collectibles that we'd like to save to our 'collected' list
[Serializable]
public class CollectiblePlacedList
{
    public int sceneID;     //Which scene the Collectibles belong to
    public List<CollectiblePlaced> placedCollectibles;       //A list of all our placed collectibles

    public CollectiblePlacedList(int newSceneID)
    {
        //Initialize our class variables
        this.sceneID = newSceneID;
        this.placedCollectibles = new List<CollectiblePlaced>();
    }
}
