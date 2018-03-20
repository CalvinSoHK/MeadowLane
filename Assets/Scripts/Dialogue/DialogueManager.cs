using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public static class DialogueManager{

    public static List<string> currentDialogueForCharacter = new List<string>(); //List that will hold the current dialogue that will be displayed
    public static string[] ShopGreetings = { "Hey there! I'm here to pick up a RECIPE please", "Yo! I need a RECIPE ASAP!", "Hello, could I get one RECIPE please",
        "Hey, how is it going? One RECIPE please" };

    static TextAsset CURRENT;
    public static string Greeting = "_Greetings_", Filler = "_Filler_", NewSection = "_NewSection_";
    public static DisplayDialogue currentDisplayDialogue = null;

    /// <summary>
    /// Will parse through the dialogue text file to find the appropriate dialogue lines based on the character name and the current situation they are in
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="currentSituation"></param>
    /// <param name="shopOwner"></param>
    ///   //Parse  through text asset
   
    public static void setUpCurrentDialogue(string characterName, string currentSituation, bool shopOwner)
    {

        //Example path: TextFiles/CharacterSpeech/Mayor
         CURRENT = Resources.Load("TextAssets/CharacterSpeech/" + characterName, typeof(TextAsset)) as TextAsset;
         string[] TEMP_MESSAGE_ARRAY = CURRENT.text.Split('\n');
         int index = -1, i = 0;
         for(; i<TEMP_MESSAGE_ARRAY.Length; i++)
         {
            //Search for the relevant event. i.e. _Tutorial_
            if (TEMP_MESSAGE_ARRAY[i].Trim().Equals("_" + currentSituation + "_"))
               {
                   index = i + 1;
                   break;
               }
         }

        for(i = index; i<TEMP_MESSAGE_ARRAY.Length; i++)
        {
            //Debug.Log(TEMP_MESSAGE_ARRAY[i]);
            if (!TEMP_MESSAGE_ARRAY[i].Trim().Equals("_" + currentSituation + "_"))
            {
                currentDialogueForCharacter.Add(TEMP_MESSAGE_ARRAY[i]);
            }
            else
            {
                break;
            }
        }        
    }

    /// <summary>
    /// fill in all the relevant dialogue for the current character (including specific event dialogue)
    /// </summary>
    /// <param name="characterDialogue"></param>
    /// <param name="Event"></param>
    public static void setUpCurrentDialogue(DisplayDialogue characterDialogue)
    {
        
        CURRENT = Resources.Load("TextAssets/CharacterSpeech/" + characterDialogue.characterName, typeof(TextAsset)) as TextAsset; //load the right text asset
        string[] TEMP_MESSAGE_ARRAY = CURRENT.text.Split('\n'); //split it by line
        
        GetEventDialogue(characterDialogue, TEMP_MESSAGE_ARRAY);
        

        int index = 0, i = 0; //init both index variables
        for(; i < TEMP_MESSAGE_ARRAY.Length; i++) // go through all the dialogue lines within the text file
        {
            //Debug.Log(TEMP_MESSAGE_ARRAY[i] + "      " + TEMP_MESSAGE_ARRAY[i].Trim().Equals(Greeting.Trim()));
            if (TEMP_MESSAGE_ARRAY[i].Trim().Equals(Greeting.Trim())){ //if the current line is equal to the greeting word breaker
                index = i + 1; //get the index of the next line
                for (; !TEMP_MESSAGE_ARRAY[index].Trim().Equals(Greeting.Trim()); index++) //from the next line, go through each line until we hit the same breaker word
                {
                    //Debug.Log("Adding greeting");
                    characterDialogue.GreetingDialogue.Add(TEMP_MESSAGE_ARRAY[index]); //add each line to the greeting dialogue for that character
                }
                //Debug.Log("Done Adding Greeting" + "       i:" + index);
                i = index + 1; //account for the lines we have now read through already
            }
            if (TEMP_MESSAGE_ARRAY[i].Trim().Equals(Filler.Trim())) // if the current line is equal to the filler word break
            {
                index = i + 1;//get the index of the next line
                int currentFillerIndex = 0; //we are at the first index section of filler dialogue                
                characterDialogue.FillerDialogue.Add(new List<string>()); //init the first list of filler dialogue
                //Debug.Log(i + "        " + index);
                for (; !TEMP_MESSAGE_ARRAY[index].Trim().Equals(Filler.Trim()); index++) //go through each line until we hit the breaker word
                {
                    if (TEMP_MESSAGE_ARRAY[index].Trim().Equals(NewSection.Trim())) //if the line is the new section breaker word
                    {
                        currentFillerIndex += 1; //increase the index section for filler
                        characterDialogue.FillerDialogue.Add(new List<string>()); //init the new list
                    }
                    else
                    {
                        characterDialogue.FillerDialogue[currentFillerIndex].Add(TEMP_MESSAGE_ARRAY[index]); //add the filler to the relevant list
                    }
                }
                i = index + 1; //account for the passed lines
                break; //break out
            }
        }
        currentDisplayDialogue = characterDialogue; //keep a reference of current character talking.
    }

    public static void setUpCurrentDialogueForShop(string recipe)
    {
        string currentGreeting = ShopGreetings[Random.Range(0, ShopGreetings.Length)];
        currentGreeting = currentGreeting.Replace("RECIPE", recipe);
        resetCurrentDialogue();
        currentDialogueForCharacter.Add(currentGreeting);
    }

    /// <summary>
    /// Clears the character dialogue so we don't double up.
    /// </summary>
    public static void resetCurrentDialogue()
    {
        currentDialogueForCharacter.Clear();
    }

    /// <summary>
    /// Finds the character's dialogue relevant to the day's specific event.
    /// </summary>
    /// <param name="characterDialogue"></param>
    /// <param name="eventName"></param>
    /// <param name="dialogue"></param>
    public static void GetEventDialogue(DisplayDialogue characterDialogue, string[] dialogue)
    {
        List<EventInfo> tempEventList = new List<EventInfo>();
        //GameManagerPointer.Instance.EVENT_MANAGER_POINTER.GetCurrentEvent(ref tempEventList);
        characterDialogue.newEvent = true;
        int i = 0, j = 0; //init index variable

        for(; i < tempEventList.Count; i++)
        {

            if (!tempEventList[i].hasOccured)
            {
                for (; j < dialogue.Length; j++)
                {
                    if (dialogue[j].Trim().Equals(tempEventList[i].NAME.Trim()))
                    {
                        j += 1;
                        characterDialogue.EventDialogue.Add(new List<string>());
                        for (; dialogue[j].Trim().Equals(tempEventList[i].NAME.Trim()); j++)
                        {
                            characterDialogue.EventDialogue[characterDialogue.EventDialogue.Count].Add(dialogue[i]);
                        }
                        j = 0;
                        tempEventList[i].hasOccured = true;
                        break;
                    }
                }
            }
            
        }
        /*eventName = "_" + eventName + "_"; //set the event name to correspond to the breaker word within the text file.
        for(; i < dialogue.Length; i++) //go through all the possible dialogue
        {
            if (dialogue[i].Equals(eventName)) //if the current line is equal to the event name
            {
                i += 1; //move the index by 1 so that we are now on the first line of the relevant diaologue
                characterDialogue.EventDialogue.Add(new List<string>()); //initialize the dialogue list
                for (; dialogue[i].Equals(eventName); i++) //go through all the lines until we hit the breaker word again
                {
                    characterDialogue.EventDialogue[characterDialogue.EventDialogue.Count].Add(dialogue[i]); //add each line to the relevant dialogue list
                }
                break; // we got all the lines, we can break out of this
            }
        }*/
    }
}
