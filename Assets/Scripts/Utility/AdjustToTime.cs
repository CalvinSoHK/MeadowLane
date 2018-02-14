using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adjusts the lighting based on time
public class AdjustToTime : MonoBehaviour {

    //Start and end rotations
    public float ROT_START = 180, ROT_END = 360;

    //Internal value for how long 8 to 5, as well as the CLOCK values for 8AM and 5PM
    private float EIGHT_AM = 28800, FIVE_PM = 61200;

    //Internal value for how much we should rotate per second
    private float ROT_STEP;

    //Internal beginning rot for the sun
    private Vector3 BEGIN_ROT;

    Scheduler SCHEDULER;

    private void Start()
    {
        //Save the beginning rot for the sun
        BEGIN_ROT = transform.eulerAngles;

        //Calculate rotation per second
        ROT_STEP = Mathf.Abs(ROT_END - ROT_START) / (FIVE_PM - EIGHT_AM);
    }

    //Function that gets called to change the rotation
    public void ChangeRotation()
    {
        if(SCHEDULER == null)
        {
            SCHEDULER = GameManagerPointer.Instance.SCHEDULER;
        }
        transform.rotation = GetRotation(SCHEDULER.CLOCK);
    }

    //Helper function that calculates at what rotation we should be at based on the time of day.
    Quaternion GetRotation(float time)
    {
        Vector3 RETURN_ROT = BEGIN_ROT;

        RETURN_ROT.x += ROT_STEP * (time - EIGHT_AM);


        return Quaternion.Euler(RETURN_ROT);
    }
}
