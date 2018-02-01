﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Script that handles phone
public class PlayerPhone : MonoBehaviour {

    //Both hands
    Hand hand1, hand2;

    //Player stats
    public PlayerStats PLAYER_STATS;

    //Where the phone is showing
    public enum ShowState { None, Hand1, Hand2 };
    public ShowState SHOW = ShowState.None;

    //Pos offset for the phone on being called
    public Vector3 POS_OFFSET;

    //Whether or not we are currently fading somewhere
    public bool FADE_RUNNING = false;

    //How long the fade takes to work
    public float FADE_TIME;

    //Reference to the actual phone object
    public GameObject PHONE;

    //Bools for the four directions on the phone
    public bool LEFT = false, RIGHT = false, UP = false, DOWN = false, PRESS_DOWN = false, PRESS_UP = false, TRIGGER_DOWN = false;

    // Use this for initialization
    void Start() {
        hand1 = transform.GetChild(0).Find("Hand1").GetComponent<Hand>();
        hand2 = transform.GetChild(0).Find("Hand2").GetComponent<Hand>();
        PLAYER_STATS = GetComponent<PlayerStats>();
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
            TRIGGER_DOWN = hand1.GetStandardInteractionButtonDown();
            
        }
        else if (SHOW == ShowState.Hand2)
        {
            PRESS_DOWN = hand2.GetTrackpadDown();
            PRESS_UP = hand2.GetTrackpadUp();
            LEFT = hand2.GetTrackpadPressLeft();
            RIGHT = hand2.GetTrackpadPressRight();
            UP = hand2.GetTrackpadPressUp();
            DOWN = hand2.GetTrackpadPressDown();
            TRIGGER_DOWN = hand2.GetStandardInteractionButtonDown();
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
    public void VibratePhone(float DURATION, float STRENGTH)
    {
        if (SHOW == ShowState.Hand1)
        {
            hand1.TriggerHaptic(DURATION,STRENGTH);
        }
        else if (SHOW == ShowState.Hand2)
        {
            hand2.TriggerHaptic(DURATION, STRENGTH);
        }
        else
        {
            Debug.Log("ERROR: No hand to vibrate.");
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
