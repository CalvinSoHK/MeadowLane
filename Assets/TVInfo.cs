using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Saves the info for the text on the TV

public class TVInfo : MonoBehaviour {

    //Time manager
    public Scheduler TM;

    //The UI text objects
    public Text DATE, TIME;

    public void Update()
    {
        string inDate = TM.date.day + " , " + TM.date.month + " " + TM.date.dayNumber + " , " + TM.date.year;
        DATE.text = inDate;

        string inTime = TM.time;
        TIME.text = inTime;

    }


}
