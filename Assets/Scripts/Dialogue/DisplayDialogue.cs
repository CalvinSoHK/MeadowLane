using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class DisplayDialogue{

    public static List<string> currentDialogueForCharacter = new List<string>();


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
}
