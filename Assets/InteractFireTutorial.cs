using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//On interact, try and send the given tutorial.
public class InteractFireTutorial : InteractableCustom {

    public string TUTORIAL_KEY;

    //Override to fire the tutorial
    public override void Use(Hand hand)
    {
        //TutorialManager
    }
}
