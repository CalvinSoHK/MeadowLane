using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Same as travel, but moves the player itself into a fixed position. Like oculus rift.
public class InteractTravelFixed : InteractableCustom {

    //Destination of player prefab
    public Transform destination;

    //If true, make child of given transform
    public bool AnchorPlayer;

    //Override use function to move the player rig
    public override void Use(Hand hand)
    {
        if (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState == ScreenTransitionImageEffect.Gamestate.wait)
        {    
            Camera.main.GetComponent<ScreenTransitionImageEffect>().MovePlayer(destination,
                hand.gameObject.transform.parent.parent, true, true);
            if (AnchorPlayer)
            {
                //Parent steamVR player to the given transform
                hand.transform.parent.parent.parent = destination;
            }
            else
            {
                hand.transform.parent.parent.parent = null;
            }
        }
    }
}
