using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Messaging app
public class MessageApp : BasicApp {

    //References for type of messages
    public GameObject LEFT_MESSAGE, LEFT_MESSAGE_NOPIC, RIGHT_MESSAGE, RIGHT_MESSAGE_NOPIC, CONVO_ENTRY;

    //Reference for where to add messages
    public GameObject CONVO_LIST, CONVERSATION_CONTENT;

    //Selection object
    public GameObject SELECTION;
    int INDEX = 0; //Index of where in our contact entries list our selection is marking.

    //Message app convo screen, to be loaded in the next screen
    public GameObject CONVO_SCREEN;

    //State of this application
    public enum MESSAGE_APP_STATE {ConvoList, Conversation, Transition};
    public MESSAGE_APP_STATE STATE = MESSAGE_APP_STATE.ConvoList;

    //Our contact list
    List<GameObject> CONVO_ENTRIES = new List<GameObject>();

    //Override base init
    public override void InitializeApp(PlayerPhone _PHONE, PhoneLinker _LINKER)
    {
        //Get the phone and linker
        base.InitializeApp(_PHONE, _LINKER);

        //Spawn the second convo screen on the THIRD SCREEn
        Transform TEMP = (Instantiate(CONVO_SCREEN, LINKER.THIRD_SCREEN.position, LINKER.THIRD_SCREEN.rotation, LINKER.THIRD_SCREEN) as GameObject).transform;
        CONVERSATION_CONTENT = TEMP.GetComponent<ReferencePasser>().REF;

        //Populate the contacts list
        PopulateConvoList();

        //Reset index
        INDEX = 0;
    }

    public override void RunApp()
    {
        //When we are manipulating the contact list
        if(STATE == MESSAGE_APP_STATE.ConvoList)
        {
            //Allows for exiting with trigger down
            base.RunApp();

            //Allow us to select a contact
            if (PHONE.DOWN && PHONE.PRESS_DOWN)
            {
                if(INDEX < CONVO_ENTRIES.Count - 1)
                {
                    INDEX++;
                }
            }
            else if (PHONE.UP && PHONE.PRESS_DOWN)
            {
                if(INDEX > 0)
                {
                    INDEX--;
                }
            }

            //Place the selection on the right index
            if(CONVO_ENTRIES.Count > 0)
            {
                SELECTION.transform.localPosition = CONVO_ENTRIES[INDEX].transform.localPosition;
                SELECTION.SetActive(true);
                if(SELECTION.transform.localPosition.y < -750f)
                {
                    //Move thing down
                }
            }

            //If we press down on the button AND no directional presses were done.
            if (PHONE.PRESS_DOWN && !PHONE.ANY_DIRECTIONAL)
            {
                //Clear the messages if there are any sitting around
                if (CONVERSATION_CONTENT.transform.childCount > 0)
                {
                    //Clear the message screen
                    ClearConversation();
                }

                //Make the first image with the profile pic
                GameObject TEMP = Instantiate(LEFT_MESSAGE, CONVERSATION_CONTENT.transform);
                TEMP.GetComponent<TextPasser>().SetText(CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().MESSAGES[0]);
                TEMP.GetComponent<TextPasser>().SetProfile(CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().PROFILE_PIC.sprite);

                //Populate convo screen
                bool ignore = true; //Helps us skip the first line
                foreach (string MESSAGE in CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().MESSAGES)
                {
                    if (!ignore)
                    {
                        TEMP = Instantiate(LEFT_MESSAGE_NOPIC, CONVERSATION_CONTENT.transform);
                        TEMP.GetComponent<TextPasser>().SetText(MESSAGE);
                    }
                    else { ignore = false; }        
                }
 
                //Transition to convo screen
                LINKER.TransitionTo(LINKER.THIRD_SCREEN);
                STATE = MESSAGE_APP_STATE.Conversation;

                //Mark the current message as viewed
                CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().SetNotification(false);
                ConvoInfo TEMP_INFO = new ConvoInfo(CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().TEXT.text, CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().PROFILE_PIC.sprite, CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().MESSAGES);
                TextMessageManager.MoveConvo(TEMP_INFO);
            }

        }
        else if(STATE == MESSAGE_APP_STATE.Conversation) // WHen we're viewing a conversation
        {
            //Allow for exitinbg back to contact list on trigger down.
            if (PHONE.TRIGGER_DOWN)
            {
                //Go back to message screen
                LINKER.TransitionTo(LINKER.SECOND_SCREEN);
                STATE = MESSAGE_APP_STATE.ConvoList;
            }
        }
    }

    public void ClearConversation()
    {
        //Get all messages and delete them all
        Transform[] ALL_MESSAGES = CONVERSATION_CONTENT.GetComponentsInChildren<Transform>();
        for(int i = 1; i < ALL_MESSAGES.Length; i++)
        {
            Destroy(ALL_MESSAGES[i].gameObject);
        }
    }

    public void PopulateConvoList()
    {
        //Clear all our contacts if we have any
        for(int i = CONVO_ENTRIES.Count-1; i > 0; i--)
        {
            Destroy(CONVO_ENTRIES[i].gameObject);
        }

        //Get conversations from some data structure
        //Remember to populate from newest to oldest.
        foreach(ConvoInfo INFO in TextMessageManager.NewPhoneConversations)
        {
            //Create a new entry
            GameObject TEMP = Instantiate(CONVO_ENTRY, CONVO_LIST.transform);

            //Assign all the important variables
            TEMP.GetComponent<TextPasser>().SetNotification(true);
            TEMP.GetComponent<TextPasser>().SetProfile(INFO.PROFILE_PIC_PATH);
            TEMP.GetComponent<TextPasser>().SetText(INFO.CONVO_NAME);
            TEMP.GetComponent<TextPasser>().CopyMessages(INFO.CONVERSATION);
        }

        //Get old conversations from some data structure
        //Remember to populate from newest to oldest.
        foreach (ConvoInfo INFO in TextMessageManager.OldPhoneConversations)
        {
            //Create a new entry
            GameObject TEMP = Instantiate(CONVO_ENTRY, CONVO_LIST.transform);

            //Assign all the important variables
            TEMP.GetComponent<TextPasser>().SetNotification(false);
            TEMP.GetComponent<TextPasser>().SetProfile(INFO.PROFILE_PIC_PATH);
            TEMP.GetComponent<TextPasser>().SetText(INFO.CONVO_NAME);
            TEMP.GetComponent<TextPasser>().CopyMessages(INFO.CONVERSATION);
        }

        //Fill our list so we have a reference to each
        bool ignore = true;
        foreach (Transform ENTRY in CONVO_LIST.GetComponentsInChildren<Transform>())
        {
            if (!ignore)
            {
                CONVO_ENTRIES.Add(ENTRY.gameObject);
            }
            else { ignore = false; }
          
        }
    }


}
