using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages texts that the player recieves
public static class TextMessageManager {

    //
    public static Queue<ConvoInfo> AllPhoneConversations = new Queue<ConvoInfo>(); //Queue that will hold all the current convos on phone



}

//Conversation info object. Stores all lines of dialogue
public class ConvoInfo
{
    //Conversation name that shows up in the list
    public string CONVO_NAME;

    //Path for the profile pic of the image and contact
    public string PROFILE_PIC_PATH;

    //The whole conversation in a list
    //Each entry is a new speech bubble within the app.
    List<string> CONVERSATION = new List<string>();

    //Constructor
    public ConvoInfo(string name, string pp_Path, List<string> messages)
    {
        //Assign the proper variables
        CONVO_NAME = name;
        PROFILE_PIC_PATH = pp_Path;
        
        //Deep copy from the beginning
        foreach(string TEMP in messages)
        {
            CONVERSATION.Add(TEMP);
        }
    }
}


