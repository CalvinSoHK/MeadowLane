using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CircularDriveCustom : CircularDrive {

    //The hand that is driving this steering wheel
    public Hand drivingHand = null;

    //Whether or not we should follow or not
    bool FOLLOW = false;

    //New function that just follows the hand given
    public void FollowHand(Hand hand)
    {

        //Set the hand to this hand
        drivingHand = hand;

        //Turn off raycast interact while held down.
        hand.GetComponent<OnTriggerRaycast>().ENABLED = false;
    }

    private void Update()
    {
        //Follow the hand given if we have a hand
        if(drivingHand != null)
        {
            ComputeAngle(drivingHand);
            UpdateAll();

            if (drivingHand.GetStandardInteractionButtonDown())
            {
                drivingHand.GetComponent<OnTriggerRaycast>().obj = null;
                drivingHand.GetComponent<OnTriggerRaycast>().ENABLED = true;
                drivingHand = null;
            }

        }
    }

    public override void HandHoverUpdate(Hand hand)
    {
       //Nothing. Don't want it to do anything.
    }
}
