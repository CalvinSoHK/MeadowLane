using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bus_Stop_Manager : MonoBehaviour {

    //Singleton code.
    private static Bus_Stop_Manager _instance;

    public static Bus_Stop_Manager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;

        }
    }

    //List of all stop names. Used to load into the bus stops.
    public List<string> STOP_LIST = new List<string>();

    //List of prices for each stop. Indexes correlate with all stops
    public List<int> STOP_PRICES = new List<int>();

    //List of the locations we can go to
    public List<string> Transition_List;

    //Bus's movement speed
    public float MAX_SPEED = 5f;

    //The list of all stops.
    public List<BusEntryManager> STOP_CONTROLLER_LIST = new List<BusEntryManager>();

    //The bus we use to move players around
    public Bus_Controller BUS;

    //Functions called by the bus to get the bus stop.
    //Used to get the relevant bus stop. Returns null if not found.
    public BusEntryManager GetBusStop(string LOCATION)
    {
        foreach(BusEntryManager ENTRY in STOP_CONTROLLER_LIST)
        {
            if(ENTRY.Location == LOCATION)
            {
                return ENTRY;
            }
        }
        return null;
    }

}
