using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// This script manages the GameState and the PlayerState.
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class GameManager : MonoBehaviour {

    public static GameManager instance;	//Static, only one possible instance of GameManager
    public delegate void SaveDelegate(object sender, System.EventArgs args);
    public static event SaveDelegate SaveEvent;

    [Header("Player State")]
    [Tooltip("List of player data we've saved into the GameState")]
    public PlayerData savedPlayerData = new PlayerData();   //Player info, used for saving and loading
    [Tooltip("List of level object's we've collected. We reference this list to know what objects we should avoid spawning")]
    public List<CollectiblePlacedList> savedLists = new List<CollectiblePlacedList>(); //List of objects in the level we have not collected, to be persistent
    [Tooltip("Current checkpoint we've activated, out of the checkpoint list")]
    public int checkPointIndex = -1;    //Keep track of which checkpoint in the array is active. Start at -1 so that we increase to 0

    [HideInInspector]
    public bool isNewGame = true;    //boolean to keep track of when we have a brand new player

    PlayerManager playerManager;

	//This is called before Start()
	void Awake(){
		//If no GameManager has been assigned yet, this is now our GameManager
		if(instance == null){
			DontDestroyOnLoad(gameObject); 	//This GameManager will not get destroyed between scenes
			instance = this;	//This Object is now our GameManager!
            isNewGame = true;   //This is a new game!
		} 
		//If GameManager (manager) already exists, Don't create a new one
		else if (instance != this){
			Destroy(gameObject);	//Destroy this GameObject, we already have a GameManager
		}
	}

    public void SaveGame()
    {
        //Debug.Log("SAVING...");
        //Save all local Player stats into our Global Stat Manager
        playerManager = FindObjectOfType<PlayerManager>();

        //GameManager.instance.savedPlayerData = playerManager.localPlayerData;
        //TODO find out why the above commented line does not work instead. It constantly matches savedPlayerData to localPlayerData
        //instead of just a single one-time change. THe blow listed assignments are a bandaid for this

        //Save Player Data
        savedPlayerData.sceneID = playerManager.localPlayerData.sceneID;
        savedPlayerData.smallCollectibles = playerManager.localPlayerData.smallCollectibles;
        savedPlayerData.largeCollectibles = playerManager.localPlayerData.largeCollectibles;
        savedPlayerData.keys = playerManager.localPlayerData.keys;

        savedPlayerData.playerStartX = playerManager.localPlayerData.playerStartX;
        savedPlayerData.playerStartY = playerManager.localPlayerData.playerStartY;
        savedPlayerData.playerStartZ = playerManager.localPlayerData.playerStartZ;

        savedPlayerData.cameraStartX = playerManager.localPlayerData.cameraStartX;
        savedPlayerData.cameraStartY = playerManager.localPlayerData.cameraStartY;
        savedPlayerData.cameraStartZ = playerManager.localPlayerData.cameraStartZ;
        //Check our uID spawns to save
        //Save our temporary list of collected items to our permanent
        SavePlayerInventory();
        //Fire off any of the associated save events, if there are any
        FireSaveCollectibleEvent();
    }

    //Copy the player's unsaved list of collected items into our Game Manager permanent list
    public void SavePlayerInventory()
    {
        //Create reference to player's unsaved collected objects list
        playerManager = FindObjectOfType<PlayerManager>();
        //Create a new local list, and fill it with the collectibles from our saved list
        CollectiblePlacedList localList = playerManager.GetUnsavedListForScene();

        //Save and clear the player's Small Collectible List
        //Iterate through all of the objects in the list, adding each one, and then removing it from the list
        CollectiblePlaced collectible = new CollectiblePlaced();
        for (int i=0; i < localList.placedCollectibles.Count; i++)
        {
            //Get a reference to the current object in our unsaved list iteration        
            collectible = localList.placedCollectibles[i];
            //Debug.Log("Saving " + collectible.uID + " into GameManager");
            //Add this object to the GameManager saved list
            GetListForScene().placedCollectibles.Add(collectible);
        }
        //Now that we've iterated through our list, it's time to clear this list. We've passed the items to permanence, no longer need to hold on to them
        playerManager.GetUnsavedListForScene().placedCollectibles.Clear();
    }

    //Fire save event, saving and storing all our Level Objects
    public void FireSaveCollectibleEvent()
    {
        //if we have Events ready to save, execute them
        //Not currently used, but functional
        if (SaveEvent != null)
        {
            SaveEvent(null, null);
        }
    }

    public void LoadGame()
    {
        //Load our previously saved stats into the local manager
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();

        //playerManager.localPlayerData = GameManager.instance.savedPlayerData;
        //TODO find out why the above commented line does not work instead. It constantly matches savedPlayerData to localPlayerData
        //instead of just a single one-time change. THe blow listed assignments are a bandaid for this

        //Load Player Data
        playerManager.localPlayerData.sceneID = savedPlayerData.sceneID;
        playerManager.localPlayerData.smallCollectibles = savedPlayerData.smallCollectibles;
        playerManager.localPlayerData.largeCollectibles = savedPlayerData.largeCollectibles;
        playerManager.localPlayerData.keys = savedPlayerData.keys;
        //playerManager.localPlayerData.lives = savedPlayerData.lives;

        playerManager.localPlayerData.playerStartX = savedPlayerData.playerStartX;
        playerManager.localPlayerData.playerStartY = savedPlayerData.playerStartY;
        playerManager.localPlayerData.playerStartZ = savedPlayerData.playerStartZ;
    }

    //Run once per level, to create initial lists
    public void InitializeSceneList()
    {
        //If our scene list is empty, make a new List
        if(savedLists == null)
        {
            savedLists = new List<CollectiblePlacedList>();
        }

        bool found = false; //Boolean for flagging when we've found the list we're looking for

        //We need to find if we already have a list of saved items for this level:
        for (int i = 0; i < savedLists.Count; i++)
        {
            //Go through our lists until we find a match to our current sceneID
            if (savedLists[i].sceneID == SceneManager.GetActiveScene().buildIndex)
            {
                found = true;   //We found it!
            }
        }

        //If not, we need to create it:
        if (!found)
        {
            //Make a new list and add it to our Save Lists
            CollectiblePlacedList newList = new CollectiblePlacedList(SceneManager.GetActiveScene().buildIndex);
            savedLists.Add(newList);
        }
    }

    //Function that gets the list we need to access
    public CollectiblePlacedList GetListForScene()
    {
        for (int i = 0; i < savedLists.Count; i++)
        {
            if (savedLists[i].sceneID == SceneManager.GetActiveScene().buildIndex)
                return savedLists[i];
        }

        //If the list doesn't exist yet in the scene, return nothing
        return null;
    }

    public void NewGame()
    {
        //Flag our boolean to start a new Game
        isNewGame = true;
        //Clear our saved Lists
        GetListForScene().placedCollectibles = new List<CollectiblePlaced>();
    }
}
