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
    Transform SELECTED_ICON, SELECTED_APP;

    //Index of selected object
    int index = 1;

    //List of apps on phone
    Transform[] APPLICATIONS;

    //State of the phone, as in is it in an app or is it on OS, i.e. home screen.
    public enum PhoneState { OS, App, Transition};
    public PhoneState RunState = PhoneState.OS;
    PhoneState NEXT_STATE;

    //The home screen transform
    public Transform HOME_SCREEN, SECOND_SCREEN, THIRD_SCREEN, ALL_APP_SCREENS;
    Transform TARGET_SCREEN;

    //The transform that has all our applications
    public Transform APP_GRID;

    //The app that is running on the phone
    //When null, we aren't running anything.
    public Transform RUNNING_APP = null;

    //Transition variables
    float DAMP_STRENGTH = 0.1f;
    Vector3 TARGET_POS = Vector3.zero;
    Vector3 REF_VELOCITY = Vector3.zero;

    private void Start()
    {
        APPLICATIONS = APP_GRID.GetComponentsInChildren<Transform>();
        SELECTED_ICON = APPLICATIONS[index];
        SELECTOR.transform.localPosition = SELECTED_ICON.transform.localPosition;
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
                    //Clear app if we try to go to another app
                    ClearApp();

                    //Transition to the second screen to the selected.
                    TransitionToApp(SECOND_SCREEN, SELECTED_ICON);
                }
            }

            if (PHONE.TRIGGER_DOWN)
            {
                PHONE.HidePhone();
            }

            //Clear the second screen if we're on OS


            //Maintain selection.
            SELECTED_ICON = APPLICATIONS[index];
            SELECTOR.transform.position = SELECTED_ICON.transform.position;
        }
        else if(RunState == PhoneState.App) //if its an app, use the app's script to run
        {
            if(SELECTED_APP != null)
            {
                SELECTED_APP.GetComponent<BasicApp>().RunApp();
            }         
        }
        else if(RunState == PhoneState.Transition) //We are transitioning to a screen
        {
            //Smooth damp to the target position
            ALL_APP_SCREENS.transform.localPosition = Vector3.SmoothDamp(ALL_APP_SCREENS.transform.localPosition, TARGET_POS, ref REF_VELOCITY, DAMP_STRENGTH);
            //Debug.Log(ALL_APP_SCREENS.transform.position);
            //When we're close enough just finish it and go to our next state.
            //Debug.Log(Vector3.Distance(ALL_APP_SCREENS.transform.localPosition, TARGET_POS));
            if(Vector3.Distance(ALL_APP_SCREENS.transform.localPosition, TARGET_POS) <= 0.1f)
            {
                ALL_APP_SCREENS.transform.localPosition = TARGET_POS;
                RunState = NEXT_STATE;
            }
        }
       
    }

    //Function that inits the app we're on
    public void InitApp()
    {
        //Only inits the app if we're in app state. If we are in transition it fails
        if(RunState == PhoneState.App || (RunState == PhoneState.Transition && NEXT_STATE == PhoneState.App))
        {
            SELECTED_APP.GetComponentInChildren<BasicApp>().InitializeApp(PHONE, this);
        }
    }

    //Helper function that loads in an app on a different screen
    public void LoadApp(Transform SCREEN, Transform APP)
    {
        Debug.Log("Loading app");

        //Instantiate and set the app
        Transform TEMP = Instantiate(APP, SCREEN.position, SCREEN.rotation, SCREEN);
        SELECTED_APP = TEMP;

        //Init the app
        SELECTED_APP.GetComponentInChildren<BasicApp>().InitializeApp(PHONE, this);
        RUNNING_APP = TEMP;
    }

    //Helper function that clears a screen
    public void ClearApp()
    {
        if(SECOND_SCREEN.childCount > 0)
        {
            Destroy(SECOND_SCREEN.GetChild(0).gameObject);
        }

        if(THIRD_SCREEN.childCount > 0)
        {
            Destroy(THIRD_SCREEN.GetChild(0).gameObject);
        }
    }

    //Helper function that helps us transition  between different screens
    public void TransitionTo(Transform SCREEN)
    {
        RunState = PhoneState.Transition;
        NEXT_STATE = PhoneState.App;
        TARGET_SCREEN = SCREEN;
        TARGET_POS = -TARGET_SCREEN.localPosition;
    }

    //Helper functions
    //Transitions to a different screen. The transform is the icon for the app on the home screen
    public void TransitionToApp(Transform SCREEN, Transform ICON)
    {
        //Go to transition state
        RunState = PhoneState.Transition;

        //Set the state based on what screen we're going to
        if(SCREEN == HOME_SCREEN)
        {
            NEXT_STATE = PhoneState.OS;
            RUNNING_APP = null;
        }
        else
        {
            NEXT_STATE = PhoneState.App;
            LoadApp(SCREEN, ICON.GetComponent<AppSelector>().APP_SCREEN);
        }

        //Set our target screen
        TARGET_SCREEN = SCREEN;

        //Calculate target pos. Should be the reverse of the actual position.
        TARGET_POS = -TARGET_SCREEN.localPosition;      
        Debug.Log(TARGET_POS);
    }

    //Helper function to transition back to home screen
    public void TransitionHome()
    {
        //Transition to the home screen
        TransitionToApp(HOME_SCREEN, null);
    }
}
