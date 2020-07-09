using UnityEngine;
using System.Collections;

/// <summary>
/// This script will send text to be displayed on the Message Panel contained in the UI manager, when the player enters the volume.
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

//TODO add an option to choose the color of the text displayed
public class TextVolume : MonoBehaviour {
    
    [Header("Text Settings")]
    [Tooltip("Text string we would like to display in the UI panel for the player to see")]
    [TextArea]
	public string textToDisplay = "";	//Message we want to display
    [Tooltip("Time in seconds that the text will remain displayed. If value is 0, text will display until trigger is exited")]
    public float secondsToDisplay = 0;	//How long to display the message

	UIManager uiManager;            //Reference to the UI Manager, so that we can display string through the UI system

	void Awake(){
		uiManager = FindObjectOfType<UIManager>();
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Player"){
			uiManager.DisplayMessage(textToDisplay, secondsToDisplay);
		}
	}

	void OnTriggerExit(Collider other){
		if(other.gameObject.tag == "Player" && secondsToDisplay == 0){
			uiManager.ClearMessage();	//If we exit the message box, clear the message field
		}
	}


}
