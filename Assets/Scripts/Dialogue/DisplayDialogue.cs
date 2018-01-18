using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DisplayDialogue: MonoBehaviour{

    public string characterName, currentSituation; //strings representing the specific character name and situation for this instance
    public string[] allSituations; //array of all possible instances for that character
    public bool shopOwner; //whether or not they are a shop owner
    public enum GameState { Wait, Idle, DialogueSetup, Typing, WaitingToProceed, TransitionToShop, StopDisplayingText} //statemachine for the displaying of dialogue
    public GameState currentState; //current state in the state machine
    float time = 0.0f, lastStateChange; //references to the amount of time that has passed since last state change
    public GameObject textBox; //reference to the specific textbox for the character
    public Text textObject; //reference to the specific ui textobject associated to the textbox.
    public GameObject blinker; //reference to object shown once line of dialogue is done displaying. Indicates either more or end of dialogue

    private int numberOfLines, indexLine, indexLetter; //private ref to the current line, letter, and total line of dialogue that are needed/currently being displayed
    private string currentLine; //string reference to the current line being displayed
    private bool inDialogue, //checks whether the character is currently in dialogue
        proceed = false,            //Whether or not we should proceed through the text.  
        showBlinker = false;        //Whether or not we should show the blinker at the end of dialogue.
    private float blinkingTimeTrue=.7f, blinkingTimeFalse = .3f;  

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //HAVE A DISTANCE CHECK BETWEEN THE PERSON TALKING AND THE PLAYER

        //DebugTextParsing();

        switch (currentState)
        {
            case GameState.Wait: //if we don't need it to do anything atm

                break;           

            case GameState.DialogueSetup: //Get the correct Dialogue from the dialogue manager based on name and situation.
                DialogueManager.setUpCurrentDialogue(characterName, currentSituation, shopOwner); //Find the lines of dialogue needed for this character and situation instance
                numberOfLines = DialogueManager.currentDialogueForCharacter.Count - 1; //assign the number of lines that are spoken by the character
                indexLine = 0; //reset current line
                indexLetter = 0; //reset current letter
                currentLine = ""; //reset the line of dialogue being displayed
                inDialogue = true; //they are now in dialogue
                textBox.SetActive(true); //display the text box
                setCurrentState(GameState.Typing); //setup up is done, we now need to display the text
                break;

            case GameState.Typing: //if dialogue is currently being typed out
                if(indexLetter < DialogueManager.currentDialogueForCharacter[indexLine].Length) //if the current number of letter displyaed is below the total number of letters in the dialogue line
                {
                    currentLine += DialogueManager.currentDialogueForCharacter[indexLine][indexLetter]; //add the next letter to the current line
                    textObject.text = currentLine;// update the text ui element
                    indexLetter += 1; //move on to the next letter
                }else
                {
                    setCurrentState(GameState.WaitingToProceed); //once all the letters have been displayed, wait for the player to proceed to the next 
                }
                break;

            case GameState.WaitingToProceed: //all dialogue has been typed out. Waiting for player to continue (possible timer)
                if(proceed) //by clicking the mouse, the player is ready to proceed
                {
                    proceed = false;
                    if (indexLine < numberOfLines) //we check if we are at the last line
                    {
                        indexLine += 1; //move to the next line
                        currentLine = ""; //reset the line to be displayed
                        indexLetter = 0; //reset the current letter to add to the line
                        setCurrentState(GameState.Typing); //go back to the typing state
                        showBlinker = false;
                        blinker.SetActive(false);
                        break;
                    }
                    else //otherwise we have reached the last line
                    {
                        setCurrentState(GameState.StopDisplayingText);// go to the stop diplaying text state
                        showBlinker = false;
                        blinker.SetActive(false);
                        break;
                    }
                }

                if(indexLine < numberOfLines) //if the whole line is being displayed and the player has not clicked yet
                {
                    if (showBlinker) //if the blinker is currently being displayed
                    {
                        if (getStateElapsed() > blinkingTimeTrue) //check if the timer has elapsed
                        {
                            determineBlink(showBlinker); //turn off blinker
                        }
                    }
                    else //blinker is not being displayed
                    {
                        if (getStateElapsed() > blinkingTimeFalse) //timer is up
                        {
                            determineBlink(showBlinker); //turn on blinker
                        }
                    }
                }else //if we have reached the last line of dialogue
                {
                    blinker.SetActive(true); //blinker stays continuously on
                }

                break;

            case GameState.StopDisplayingText: //we need to stop displaying text as we have reached the end of the dialogue
                textBox.SetActive(false); //turn off the dialogue box
                textObject.text = "";//remove the text in the text object                
                if (shopOwner && inDialogue) //check if they are shop owner and that the player is still near the character when the conversation ended
                {
                    setCurrentState(GameState.TransitionToShop); //transition to the shop stuff (NOT YET DONE)
                }else //if they are not a shop owner
                {
                    setCurrentState(GameState.Wait);
                }
                inDialogue = false; //they are no longer in dialogue
                DialogueManager.resetCurrentDialogue();
                break;

            case GameState.TransitionToShop: //If you are talking to a shop owner have it transition to buy items (maybe not necessary)

                break;           
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

    /// <summary>
    /// turn on and off dialogue blinker
    /// </summary>
    /// <param name="isBlinking"></param>
    void determineBlink(bool isBlinking)
    {
        if (isBlinking) //if the blinker was on
        {
            showBlinker = false; //turn off the blinker
            blinker.SetActive(false);
        }else //if it was off
        {
            showBlinker = true; //turn is back on
            blinker.SetActive(true);
        }
        lastStateChange = Time.time; //reset the timer
    }

    /// <summary>
    /// Set proceed bool. Proceeds through dialogue.
    /// </summary>
    /// <param name="SET"></param>
    public void SetProceed(bool SET)
    {
        proceed = SET;
    }

    /// <summary>
    /// Getter for indialogue bool.
    /// </summary>
    /// <returns></returns>
    public bool GetInDialogue()
    {
        return inDialogue;
    }

    /*
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
    }*/
}
