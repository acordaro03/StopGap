using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    [Header("Respawn Settings")]
    [Tooltip("Time until respawn after player death")]
    public float respawnTime = 2f;	//Time until Respawn
    [Tooltip("Player camera to keep active during player death animation")]
    public Camera cameraWhileDead;  //Reference to camera, to keep active while player disabled

    [Header("SFX")]
    public AudioClip sfx_respawn;       //Sound effect for respawning
    public AudioClip sfx_lose;  //Sound to play when the player loses
    public AudioClip sfx_win;   //Sound to play when the player wins

    //Collect our spawn objects into arrays for comparing to our saved list of objects to despawn
    private SaveState_Collectible[] collectiblesInScene;

    CheckPoint[] checkPointArray;    //Our array of checkpoints
    UIManager uiManager;
	PlayerManager playerManager;

	void Awake(){
        //Fill references
        if (cameraWhileDead == null)
        {
            cameraWhileDead = Camera.main;
        }
		uiManager = FindObjectOfType<UIManager>();
		playerManager = FindObjectOfType<PlayerManager>();

        //Fill our spawn collectibles arrays
        collectiblesInScene = FindObjectsOfType<SaveState_Collectible>();

        //Find all of the checkpoints in the scene
        checkPointArray = FindObjectsOfType<CheckPoint>();
    }

    void Start()
    {
        //Check to see if this is a new game, if so set player defaults
        if (GameManager.instance.isNewGame)
        {
            //Initialize all the level setup stuff, then flag GameManager for no longer new game
            playerManager.InitializePlayer();   //Initialize the player, get initial position
            DestroyAlreadyCollected();                  //Destroy collectibles, based on our saved Lists of things we've already collected
            GameManager.instance.SaveGame();    //Save the new Initialized Game State
            GameManager.instance.isNewGame = false;       //This is no longer a new game
        }
        else
        {
            //Load Player Stats
            GameManager.instance.LoadGame();
            DestroyAlreadyCollected();                  //Destroy collectibles, based on our saved Lists of things we've already collected

            //Activate Player to begin level, moving to new location
            ActivatePlayer();
        }
        
        //Update the UI to represent our new Player Values
        uiManager.UpdateScore();

        //Get our current checkpoint index from the GameManager, because the information persists

    }

    /// <summary>
    /// Disable objects in this level that are contained within our 'saved despawn list'
    /// </summary>
    void DestroyAlreadyCollected()
    {
        //Check to make sure a list for our current level exists
        GameManager.instance.InitializeSceneList();
        playerManager.InitializeUnsavedSceneList();

        //Create a new local list, and fill it with the collectibles from our saved list
        CollectiblePlacedList localList = GameManager.instance.GetListForScene();

        //Destroy small collectibles
        //Destroy large collectibles
        //Destroy keys
        //Destroy doors

        //If we have things in our list, 
        if (localList != null)
        {
            //Debug.Log("DESTRUCTION!");
            //Search and compare Small Collectibles
            //Go through each object in our scene, with the saveState script attached
            for (int i = 0; i < collectiblesInScene.Length; i++)
            {
                //Debug.Log("Checking collectible - " + collectiblesInScene[i].uID);
                //Iterate through our saved uID's, comparing to the one we're checking in the world
                for(int j = 0; j < localList.placedCollectibles.Count; j++)
                {
                    //Debug.Log("Comparing collectible - " + localList.placedCollectibles[j].uID);
                    //If we find a Unique ID match between our saved list and our spawn objects list, Despawn the object
                    if (collectiblesInScene[i].uID == localList.placedCollectibles[j].uID)
                    {
                        //Debug.Log("It's a match! Destroy - " + collectiblesInScene[i].uID);
                        //It's a match! despawn the object
                        collectiblesInScene[i].gameObject.SetActive(false);
                        //Quit comparing this scene collectible, move to the next
                        break;  
                    }
                }
            }

            //Object Destruction completed
        } 
    }

    /// <summary>
    /// Move player to the previously stored checkpoint position
    /// </summary>
    public void ActivatePlayer()
    {
        //Move the player to the spawnLocation 
        float posX = playerManager.localPlayerData.playerStartX;
        float posY = playerManager.localPlayerData.playerStartY;
        float posZ = playerManager.localPlayerData.playerStartZ;
        playerManager.player.transform.position = new Vector3(posX, posY, posZ);

        //Start particle Effects One Shot
        //Play Spawn Sound Effect
        SoundManager.instance.PlaySound2DOneShot(sfx_respawn, 1f, false);
    }

    /// <summary>
    /// Save a new spawn location into the player data, so that we may now respawn here
    /// </summary>
    /// <param name="newSpawnLocation"></param>
    public void UpdateSpawnLocation(Vector3 newSpawnLocation)
    {
        //The player will now respawn at the new location, store these values
        playerManager.localPlayerData.playerStartX = newSpawnLocation.x;
        playerManager.localPlayerData.playerStartY = newSpawnLocation.y;
        playerManager.localPlayerData.playerStartZ = newSpawnLocation.z;
    }

    /// <summary>
    /// Start the Respawn Coroutine sequence
    /// </summary>
    public void Respawn()
    {
        //Reload Scene
        StartCoroutine("IERespawn");
    }

    /// <summary>
    /// Dsiable the player, wait for respawn time, then reload the scene
    /// </summary>
    /// <returns></returns>
    IEnumerator IERespawn()
    {
        //DisablePlayer and activate DeathCam
        DisablePlayer();

        //Wait a few seconds for player to digest the death
        yield return new WaitForSeconds(respawnTime);

        //Reload level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Disable the player object while retaining the camera (in case it is a child of the player)
    /// </summary>
    void DisablePlayer()
    {
        //Unparent the camera so that it does not get disabled with the player object
        cameraWhileDead.gameObject.transform.parent = null;
        //Disable the player, so they can no longer move and we no longer accept collisions
        playerManager.player.SetActive(false);
        //Reactivate camera, just in case camera was disabled with player through hierarchy
        cameraWhileDead.gameObject.SetActive(true);
    }

    /// <summary>
    /// Activate the win sequence, and start a new game
    /// </summary>
    public void WinGame()
    {
        //DisablePlayer and activate DeathCam
        DisablePlayer();

        uiManager.DisplayWinScreen();   //Display the win screen
        SoundManager.instance.PlaySound2DOneShot(sfx_win, 1f, false);       //Play the "Win" audio

        //Tell our GameManager to use a new game setup
        GameManager.instance.NewGame();
    }

    /// <summary>
    /// Disable the player, activate lose screen and start a new game
    /// </summary>
    public void LoseGame()
    {
        //DisablePlayer and activate DeathCam
        DisablePlayer();

        uiManager.DisplayLoseScreen();  //Display the lose screen
        SoundManager.instance.PlaySound2DOneShot(sfx_lose, 1f, false);  //Play the "Lose" audio

        //Tell our GameManager to use a new game setup
        GameManager.instance.NewGame();
    }

    /// <summary>
    /// Reload from last checkpoint without saving
    /// </summary>
    public void ResetCheckpoint()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Exit the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();			//Quit the game (does not work in editor)
    }

    /// <summary>
    /// Spawn at the next checkpoint in the array. This is helpful for debugging/playtesting
    /// </summary>
    public void AdvanceCheckpoint()
    {
        //If there are no checkpoints in the scene, don't continue
        if (checkPointArray.Length == 0)
        {
            return;
        }

        //If our array is less than 0, set the next increase to 0
        if (GameManager.instance.checkPointIndex < 0)
        {
            GameManager.instance.checkPointIndex = 0;
        }
        //If we're at the end of our index, reset array index to 0
        else if (GameManager.instance.checkPointIndex >= checkPointArray.Length - 1)
        {
            GameManager.instance.checkPointIndex = 0;
        }
        //Otherwise move up the index
        else
        {
            GameManager.instance.checkPointIndex++;
        }
        //TODO WHY does it spawn at wrong location on initial checkpoint advance, even though the index is correct???
        //Debug.Log("CheckPointIndex " + GameManager.instance.checkPointIndex);
        //Update Spawn Location
        Vector3 newLocation = checkPointArray[GameManager.instance.checkPointIndex].gameObject.transform.position;
        UpdateSpawnLocation(newLocation);
        //Save our new Location for when we respawn
        GameManager.instance.SaveGame();
    }

    /// <summary>
    /// Spawn at the previous checkpoint in the checkpoint array
    /// </summary>
    public void ReverseCheckPoint()
    {
        
        //If there are no checkpoints in the scene, don't continue
        if (checkPointArray.Length == 0)
        {
            return;
        }

        //If we're at the first index of the array, instead jump to the end
        if (GameManager.instance.checkPointIndex <= 0)
        {
            GameManager.instance.checkPointIndex = checkPointArray.Length-1;
        }
        //Otherwise move down the index
        else
        {
            GameManager.instance.checkPointIndex--;
        }
        //Debug.Log("CheckPointIndex " + GameManager.instance.checkPointIndex);
        //Update Spawn Location
        Vector3 newLocation = checkPointArray[GameManager.instance.checkPointIndex].gameObject.transform.position;
        UpdateSpawnLocation(newLocation);
        //Save our new Location for when we respawn
        GameManager.instance.SaveGame();
    }
}
