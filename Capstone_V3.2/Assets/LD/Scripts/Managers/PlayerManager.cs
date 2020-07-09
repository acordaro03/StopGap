using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// This script handles all of the player's stats. Put this on the Level Manager object so that we can set player stats on a per-level basis.
/// It also holds the player's inventory/collectible list for the GameManager to access when loading.
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class PlayerManager : MonoBehaviour {

    [Header("General Settings")]
    [Tooltip("Player GameObject. If empty, this script will fill it autmoatically by searching for Player tag")]
    public GameObject player;      //If empty, will assume this is attached to player already
    [Tooltip("Reference to the Main Camera. If empty this script will fiill it automatically by setting it to the current Main camera in camera.Main")]
    public Camera mainCamera;       //If empty, will assume this is the main camera in the scene

    [Header("Health Settings")]
    [Tooltip("Maximum amount of health the player can achieve")]
    public int maxHealth = 10;       //Max health for player
    [Tooltip("Starting health amount at level load. This cannot go above the max health")]
    public int currentHealth = 10;  //Health player starts off with
    [Tooltip("How long is player invulnerable frames after being hit, before they can be hit again")]
    public float hitInvulnerableTime = .2f;    //How long is the player invulnerable while hit

    [Header("Life Settings")]
    [Tooltip("Maximum number of lives the player can achieve")]
    public int maxLives = 3;    //Max number of lives
    [Tooltip("Current number of lives the player starts with at level load. This number cannot go above max lives.")]
    public int startingLives = 3;   //Number of lives the player starts with
    [Tooltip("If true, the player will still lose lives but will not gameOver once passing 0. This is useful for playtesting and challenge metrics")]
    public bool infiniteLives = true;

    [Header("Player Inventory")]
    [Tooltip("List containing player saved data (keys, spawn, etc.)")]
    public PlayerData localPlayerData = new PlayerData();   //Create place to store local Player data, keys collectibles, etc.
    [Tooltip("List containing player unsaved data. This list will be emptied and stored when a checkpoint is reached. This data will be lost if the player dies before reaching a checkpoint")]
    public List<CollectiblePlacedList> unSavedCollectList = new List<CollectiblePlacedList>();  //List of objects in the level we have collected, but not yet saved

    [Header("SFX")]
    [Tooltip("Sound clip to play when the player is damaged")]
    public AudioClip sfx_damagePlayer;		//Sound effect for damaging the player

    [HideInInspector] public bool isInvulnerable = false;	//Is the player invulnerable

    private Vector3 cameraOffset;   //Offset saved for spawning camera
    private bool isDead = false;      //Use this bool to lock out events that happen while player is dead and waiting to respawn

    UIManager uiManager;
	LevelManager levelManager;
    CharacterMotor motor;

    void Awake(){
        //If we haven't specified a player, get it from the Tag
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        //If we haven't specified a main camera, get it from the Static Main Camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
       
        levelManager = FindObjectOfType<LevelManager>();
		uiManager = FindObjectOfType<UIManager>();
        motor = player.GetComponent<CharacterMotor>();
    }

    private void Start()
    {
        //Make sure Player starts off without death state
        isDead = false;
    }

    /// <summary>
    /// Add lives to the player by adding to the current amount and updating the UI to display it
    /// </summary>
    /// <param name="numberOfLivesAdded"></param>
	public void AddLife(int numberOfLivesAdded){
		//Add the lives to the current lives
        //Also add to global lives, to carry over data on scene load
        GameManager.instance.savedPlayerData.lives += numberOfLivesAdded;
		//If we've added more than the max number of lives, reduce down to the max
		if(GameManager.instance.savedPlayerData.lives > maxLives){
			GameManager.instance.savedPlayerData.lives = maxLives;	//Reduce lives to max
		}
		uiManager.UpdateScore();    //We've updated logic, now update our display
	}

	/// <summary>
    /// When the player gets hit, apply damage, pushback, stun and impact location for force direction calculations
    /// </summary>
    /// <param name="damageTaken"></param>
    /// <param name="pushBack"></param>
    /// <param name="stunTime"></param>
    /// <param name="impactLocation"></param>
    //TODO change the playerHit event to happen on the weapon not the 'hit' level, so that we can make different weapons with different damage/pushbacks/stun amounts in the future
	public void PlayerHit(int damageTaken, float pushBack, float stunTime, Vector3 impactLocation){
		//If we're not invulnerable, take damage
		if(!isInvulnerable){
 
            StartCoroutine("MakeInvulnerable", hitInvulnerableTime);      //We just got hit, give the player invulnerability frames
            //Apply a force on the player
            PushBack(pushBack, impactLocation);
            //Apply a stun on the player
            StartCoroutine("Stun", stunTime);
            //Apply damage
            currentHealth -= damageTaken;

            //Feedback
            uiManager.FlashScreen(Color.red, .25f);                                         
            SoundManager.instance.PlaySound2DOneShot(sfx_damagePlayer, 1f, true);		//Play Damaged sound effect
            uiManager.UpdateHealth();                                                   //Update our health GUI
            
            //if we've just hit 0, and player hasn't entered death state yet, lose a life
            if (IsHealthZero() && !isDead)                                                     
            {
                LoseLife(1);	//Subtract a life
                //They are now dead
                isDead = true;
            }
		}
	}

    //Call this function to detect whether player has run out of health
    public bool IsHealthZero()
    {
        if (currentHealth <= 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    /// <summary>
    /// Apply a force to the player, knocking them backwards from the impact location
    /// </summary>
    /// <param name="pushAmount"></param>
    /// <param name="impactLocation"></param>
    public void PushBack(float pushAmount, Vector3 impactLocation)
    {
        //If we're not invulnerable, calculuate and apply the KnockBack force
        Vector3 pushDirection = player.transform.position - impactLocation;
        //motor.SetVelocity(-player.transform.forward * pushAmount);
        motor.SetVelocity(pushDirection * pushAmount);
    }

    /// <summary>
    /// Temporarily disable player controls while they are stunned
    /// </summary>
    /// <param name="stunLength"></param>
    /// <returns></returns>
    IEnumerator Stun(float stunLength)
    {
        //Disable player movement
        motor.canControl = false;
        //Disable player Attacks
      
        //Wait designated time
        yield return new WaitForSeconds(stunLength);
        //Enable player movement
        motor.canControl = true;
    }

    /// <summary>
    /// Heal the player a specified amount, adding it to the total (but not going over the max). Update the UI
    /// </summary>
    /// <param name="healAmount"></param>
    public void AddHealth(int healAmount)
    {
        //Heal the player
        currentHealth += healAmount;
        //Make sure we haven't gone over our max health
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        //Update our GUI
        uiManager.UpdateHealth();
    }

    /// <summary>
    /// Subtrack a player life from the current life pool. Play a sound and update the UI. If lives have reached 0, respawn the player.
    /// </summary>
    /// <param name="numberOfLivesLost"></param>
	public void LoseLife(int numberOfLivesLost){
        //Subtract lives and check to see if Game State has changed. 
        //We also do this globally because the previous checkpoint values will reset, and we need to save player death state but not other values
        GameManager.instance.savedPlayerData.lives -= numberOfLivesLost;
        //Update the UI
        uiManager.UpdateScore();

        //Play Audio for player damaged
        SoundManager.instance.PlaySound2DOneShot(sfx_damagePlayer, 1f, true);

        //We are now dead, set the state to lock out other events while we pause for respawning
        isDead = true;

        //If we don't have infinite lives, subtract lives and respawn
        if(infiniteLives != true)
        {
            //If we have more than one life left, respawn the player
            if (GameManager.instance.savedPlayerData.lives > 0)
            {
                levelManager.Respawn(); //Respawn Player from last checkpoint
            }
            else
            {
                levelManager.LoseGame();    //You lose!
            }
        }
        //else we have inifinite lives, respawn the player
        else
        {
            levelManager.Respawn();
        }
	}

    /// <summary>
    /// Add temporary invulnerability to the player (mainly used after receiving damage). While invulnerable player will not receive hit events from damaging objects
    /// </summary>
    /// <param name="secondsInvulnerable"></param>
    /// <returns></returns>
	IEnumerator MakeInvulnerable(float secondsInvulnerable){
        //We are now invulnerable
        isInvulnerable = true;
        //Wait for our invulnerability time
        yield return new WaitForSeconds(secondsInvulnerable);
        //We are no longer invulnerable
        isInvulnerable = false;
	}

    /// <summary>
    /// Initialize player values if none have been previously saved or if we've started a new game
    /// </summary>
    public void InitializePlayer()
    {
        //Get initial position of Player and store it
        localPlayerData.playerStartX = player.transform.position.x;
        localPlayerData.playerStartY = player.transform.position.y;
        localPlayerData.playerStartZ = player.transform.position.z;

        //No collectibles or keys
        localPlayerData.smallCollectibles = 0;
        localPlayerData.largeCollectibles = 0;
        localPlayerData.keys = 0;

        //How many lives do we want to start off with?
        GameManager.instance.savedPlayerData.lives = startingLives;

        uiManager.UpdateScore();    //Update the UI Manager to display our new player Stats
    }

    /// <summary>
    /// Retrieves the list for this scene, to be accessed when deciding what collectibles in this level to spawn
    /// </summary>
    /// <returns></returns>
    public CollectiblePlacedList GetUnsavedListForScene()
    {
        for (int i = 0; i < unSavedCollectList.Count; i++)
        {
            if (unSavedCollectList[i].sceneID == SceneManager.GetActiveScene().buildIndex)
                return unSavedCollectList[i];
        }

        //If the list doesn't exist yet in the scene, return nothing
        return null;
    }

    /// <summary>
    /// Initialize our lists on this level, if they have not previously been accessed
    /// </summary>
    public void InitializeUnsavedSceneList()
    {
        //If our scene list is empty, make a new List
        if (unSavedCollectList == null)
        {
            unSavedCollectList = new List<CollectiblePlacedList>();
        }

        bool found = false; //Boolean for flagging when we've found the list we're looking for

        //We need to find if we already have a list of saved items for this level:
        for (int i = 0; i < unSavedCollectList.Count; i++)
        {
            //Go through our lists until we find a match to our current sceneID
            if (unSavedCollectList[i].sceneID == SceneManager.GetActiveScene().buildIndex)
            {
                found = true;   //We found it!
            }
        }

        //If not, we need to create it:
        if (!found)
        {
            //Make a new list and add it to our Save Lists
            CollectiblePlacedList newList = new CollectiblePlacedList(SceneManager.GetActiveScene().buildIndex);
            unSavedCollectList.Add(newList);
        }
    }
}
