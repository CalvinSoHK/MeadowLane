using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Messaging app
public class MessageApp : BasicApp {

    //References for type of messages
    public GameObject LEFT_MESSAGE, LEFT_MESSAGE_NOPIC, RIGHT_MESSAGE, RIGHT_MESSAGE_NOPIC, CONTACT_ENTRY;

    //Reference for where to add messages
    public GameObject CONTACT_LIST, CONVERSATION_CONTENT;

    //Selection object
    public GameObject SELECTION;
    int INDEX = 0; //Index of where in our contact entries list our selection is marking.

    //Message app convo screen, to be loaded in the next screen
    public GameObject CONVO_SCREEN;

    //State of this application
    public enum MESSAGE_APP_STATE {ContactList, Conversation, Transition};
    public MESSAGE_APP_STATE STATE = MESSAGE_APP_STATE.ContactList;

    //Our contact list
    List<GameObject> CONTACT_ENTRIES = new List<GameObject>();

    //Override base init
    public override void InitializeApp(PlayerPhone _PHONE, PhoneLinker _LINKER)
    {
        //Get the phone and linker
        base.InitializeApp(_PHONE, _LINKER);

        //Spawn the second convo screen on the THIRD SCREEn
        Transform TEMP = (Instantiate(CONVO_SCREEN, LINKER.THIRD_SCREEN.position, LINKER.THIRD_SCREEN.rotation, LINKER.THIRD_SCREEN) as GameObject).transform;

        //Populate the contacts list
        PopulateContactList();

        //Enable selection if there are contacts
        if(CONTACT_ENTRIES.Count > 0)
        {
            SELECTION.SetActive(true);
            SELECTION.transform.localPosition = CONTACT_ENTRIES[0].transform.localPosition;
        }

        //Reset index
        INDEX = 0;
    }

    public override void RunApp()
    {
        //When we are manipulating the contact list
        if(STATE == MESSAGE_APP_STATE.ContactList)
        {
            //Allows for exiting with trigger down
            base.RunApp();

            //Allow us to select a contact
            if (PHONE.DOWN && PHONE.PRESS_DOWN)
            {
                if(INDEX < CONTACT_ENTRIES.Count - 1)
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
            if(CONTACT_ENTRIES.Count > 0)
            {
                SELECTION.transform.localPosition = CONTACT_ENTRIES[INDEX].transform.localPosition;
            }
         
            //If we press down on the button AND no directional presses were done.
            if (PHONE.PRESS_DOWN && !PHONE.ANY_DIRECTIONAL)
            {
                //Populate convo screen

                //Transition to convo screen
                LINKER.TransitionTo(LINKER.THIRD_SCREEN);
                STATE = MESSAGE_APP_STATE.Conversation;
            }

        }
        else if(STATE == MESSAGE_APP_STATE.Conversation) // WHen we're viewing a conversation
        {
            //Allow for exitinbg back to contact list on trigger down.
            if (PHONE.TRIGGER_DOWN)
            {
                //Go back to message screen
                LINKER.TransitionTo(LINKER.SECOND_SCREEN);
                STATE = MESSAGE_APP_STATE.ContactList;
            }

        }
    }

    public void PopulateContactList()
    {
        //Get contacts from some data structure

        //Fill our list so we have a reference to each
        foreach (Transform ENTRY in CONTACT_LIST.GetComponentInChildren<Transform>())
        {
            CONTACT_ENTRIES.Add(ENTRY.gameObject);
        }
    }


}
