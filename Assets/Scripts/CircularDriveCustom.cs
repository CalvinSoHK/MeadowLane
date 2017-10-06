using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CircularDriveCustom : CircularDrive {

    //If true, drive is engaged in use, else not.
    public bool isUse = false;

    //The hand that is driving this steering wheel
    public Hand drivingHand = null;

	public override void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            //Set flag to true
            isUse = true;

            //Set the hand to this hand
            drivingHand = hand;

            // Trigger was just pressed
            lastHandProjected = ComputeToTransformProjected(hand.hoverSphereTransform);

            if (hoverLock)
            {
                hand.HoverLock(GetComponent<Interactable>());
                handHoverLocked = hand;
            }

            driving = true;

            ComputeAngle(hand);
            UpdateAll();

            ControllerButtonHints.HideButtonHint(hand, Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);

            //Turn off raycast interact while held down.
            hand.GetComponent<OnTriggerRaycast>().ENABLED = false;
        }
        else if (hand.GetStandardInteractionButtonUp())
        {
            //Reset flag, and hand
            drivingHand = null;
            isUse = false;

            // Trigger was just released
            if (hoverLock)
            {
                hand.HoverUnlock(GetComponent<Interactable>());
                handHoverLocked = null;
            }

            //Turn on ray cast interact on let go
            hand.GetComponent<OnTriggerRaycast>().ENABLED = true;
        }
        else if (driving && hand.GetStandardInteractionButton() && hand.hoveringInteractable == GetComponent<Interactable>())
        {
            ComputeAngle(hand);
            UpdateAll();
        }
    }
}
