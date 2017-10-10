using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Links some references for the phone to use
//Applications will hold their own images and such inside them as children.
//Everything that is above applications will be shown at all times, i.e. time, money.
//Everything starting from applications below will hide if an application is selected.
public class PhoneLinker : MonoBehaviour {

    //PlayerPhone script
    public PlayerPhone PHONE;

    //Selector image
    public Image SELECTOR;

    //Selected APP
    Transform SELECTED;

    //Index of selected object
    int index = 1;

    //List of apps on phone
    Transform[] APPLICATIONS;

    //State of the phone, as in is it in an app or is it on OS, i.e. home screen.
    public enum PhoneState { OS, App };
    public PhoneState RunState = PhoneState.OS;

    //Internal bool to use the selector
    bool IS_SELECTOR = true;

    private void Start()
    {
        APPLICATIONS = transform.GetChild(0).Find("Applications").GetComponentsInChildren<Transform>();
        SELECTED = APPLICATIONS[index];
        SELECTOR.transform.localPosition = SELECTED.transform.localPosition;
    }

    private void Update()
    {
        //If we are running the OS, this is the behavior we want.
        if(RunState == PhoneState.OS)
        {
            //if we press down, on the right or left, adjust index. Watch for bounds.
            if (PHONE.PRESS_DOWN)
            {
                if (PHONE.RIGHT)
                {
                    if (index < APPLICATIONS.Length - 1)
                    {
                        index++;
                    }
                }
                else if (PHONE.LEFT)
                {
                    if (index > 1)
                    {
                        index--;
                    }
                }//Going up or down is +/- 3
                else if (PHONE.DOWN)
                {
                    if (index + 3 < APPLICATIONS.Length)
                    {
                        index += 3;
                    }
                }
                else if (PHONE.UP)
                {
                    if (index - 3 > 0)
                    {
                        index -= 3;
                    }
                }
                else //No direction was clicked, select the app.
                {
                    TransitionTo(SELECTED);
                }
            }

            //Maintain selection.
            SELECTED = APPLICATIONS[index];
            SELECTOR.transform.localPosition = SELECTED.transform.localPosition;
        }
        else if(RunState == PhoneState.App) //if its an app, use the app's script to run
        {
            SELECTED.GetComponent<BasicApp>().RunApp();
        }
       
    }

    //Helper functions
    //Transitions to a different screen. The transform is the icon for the app on the home screen
    public void TransitionTo(Transform app)
    {
        //Disable selection
        IS_SELECTOR = false;

        //Disable everything but the app and the parent
        for(int i = 1; i < APPLICATIONS.Length; i++)
        {
            if(APPLICATIONS[i] != app)
            {
                APPLICATIONS[i].gameObject.SetActive(false);
            }
        }

        //Init the app
        app.GetComponent<BasicApp>().InitializeApp(PHONE, this);

        //Set the phone state
        RunState = PhoneState.App;
    }

    //Helper function to transition back to home screen
    public void TransitionHome(Transform app)
    {
        //Disable children
        foreach(Transform child in app.GetComponentsInChildren<Transform>())
        {
            child.gameObject.SetActive(false);
        }

        //Re-enable everything.
        foreach(Transform APP in APPLICATIONS)
        {
            APP.gameObject.SetActive(true);
        }

        //Set the phone state
        RunState = PhoneState.OS;
    }
}
