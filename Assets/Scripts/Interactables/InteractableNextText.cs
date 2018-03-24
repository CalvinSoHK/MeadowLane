using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//On click goes to next text.

public class InteractableNextText : InteractableCustom {

    //Display dialogue script
    public DisplayDialogue DD;

    //On use do x
    public override void Use(Hand hand)
    {
        //Trigger next speech IF we are waiting to proceed. Else do nothing.
        if(DD.currentState == DisplayDialogue.GameState.WaitingToProceed)
        {
            DD.SetProceed(true);
           
        }else if (DD.currentState == DisplayDialogue.GameState.DisplayTheAction) //turn off text if the NPC is reating to a player action
        {
            DD.setCurrentState(DisplayDialogue.GameState.StopDisplayingText);
        }
        base.Use(hand);
    }
}
