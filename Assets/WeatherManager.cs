using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class WeatherManager : MonoBehaviour {

    //The weather we currently have
    public EventClass.WEATHER_TYPE WEATHER;

    //The object that our weather particle systems will spawn in
    public Transform WEATHER_OBJ;

    //The profile preset we are currently using
    public PostProcessingBehaviour PROFILE;

    //Based on weather enums
    //WEATHER_TYPE { None, Sunny, Rain, Snow, Misty, Thunderstorm, Cloudy, Hail };
    public List<PostProcessingProfile> PROFILES_IN_ORDER;

    //Particle systems
    public Transform[] WEATHER_PREFABS;

    public EventManager EM;

    //Updates weather based on what is in the WEATHER variable
    public void UpdateWeather()
    {
        if(EM == null)
        {
            EM = GameManagerPointer.Instance.EVENT_MANAGER_POINTER;
        }

        if(EM != null)
        {
            WEATHER = EM.GetWeather();
        }    

        if(WEATHER != EventClass.WEATHER_TYPE.None)
        {
            //If we have any particle systems going, kill them.
            if (WEATHER_OBJ.childCount > 0)
            {
                Destroy(WEATHER_OBJ.GetChild(0).gameObject);
            }

            //Set the profile to the right one. Minus one because we don't have one for the NONE enum.
            if(PROFILES_IN_ORDER[(int)WEATHER - 1] != null)
            {
                
                PROFILE.profile = PROFILES_IN_ORDER[(int)WEATHER - 1];
            }
            else
            {
                Debug.Log("Null " + (int)WEATHER);
            }

            //Spawn the right weather prefab. If it is null, that weather enum doesn't have a prefab object to spawn
            if (WEATHER_PREFABS[(int)WEATHER - 1] != null)
            {
                Instantiate(WEATHER_PREFABS[(int)WEATHER - 1], WEATHER_OBJ);
            }
        }
        else
        {
            Debug.Log("Error: Weather is none.");
        }
    }
}
