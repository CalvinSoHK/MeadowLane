using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Travel point manager. Has a list of all possible travel points.
public class TravelPointManager : MonoBehaviour {

    //Singleton code.
    private static TravelPointManager _instance;

    public static TravelPointManager Instance
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
            TravelPointController[] TPC_ARRAY = FindObjectsOfType<TravelPointController>();
            for (int i = 0; i < TPC_ARRAY.Length; i++)
            {
                TRAVEL_POINT_LIST.Add(TPC_ARRAY[i]);
            }
        }
    }


    //List of all travel points
    public List<TravelPointController> TRAVEL_POINT_LIST;

    //The current travel point the player is on
    //-1 is not valid.
    public int playerIndex = -1;

    //Function that gets the index of a given travel point controller
    //Returns negative 1 if it can't find it.
    public int GetIndex(TravelPointController TPC)
    {
        int index = 0;
        foreach (TravelPointController TEMP in TRAVEL_POINT_LIST)
        {
            if (TEMP == TPC)
            {
                return index;
            }
            else
            {
                index++;
            }
        }
        return -1;
    }

    //Function that sets the player index when called
    public void SetIndex(int i)
    {
        playerIndex = i;
    }

    //Function that resets the valid bool on a travel point if there is one to reset
    public void ResetValid()
    {
        if(playerIndex >= 0)
        {
            TRAVEL_POINT_LIST[playerIndex].isValid = true;
        }
    }

    


}
