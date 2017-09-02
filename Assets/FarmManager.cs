using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Top level script that knows all the plots here.
public class FarmManager : MonoBehaviour {

    //List of all plots under this manager
    public List<PlotManager> PlotList;

    //Calls day end for all plots under this farm manager.
    public void DayEndAll()
    {
        foreach(PlotManager plot in PlotList)
        {
            plot.DayEnd();
        }
    }
}
