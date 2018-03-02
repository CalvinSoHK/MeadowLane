using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Class that contains all information for a given event so it can be incorporated into the schedule.
/// </summary>
[System.Serializable]
public class EventClass : MonoBehaviour {

    //Name of the event
    public string NAME;

    //Importance weight
    public float IMPORTANCE_WEIGHT;

    // How this event occurs type
    // Singular : Happens only once
    // Multi: Happens over a period of days
    // Repeat: Happens repeatedly
    public enum TIME_TYPE { Singular, Multi, Repeat};
    public TIME_TYPE TIME;

    //Date for when this event should occur
    public Date START_DATE, END_DATE;

    public enum REPEAT_TYPE { None, Weekend, Week, Mondays, Tuesdays, Wednesdays, Thursdays, Fridays, Saturdays, Sundays, Odd, Even, Rain, Snow, Sunny, FirstOfTheMonth, LastOfTheMonth };
    public REPEAT_TYPE REPEAT;

    //The type of event, used in comparisons to know if we can overide this event
    public enum EVENT_TYPE { None, Stock };
    public EVENT_TYPE TYPE;

    //Whether or not this event is constricted to a given time.
    public enum DAY_TYPE { AllDay, Morning, Noon, Evening, Night, Specific};
    public DAY_TYPE DAY;

    //If we want a specific time in day doesn't fall into the normal time settings.
    public float TIME_START, TIME_END;

    //If we want an event to not be guaranteed 
    public enum CONSISTENCY_TYPE { Consistent, Chance };
    public CONSISTENCY_TYPE CONSISTENCY;

    //The chance the event will occur on a day that it can occur on
    public float CHANCE;

    //Whether or not we can override this event with an event of the same EVENT_TYPE
    public enum OVERRIDEABLE_TYPE { Overrideable, NonOverrideable };
    public OVERRIDEABLE_TYPE OVERRIDEABLE;

    //Whether or not we can override the weather on the given day
    public enum WEATHER_OVERRIDEABLE_TYPE { Overrideable, NonOverrideable };
    public WEATHER_OVERRIDEABLE_TYPE WEATHER_OVERRIDEABLE;

    //What weather we are going to enforce
    public enum WEATHER_TYPE { Sunny, Rainy, Snowing, Misty, Thunderstorm, Cloudy, Hail };
    public WEATHER_TYPE WEATHER;
}
/*
#if UNITY_EDITOR
[CustomEditor(typeof(EventClass))]
public class EventClassInspector : Editor
{

    public override void OnInspectorGUI()
    {
        EventClass 

        SerializedProperty name = prop.FindPropertyRelative("NAME");
        SerializedProperty import_weight = prop.FindPropertyRelative("IMPORTANCE_WEIGHT");
        SerializedProperty time = prop.FindPropertyRelative("TIME");
        SerializedProperty start_date = prop.FindPropertyRelative("START_DATE");
        SerializedProperty end_date = prop.FindPropertyRelative("END_DATE");
        SerializedProperty repeat = prop.FindPropertyRelative("REPEAT");
        SerializedProperty type = prop.FindPropertyRelative("TYPE");
        SerializedProperty day = prop.FindPropertyRelative("DAY");
        SerializedProperty time_start = prop.FindPropertyRelative("TIME_START");
        SerializedProperty time_end = prop.FindPropertyRelative("TIME_END");
        SerializedProperty consistency = prop.FindPropertyRelative("CONSISTENCY");
        SerializedProperty chance = prop.FindPropertyRelative("CHANCE");
        SerializedProperty overrideable = prop.FindPropertyRelative("OVERRIDEABLE");
        SerializedProperty weather_overrideable = prop.FindPropertyRelative("WEATHER_OVERRIDEABLE");
        SerializedProperty weather = prop.FindPropertyRelative("WEATHER");

        EditorGUI.BeginProperty(position, label, prop);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        //Display name
        var nameRect = new Rect(position.x, position.y, 30, position.height);
        EditorGUI.PropertyField(nameRect, name, GUIContent.none);


        //Depending on the time type of the event we have to show different time opt ions
        if(time.enumValueIndex == (int)(EventClass.TIME_TYPE.Singular))
        {

        }
        else if(time.enumValueIndex == (int)(EventClass.TIME_TYPE.Repeat))
        {

        }
        else if(time.enumValueIndex == (int)(EventClass.TIME_TYPE.Multi))
        {

        }

        EditorGUI.EndProperty();
    }


}
#endif*/