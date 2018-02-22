using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(LineRenderer))]
//Script that enables the controller to raycast trigger items
public class OnTriggerRaycast : MonoBehaviour {

    public enum RaycastMode { Default, Decorating };
    public RaycastMode MODE = RaycastMode.Default;

    enum State { Idle, Raycast, Select };
    State STATE = State.Idle;

    //Layermask
    public LayerMask INTERACTABLE_LAYERS, HITTABLE_LAYERS, EDITABLE_LAYERS;

    //The hand we're attached to.
    Hand hand;

    //Raycasting for interaction
    bool customization = false;

    //Whether or not we are allowed to raycast
    public bool ENABLED = true;

    //Whether or not we need to clear the UI;
    bool REMOVE_LINE = false;

    //Raycasted object
    public GameObject obj;

    //The player object
    Transform VR_CAMERA, PLAYER;

    //Raycast hit info
    RaycastHit HIT = new RaycastHit();

    void Start()
    {
        hand = GetComponent<Hand>();
        PLAYER = GameManagerPointer.Instance.PLAYER_POINTER.PLAYER.transform;
        VR_CAMERA = transform.parent.Find("VRCamera");
    }

    //obj is the target we're raycasting.
    void Update()
    {
        //Always try to have Player assigned to.
        if (PLAYER == null)
        {
            PLAYER = GameManagerPointer.Instance.PLAYER_POINTER.PLAYER.transform;
        }

        //State machine
        switch (STATE)
        {
            //While idle
            case State.Idle:
                //If we get trigger down, move us to the raycast state
                if(hand.GetStandardInteractionButtonDown())
                {
                    if (ENABLED)
                    {
                        STATE = State.Raycast;
                    }
                    else
                    {
                        ENABLED = true;
                    }                
                }
                break;
            //While we need to raycast
            case State.Raycast:
                //Turn on the raycast renderer
                GetComponent<LineRenderer>().enabled = true;

                //Set line renderer to our position
                GetComponent<LineRenderer>().SetPosition(0, transform.position);

                //Raycast real far forward from this hand and only hit hittable layers
                if (Physics.Raycast(transform.position, transform.forward, out HIT, 1000f, HITTABLE_LAYERS))
                {
                    //Set the position of the line rendered wherever we hit, regardless of the mode
                    GetComponent<LineRenderer>().SetPosition(1, HIT.point);

                    //If the raycasted object is not the same as last frame, get rid of highlights off the old obj and the UI popup
                    if (!HIT.collider.gameObject.Equals(obj) && obj != null)
                    {
                        RemoveHighlight(obj);
                        DisableUI(obj);
                        obj = null;
                    }
                    
                    //Handle object validity based on mode.
                    switch (MODE)
                    {
                        case RaycastMode.Default:
                            //If it is within the interactable layers
                            if (INTERACTABLE_LAYERS == (INTERACTABLE_LAYERS | (1 << HIT.collider.gameObject.layer)))
                            {
                                //And has the interactable script.
                                if (HIT.collider.gameObject.GetComponent<InteractableCustom>())
                                {
                                    obj = HIT.collider.gameObject;
                                    ApplyHighlight(obj);
                                    EnableUI(obj);
                                }
                            }
                            break;
                        case RaycastMode.Decorating:
                            //If it is within our editable layer mask
                            if (EDITABLE_LAYERS == (EDITABLE_LAYERS | (1 << HIT.collider.gameObject.layer)))
                            {
                                //If we are a base item with the decoration tag.
                                if (HIT.collider.gameObject.GetComponent<BaseItem>() 
                                    && (HIT.collider.GetComponent<BaseItem>().hasTag(BaseItem.ItemTags.Decoration) 
                                    || HIT.collider.GetComponent<BaseItem>().CATEGORY.Equals("Decoration")))
                                {
                                    obj = HIT.collider.gameObject;
                                    ApplyHighlight(obj);                                   
                                }
                            }
                            break;
                        default:
                            Debug.Log("Invalid mode.");
                            break;
                    }
                }
                else //We aren't hitting anything but we still draw the line for players.
                {
                    //Render the line to its max distance.
                    GetComponent<LineRenderer>().SetPosition(1, transform.forward * 1000 + transform.position);

                    //If we had something highlighted before, turn it off.
                    if (obj != null)
                    {
                        RemoveHighlight(obj);
                        DisableUI(obj);
                    }
                    obj = null;
                }

                //Once we get button up we can move on to select.
                if (hand.GetStandardInteractionButtonUp())
                {
                    STATE = State.Select;
                }

                break;
            //When we are told to select.
            case State.Select:
                //Disable the raycast render
                GetComponent<LineRenderer>().enabled = false;

                //If we have an obj we can try to select
                if (obj != null)
                {
                    //Depending on what mode we are trying to use
                    switch (MODE)
                    {
                        //Default use. Fire all interaction scripts on use.
                        case RaycastMode.Default:
                            //If the object has at least one interactable script
                            if (obj.GetComponent<InteractableCustom>())
                            {
                                //Use every interact on an object.
                                foreach (InteractableCustom interact in obj.GetComponents<InteractableCustom>())
                                {
                                    interact.Use(hand);
                                }

                                //If the object has interaction pickup, disable raycasting.
                                if (obj.GetComponent<InteractionPickup>())
                                {
                                    ENABLED = false;
                                }
                                else
                                {
                                    ENABLED = true;
                                }
                            }
                            break;
                        //Decoration. Communicate with home customizatioin manager.
                        case RaycastMode.Decorating:

                            //Set the use state to the right hand depending on who called it.
                            if (hand.name.Contains("1"))
                            {
                                PLAYER.GetComponent<HomeCustomizationManager>().SetUseState(HomeCustomizationManager.UseState.Hand1);
                            }
                            else
                            {
                                PLAYER.GetComponent<HomeCustomizationManager>().SetUseState(HomeCustomizationManager.UseState.Hand2);
                            }

                            //Then tell the customization manager to select the given object.
                            PLAYER.GetComponent<HomeCustomizationManager>().selectObject(obj);

                            //Since we are "holding" that object, disable raycasting
                            ENABLED = false;
                            break;
                    }

                    //Remove the highlight effect
                    RemoveHighlight(obj);

                    //No matter what happens remove the selected object.
                    obj = null;
                }

                //After passing through once cut to idle again, regardless if there was valid select
                STATE = State.Idle;
                break;
            default:
                Debug.Log("Error: Non valid state.");
                break;
        }
    }

    public void ToggleMode()
    {
        if(MODE == RaycastMode.Default)
        {
            MODE = RaycastMode.Decorating;
        }
        else if(MODE == RaycastMode.Decorating)
        {
            MODE = RaycastMode.Default;
        }

        //Clear object if we toggle mode just so if we are raycasting at something it doesnt trick it into thinking its valid.
        obj = null;
    }

    public void SetMode(int MODE_INT)
    {
        MODE = (RaycastMode)MODE_INT;
    }

    public void SetMode(RaycastMode MODE_STATE)
    {
        MODE = MODE_STATE;
    }

    //Helper function to manually pick something up with script
    public void PickUpObj(GameObject OBJECT)
    {
        hand.AttachObject(OBJECT);
        ENABLED = false;
        obj = OBJECT;
    }

    //Helper function to manually detach something 
    public void DropObj(GameObject OBJECT)
    {
        hand.DetachObject(OBJECT);
        ENABLED = true; 
        obj = null;
    }

    //Helper function that finds the interactable that shows UI and shows it
    public void EnableUI(GameObject OBJ)
    {
        if (OBJ.GetComponent<InteractableCustom>())
        {
            //Get all interactables in obj
            InteractableCustom[] LIST = OBJ.GetComponents<InteractableCustom>();

            //Find the relevant object
            foreach (InteractableCustom INTERACTABLE in LIST)
            {
                //IF it has show_UI enabled
                if (INTERACTABLE.SHOW_UI)
                {
                    INTERACTABLE.DisplayUI(VR_CAMERA);
                    return;
                }
            }
        }  
    }

    //Helper function that finds the interactable that shows UI and disables it
    public void DisableUI(GameObject OBJ)
    {
        if (OBJ.GetComponent<InteractableCustom>())
        {
            //Get all interactables in obj
            InteractableCustom[] LIST = OBJ.GetComponents<InteractableCustom>();

            //Find the relevant object
            foreach (InteractableCustom INTERACTABLE in LIST)
            {
                //IF it has show_UI enabled
                if (INTERACTABLE.SHOW_UI && INTERACTABLE.UI_PREFAB != null)
                {
                    INTERACTABLE.HideUI();
                    return;
                }
            }
        }
    }

    //Helper function that applies the highlight to given obj
    public void ApplyHighlight(GameObject OBJ)
    {
        if (OBJ.GetComponent<MeshRenderer>())
        {
            //Make new material array with highlight as well
            Material[] matArray = new Material[2];

            //Apply highlight
            matArray[0] = OBJ.GetComponent<Renderer>().material;
            matArray[1] = Resources.Load("Materials/HandHighlight", typeof(Material)) as Material;
            OBJ.GetComponent<Renderer>().materials = matArray;
        }      
    }

    //Helper function that removes the highlight to a given obj
    public void RemoveHighlight(GameObject OBJ)
    {
        if (OBJ.GetComponent<MeshRenderer>())
        {
            //Remove highlight from previous
            Material[] array = new Material[1];
            array[0] = OBJ.GetComponent<Renderer>().materials[0];
            OBJ.GetComponent<Renderer>().materials = array;
        } 
    }
}
