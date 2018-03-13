using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerPointer : MonoBehaviour {

    //Singleton code.
    private static GameManagerPointer _instance;

    public static GameManagerPointer Instance
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
            SCHEDULER = GetComponent<Scheduler>();
            DELIVERY_MANAGER = GetComponent<DeliveryManager>();
            BUS_STOP_MANAGER = GetComponent<Bus_Stop_Manager>();
            TRAVEL_POINT_MANAGER = GetComponent<TravelPointManager>();
            PLAYER_POINTER = GetComponent<PlayerPointer>();
            TUTORIAL_MANAGER = GetComponent<TutorialManager>();
            LIGHTING_MANAGER = GetComponent<LightingManager>();
            FARM_MANAGER_POINTER = GetComponent<FarmManagerPointer>();
            FURNITURE_MANAGER_POINTER = GetComponent<FurnitureManagerPointer>();
            EVENT_MANAGER_POINTER = GetComponent<EventManager>();
        }
    }

    //All the scripts we want to be pointing to
    public Scheduler SCHEDULER;
    public DeliveryManager DELIVERY_MANAGER;
    public Bus_Stop_Manager BUS_STOP_MANAGER;
    public TravelPointManager TRAVEL_POINT_MANAGER;
    public PlayerPointer PLAYER_POINTER;
    public TutorialManager TUTORIAL_MANAGER;
    public LightingManager LIGHTING_MANAGER;
    public FarmManagerPointer FARM_MANAGER_POINTER;
    public FurnitureManagerPointer FURNITURE_MANAGER_POINTER;
    public EventManager EVENT_MANAGER_POINTER;

    //Turns on and off pointers based on the stop given.
    //i.e. When going to happymart it disables farm/furniture pointers so they keep uselessly searching
    public void ManagePointers(string STOP_NAME)
    {
        if (!STOP_NAME.Equals("PlayerHome"))
        {
            FURNITURE_MANAGER_POINTER.ENABLED = false;
            FARM_MANAGER_POINTER.ENABLED = false;
        }
    }

}
