using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Customizing stuff
public class HomeCustomizationManager : MonoBehaviour {

    //Whether or not we are currently customizing
    public bool currentlyCustomizingHome = false;

    //What state we are in within the customization state machine
    public enum CustomizeState { Idle, Selected, Stop}
    public CustomizeState currentCustomizeState;

    //How long we have been in a given state
    float stateTimer = 0.0f;

    //The selected object and its hologram reference object
    GameObject currentlySelectedObject, hologramRefToSelectedObject;

    //Array of materials that we will constantly be using. Init here for memory optimization
    Material[] hologramMaterial = new Material[2];

    //Bools for the four directions on the hand
    public bool LEFT = false, RIGHT = false, UP = false, DOWN = false, PRESS_DOWN = false, PRESS_UP = false, TRIGGER_DOWN = false, TRIGGER_UP = false, ANY_DIRECTIONAL = false, HOLD_DOWN = false, TRIGGER_HOLD_DOWN = false;

    //Both hands
    public Hand hand1, hand2;

    //Where the phone is showing
    public enum UseState { None, Hand1, Hand2 };
    public UseState SHOW_STATE= UseState.None;

    //Vibration enum
    public enum VibrationHand { Both, Hand1, Hand2 };

    //Ray for our raycasting
    Ray HAND_POINT = new Ray();
    RaycastHit HIT = new RaycastHit();

    //Layermask for our raycast
    public LayerMask PLACEMENT_LAYERMASK, RAYCAST_LAYERMASK;

    // Use this for initialization
    void Start () {
        hand1 = transform.GetChild(0).Find("Hand1").GetComponent<Hand>();
        hand2 = transform.GetChild(0).Find("Hand2").GetComponent<Hand>();
    }
	
	// Update is called once per frame
	void Update () {

        //Update the directional bools based off of the current hand's trackpad
        if (SHOW_STATE == UseState.Hand1)
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
        else if (SHOW_STATE == UseState.Hand2)
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

        if (currentlyCustomizingHome)
        {
            switch (currentCustomizeState)
            {
                case CustomizeState.Idle:
                    //Assign the right ray to raycast
                    if (SHOW_STATE == UseState.Hand1)
                    {
                        HAND_POINT = new Ray(hand1.transform.position, hand1.transform.forward);
                    }
                    else if (SHOW_STATE == UseState.Hand2)
                    {
                        HAND_POINT = new Ray(hand2.transform.position, hand2.transform.forward);
                    }
                    break;

                case CustomizeState.Selected: //Make the object follow our raycast location
                    //If we don't have a selectable object, its an error and move to the stop state.
                    if(currentlySelectedObject == null)
                    {
                        Debug.Log("Error: Don't actually have a selected object.");
                        SetCurrentHomeState(CustomizeState.Stop);
                    }
                    else
                    {
                        //Assign the right ray to raycast
                        if(SHOW_STATE == UseState.Hand1)
                        {
                            HAND_POINT = new Ray(hand1.transform.position, hand1.transform.forward);
                        }
                        else if(SHOW_STATE == UseState.Hand2)
                        {
                            HAND_POINT = new Ray(hand2.transform.position, hand2.transform.forward);
                        }

                        //As long as we're not none, and thus hand-pointer is not null.
                        if(SHOW_STATE != UseState.None)
                        {
                            //If we hit a point
                            if (Physics.Raycast(HAND_POINT, out HIT, 1000f, PLACEMENT_LAYERMASK))
                            {
                                hologramRefToSelectedObject.transform.position = HIT.point;
                            }

                            if (RIGHT)
                            {
                                hologramRefToSelectedObject.transform.Rotate(new Vector3(0, 1, 0));
                            }
                            else if(LEFT)
                            {
                                hologramRefToSelectedObject.transform.Rotate(new Vector3(0, -1, 0));
                            }

                            if (PRESS_DOWN)
                            {
                                if (hologramRefToSelectedObject.GetComponent<CheckIfColliding>().IS_VALID)
                                {
                                    Debug.Log("Valid. Placing.");
                                    placeObject();
                                }
                                else
                                {
                                    Debug.Log("Invalid. Destroyed.");
                                    Destroy(hologramRefToSelectedObject.gameObject);
                                }
                              
                                SetCurrentHomeState(CustomizeState.Stop);
                            }
                        }
                    }
                    break;

                case CustomizeState.Stop:
                    if (SHOW_STATE == UseState.Hand1)
                    {
                        hand1.GetComponent<OnTriggerRaycast>().ENABLED = true;
                    }
                    else if (SHOW_STATE == UseState.Hand2)
                    {
                        hand1.GetComponent<OnTriggerRaycast>().ENABLED = true;
                    }
                    SetCurrentHomeState(CustomizeState.Idle);
                    break;
            }
        }
	}

    /// <summary>
    /// set whether or not we are currently customizing the home
    /// should be activated on press of the top vive controller button
    /// </summary>
    /// <param name="customized"></param>
    public void CustomizeHome()
    {
        if (currentlyCustomizingHome) // if the player is already in the customize state then turn it off
        {
            currentlyCustomizingHome = false;

        }else // other set it to true
        {
            currentlyCustomizingHome = true;
        }
        
    }
    /// <summary>
    /// getter to see if we are currently customizing the home
    /// </summary>
    /// <returns></returns>
    public bool CustomizingHome()
    {
        return currentlyCustomizingHome;
    }

    public void SetCurrentHomeState(CustomizeState state)
    {
        currentCustomizeState = state;
        stateTimer = Time.time;
    }

    public void SetUseState(UseState state)
    {
        SHOW_STATE = state;
    }

    public float getElapsedTime()
    {
        return Time.time - stateTimer;
    }

    public void selectObject(GameObject og)
    {
        //Disable raycasting
        hand1.GetComponent<OnTriggerRaycast>().ENABLED = false;
        hand2.GetComponent<OnTriggerRaycast>().ENABLED = false;

        currentlySelectedObject = og; //set the ref to the currently referenced object
        if(hologramRefToSelectedObject != null) //if the hologram ref is not equal to null
        {
            Destroy(hologramRefToSelectedObject); //destroy the previous hologram object
        }
        hologramRefToSelectedObject = Instantiate(currentlySelectedObject); //create the hologram version of the currently selected object
        //hologramRefToSelectedObject.SetActive(false); // turn of the new object
        hologramMaterial = new Material[2]; // get two new empty material slots
        hologramMaterial[0] = Resources.Load("Materials/Transparent", typeof(Material)) as Material; // load the hologram and highlight materials 
        hologramMaterial[1] = Resources.Load("Materials/HandHighlight", typeof(Material)) as Material;
        hologramRefToSelectedObject.GetComponent<Renderer>().materials = hologramMaterial; //assign the new materials to the hologram object
        hologramRefToSelectedObject.AddComponent<CheckIfColliding>(); //Adds the script checkifcolliding.
        if(hologramRefToSelectedObject.GetComponent<MeshCollider>() != null)
        {
            hologramRefToSelectedObject.GetComponent<MeshCollider>().convex = true;
            hologramRefToSelectedObject.GetComponent<MeshCollider>().isTrigger = true;
        }
        hologramRefToSelectedObject.AddComponent<Rigidbody>();
        hologramRefToSelectedObject.GetComponent<Rigidbody>().isKinematic = true;
      
        SetCurrentHomeState(CustomizeState.Selected); //Set the current state to the selected one
    }

    public void placeObject()
    {
        currentlySelectedObject.transform.position = hologramRefToSelectedObject.transform.position;
        currentlySelectedObject.transform.rotation = hologramRefToSelectedObject.transform.rotation;
        Destroy(hologramRefToSelectedObject.gameObject);
        currentlySelectedObject = null;
        
    }
}
