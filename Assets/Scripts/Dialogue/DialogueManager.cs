using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public static class DialogueManager{

    public static List<string> currentDialogueForCharacter = new List<string>(); //List that will hold the current dialogue that will be displayed
    public static string[] greetings = { "Hey there! I'm here to pick up a RECIPE please", "Yo! I need a RECIPE ASAP!", "Hello, could I get one RECIPE please",
        "Hey, how is it going? One RECIPE please" };

    static TextAsset CURRENT;

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

        /*Outdated code. Streamreader sucks.
        using (StreamReader reader = new StreamReader("Assets/TextFiles/AllDialogue.txt"))
        {
            while (!reader.ReadLine().Trim().Equals(characterName.Trim()))
            {
                //go through the lines until we reach the right character
            }
            //we should now be at the line in the text file for the character
            while (!reader.ReadLine().Trim().Equals(currentSituation.Trim()))
            {
                //go through the lines within that character until we get to the right situation
            }
            while (true)
            {
                string line = reader.ReadLine();
                if (line.Trim().Equals("_STOP_"))
                {
                    break;
                }
                else
                {
                    currentDialogueForCharacter.Add(line);
                }
            }
        }*/
    }

    public static void setUpCurrentDialogueForShop(string recipe)
    {
        string currentGreeting = greetings[Random.Range(0, greetings.Length)];
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
}
