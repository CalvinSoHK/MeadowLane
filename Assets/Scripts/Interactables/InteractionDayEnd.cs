using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Interact script that calls end of day on use.
public class InteractionDayEnd : InteractableCustom {

    //Link to the farm manager
    public FarmManager FM;

    Scheduler SCHEDULER;
    DeliveryManager DM;
    LightingManager LM;

    public override void Use(Hand hand)
    {
        if (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState == ScreenTransitionImageEffect.Gamestate.wait)
        {
            Camera.main.GetComponent<ScreenTransitionImageEffect>().EndDay(hand.transform.parent.parent);

            StartCoroutine(EndDayAfterTransition());
           
        }
        base.Use(hand);
 

    }

    public IEnumerator EndDayAfterTransition()
    {
        while (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState != ScreenTransitionImageEffect.Gamestate.open)
        {
            yield return new WaitForEndOfFrame();
        }
        FM.DayEndAll();
        if (SCHEDULER == null || DM == null | LM == null)
        {
            SCHEDULER = GameManagerPointer.Instance.SCHEDULER;
            DM = GameManagerPointer.Instance.DELIVERY_MANAGER;
            LM = GameManagerPointer.Instance.LIGHTING_MANAGER;
        }
        SCHEDULER.NextDay();
        DM.ManageDeliveries();
    }
}
