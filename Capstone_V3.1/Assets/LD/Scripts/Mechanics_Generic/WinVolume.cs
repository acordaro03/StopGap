using UnityEngine;
using System.Collections;

/// <summary>
/// This script will receive an OnTriggerEnter Event and activate the UI Manager winPanel.
/// Note: Because there are other things that happen when we win the game, we're accessing it through the Level manager, which will enable the Win panel there.
/// Note: We need a Trigger collider on this object in order to receive the event.
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class WinVolume : MonoBehaviour {

    LevelManager levelManager;

    void Awake()
    {
        //Fill our references
        levelManager = FindObjectOfType<LevelManager>();
    }

	void OnTriggerEnter(Collider other){
		//if the player enters the trigger, Win the game!
		if(other.gameObject.tag == "Player"){
			levelManager.WinGame();
		}
	}
}
