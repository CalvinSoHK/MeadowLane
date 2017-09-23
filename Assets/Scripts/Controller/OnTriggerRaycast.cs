﻿using System.Collections;
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

    //obj is the target we're raycasting.
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
            if (obj != null)
            {
                if (obj.GetComponent<InteractableCustom>())
                {
                    obj.GetComponent<InteractableCustom>().Use(hand);
                   
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
            else
            {
                ENABLED = true;
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
                if (rayHit.collider.gameObject.layer == 8) {
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
                                    Material[] array = new Material[1];
                                    array[0] = obj.GetComponent<Renderer>().materials[0];
                                    obj.GetComponent<Renderer>().materials = array;

                                    obj = rayHit.collider.gameObject;

                                    //Make new material array with highlight as well
                                    Material[] matArray = new Material[2];

                                    matArray[0] = obj.GetComponent<Renderer>().material;
                                    matArray[1] = Resources.Load("Materials/HandHighlight", typeof(Material)) as Material;
                                    obj.GetComponent<Renderer>().materials = matArray;
                                }

                            }
                            else
                            {
                                obj = rayHit.collider.gameObject;

                                //Make new material array with highlight as well
                                Material[] matArray = new Material[2];

                                matArray[0] = obj.GetComponent<Renderer>().material;
                                matArray[1] = Resources.Load("Materials/HandHighlight", typeof(Material)) as Material;
                                obj.GetComponent<Renderer>().materials = matArray;
                            }
                        }
                        else
                        {
                            //If we previously had an object AND it isnt the same as the thing we're looking at now, get rid of its highlight.
                            if (obj != null && obj != rayHit.collider.gameObject && obj.GetComponent<Renderer>() != null)
                            {
                                Material[] array = new Material[1];
                                array[0] = obj.GetComponent<Renderer>().materials[0];
                                obj.GetComponent<Renderer>().materials = array;
                            }

                            //Always set obj
                            obj = rayHit.collider.gameObject;
                        }

                    }
                    else
                    {
                        //If we previously had an object, get rid of its highlight.
                        if (obj != null)
                        {
                            if (obj.GetComponent<Renderer>())
                            {
                                Material[] array = new Material[1];
                                array[0] = obj.GetComponent<Renderer>().materials[0];
                                obj.GetComponent<Renderer>().materials = array;

                            }

                        }
                        obj = null;
                    }
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
                        Material[] array = new Material[1];
                        array[0] = obj.GetComponent<Renderer>().materials[0];
                        obj.GetComponent<Renderer>().materials = array;
                    }

                }
                obj = null;
            }
        }
        else
        {
            GetComponent<LineRenderer>().enabled = false;
        }
    }
}
