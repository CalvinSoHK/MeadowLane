using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Interact script that calls end of day on use.
public class InteractionDayEnd : InteractableCustom {

    //Link to the farm manager.
    public FarmManager FM;

    //Link to the scheduler
    public Scheduler TM;

    public void Update()
    {
        //Debug command
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FM.DayEndAll();
            TM.NextDay(TM.date);
        }
    }

    public override void Use(Hand hand)
    {
        if (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState == ScreenTransitionImageEffect.Gamestate.wait)
        {
            Camera.main.GetComponent<ScreenTransitionImageEffect>().EndDay(hand.transform.parent.parent);

            StartCoroutine(EndDayAfterTransition());
           
        }
 

    }

    public IEnumerator EndDayAfterTransition()
    {
        while (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState != ScreenTransitionImageEffect.Gamestate.open)
        {
            yield return new WaitForEndOfFrame();
        }
        FM.DayEndAll();
        TM.NextDay(TM.date);
        DeliveryManager.Instance.ManageDeliveries();
    }
}
