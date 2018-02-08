using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Basic app script that will be  inherited by all applications
public class BasicApp : MonoBehaviour {

    //Whether or not the app has been initialized
    bool INIT = false;

    //References to PlayerPhone and PhoneLinker
    //Note: PlayerPhone references the master script that is handling input from the player and getting stats
    //      PhoneLinker is what is actually running the phone, switching between states.
    public PlayerPhone PHONE;
    public PhoneLinker LINKER;

    //Timer for holding down the trigger
    float EXIT_TIME = 1f, TIMER = 0f;

    bool downInitiated = false;

    //Function that will be called by the PhoneLinker if the app is running
	public virtual void RunApp()
    {
        if (PHONE.TRIGGER_HOLD_DOWN && downInitiated)
        {
            if (Time.time - TIMER >= EXIT_TIME)
            {
                //Drop phone
                downInitiated = false;
                PHONE.HidePhone();
            }
        }
        else if (PHONE.TRIGGER_HOLD_DOWN && !downInitiated)
        {
            TIMER = Time.time;
            downInitiated = true;
        }
        else if (PHONE.TRIGGER_UP)
        {
            //Exit app
            downInitiated = false;
            ExitApp();
        }
    }

    //Function that will allow us to drop the phone where we are but still use trigger
    public bool GetTriggerUp()
    {
        if (PHONE.TRIGGER_HOLD_DOWN && downInitiated)
        {
            if (Time.time - TIMER >= EXIT_TIME)
            {
                //Drop phone
                downInitiated = false;
                PHONE.HidePhone();
            }
        }
        else if (PHONE.TRIGGER_HOLD_DOWN && !downInitiated)
        {
            TIMER = Time.time;
            downInitiated = true;
        }
        else if (PHONE.TRIGGER_UP)
        {
            //Exit app
            downInitiated = false;
            return true;
        }
        return false;
    }

    //Helper functions that will be called in RunApp
    //Initializes the app
    public virtual void InitializeApp(PlayerPhone _PHONE, PhoneLinker _LINKER)
    {
        Debug.Log("Init the phone: " + _PHONE.name + _LINKER.name);

        //Base should set the references, and enable all children
        PHONE = _PHONE;
        LINKER = _LINKER;
    }

    //Exits the app back to the home screen
    public virtual void ExitApp()
    {
        Debug.Log("exit app");
        //Should end in returning to home screen
        LINKER.TransitionHome();
    }
}
