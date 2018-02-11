using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public List<BusStopInfo> STOP_LIST = new List<BusStopInfo>();

    //List of prices for each stop. Indexes correlate with all stops
    public List<int> STOP_PRICES = new List<int>();

    //List of the locations we can go to
    public List<string> Transition_List;

    //List of scenes
    public List<Scene> Scene_List;

    //Bus's movement speed
    public float MAX_SPEED = 5f;

    //The bus we use to move players around
    public Bus_Controller BUS;

    //The stop we are currently on
    public BusStopInfo CURRENT_STOP;

    //Functions called by the bus to get the bus stop in a given scene.
    //Used to get the relevant bus stop. Returns null if not found.
    public BusStopInfo GetBusStop(string LOCATION)
    {
        LOCATION = LOCATION.Replace(" ", "");
        for (int i = 0; i < STOP_LIST.Count - 1; i++)
        {
            if (LOCATION.Equals(STOP_LIST[i].GetName()))
            {
                return STOP_LIST[i];
            }
        }
        return null;
    }

    //Gives the index of a given location
    public int GetIndexOf(string LOCATION)
    {
        LOCATION = LOCATION.Replace(" ", "");
        for (int i = 0; i < STOP_LIST.Count; i++)
        {
            if (LOCATION.Trim().Equals(STOP_LIST[i].GetName()))
            {
                return i;
            }
        }
        return -1;
    }

    //Gives the scene index for a given transition type
    public string GetTransitionSceneName(BusStopInfo.TransitionType TRANSITION)
    {
        switch (TRANSITION)
        {
            case BusStopInfo.TransitionType.Village:
                return Transition_List[0];
            case BusStopInfo.TransitionType.Outskirts:
                return Transition_List[1];
            case BusStopInfo.TransitionType.Beachside:
                return Transition_List[2];
            default:
                Debug.Log("Invalid transition type.");
                return Transition_List[0];
        }
    }

}

//Custom class that will store info on each bus stop
[System.Serializable]
public class BusStopInfo
{
    public string SCENE_NAME;

    //The type of transition we need to go here
    public enum TransitionType { Village, Outskirts, Beachside };

    //The transition for this stop
    public TransitionType TRANSITION;

    //The bus's position and rotation on arriving in this location
    public Vector3 BUS_POSITION, BUS_ROTATION;

    public BusStopInfo(string NAME_T, TransitionType TYPE_T)
    {
        SCENE_NAME = NAME_T;
        TRANSITION = TYPE_T;
    }

    public string GetName()
    {
        return SCENE_NAME.Replace(" ","");
    }
}
