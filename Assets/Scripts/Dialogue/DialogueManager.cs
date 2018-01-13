using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public static class DialogueManager{

    public static List<string> currentDialogueForCharacter = new List<string>(); //List that will hold the current dialogue that will be displayed

    /// <summary>
    /// Will parse through the dialogue text file to find the appropriate dialogue lines based on the character name and the current situation they are in
    /// </summary>
    /// <param name="characterName"></param>
    /// <param name="currentSituation"></param>
    /// <param name="shopOwner"></param>
    public static void setUpCurrentDialogue(string characterName, string currentSituation, bool shopOwner)
    {
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
        }
    }

    /// <summary>
    /// Clears the character dialogue so we don't double up.
    /// </summary>
    public static void resetCurrentDialogue()
    {
        currentDialogueForCharacter.Clear();
    }
}
