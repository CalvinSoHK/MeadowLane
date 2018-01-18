﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour {

    //Object to face
    public Transform TARGET;

    //Parent object
    public Transform HOME;

    //Distance to maintain towards the player
    public float DISTANCE = 0.2f;

    //Position of the object
    Vector3 POSITION;

    //Ref of velocity
    Vector3 REF_VELOCITY;

    //Is the image supposed to be showing
    public bool IS_SHOWING = true;

    //The image we are showing
    public Image IMG;

    //Ref for Alpha "velocity"
    float REF_ALPHA;

    void Start()
    {
        HOME = transform.parent;
        POSITION = transform.position;
    }

    void Update()
    {
        //If we have a target
        if (TARGET != null)
        {
            //Calculate the direction to the player
            Vector3 DIR_TO_PLAYER = (TARGET.position - HOME.position).normalized;

            //Calculate the position we should be in
            Vector3 TARGET_POSITION = HOME.position + DIR_TO_PLAYER * DISTANCE;

            //Smooth damp to target position
            POSITION = Vector3.SmoothDamp(POSITION, TARGET_POSITION, ref REF_VELOCITY, 0.5f);

            //Apply the position
            transform.position = POSITION;

            //Look at target
            transform.LookAt(TARGET);
        }

        //If showing
        if (IS_SHOWING)
        {
            //Lerp the alpha to opaque
            IMG.GetComponent<Image>().color =
                new Color(
                    IMG.color.r,
                    IMG.color.g,
                    IMG.color.b,
                    Mathf.SmoothDamp(IMG.color.a, 1, ref  REF_ALPHA, 0.1f));
        }
        else
        {
            //Lerp the alpha to opaque
            IMG.GetComponent<Image>().color =
                new Color(
                    IMG.color.r,
                    IMG.color.g,
                    IMG.color.b,
                    Mathf.SmoothDamp(IMG.color.a, 0, ref REF_ALPHA, 0.1f));

            //Once alpha reaches 0, destroy image
            if(IMG.GetComponent<Image>().color.a <= 0.01f)
            {
                Destroy(gameObject);
            }
        }

        
    }
}