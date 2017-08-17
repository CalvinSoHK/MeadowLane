using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Script is put onto objects we want to be able to pickup
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
        if (hand.GetTrackpadDown())
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
        if (hand.GetTrackpadDown())
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
