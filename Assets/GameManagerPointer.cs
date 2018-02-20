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

}
