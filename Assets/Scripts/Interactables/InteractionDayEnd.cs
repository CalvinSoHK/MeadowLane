using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Interact script that calls end of day on use.
public class InteractionDayEnd : InteractableCustom {

    //Link to the farm manager
    FarmManager FM;

    Scheduler SCHEDULER;
    DeliveryManager DM;
    LightingManager LM;
    EventManager EM;
    WeatherManager WM;

    public override void Use(Hand hand)
    {
        if (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState == ScreenTransitionImageEffect.Gamestate.wait)
        {
            Camera.main.GetComponent<ScreenTransitionImageEffect>().EndDay(hand.transform.parent.parent);

            StartCoroutine(EndDayAfterTransition());
           
        }
        base.Use(hand);
 

    }

    //Coroutine
    public IEnumerator EndDayAfterTransition()
    {
        while (Camera.main.GetComponent<ScreenTransitionImageEffect>().currentState != ScreenTransitionImageEffect.Gamestate.open)
        {
            yield return new WaitForEndOfFrame();
        }
        
       
        if (SCHEDULER == null || DM == null | LM == null)
        {
            SCHEDULER = GameManagerPointer.Instance.SCHEDULER;
            DM = GameManagerPointer.Instance.DELIVERY_MANAGER;
            LM = GameManagerPointer.Instance.LIGHTING_MANAGER;
            FM = GameManagerPointer.Instance.FARM_MANAGER_POINTER.FM;
            EM = GameManagerPointer.Instance.EVENT_MANAGER_POINTER;
            WM = GameManagerPointer.Instance.WEATHER_MANAGER_POINTER;
        }
        FM.DayEndAll();
        SCHEDULER.NextDay();
        DM.ManageDeliveries();
        EM.LoadEvents();
        WM.UpdateWeather();
        GetComponent<InteractionSaveGame>().EYES_CLOSED = true;
    }
}
