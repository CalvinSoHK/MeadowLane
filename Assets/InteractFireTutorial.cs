using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//On interact, try and send the given tutorial.
public class InteractFireTutorial : InteractableCustom {

    public string TUTORIAL_KEY;

    TutorialManager TM;

    private void Awake()
    {
        TM = GameManagerPointer.Instance.TUTORIAL_MANAGER;
    }

    //Override to fire the tutorial
    public override void Use(Hand hand)
    {
        if(TM != null)
        {
            if (!TM.IsComplete(TUTORIAL_KEY))
            {
                TM.LoadTutorial(TUTORIAL_KEY);
                TM.SetComplete(TUTORIAL_KEY);
            }
        }
    
    }
}
