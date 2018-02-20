using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Script that handles phone
public class PlayerPhone : MonoBehaviour {

    //Both hands
    public Hand hand1, hand2;

    //Player stats
    public PlayerStats PLAYER_STATS;

    //Where the phone is showing
    public enum ShowState { None, Hand1, Hand2 };
    public ShowState SHOW = ShowState.None;

    //Vibration enum
    public enum VibrationHand { Both, Hand1, Hand2 };

    //Pos offset for the phone on being called
    public Vector3 POS_OFFSET;

    //Whether or not we are currently fading somewhere
    public bool FADE_RUNNING = false;

    //How long the fade takes to work
    public float FADE_TIME;

    //Reference to the actual phone object
    public GameObject PHONE;

    //Bools for the four directions on the phone
    public bool LEFT = false, RIGHT = false, UP = false, DOWN = false, PRESS_DOWN = false, PRESS_UP = false, TRIGGER_DOWN = false, TRIGGER_UP = false, ANY_DIRECTIONAL = false, HOLD_DOWN = false, TRIGGER_HOLD_DOWN = false;

    //Bool to notify the player the next time the player's eyes are open
    public bool VIBRATE_NEXT = false;
    public ScreenTransitionImageEffect STIE;

    //Tutorial script
    public TutorialManager TUT_INFO;
    string TUT_KEY;

    //Notification object
    public GameObject NOTIFICATION_OBJ;

    // Use this for initialization
    void Start() {
        hand1 = transform.GetChild(0).Find("Hand1").GetComponent<Hand>();
        hand2 = transform.GetChild(0).Find("Hand2").GetComponent<Hand>();
        PLAYER_STATS = GetComponent<PlayerStats>();

        LoadConversationNow("Triangle/Start");
    }

    private void Update()
    {
        //Update the directional bools based off of the current hand's trackpad
        if (SHOW == ShowState.Hand1)
        {
            PRESS_DOWN = hand1.GetTrackpadDown();
            PRESS_UP = hand1.GetTrackpadUp();
            LEFT = hand1.GetTrackpadPressLeft();
            RIGHT = hand1.GetTrackpadPressRight();
            UP = hand1.GetTrackpadPressUp();
            DOWN = hand1.GetTrackpadPressDown();
            TRIGGER_UP = hand1.GetStandardInteractionButtonUp();
            TRIGGER_DOWN = hand1.GetStandardInteractionButtonDown();
            TRIGGER_HOLD_DOWN = hand1.GetStandardInteractionButton();
            HOLD_DOWN = hand1.GetTrackpad();

            //Any directional lets us know if any directions were pressed at all.
            if (LEFT || RIGHT || UP || DOWN)
            {
                ANY_DIRECTIONAL = true;
            }
            else
            {
                ANY_DIRECTIONAL = false;
            }
        }
        else if (SHOW == ShowState.Hand2)
        {
            PRESS_DOWN = hand2.GetTrackpadDown();
            PRESS_UP = hand2.GetTrackpadUp();
            LEFT = hand2.GetTrackpadPressLeft();
            RIGHT = hand2.GetTrackpadPressRight();
            UP = hand2.GetTrackpadPressUp();
            DOWN = hand2.GetTrackpadPressDown();
            TRIGGER_UP = hand2.GetStandardInteractionButtonUp();
            TRIGGER_DOWN = hand2.GetStandardInteractionButtonDown();
            TRIGGER_HOLD_DOWN = hand2.GetStandardInteractionButton();
            HOLD_DOWN = hand2.GetTrackpad();

            //Any directional lets us know if any directions were pressed at all.
            if (LEFT || RIGHT || UP || DOWN)
            {
                ANY_DIRECTIONAL = true;
            }
            else
            {
                ANY_DIRECTIONAL = false;
            }
        }

        if (VIBRATE_NEXT)
        {
            if(STIE.isOpen)
            {
                if (VibratePhone(0.5f, 300, VibrationHand.Both))
                {
                    NOTIFICATION_OBJ.SetActive(true);
                    LoadConversation(TUT_KEY);
                    VIBRATE_NEXT = false;
                }           
            }
        }
    }

    //Menu button was pressed, use the phone on the hand that called it. Never turns it off.
    public void UsePhone(Hand hand)
    {
        //If it is not holding something
        if (hand.currentAttachedObject.name == hand.controllerPrefab.name + "_" + hand.name)
        {
            //If we are in the NONE show state, just show the phone on this hand
            if (SHOW == ShowState.None)
            {
                //Init the app if its on an app
                PHONE.GetComponent<PhoneLinker>().InitApp();
                ShowPhone(hand);
                if (hand == hand1)
                {
                    SHOW = ShowState.Hand1;
                }
                else
                {
                    SHOW = ShowState.Hand2;
                }
            }
            else if (SHOW == ShowState.Hand1)
            {
                //If the hand that called it was hand1, just hide the phone
                if (hand != hand1)
                {
                    HidePhone(hand1);
                    ShowPhone(hand);
                    SHOW = ShowState.Hand2;
                }
            }
            else if (SHOW == ShowState.Hand2)
            {
                //If the hand that called it was hand2, just hide the phone
                if (hand != hand2)
                {
                    HidePhone(hand2);
                    ShowPhone(hand1);
                    SHOW = ShowState.Hand1;
                }
            }
        }
    }

    //Helper function to vibrate the phone + controller
    public bool VibratePhone(float DURATION, float STRENGTH)
    {
        if (SHOW == ShowState.Hand1)
        {
            return hand1.TriggerHaptic(DURATION,STRENGTH);
        }
        else if (SHOW == ShowState.Hand2)
        {
            return hand2.TriggerHaptic(DURATION, STRENGTH);
        }
        else
        {
            Debug.Log("ERROR: No hand to vibrate.");
            return false;
        }
    }

    //Override for vibrate to vibrate a certain hand or both
    public bool VibratePhone(float DURATION, float STRENGTH, VibrationHand HAND)
    {
        if (HAND == VibrationHand.Hand1)
        {
            return hand1.TriggerHaptic(DURATION, STRENGTH);
        }
        else if (HAND == VibrationHand.Hand2)
        {
            return hand2.TriggerHaptic(DURATION, STRENGTH);
        }
        else if(HAND== VibrationHand.Both)
        {
            hand1.TriggerHaptic(DURATION, STRENGTH);
            return hand2.TriggerHaptic(DURATION, STRENGTH);
        }
        return false;
    }

    /// <summary>
    /// Notifies the player that they have received something on their phone.
    /// If true, waits till next travel to notify player.
    /// If false, notifies immediately.
    /// </summary>
    /// <param name="WAIT"></param>
    public void NotifyPlayer(bool WAIT)
    {
        if (WAIT)
        {
            //Fire them when vibrate next is viable
            VIBRATE_NEXT = true;
        }
        else
        {
            //All the notification things should fire now
            VibratePhone(0.5f, 300, VibrationHand.Both);
            NOTIFICATION_OBJ.SetActive(true);
        }
    }

    //Load conversation and notify the player
    public void LoadConversationNow(string KEY)
    {
        NotifyPlayer(false);
        LoadConversation(KEY);
    }


    //Load conversation
    public void LoadConversation(string KEY)
    {        
        TextMessageManager.LoadConversation(KEY, true);
        TextMessageManager.NewMessageReceived = true;
    }

    //Function that checks if a tutorial needs to be done 
    public void LoadTutorial(string KEY)
    {
        //Separate our tutorial name from the key
        string[] KEY_SEP = KEY.Split('/');

        //Check if that tutorial has been done yet
        if (!TUT_INFO.IsComplete(KEY_SEP[1]))
        {
            NotifyPlayer(true);
            TUT_KEY = KEY;
            TUT_INFO.SetComplete(KEY_SEP[1]);
        }
    }


    //Helper functions to show and hide the phone
    void ShowPhone(Hand hand)
    {
        //Get the transform of the hand
        Transform t_hand = hand.transform;

        //Position the phone then set it on

        hand.GetComponent<OnTriggerRaycast>().PickUpObj(PHONE);
        PHONE.transform.localPosition = POS_OFFSET;
        PHONE.transform.localRotation = Quaternion.identity;
        PHONE.SetActive(true);

        //Disable the notification obj if its on
        if (NOTIFICATION_OBJ.activeSelf)
        {
            NOTIFICATION_OBJ.SetActive(false);
        }

        //StartCoroutine(FadeIn(PHONE, FADE_TIME));

        PHONE.GetComponent<PhoneLinker>().PHONE = this;
    }

    public void HidePhone(Hand hand)
    {
        //Detach the object in script, then destroy it
        if((hand == hand1 && SHOW == ShowState.Hand1) || (hand == hand2 && SHOW == ShowState.Hand2))
        {
            
            hand.GetComponent<OnTriggerRaycast>().DropObj(PHONE);
            SHOW = ShowState.None;
            PHONE.SetActive(false);
        }
     
    }

    public void HidePhone()
    {
        //Checks show state and hides the correct phone
        if (SHOW == ShowState.Hand1)
        {
            hand1.GetComponent<OnTriggerRaycast>().DropObj(PHONE);
            SHOW = ShowState.None;
            PHONE.SetActive(false);
        }
        else if(SHOW == ShowState.Hand2)
        {
            hand2.GetComponent<OnTriggerRaycast>().DropObj(PHONE);
            SHOW = ShowState.None;
            PHONE.SetActive(false);
        }
    }

    
    //Helper coroutine that fades the phone in and moves it in
    IEnumerator FadeIn(GameObject phone, float FADE_TIME)
    {
        //Set the bool to true
        FADE_RUNNING = true;

        //Timer for how long the fade effect will work
        float FADE_TIMER = FADE_TIME;

        //The material on the phone that we want to adjust transparency
        Material MAT = phone.GetComponent<Renderer>().materials[0];
        MAT.SetColor("Albedo", new Color(1, 1, 1, 0));

        //Calculate how much transparency is applied per iteration
        float TRANS_STEP = 1f / FADE_TIME;
        float ALPHA = 0;

        while(FADE_TIMER > 0)
        {
            FADE_TIMER -= Time.deltaTime;
            ALPHA += TRANS_STEP;
            MAT.SetColor("Albedo", new Color(1, 1, 1, ALPHA));

            yield return new WaitForEndOfFrame();
        }

        //Set the bool to false
        FADE_RUNNING = false;
    }
}
