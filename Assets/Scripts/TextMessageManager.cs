using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Manages texts that the player recieves
public static class TextMessageManager {

    //Two stuctures holding our messages.
    //Save both in a text file on quit. 
    public static List<ConvoInfo> NewPhoneConversations = new List<ConvoInfo>(); //List that will hold all the current convos on phone
    public static List<ConvoInfo> OldPhoneConversations = new List<ConvoInfo>(); //List that will hold all old convos on the phone

    //Bool that signifies if we need to update
    public static bool NewMessageReceived = false;

    //Public function that loads in messages into the queue
    //KEY: name/event
    public static void LoadConversation(string KEY)
    {
        //Split the path files we need to use
        string NAME, EVENT;
        string[] TEMP = KEY.Split('/');
        NAME = TEMP[0];
        EVENT = "_" + TEMP[1] + "_";

        //Load from resources
        //Assuming text file is written as:
        /* _Tutorial_
         * Hey there. Use the trigger on the back of the controller to exit this app.
         * Use up and down to blah
         * Use the menu button to exit the phone
         * _Tutorial_
         */
        string MESSAGE_PATH = "Assets/TextFiles/TextMessages/" + NAME + ".txt";
        Sprite PROFILE_PIC = Resources.Load("ProfilePics/" + NAME, typeof(Sprite)) as Sprite;

        List<string> MESSAGES = new List<string>();

        //Parse  through text asset
        using (StreamReader READER = new StreamReader(MESSAGE_PATH))
        {
            //While we aren't on the right line, just keep reading.
            while (!READER.ReadLine().Trim().Equals(EVENT))
            {
                //Does nothing. Parsing text.
            }

            while (true)
            {
                //Keep line reference
                string LINE = READER.ReadLine();

                //As long as we're not at the end of the event, keep adding to list
                if (LINE.Trim().Equals(EVENT))
                {
                    break;
                }
                else
                {
                    MESSAGES.Add(LINE);
                }
            }
        }

        //Finished parsing, make new convoInfo and add to new messages
        ConvoInfo TEMP_CONVO = new ConvoInfo(NAME, PROFILE_PIC, MESSAGES);
        NewPhoneConversations.Add(TEMP_CONVO);
    }

    //Function that will move a new phone convo to the old one
    public static void MoveConvo(ConvoInfo INFO)
    {
        //Find the index of the given info
        int index = -1;
        
        //Go through all of the convos we have in new
        for(int i = 0; i < NewPhoneConversations.Count; i++)
        {
            //If the messages are all the same
            if (NewPhoneConversations[i].Equals(INFO))
            {
                index = i;
            }
        }

        if(index >= 0)
        {
            //Move it over to the next one
            ConvoInfo TEMP = new ConvoInfo(NewPhoneConversations[index].CONVO_NAME, NewPhoneConversations[index].PROFILE_PIC_PATH, NewPhoneConversations[index].CONVERSATION);
            NewPhoneConversations.RemoveAt(index);
            OldPhoneConversations.Add(TEMP);
        }
        else
        {
            Debug.Log("Invalid. Convo is not in new phone conversations.");
        }
    
    }
}

//Conversation info object. Stores all lines of dialogue
public class ConvoInfo
{
    //Conversation name that shows up in the list
    public string CONVO_NAME;

    //Path for the profile pic of the image and contact
    public Sprite PROFILE_PIC_PATH;

    //The whole conversation in a list
    //Each entry is a new speech bubble within the app.
    public List<string> CONVERSATION = new List<string>();

    //Constructor
    public ConvoInfo(string name, Sprite pp_Path, List<string> messages)
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

    public bool Equals(ConvoInfo INFO)
    {
        //If the convo names arent the same we can return false
        if (INFO.CONVO_NAME.Equals(this.CONVO_NAME))
        {
            //If the message count is different we can return false
            if(INFO.CONVERSATION.Count == this.CONVERSATION.Count)
            {
                int index = 0;
                //Once we find a line thats not the same we can return false.
                foreach(string LINE in INFO.CONVERSATION)
                {
                    if (!LINE.Equals(this.CONVERSATION[index]))
                    {
                        return false;
                    }
                    index++;
                }
                return true;
            }
        }
        return false;
    }
}


