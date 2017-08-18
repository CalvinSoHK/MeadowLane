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
    public virtual void HandHoverUpdate(Hand hand)
    {
        //If we get the main button down
        if (hand.GetStandardInteractionButtonDown())
        {
            hand.AttachObject(gameObject);
        }
    }

    //Happens whenever this object is attached to a hand
    public virtual void OnAttachedToHand(Hand hand)
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    //Happens every frame while held by a hand
    public virtual void HandAttachedUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            hand.DetachObject(gameObject);
        } 
    }

    //Happens when the object is detaches from the hand
    public virtual void OnDetachedFromHand(Hand hand)
    {
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
