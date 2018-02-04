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

    //Function that will be called by the PhoneLinker if the app is running
	public virtual void RunApp()
    {
        Debug.Log("RUN APP");
        //By default allow for exit on trigger
        if (PHONE.TRIGGER_DOWN)
        {
            ExitApp();
        }
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
