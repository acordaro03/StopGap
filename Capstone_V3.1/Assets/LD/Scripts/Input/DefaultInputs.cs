using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// This script handles all general inputs independent of the character controller. This is also an appropriate place to put Debug commands.
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class DefaultInputs : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode escapeKey = KeyCode.Escape;
    public KeyCode reloadKey = KeyCode.Backspace;
    public KeyCode advanceCheckpointKey = KeyCode.RightBracket;
    public KeyCode previousCheckpointKey = KeyCode.LeftBracket;
    public KeyCode displayInfoKey = KeyCode.Tab;

    LevelManager levelManager;
    UIManager uiManager;

    void Awake()
    {
        //Fill our references
        levelManager = FindObjectOfType<LevelManager>();
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        //If the Cancel button (in Input Manager) is pressed, quit the game
        if (Input.GetKeyUp(escapeKey))
        {		//If the escape button is pressed, do this code block
            levelManager.QuitGame();
        }
        //Reload current life
        if (Input.GetKeyUp(reloadKey))
        {
            //Reload the Level
            levelManager.ResetCheckpoint();
        }
        //Cycle forward through checkpoints
        if (Input.GetKeyUp(advanceCheckpointKey))
        {
            levelManager.AdvanceCheckpoint();
            //levelManager.UpdateSpawnLocation(spawnPosition);
            //Reload the level
            levelManager.ResetCheckpoint();
        }
        //Cycle backwards through checkpoints
        if (Input.GetKeyUp(previousCheckpointKey))
        {
            levelManager.ReverseCheckPoint();
            //levelManager.UpdateSpawnLocation(spawnLocation);
            //Reload the level
            levelManager.ResetCheckpoint();
        }
        //Info Screen inputs
        if (Input.GetKeyDown(displayInfoKey))
        {
            uiManager.InfoScreen(true);
        }
        if (Input.GetKeyUp(displayInfoKey))
        {
            uiManager.InfoScreen(false);
        }
    }
}
