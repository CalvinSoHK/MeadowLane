using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Script is put onto objects we want to be able to pickup
[RequireComponent(typeof(Rigidbody))]
public class InteractionPickup : InteractableCustom {

    //Write use function to pick up object
    public override void Use(Hand hand)
    {
        hand.AttachObject(gameObject);
    }

    //Happens whenever a hand is near the object
    void HandHoverUpdate(Hand hand)
    {
        //If we get the main button down
        if (hand.GetStandardInteractionButtonDown())
        {
            hand.AttachObject(gameObject);
        }
    }

    //Happens whenever this object is attached to a hand
    void OnAttachedToHand(Hand hand)
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    //Happens every frame while held by a hand
    void HandAttachedUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            hand.DetachObject(gameObject);
        } 
    }

    //Happens when the object is detaches from the hand
    void OnDetachedFromHand(Hand hand)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
