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

    //Saves all the conversations the player has
    public static void SaveData()
    {
        string DATA = "";

        //As long as we have new conversations
        if(NewPhoneConversations.Count > 0)
        {
            //Marker for new convos below
            DATA += "[NEW]\n";

            //Add all the keys to load them in next time
            foreach(ConvoInfo CONVO in NewPhoneConversations)
            {
                DATA += CONVO.CONTACT_NAME + "/" + CONVO.EVENT_NAME +"\n";
            }        
        }

        //As long as we have old conversations
        if(OldPhoneConversations.Count > 0)
        {
            //Marker for old convos below
            DATA += "[OLD]\n";

            //Add all keys to load them in next time
            foreach (ConvoInfo CONVO in OldPhoneConversations)
            {
                DATA += CONVO.CONTACT_NAME + "/" + CONVO.EVENT_NAME + "\n";
            }
        }

        //If for some reason there are no messages, save an empty in its place.
        if(DATA.Length == 0)
        {
            DATA = "EMPTY\n";
        }

        SaveSystem.SaveTo(SaveSystem.SaveType.Messages, "/Messages\n" + DATA + "/");
    }

    public static void LoadData(string DATA)
    {
        string[] INPUT = DATA.Split('\n');
        int i = 0;

        //Length is less one because the less index in the array is blank since every line ends in \n.
        for(; i < INPUT.Length - 1; i++)
        {
            if (INPUT[i].Equals("[NEW]"))
            {
                i++;
                break;
            }
            else
            {
                LoadConversation(INPUT[i], false);
            }
        }

        //Length is less one because the less index in the array is blank since every line ends in \n.
        for (; i < INPUT.Length - 1; i++)
        {
            LoadConversation(INPUT[i], true);
        }

    }

    //Public function that loads in messages into the queue
    //KEY: name/event
    public static void LoadConversation(string KEY, bool isNew)
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
        TextAsset MESSAGE = Resources.Load("TextAssets/TextMessages/" + NAME, typeof(TextAsset)) as TextAsset;
        Sprite PROFILE_PIC = Resources.Load("ProfilePics/" + NAME, typeof(Sprite)) as Sprite;

        List<string> MESSAGES = new List<string>();

        //Parse  through text asset
        string[] TEMP_MESSAGE_ARRAY = MESSAGE.text.Split('\n');
        int index = -1, i = 0;
        for(; i < TEMP_MESSAGE_ARRAY.Length; i++)
        {
            //Debug.Log(TEMP_MESSAGE_ARRAY[i]);
            if (TEMP_MESSAGE_ARRAY[i].Trim().Equals(EVENT))
            {
                index = i + 1;
                break;
            }
        }

        for(i = index; i < TEMP_MESSAGE_ARRAY.Length; i++)
        {
            //Debug.Log(TEMP_MESSAGE_ARRAY[i]);
            if (!TEMP_MESSAGE_ARRAY[i].Trim().Equals(EVENT))
            {
                MESSAGES.Add(TEMP_MESSAGE_ARRAY[i]);
            }
            else
            {
                break;
            }
        }

        //Finished parsing, make new convoInfo and add to new messages
        ConvoInfo TEMP_CONVO = new ConvoInfo(NAME, TEMP[1], PROFILE_PIC, MESSAGES);
        if (isNew)
        {
            NewPhoneConversations.Add(TEMP_CONVO);
        }
        else
        {
            OldPhoneConversations.Add(TEMP_CONVO);
        }
      
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
            ConvoInfo TEMP = new ConvoInfo(NewPhoneConversations[index].CONTACT_NAME, NewPhoneConversations[index].EVENT_NAME, NewPhoneConversations[index].PROFILE_PIC_PATH, NewPhoneConversations[index].CONVERSATION);
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
    public string CONTACT_NAME;

    //Event name
    public string EVENT_NAME;

    //Path for the profile pic of the image and contact
    public Sprite PROFILE_PIC_PATH;

    //The whole conversation in a list
    //Each entry is a new speech bubble within the app.
    public List<string> CONVERSATION = new List<string>();

    //Constructor
    public ConvoInfo(string name, string temp_event, Sprite pp_Path, List<string> messages)
    {
        //Assign the proper variables
        CONTACT_NAME = name;
        EVENT_NAME = temp_event;
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
        if (INFO.CONTACT_NAME.Equals(this.CONTACT_NAME))
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


