using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//On click goes to next text.

public class InteractableInitiateText : InteractableCustom
{

    //Display dialogue script
    DisplayDialogue DD;

    private void Start()
    {
        DD = GetComponent<DisplayDialogue>();
    }

    //On use do x
    public override void Use(Hand hand)
    {
        //Trigger speech if we aren't doing anything (Wait) AND we're not already talking.
        if (DD.currentState == DisplayDialogue.GameState.Wait && !DD.GetInDialogue())
        {
            DD.setCurrentState(DisplayDialogue.GameState.DialogueSetup);
        }
    }
}