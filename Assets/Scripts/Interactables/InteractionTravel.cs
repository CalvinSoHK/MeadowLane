using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Script that, on use, lets the player travel through this object.
public class InteractionTravel : InteractableCustom {

    //Destination of player prefab
    public Transform destination;

    //Override use function to move the player rig
    public override void Use(Hand hand)
    {
        if (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState == ScreenTransitionImageEffect.Gamestate.wait)
        {
            Camera.main.GetComponent<ScreenTransitionImageEffect>().MovePlayer(destination,
                hand.transform.parent.parent, false, true);
        }
    }
}
