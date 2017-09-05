using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Script that every item that can be stored and used will come from.
[RequireComponent(typeof(BaseItem))]
public class InteractionItem : InteractionPickup {

    public override void OnDetachedFromHand(Hand hand)
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        base.OnDetachedFromHand(hand);
        transform.parent = null;
    }
}
