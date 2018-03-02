using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class PlayerInputManager : MonoBehaviour {

    public enum InputMode { Default, Phone, Inventory, Edit };
    public InputMode MODE = InputMode.Default;

    public enum InputIdentity { Hand1, Hand2, None };

    public HandInputs HAND1 = new HandInputs(), HAND2 = new HandInputs();

    public Hand hand1, hand2;

	// Use this for initialization
	void Start () {
        hand1 = transform.GetChild(0).Find("Hand1").GetComponent<Hand>();
        hand2 = transform.GetChild(0).Find("Hand2").GetComponent<Hand>();
    }
	
	// Update is called once per frame
	void Update () {
        if(hand1 == null)
        {
            hand1 = transform.GetChild(0).Find("Hand1").GetComponent<Hand>();
        }
		else if(hand1 != null)
        {
            HAND1.LEFT = hand1.GetTrackpadPressLeft();
            HAND1.RIGHT = hand1.GetTrackpadPressRight();
            HAND1.UP = hand1.GetTrackpadPressUp();
            HAND1.DOWN = hand1.GetTrackpadPressDown();
            HAND1.TRACKPAD = hand1.GetTrackpad();
            HAND1.TRACKPAD_DOWN = hand1.GetTrackpadDown();
            HAND1.TRACKPAD_UP = hand1.GetTrackpadUp();
            HAND1.TRIGGER_DOWN = hand1.GetStandardInteractionButtonDown();
            HAND1.TRIGGER = hand1.GetStandardInteractionButton();
            HAND1.TRIGGER_UP = hand1.GetStandardInteractionButtonUp();
            HAND1.MENU = hand1.GetMenuButton();
            HAND1.MENU_DOWN = hand1.GetMenuButtonDown();
            HAND1.MENU_UP = hand1.GetMenuButtonUp();
        }
        
        if(hand2 == null)
        {
            //Debug.Log("Hand 2 missing.");
            hand2 = transform.GetChild(0).Find("Hand2").GetComponent<Hand>();
        }
        else if(hand2 != null)
        {
            //Debug.Log("Hand 2 inputs retrieving.");
            HAND2.LEFT = hand2.GetTrackpadPressLeft();
            HAND2.RIGHT = hand2.GetTrackpadPressRight();
            HAND2.UP = hand2.GetTrackpadPressUp();
            HAND2.DOWN = hand2.GetTrackpadPressDown();
            HAND2.TRACKPAD = hand2.GetTrackpad();
            HAND2.TRACKPAD_DOWN = hand2.GetTrackpadDown();
            HAND2.TRACKPAD_UP = hand2.GetTrackpadUp();
            HAND2.TRIGGER_DOWN = hand2.GetStandardInteractionButtonDown();
            HAND2.TRIGGER = hand2.GetStandardInteractionButton();
            HAND2.TRIGGER_UP = hand2.GetStandardInteractionButtonUp();
            HAND2.MENU = hand2.GetMenuButton();
            HAND2.MENU_DOWN = hand2.GetMenuButtonDown();
            HAND2.MENU_UP = hand2.GetMenuButtonUp();
        }

        //If we are in the default mode.
        if(MODE == InputMode.Default)
        {
            //If the trackpad is pressed down
            if (HAND1.TRACKPAD_DOWN)
            {
                //Debug.Log("Trackpad");
                //Trigger the phone if it is the up on the trackpad
                if (HAND1.UP)
                {
                    //Debug.Log("Phone");
                    GetComponent<PlayerPhone>().UsePhone(hand1);
                    changeMode(InputMode.Phone);
                }          

                //Trigger the player inventory if it is down on the trackpad
                if (HAND1.DOWN)
                {
                    //Debug.Log("Inventory");
                    GetComponent<PlayerInventory>().CallInventory(hand1);
                    //changeMode(InputMode.Inventory);
                }
            }

            //If the menu button is pressed
            if (HAND1.MENU_DOWN)
            {
                /*
                if (GetComponent<PlayerInventory>().isInventoryOn)
                {
                    GetComponent<PlayerInventory>().CheckInventoryUI(false);
                }*/
             
                hand1.GetComponent<OnTriggerRaycast>().obj = null;

                //Just in case the other hand is out, empty that object as well.
                if(hand2 != null)
                {
                    hand2.GetComponent<OnTriggerRaycast>().obj = null;
                }

                //Toggle between default or edit mode.
                if (isMode(InputMode.Default))
                {
                    Debug.Log("Set to true");
                    GetComponent<HomeCustomizationManager>().currentlyCustomizingHome = true;
                    changeMode(InputMode.Edit);
                }            
            }

            //If the menu button is pressed
            if (HAND2.MENU_DOWN)
            {
                /*
                if (GetComponent<PlayerInventory>().isInventoryOn)
                {
                    GetComponent<PlayerInventory>().CheckInventoryUI(false);
                }*/

                hand2.GetComponent<OnTriggerRaycast>().obj = null;

                //Just in case the other hand is out, empty that object as well.
                if (hand1 != null)
                {
                    hand1.GetComponent<OnTriggerRaycast>().obj = null;
                }

                //Toggle between default or edit mode.
                if (isMode(InputMode.Default))
                {
                    //Debug.Log("Set to true");
                    GetComponent<HomeCustomizationManager>().currentlyCustomizingHome = true;
                    changeMode(InputMode.Edit);
                }
            }

            //If the trackpad is pressed down
            if (HAND2.TRACKPAD_DOWN)
            {
                //Debug.Log("Trackpad press down.");
                //Trigger the phone if it is the up on the trackpad
                if (HAND2.UP)
                {
                    //Debug.Log("Phone");
                    GetComponent<PlayerPhone>().UsePhone(hand2);
                    changeMode(InputMode.Phone);
                }
                //Trigger the player inventory if it is down on the trackpad
                if (HAND2.DOWN)
                {
                    //Debug.Log("Inventory");
                    GetComponent<PlayerInventory>().CallInventory(hand2);
                    //changeMode(InputMode.Inventory);
                }
            }

        }
        else if (isMode(InputMode.Edit))
        {
            if(HAND2.MENU_DOWN || HAND1.MENU_DOWN)
            {
                //Debug.Log("Firing again");
                GetComponent<HomeCustomizationManager>().currentlyCustomizingHome = false;
                if(GetComponent<HomeCustomizationManager>().currentCustomizeState == HomeCustomizationManager.CustomizeState.Selected)
                {
                    GetComponent<HomeCustomizationManager>().cancelObject();
                }
                changeMode(InputMode.Default);
            }
            
            if(HAND1.DOWN && HAND1.TRACKPAD_DOWN)
            {
                GetComponent<PlayerInventory>().CallInventory(hand1);
            }

            if(HAND2.DOWN && HAND2.TRACKPAD_DOWN)
            {
                GetComponent<PlayerInventory>().CallInventory(hand2);
            }
        }
    }

    /// <summary>
    /// Checks to see if we are in the given mode.
    /// </summary>
    /// <param name="TEST"></param>
    /// <returns></returns>
    public bool isMode(InputMode TEST)
    {
        return MODE == TEST;
    }

    /// <summary>
    /// Just changes the mode we're in.
    /// </summary>
    /// <param name="NEW"></param>
    public void changeMode(InputMode NEW)
    {
        MODE = NEW;

        //Reset on trigger raycast for both hands on mode change
        hand1.GetComponent<OnTriggerRaycast>().ENABLED = true;
        hand2.GetComponent<OnTriggerRaycast>().ENABLED = true;
    }

    /// <summary>
    /// Returns the HandInputs object which has all the boolean statuses of the given hand.
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public HandInputs GetInput(InputIdentity ID)
    {
        if(ID == InputIdentity.Hand1)
        {
            return HAND1;
        }
        else if(ID == InputIdentity.Hand2)
        {
            return HAND2;
        }
        else
        {
            Debug.Log("What the hell.");
            return null;
        }
    }
}

[System.Serializable]
public class HandInputs
{
    public bool LEFT = false, RIGHT = false, UP = false, DOWN = false,
        TRACKPAD = false, TRACKPAD_DOWN = false, TRACKPAD_UP = false, TRIGGER = false, TRIGGER_DOWN = false, TRIGGER_UP = false,
        MENU_DOWN = false, MENU = false, MENU_UP = false;

    public void ClearValues()
    {
        LEFT = false;
        RIGHT = false;
        UP = false;
        DOWN = false;
        TRACKPAD = false;
        TRACKPAD_DOWN = false;
        TRACKPAD_UP = false;
        TRIGGER = false;
        TRIGGER_DOWN = false;
        TRIGGER_UP = false;
        MENU_DOWN = false;
        MENU = false;
        MENU_UP = false;
    }

    public void CopyValues(HandInputs INPUT)
    {
        LEFT = INPUT.LEFT;
        RIGHT = INPUT.RIGHT;
        UP = INPUT.UP;
        DOWN = INPUT.DOWN;
        TRACKPAD = INPUT.TRACKPAD;
        TRACKPAD_DOWN = INPUT.TRACKPAD_DOWN;
        TRACKPAD_UP = INPUT.TRACKPAD_UP;
        TRIGGER = INPUT.TRIGGER;
        TRIGGER_DOWN = INPUT.TRIGGER_DOWN;
        TRIGGER_UP = INPUT.TRIGGER_UP;
        MENU_DOWN = INPUT.MENU_DOWN;
        MENU = INPUT.MENU;
        MENU_UP = INPUT.MENU_UP;
    }
}
