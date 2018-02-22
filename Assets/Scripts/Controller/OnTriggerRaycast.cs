using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(LineRenderer))]
//Script that enables the controller to raycast trigger items
public class OnTriggerRaycast : MonoBehaviour {

    //Layermask
    public LayerMask INTERACTABLE_LAYERS, HITTABLE_LAYERS, EDITABLE_LAYERS;

    //The hand we're attached to.
    Hand hand;

    //Raycasting for interaction
    bool raycast = false, customization = false;

    //Whether or not we are allowed to raycast
    public bool ENABLED = true;

    //Whether or not we need to clear the UI;
    bool REMOVE_LINE = false;

    //Raycasted object
    public GameObject obj;

    //The player object
    Transform VR_CAMERA, PLAYER;

    void Start()
    {
        hand = GetComponent<Hand>();
        PLAYER = GameManagerPointer.Instance.PLAYER_POINTER.PLAYER.transform;
        VR_CAMERA = transform.parent.Find("VRCamera");
    }

    //obj is the target we're raycasting.
    void Update()
    {
        //Set line renderer to our position
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        if(PLAYER == null)
        {
            PLAYER = GameManagerPointer.Instance.PLAYER_POINTER.PLAYER.transform;
        }
        customization = PLAYER.GetComponent<HomeCustomizationManager>().currentlyCustomizingHome;

        //When we get Trigger down...
        if (hand.GetStandardInteractionButtonDown() && ENABLED)
        {
            //Start raycasting
            raycast = true;
        }

        //When we get Trigger up...
        if (hand.GetStandardInteractionButtonUp())
        {
            if (obj != null)
            {
                //If we aren't doing customization, use all interaction scripts.
                if (!customization)
                {
                    if (obj.GetComponent<InteractableCustom>())
                    {
                        //Use every interact on an object.
                        foreach (InteractableCustom interact in obj.GetComponents<InteractableCustom>())
                        {
                            interact.Use(hand);

                        }

                        Material[] array = new Material[1];
                        array[0] = obj.GetComponent<Renderer>().materials[0];
                        obj.GetComponent<Renderer>().materials = array;
                        if (obj.GetComponent<InteractionPickup>())
                        {
                            ENABLED = false;
                            obj = null;
                        }
                    }
                }
                else if(ENABLED)
                {
                    Material[] array = new Material[1];
                    array[0] = obj.GetComponent<Renderer>().materials[0];
                    obj.GetComponent<Renderer>().materials = array;

                    if (hand.name.Contains("1"))
                    {
                         PLAYER.GetComponent<HomeCustomizationManager>().SetUseState(HomeCustomizationManager.UseState.Hand1);
                    }
                    else
                    {
                        PLAYER.GetComponent<HomeCustomizationManager>().SetUseState(HomeCustomizationManager.UseState.Hand2);
                    }
                    PLAYER.GetComponent<HomeCustomizationManager>().selectObject(obj);
                    obj = null;
                }
               
            }
            else
            {
                if (!customization)
                {
                    ENABLED = true;
                }              
            }


            //Stop raycasting
            raycast = false;
          
        }

        //If we need to raycast...
        if (raycast && ENABLED)
        {
            GetComponent<LineRenderer>().enabled = true;
            RaycastHit rayHit = new RaycastHit();
            //Raycast, if we hit something...

            //If we are customizing something
            if (customization)
            {
                //Raycast real far forward from this hand
                if (Physics.Raycast(transform.position, transform.forward, out rayHit, 1000f, HITTABLE_LAYERS))
                {
                    //Set the position of the line rendered wherever we hit.
                    GetComponent<LineRenderer>().SetPosition(1, rayHit.point);

                    //If it is within our editable layer mask
                    if(EDITABLE_LAYERS == (EDITABLE_LAYERS | (1 << rayHit.collider.gameObject.layer)))
                    {
                        //If it is an item
                        if (rayHit.collider.gameObject.GetComponent<BaseItem>() && (rayHit.collider.GetComponent<BaseItem>().hasTag(BaseItem.ItemTags.Decoration) || rayHit.collider.GetComponent<BaseItem>().CATEGORY.Equals("Decoration")))
                        {
                            obj = rayHit.collider.gameObject;

                            //If there is a renderer....
                            if (rayHit.collider.gameObject.GetComponent<Renderer>() != null)
                            {
                                //If we previously had an object AND it isnt the same as the thing we're looking at now, get rid of its highlight.
                                if (obj != null && obj != rayHit.collider.gameObject)
                                {
                                    if (obj.GetComponent<Renderer>())
                                    {
                                        //Remove highlight
                                        RemoveHighlight(obj);

                                        //Disable UI if possible
                                        DisableUI(obj);
                                    }

                                }
                                //Note: We always apply highlight to the new object
                                //Apply highlight to the given object
                                obj = rayHit.collider.gameObject;
                                ApplyHighlight(obj);

                                //Enable UI of object
                                EnableUI(obj);
                            }
                            else //This else is for when we had models that had multiple colliders. The hit collider sometimes didnt have a renderer.
                            {
                                //If we previously had an object AND it isnt the same as the thing we're looking at now, get rid of its highlight.
                                if (obj != null && obj != rayHit.collider.gameObject && obj.GetComponent<Renderer>() != null)
                                {
                                    RemoveHighlight(obj);
                                    DisableUI(obj);
                                }

                                //Always set obj
                                obj = rayHit.collider.gameObject;
                            }
                        }
                        else//This is for if it is within our layers but doesnt have a baseItemScript
                        {
                            //If we previously had an object, get rid of its highlight.
                            if (obj != null)
                            {
                                if (obj.GetComponent<Renderer>())
                                {
                                    RemoveHighlight(obj);
                                    DisableUI(obj);
                                }

                            }
                            obj = null;
                        }
                    }
                    else
                    {
                        //If we previously had an object, get rid of its highlight.
                        if (obj != null)
                        {
                            if (obj.GetComponent<Renderer>())
                            {
                                RemoveHighlight(obj);
                                DisableUI(obj);
                            }

                        }
                        obj = null;
                    }
                }
                else //If we don't hit something...
                {
                    //Render the line to its max distance.
                    GetComponent<LineRenderer>().SetPosition(1, transform.forward * 1000 + transform.position);
                    //If we had something highlighted before, turn it off.
                    if (obj != null)
                    {
                        if (obj.GetComponent<Renderer>() != null)
                        {
                            RemoveHighlight(obj);
                            DisableUI(obj);
                        }

                    }
                    obj = null;
                }
            }
            else//Handle normal interactions below
            {
                if (Physics.Raycast(transform.position, transform.forward, out rayHit, 1000f, HITTABLE_LAYERS))
                //if (Physics.SphereCast(transform.position, transform.forward, out rayHit, ))
                {
                    //Render the line to wherever the raycast ends.
                    GetComponent<LineRenderer>().SetPosition(1, rayHit.point);

                    //If it is in the interactable layer
                    if (INTERACTABLE_LAYERS == (INTERACTABLE_LAYERS | (1 << rayHit.collider.gameObject.layer)))
                    {
                        //And has the interactable script.
                        if (rayHit.collider.gameObject.GetComponent<InteractableCustom>())
                        {
                            //If there is a renderer....
                            if (rayHit.collider.gameObject.GetComponent<Renderer>() != null)
                            {
                                //If we previously had an object AND it isnt the same as the thing we're looking at now, get rid of its highlight.
                                if (obj != null && obj != rayHit.collider.gameObject)
                                {
                                    if (obj.GetComponent<Renderer>())
                                    {
                                        //Remove highlight
                                        RemoveHighlight(obj);

                                        //Disable UI if possible
                                        DisableUI(obj);
                                    }

                                }
                                //Note: We always apply highlight to the new object
                                //Apply highlight to the given object
                                obj = rayHit.collider.gameObject;
                                ApplyHighlight(obj);

                                //Enable UI of object
                                EnableUI(obj);
                            }
                            else //This else is for when we had models that had multiple colliders. The hit collider sometimes didnt have a renderer.
                            {
                                //If we previously had an object AND it isnt the same as the thing we're looking at now, get rid of its highlight.
                                if (obj != null && obj != rayHit.collider.gameObject && obj.GetComponent<Renderer>() != null)
                                {
                                    RemoveHighlight(obj);
                                    DisableUI(obj);
                                }

                                //Always set obj
                                obj = rayHit.collider.gameObject;
                            }

                        }
                    }
                    else
                    {
                        //If we previously had an object, get rid of its highlight.
                        if (obj != null)
                        {
                            if (obj.GetComponent<Renderer>())
                            {
                                RemoveHighlight(obj);
                                DisableUI(obj);
                            }

                        }
                        obj = null;
                    }
                }
                else //If we don't hit something...
                {
                    //Render the line to its max distance.
                    GetComponent<LineRenderer>().SetPosition(1, transform.forward * 1000 + transform.position);
                    //If we had something highlighted before, turn it off.
                    if (obj != null)
                    {
                        if (obj.GetComponent<Renderer>() != null)
                        {
                            RemoveHighlight(obj);
                            DisableUI(obj);
                        }

                    }
                    obj = null;
                }
            }
        }
        else
        {
            GetComponent<LineRenderer>().enabled = false;
        }
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
        //Get all interactables in obj
        InteractableCustom[] LIST = OBJ.GetComponents<InteractableCustom>();

        //Find the relevant object
        foreach(InteractableCustom INTERACTABLE in LIST)
        {
            //IF it has show_UI enabled
            if (INTERACTABLE.SHOW_UI)
            {
                INTERACTABLE.DisplayUI(VR_CAMERA);
                return;
            }
        }
    }

    //Helper function that finds the interactable that shows UI and disables it
    public void DisableUI(GameObject OBJ)
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

    //Helper function that applies the highlight to given obj
    public void ApplyHighlight(GameObject OBJ)
    {
        //Make new material array with highlight as well
        Material[] matArray = new Material[2];

        //Apply highlight
        matArray[0] = OBJ.GetComponent<Renderer>().material;
        matArray[1] = Resources.Load("Materials/HandHighlight", typeof(Material)) as Material;
        OBJ.GetComponent<Renderer>().materials = matArray;
    }

    //Helper function that removes the highlight to a given obj
    public void RemoveHighlight(GameObject OBJ)
    {
        //Remove highlight from previous
        Material[] array = new Material[1];
        array[0] = OBJ.GetComponent<Renderer>().materials[0];
        OBJ.GetComponent<Renderer>().materials = array;
    }
}
