using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(LineRenderer))]
//Script that enables the controller to raycast trigger items
public class OnTriggerRaycast : MonoBehaviour {

    //Layermask
    public LayerMask layerMask;

    //The hand we're attached to.
    Hand hand;

    //Raycasting for interaction
    bool raycast = false;

    //Whether or not we are allowed to raycast
    public bool ENABLED = true;

    //Raycasted object
    GameObject obj;

    void Start()
    {
        hand = GetComponent<Hand>();
    }

    void Update()
    {
        //Set line renderer to our position
        GetComponent<LineRenderer>().SetPosition(0, transform.position);

        //When we get Trigger down...
        if (hand.GetStandardInteractionButtonDown())
        {
            //Start raycasting
            raycast = true;
        }

        //When we get Trigger up...
        if (hand.GetStandardInteractionButtonUp())
        {
            if(obj != null)
            {
                if (obj.GetComponent<InteractableCustom>())
                {
                    obj.GetComponent<InteractableCustom>().Use(hand);
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
            if (Physics.Raycast(transform.position, transform.forward, out rayHit, 1000f))
            {
                //Render the line to wherever the raycast ends.
                GetComponent<LineRenderer>().SetPosition(1, rayHit.point);
                //If it is in the interactable layer
                if (rayHit.collider.gameObject.layer == 8){
                    //And has the interactable script.
                    if (rayHit.collider.gameObject.GetComponent<InteractableCustom>())
                    {
                        obj = rayHit.collider.gameObject;
                    }
                    else
                    {
                        obj = null;
                    }
                }  
            }
            else //If we don't hit something...
            {
                //Render the line to its max distance.
                GetComponent<LineRenderer>().SetPosition(1, transform.forward * 1000 + transform.position);
                obj = null;
            }
        }
        else
        {
            GetComponent<LineRenderer>().enabled = false;
        }  
    }
}
