using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DisplayDialogue: MonoBehaviour{


    public enum GameState { Wait, Typing, WaitingToProceed, TransitionToShop }
    public GameState currentState;
    float time = 0.0f, lastStateChange;
    bool shopOwner;
    public string[] allSituations;


    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {


        DebugTextParsing();

        switch (currentState)
        {
            case GameState.Wait: //if we don't need it to do anything atm

                break;

            case GameState.Typing: //if dialogue is currently being typed out

                break;

            case GameState.WaitingToProceed: //all dialogue has been typed out. Waiting for player to continue (possible timer)

                break;

            case GameState.TransitionToShop: //If you are talking to a shop owner have it transition to buy items (maybe not necessary)

                break;
        }




    }



    public void DebugTextParsing()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            DialogueManager.setUpCurrentDialogue("Aleksei", "TestingCode", false);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            for (int i = 0; i < DialogueManager.currentDialogueForCharacter.Count; i++)
            {
                Debug.Log(DialogueManager.currentDialogueForCharacter[i]);
            }
        }
    }

    /// <summary>
    /// sets the current state of the game manager
    /// </summary>
    /// <param name="state"></param>
    public void setCurrentState(GameState state)
    {
        //update the state and the time since the state has been changes
        currentState = state;
        lastStateChange = Time.time;
    }

    /// <summary>
    /// returns the amount of time that has passed since the last state change
    /// </summary>
    /// <returns></returns>
    float getStateElapsed()
    {
        return Time.time - lastStateChange; //return time since the last change in state
    }

    
}
