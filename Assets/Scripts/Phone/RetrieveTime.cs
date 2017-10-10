using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Retrieves time from the scheduler
public class RetrieveTime : MonoBehaviour {

    Scheduler SCHEDULER = null;

    private void Update()
    {
        if(SCHEDULER == null)
        {
            SCHEDULER = GameObject.Find("TimeManager").GetComponent<Scheduler>();
        }
        else
        {
            //Retrieve the time
            string TIME = SCHEDULER.time;

            //Remove the seconds from the time
            string[] segments = TIME.Split(':');
            TIME = segments[0].Trim() + ":" + segments[1].Trim();

            //If we are past 43200 or 1200, then display PM
            if(SCHEDULER.CLOCK > 43200)
            {
                TIME += "pm";
            }
            else
            {
                TIME += "am";
            }

            GetComponent<Text>().text = TIME;

        }
    }
}
