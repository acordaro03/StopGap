using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script holds all of the UI info needed for the game. It handles updating the UI, as well as enabling/disabling UI windows where appropriate
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class UIManager : MonoBehaviour {

    [HideInInspector]   public bool isNewGame = false;    //Use this to determine when to show the intro screen

    [Header("UI Panels")]
    [Tooltip("Reference to the Panel displayed when the player Wins")]
    public GameObject winPanel;			//This is the Panel displayed when the 
    [Tooltip("Reference to the Panel displayed when the player Loses")]
    public GameObject losePanel;
    [Tooltip("Reference to the Panel displayed when the player holds down the Display input button")]
    public GameObject introPanel;

    [Header("Text References")]
    [Tooltip("Reference to the Text Object that displays the key collectible amount")]
    public Text collectibleKeyText;
    [Tooltip("Reference to the Text Object that displays the small collectible amount")]
    public Text collectibleSmallText;
    [Tooltip("Reference to the Text Object that displays the large collectible amount")]
    public Text collectibleLargeText;
    [Tooltip("Reference to the Text Object that displays the current number of lives")]
    public Text livesText;
    [Tooltip("Reference to the Text Object that displays the current framerate")]
    public Text fpsCounterText;	//Use this for fps
    [Tooltip("Reference to the Text Object that displays text to display on the screen (activated by level objects)")]
    public Text messageText;	//Use this for displaying messages
    [Tooltip("Reference to the Slider Object that displays the health amount converted into a bar display")]
    public Slider healthBar;

    [Header("General Settings")]
    [Tooltip("Enable/Disable the FPS Counter")]
    public bool useFPS;	//Turn this on if you want to show the FPS counter

    //FPS calculation values
    float frameCount = 0f;
	float dt = 0.0f;
	float fps = 0.0f;
	float updateRate = 3.0f;	//3 Updates per sec.

    PlayerManager playerManager;
    ScreenFlash screenFlash;

    void Awake(){
    	playerManager = FindObjectOfType<PlayerManager>();
        screenFlash = GetComponent<ScreenFlash>();
    }

	void Start(){
		winPanel.gameObject.SetActive(false);	//Make sure that the Win Panel is not visible on level load
		losePanel.gameObject.SetActive(false);	//Make sure that the Win Panel is not visible on level load
      //  introPanel.gameObject.SetActive(false); //Disable all of our UI windows at start

		//if useFPS is enabled, enable visuals accordingly
		if(useFPS){
			fpsCounterText.gameObject.SetActive(true);
		} else {
			fpsCounterText.gameObject.SetActive(false);
		}

        //Match Health bar settings to player health settings
        InitializeHealthSlider();
		//Update the UI at the start of the game
		UpdateScore();
        UpdateHealth();
	}

	void Update(){
		//If we have useFPS active, calculate the frames per second and display
		if(useFPS){
			FPSCounter();
		}
	}

    /// <summary>
    /// Calculates current frame rate and displays it
    /// </summary>
	void FPSCounter(){
        //Calculate frames
		frameCount ++;
		dt += Time.deltaTime;
		if(dt > 1.0/updateRate){
			fps = frameCount / dt;
			frameCount = 0;
			dt -= 1.0f/updateRate;
		}
		//Display the FPS
		fpsCounterText.text = "FPS: " + fps.ToString("F2");
	}

	/// <summary>
    /// Update major player UI components. We will frequently call this when receiving a new pickup, losing/gaining health, etc.
    /// </summary>
	public void UpdateScore(){
        collectibleKeyText.text = playerManager.localPlayerData.keys.ToString();
		collectibleSmallText.text = playerManager.localPlayerData.smallCollectibles.ToString();
		collectibleLargeText.text = playerManager.localPlayerData.largeCollectibles.ToString();
		livesText.text = GameManager.instance.savedPlayerData.lives.ToString();
	}

    /// <summary>
    /// Displays the Win Panel
    /// </summary>
	public void DisplayWinScreen(){
		winPanel.gameObject.SetActive(true);
	}

    /// <summary>
    /// Displays the Lose Panel
    /// </summary>
    public void DisplayLoseScreen(){
		losePanel.gameObject.SetActive(true);
	}

    /// <summary>
    /// Set the activate state fo the info panel, containing level information
    /// </summary>
    public void InfoScreen(bool isEnabled)
    {
        introPanel.gameObject.SetActive(isEnabled);
        Debug.Log("fd");
    }

    /// <summary>
    /// Display the string received to the Message panel. This is useful for displaying helpful text in the level. 
    /// Text can be set to be cleared after a designated amount of time
    /// </summary>
    /// <param name="messageToDisplay"></param>
    /// <param name="secondsToDisplay"></param>
	public void DisplayMessage(string messageToDisplay, float secondsToDisplay){

		CancelInvoke();	//Cancel all other timed invokes in process
		//If our display time is greater than 0, display for that amount of time and then clear it
		if(secondsToDisplay == 0) {
			messageText.text = messageToDisplay;	//Display the Message onto the message panel
		}
		else if(secondsToDisplay > 0){
			messageText.text = messageToDisplay;
			Invoke("ClearMessage", secondsToDisplay);
		}
	}

    /// <summary>
    /// Clear the Message panel by passing it an empty string
    /// </summary>
	public void ClearMessage(){
		messageText.text = "";	//Clear the message panel
	}

    /// <summary>
    /// Update the Health slider display
    /// </summary>
    public void UpdateHealth()
    {
        //Convert bar graphic to fraction of health between 0 and 1
        healthBar.value = playerManager.currentHealth;
    }

    /// <summary>
    /// Initialize the Health slider to the player settings
    /// </summary>
    void InitializeHealthSlider()
    {
        healthBar.maxValue = playerManager.maxHealth;   //Make sure our max value on slider matches max health
        healthBar.minValue = 0;     //0 health is dead
    }

    public void FlashScreen(Color colorToFlash, float duration)
    {
        if(screenFlash != null)
        {
            screenFlash.FlashColor(colorToFlash, duration);
        }
    }
}

