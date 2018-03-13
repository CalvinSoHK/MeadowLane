using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bus_Stop_Manager : MonoBehaviour {

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
    private string SPACES_SCENE_NAME;

    //The type of transition we need to go here
    public enum TransitionType { Village, Outskirts, Beachside };

    //The transition for this stop
    public TransitionType TRANSITION;

    //How much this stop costs
    public int PRICE;

    //The bus's position and rotation on arriving in this location
    public Vector3 BUS_POSITION, BUS_ROTATION;

    public BusStopInfo(string NAME_T, TransitionType TYPE_T)
    {
        SCENE_NAME = NAME_T;
        TRANSITION = TYPE_T;
        SPACES_SCENE_NAME = InsertSpaces(SCENE_NAME);
        //Debug.Log(SPACES_SCENE_NAME);
    }

    public string GetName()
    {
        return SCENE_NAME.Replace(" ","");
    }

    //Returns the spaces scene name. Calls insert spaces if it hasn't been done yet.
    public string GetSpacesName()
    {
        if (string.IsNullOrEmpty(SPACES_SCENE_NAME))
        {
            SPACES_SCENE_NAME = InsertSpaces(SCENE_NAME);
        }
        return SPACES_SCENE_NAME;
    }

    //Insert spaces into the line
    public string InsertSpaces(string INPUT)
    {
        if (string.IsNullOrEmpty(INPUT))
        {
            Debug.Log("Is null");
            
            return "";
        }
        StringBuilder newText = new StringBuilder(INPUT.Length * 2);
        newText.Append(INPUT[0]);
        for (int i = 1; i < INPUT.Length; i++)
        {
            //If the char is upper case AND the spot before us is not a space
            if (char.IsUpper(INPUT[i]) && INPUT[i - 1] != ' ')
            {
                newText.Append(' ');
            }
            newText.Append(INPUT[i]);
        }

        return newText.ToString();
    }
}
