using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.EventSystems;

//Manager for the button, firing off events and using the animation properly.
public class InteractActivateButton : InteractableCustom {

    //The animation controller
    Animator animator;

    void Start()
    {
        animator = transform.parent.GetComponent<Animator>();
    }

    //Override use function to press the button.
    public override void Use(Hand hand)
    {
        //Set the trigger for the animation. 
        animator.SetTrigger("PressTrigger");

        //Despawn the UI
        base.Use(hand);
    }
}
