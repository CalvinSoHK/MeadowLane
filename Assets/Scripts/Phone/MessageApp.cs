using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //Our scroll bar
    ScrollRect SCROLL, CONVO_SCROLL;
    float TARGET_NORMALIZEDPOSITION = 1, DAMP_REF;

    //Scroll bar sensitivity
    public float SCROLL_SENSITIVITY = 0.5f;

    //Override base init
    public override void InitializeApp(PlayerPhone _PHONE, PhoneLinker _LINKER)
    {
        //Debug.Log("State of message app: " + STATE);

        //Get the phone and linker
        base.InitializeApp(_PHONE, _LINKER);

        //Spawn the second convo screen on the THIRD SCREEn
        if(CONVERSATION_CONTENT == null)
        {
            Transform TEMP = (Instantiate(CONVO_SCREEN, LINKER.THIRD_SCREEN.position, LINKER.THIRD_SCREEN.rotation, LINKER.THIRD_SCREEN) as GameObject).transform;
            CONVERSATION_CONTENT = TEMP.GetComponent<ReferencePasser>().REF;
            CONVO_SCROLL = CONVERSATION_CONTENT.transform.parent.parent.GetComponent<ScrollRect>();
        }  

        //Populate the contacts list
        PopulateConvoList();

        //Reset index
        INDEX = 0;

        //Get the scroll bar
        SCROLL = transform.Find("Scroll View").GetComponent<ScrollRect>();
    }

    public override void RunApp()
    {
        //No matter what state we're in, if we get a new messagwe should update
        if (TextMessageManager.NewMessageReceived)
        {
            InitializeApp(PHONE,LINKER);
            TextMessageManager.NewMessageReceived = false;
        }

        //When we are manipulating the contact list
        if (STATE == MESSAGE_APP_STATE.ConvoList)
        {
            //Allows for exiting with trigger down
            base.RunApp();

            if (CONVO_ENTRIES != null && CONVO_ENTRIES[0] != null)
            {
                //Allow us to select a contact
                if (PHONE.DOWN && PHONE.PRESS_DOWN)
                {
                    if (INDEX < CONVO_ENTRIES.Count - 1)
                    {
                        INDEX++;
                        Debug.Log(INDEX);
                        //Place the selection on the right index
                        if (CONVO_ENTRIES.Count > 0)
                        {
                            if (CONVO_ENTRIES.Count > 2 && INDEX != 0)
                            {
                                //The total distance between the first and last entry
                                float DISTANCE = Vector3.Distance(CONVO_ENTRIES[0].GetComponent<RectTransform>().anchoredPosition, CONVO_ENTRIES[CONVO_ENTRIES.Count - 1].GetComponent<RectTransform>().anchoredPosition +
                                    new Vector2(0, CONVO_ENTRIES[CONVO_ENTRIES.Count - 1].GetComponent<RectTransform>().rect.height));
                                //float DIST_TO = Vector3.Distance(CONVO_ENTRIES[0].GetComponent<RectTransform>().anchoredPosition, CONVO_ENTRIES[INDEX].GetComponent<RectTransform>().anchoredPosition);
                                float HEIGHT = CONVO_ENTRIES[0].GetComponent<RectTransform>().rect.height + CONVO_LIST.GetComponent<VerticalLayoutGroup>().spacing; //Account for spacing
                                //DISTANCE += (HEIGHT - CONVO_LIST.GetComponent<VerticalLayoutGroup>().spacing);
                                TARGET_NORMALIZEDPOSITION -= HEIGHT / DISTANCE;
                                //Debug.Log("Scroll position: " + SCROLL.verticalNormalizedPosition);
                            }

                            if (INDEX == CONVO_ENTRIES.Count)
                            {
                                Debug.Log("Go to zero.");
                                TARGET_NORMALIZEDPOSITION = 0;
                            }
                        }
                    }
                }
                else if (PHONE.UP && PHONE.PRESS_DOWN)
                {
                    if (INDEX > 0)
                    {
                        INDEX--;
                        Debug.Log(INDEX);
                        //Place the selection on the right index
                        if (CONVO_ENTRIES.Count > 0)
                        {
                            //Debug.Log("Local y position: " + SELECTION.GetComponent<RectTransform>().anchoredPosition.y);
                            //If we have more than two entries, and the selected is not 0 calculate our total distance we have to map each entry to
                            if (CONVO_ENTRIES.Count > 2 && INDEX != 0)
                            {
                                //The total distance between the first and last entry
                                float DISTANCE = Vector3.Distance(CONVO_ENTRIES[0].GetComponent<RectTransform>().anchoredPosition, CONVO_ENTRIES[CONVO_ENTRIES.Count - 1].GetComponent<RectTransform>().anchoredPosition +
                                    new Vector2(0, CONVO_ENTRIES[CONVO_ENTRIES.Count - 1].GetComponent<RectTransform>().rect.height));
                                //float DIST_TO = Vector3.Distance(CONVO_ENTRIES[0].GetComponent<RectTransform>().anchoredPosition, CONVO_ENTRIES[INDEX].GetComponent<RectTransform>().anchoredPosition);
                                float HEIGHT = CONVO_ENTRIES[0].GetComponent<RectTransform>().rect.height + CONVO_LIST.GetComponent<VerticalLayoutGroup>().spacing; //Account for spacing
                                DISTANCE += (HEIGHT - CONVO_LIST.GetComponent<VerticalLayoutGroup>().spacing);
                                TARGET_NORMALIZEDPOSITION += HEIGHT / DISTANCE;
                                //Debug.Log("Scroll position: " + SCROLL.verticalNormalizedPosition);
                            }
                            if (INDEX == 0)
                            {
                                TARGET_NORMALIZEDPOSITION = 1;
                            }
                        }
                    }
                }

                //Smooth damp the scroll view
                SCROLL.verticalNormalizedPosition = Mathf.SmoothDamp(SCROLL.verticalNormalizedPosition, TARGET_NORMALIZEDPOSITION, ref DAMP_REF, SCROLL_SENSITIVITY);

                //If we're close to the position, just set it
                if (Mathf.Abs(SCROLL.verticalNormalizedPosition - TARGET_NORMALIZEDPOSITION) <= 0.005f)
                {
                    SCROLL.verticalNormalizedPosition = TARGET_NORMALIZEDPOSITION;
                }

                if (CONVO_ENTRIES.Count > 0)
                {
                    //Debug.Log("Index: " + INDEX + " Selection: " + SELECTION);
                    //Debug.Log("Problem line: " + CONVO_ENTRIES[INDEX]);
                    //Debug.Log("Object attached to is: " + CONVO_ENTRIES[INDEX].name);
                    SELECTION.GetComponent<RectTransform>().position = CONVO_ENTRIES[INDEX].GetComponent<RectTransform>().position;
                    //SELECTION.SetActive(true);
                }

                //If we press down on the button AND no directional presses were done.
                if (PHONE.PRESS_DOWN && !PHONE.ANY_DIRECTIONAL)
                {
                    //Clear the messages if there are any sitting around
                    if (CONVERSATION_CONTENT.transform.childCount > 0)
                    {
                        //Clear the message screen
                        ClearConversation();

                        //Reset the scroll
                        CONVO_SCROLL.verticalNormalizedPosition = 1;

                        //Reset velocity
                        DAMP_REF = 0;
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
                    TARGET_NORMALIZEDPOSITION = 1;
                    LINKER.TransitionTo(LINKER.THIRD_SCREEN);
                    STATE = MESSAGE_APP_STATE.Conversation;

                    //Mark the current message as viewed
                    CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().SetNotification(false);
                    ConvoInfo TEMP_INFO = new ConvoInfo(CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().TEXT.text, CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().PROFILE_PIC.sprite, CONVO_ENTRIES[INDEX].GetComponent<TextPasser>().MESSAGES);
                    TextMessageManager.MoveConvo(TEMP_INFO);
                }
            }
        }
        else if (STATE == MESSAGE_APP_STATE.Conversation) // WHen we're viewing a conversation
        {
            //Allow for exitinbg back to contact list on trigger down.
            if (PHONE.TRIGGER_DOWN)
            {
                //Go back to message screen
                LINKER.TransitionTo(LINKER.SECOND_SCREEN);
                STATE = MESSAGE_APP_STATE.ConvoList;
            }
            else if (PHONE.HOLD_DOWN)
            {
                if (PHONE.UP)
                {
                    if (TARGET_NORMALIZEDPOSITION < 1)
                    {
                        TARGET_NORMALIZEDPOSITION += 0.05f;
                        if (TARGET_NORMALIZEDPOSITION > 1)
                        {
                            TARGET_NORMALIZEDPOSITION = 1;
                        }
                    }
                }
                else if (PHONE.DOWN)
                {
                    //Debug.Log(TARGET_NORMALIZEDPOSITION);
                    if (TARGET_NORMALIZEDPOSITION > 0)
                    {
                        TARGET_NORMALIZEDPOSITION -= 0.05f;
                        if (TARGET_NORMALIZEDPOSITION < 0)
                        {
                            TARGET_NORMALIZEDPOSITION = 0;
                        }
                    }
                }
            }

            //Smooth damp the scroll view
            CONVO_SCROLL.verticalNormalizedPosition = Mathf.SmoothDamp(CONVO_SCROLL.verticalNormalizedPosition, TARGET_NORMALIZEDPOSITION, ref DAMP_REF, SCROLL_SENSITIVITY);
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
        for(int i = CONVO_ENTRIES.Count-1; i >= 0; i--)
        {
            Destroy(CONVO_ENTRIES[i].gameObject);
        }
        CONVO_ENTRIES.Clear();

        //Get conversations from some data structure
        //Remember to populate from newest to oldest.
        for(int i = TextMessageManager.NewPhoneConversations.Count-1; i >= 0; i--)
        {
            //Create a new entry
            GameObject TEMP = Instantiate(CONVO_ENTRY, CONVO_LIST.transform);
            ConvoInfo INFO = TextMessageManager.NewPhoneConversations[i];

            //Assign all the important variables
            TEMP.GetComponent<TextPasser>().SetNotification(true);
            TEMP.GetComponent<TextPasser>().SetProfile(INFO.PROFILE_PIC_PATH);
            TEMP.GetComponent<TextPasser>().SetText(INFO.CONVO_NAME);
            TEMP.GetComponent<TextPasser>().CopyMessages(INFO.CONVERSATION);
            CONVO_ENTRIES.Add(TEMP);
        }

        //Get old conversations from some data structure
        //Remember to populate from newest to oldest.
        for(int i = TextMessageManager.OldPhoneConversations.Count-1; i >= 0; i--)
        {
            //Create a new entry
            GameObject TEMP = Instantiate(CONVO_ENTRY, CONVO_LIST.transform);
            ConvoInfo INFO = TextMessageManager.OldPhoneConversations[i];

            //Assign all the important variables
            TEMP.GetComponent<TextPasser>().SetNotification(false);
            TEMP.GetComponent<TextPasser>().SetProfile(INFO.PROFILE_PIC_PATH);
            TEMP.GetComponent<TextPasser>().SetText(INFO.CONVO_NAME);
            TEMP.GetComponent<TextPasser>().CopyMessages(INFO.CONVERSATION);
            CONVO_ENTRIES.Add(TEMP);
        }

        if (!SELECTION.activeSelf)
        {
            SELECTION.SetActive(true);
        }

        //Fill our list so we have a reference to each
        /*foreach (TextPasser ENTRY in CONVO_LIST.GetComponentsInChildren<TextPasser>())
        {
            CONVO_ENTRIES.Add(ENTRY.gameObject);         
        }*/
    }
}
