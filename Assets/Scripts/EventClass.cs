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
    [Range(0.1f, 1.0f)]
    public float IMPORTANCE_WEIGHT;

    // How this event occurs type
    // Singular : Happens only once
    // Multi: Happens over a period of days
    // Repeat: Happens repeatedly
    public enum OCCURENCE_TYPE { Singular, Multi, Repeat};
    public OCCURENCE_TYPE TIME;

    //Whether this event should be a numbered day in a given month or a named day of that month
    public enum DATE_DAY_TYPE { Number, Named};
    public DATE_DAY_TYPE DATE_DAY_START, DATE_DAY_END;

    //If we are a number day which number is it
    public int DAY_NUMBER_START, DAY_NUMBER_END;

    //If we are a named day, which name is it, and which named one of the month is it
    public Scheduler.Day DAY_NAME_START, DAY_NAME_END;
    [Range(1, 4)]
    public int DAY_NAMED_NUMBER_START, DAY_NAMED_NUMBER_END;

    //Date for when this event should occur
    public Scheduler.Month START_MONTH, END_MONTH;

    public enum REPEAT_TYPE { None, Weekend, Week, Mondays, Tuesdays, Wednesdays, Thursdays, Fridays, Saturdays, Sundays, Odd, Even, Sunny, Rain, Snow, Misty, Thunderstorm, Cloudy, Hail, FirstOfTheMonth, LastOfTheMonth, SpecificDays};
    public REPEAT_TYPE REPEAT;

    //The type of event, used in comparisons to know if we can overide this event
    public enum EVENT_TYPE { None, Stock, Message};
    public EVENT_TYPE TYPE;

    //Whether or not this event should fire off in a specific scene
    public enum SCENE_SPECIFIC { NonSpecific, Specific };
    public SCENE_SPECIFIC SCENE_TYPE;
    //Scene that this event fires off in
    public enum SCENES { PlayerHome, HappyMart, TownSquare };
    public SCENES SCENE;

    //Whether or not this event is constricted to a given time.
    public enum TIME_TYPE { AllDay, Morning, Noon, Evening, Night, Midnight, Wake, Sleep, Specific};
    public TIME_TYPE DAY;

    //If we want a specific time in day doesn't fall into the normal time settings.
    
    public int TIME_START, TIME_END;
    [HideInInspector]
    public int TIME_START_HOUR, TIME_END_HOUR;

    //If we want an event to not be guaranteed 
    public enum CONSISTENCY_TYPE { Consistent, Chance };
    public CONSISTENCY_TYPE CONSISTENCY;

    //The chance the event will occur on a day that it can occur on
    public float CHANCE = 1;

    //Whether or not we can override this event with an event of the same EVENT_TYPE
    public enum OVERRIDEABLE_TYPE { Overrideable, NonOverrideable };
    public OVERRIDEABLE_TYPE OVERRIDEABLE;

    //Whether or not we can override the weather on the given day
    public enum WEATHER_OVERRIDEABLE_TYPE { Overrideable, NonOverrideable };
    public WEATHER_OVERRIDEABLE_TYPE WEATHER_OVERRIDEABLE;

    //What weather we are going to enforce
    public enum WEATHER_TYPE { None, Sunny, Rain, Snow, Misty, Thunderstorm, Cloudy, Hail };
    public WEATHER_TYPE WEATHER;

    //Whether or not this event is only past the first year (i.e. NG+)
    public enum YEAR_TYPE { Default, NewGamePlus };
    public YEAR_TYPE YEAR;
}

#if UNITY_EDITOR
[CustomEditor(typeof(EventClass))]
public class EventClassInspector : Editor
{
    SerializedObject obj;

    private void OnEnable()
    {
        obj = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        EventClass script = (EventClass)target;
        

        //Display name
        GUIContent name_label = new GUIContent("Name", "The given name for the event");

        //Get the values we need
        SerializedProperty NAME = obj.FindProperty("NAME");
        NAME.stringValue = EditorGUILayout.TextField(name_label, NAME.stringValue);

        //Display importance weight
        GUIContent weight_label = new GUIContent("Weight", "The priority of this event against others");
        
        script.IMPORTANCE_WEIGHT = EditorGUILayout.Slider(weight_label, script.IMPORTANCE_WEIGHT, 0.01f, 1.0f);

        //Display the type of event this is
        GUIContent eventtype_label = new GUIContent("Event Type", "The category we use to filter events");
        script.TYPE = (EventClass.EVENT_TYPE)EditorGUILayout.EnumPopup(eventtype_label, script.TYPE);

        EditorGUI.indentLevel++;
        if (script.TYPE != EventClass.EVENT_TYPE.None)
        {
            GUIContent overrideable_label = new GUIContent("Overrideable", "Whether or not this event can be overridden");
            script.OVERRIDEABLE = (EventClass.OVERRIDEABLE_TYPE)EditorGUILayout.EnumPopup(overrideable_label, script.OVERRIDEABLE);
        }
        EditorGUI.indentLevel--;

        //Display the scene type the event is on
        GUIContent scenetype_label = new GUIContent("Scene Type", "The scene type this event is");
        script.SCENE_TYPE = (EventClass.SCENE_SPECIFIC)EditorGUILayout.EnumPopup(scenetype_label, script.SCENE_TYPE);

        EditorGUI.indentLevel += 1;
        if(script.SCENE_TYPE == EventClass.SCENE_SPECIFIC.Specific)
        {
            GUIContent scenespecific_label = new GUIContent("Specific Scene", "The specific scene this event will occur on");
            script.SCENE = (EventClass.SCENES)EditorGUILayout.EnumPopup(scenespecific_label, script.SCENE);
        }
        EditorGUI.indentLevel -= 1;

        //Display time choice
        GUIContent time_label = new GUIContent("Time Occurence", "When on the given days the event will occur on");
        script.DAY = (EventClass.TIME_TYPE)EditorGUILayout.EnumPopup(time_label, script.DAY);

        if(script.DAY == EventClass.TIME_TYPE.Specific)
        {
            GUIContent timestart_label = new GUIContent("Time Start", "The time when this event will start");
            GUIContent timeend_label = new GUIContent("Time End", "The time when this event will end");
            script.TIME_START_HOUR = EditorGUILayout.IntSlider(timestart_label, script.TIME_START_HOUR, 8, 24);
            script.TIME_END_HOUR = EditorGUILayout.IntSlider(timeend_label, script.TIME_END_HOUR, script.TIME_START_HOUR, 24);
            script.TIME_START = script.TIME_START_HOUR * 3600;
            script.TIME_END = script.TIME_END_HOUR * 3600;
        }

        //Display day choice
        GUIContent day_label = new GUIContent("Day Occurence", "The type of days this event will occur on");
        script.TIME = (EventClass.OCCURENCE_TYPE)EditorGUILayout.EnumPopup(day_label, script.TIME);

        EditorGUI.indentLevel += 1;

        //Depending on the time type of the event we have to show different time opt ions
        if (script.TIME == EventClass.OCCURENCE_TYPE.Singular)
        {
            //Display just the time_start field
            GUIContent startmonth_label = new GUIContent("Month", "The month that the event will start in");
            GUIContent daytype_label = new GUIContent("Day Type", "Whether to use a named day or a numbered day");
            GUIContent startday_label = new GUIContent("Day", "The day that the event will start in");
            script.START_MONTH = (Scheduler.Month)EditorGUILayout.EnumPopup(startmonth_label, script.START_MONTH);
            script.DATE_DAY_START = (EventClass.DATE_DAY_TYPE)EditorGUILayout.EnumPopup(daytype_label, script.DATE_DAY_START);
            if (script.DATE_DAY_START == EventClass.DATE_DAY_TYPE.Named)
            {
                script.DAY_NAME_START = (Scheduler.Day)EditorGUILayout.EnumPopup(script.DAY_NAME_START);
                script.DAY_NAMED_NUMBER_START = EditorGUILayout.IntSlider(startday_label, script.DAY_NAMED_NUMBER_START, 1, 4);
            }
            else if (script.DATE_DAY_START == EventClass.DATE_DAY_TYPE.Number)
            {
                script.DAY_NUMBER_START = DisplayDays(script, script.DAY_NUMBER_START, script.START_MONTH, startday_label);
            }
        }
        else if (script.TIME == EventClass.OCCURENCE_TYPE.Repeat)
        {
            //Display the choices for repeat
            GUIContent repeat_label = new GUIContent("Repeat", "The days this event will repeat on");
            GUIContent repeatdaytype_label = new GUIContent("Repeat Day Type", "Whether to use a named day or a numbered day to repeat");
            //GUIContent startday_label = new GUIContent("Start Day", "The day that the event will start in, inclusive");
            //GUIContent endday_label = new GUIContent("End Day", "The day that the event will end on, inclusive");        
            script.REPEAT = (EventClass.REPEAT_TYPE)EditorGUILayout.EnumPopup(repeat_label, script.REPEAT);
            if (script.REPEAT == EventClass.REPEAT_TYPE.SpecificDays)
            {
                GUIContent daytype_label = new GUIContent("Day Type", "Whether to use a named day or a numbered day");
                GUIContent startday_label = new GUIContent("Day", "The day that the event will repoeat on");
                script.DATE_DAY_START = (EventClass.DATE_DAY_TYPE)EditorGUILayout.EnumPopup(daytype_label, script.DATE_DAY_START);
                if (script.DATE_DAY_START == EventClass.DATE_DAY_TYPE.Named)
                {
                    script.DAY_NAME_START = (Scheduler.Day)EditorGUILayout.EnumPopup(script.DAY_NAME_START);
                    script.DAY_NAMED_NUMBER_START = EditorGUILayout.IntSlider(startday_label, script.DAY_NAMED_NUMBER_START, 1, 4);
                }
                else if (script.DATE_DAY_START == EventClass.DATE_DAY_TYPE.Number)
                {
                    script.DAY_NUMBER_START = DisplayDays(script, script.DAY_NUMBER_START, script.START_MONTH, startday_label);
                }
            }
        }
        else if (script.TIME == EventClass.OCCURENCE_TYPE.Multi)
        {
            //Display just the time_start field
            GUIContent startmonth_label = new GUIContent("Start Month", "The month that the event will start in");
            GUIContent startdaytype_label = new GUIContent("Start Day Type", "Whether to use a named day or a numbered day");
            GUIContent startday_label = new GUIContent("Start Day", "The day that the event will start in");
            GUIContent endmonth_label = new GUIContent("End Month", "The month that the event will end on");
            GUIContent enddaytype_label = new GUIContent("End Day Type", "Whether to use a named or a numbered day");
            GUIContent endday_label = new GUIContent("End Day", "The day that the event will end on");

            script.START_MONTH = (Scheduler.Month)EditorGUILayout.EnumPopup(startmonth_label, script.START_MONTH);
            script.DATE_DAY_START = (EventClass.DATE_DAY_TYPE)EditorGUILayout.EnumPopup(startdaytype_label, script.DATE_DAY_START);
            if (script.DATE_DAY_START == EventClass.DATE_DAY_TYPE.Named)
            {
                script.DAY_NAME_START = (Scheduler.Day)EditorGUILayout.EnumPopup(script.DAY_NAME_START);
                script.DAY_NAMED_NUMBER_START = EditorGUILayout.IntSlider(startday_label, script.DAY_NAMED_NUMBER_START, 1, 4);
            }
            else if (script.DATE_DAY_START == EventClass.DATE_DAY_TYPE.Number)
            {
                script.DAY_NUMBER_START = DisplayDays(script, script.DAY_NUMBER_START, script.START_MONTH, startday_label);
            }

            script.END_MONTH = (Scheduler.Month)EditorGUILayout.EnumPopup(endmonth_label, script.END_MONTH);
            script.DATE_DAY_END = (EventClass.DATE_DAY_TYPE)EditorGUILayout.EnumPopup(enddaytype_label, script.DATE_DAY_END);
            if (script.DATE_DAY_END == EventClass.DATE_DAY_TYPE.Named)
            {
                script.DAY_NAME_END = (Scheduler.Day)EditorGUILayout.EnumPopup(script.DAY_NAME_END);
                script.DAY_NAMED_NUMBER_END = EditorGUILayout.IntSlider(endday_label, script.DAY_NAMED_NUMBER_END, 1, 4);
            }
            else if (script.DATE_DAY_END == EventClass.DATE_DAY_TYPE.Number)
            {
                script.DAY_NUMBER_END = DisplayDays(script, script.DAY_NUMBER_END, script.END_MONTH, endday_label);
            }
        }
        EditorGUI.indentLevel -= 1;

        GUIContent year_label = new GUIContent("Year", "Whether or not it should appear in year one or only after");
        script.YEAR = (EventClass.YEAR_TYPE)EditorGUILayout.EnumPopup(year_label, script.YEAR);

        GUIContent consistency_label = new GUIContent("Consistency", "Whether or not the event should have a chance of occuring or not");
        script.CONSISTENCY = (EventClass.CONSISTENCY_TYPE)EditorGUILayout.EnumPopup(consistency_label, script.CONSISTENCY);
        if(script.CONSISTENCY == EventClass.CONSISTENCY_TYPE.Chance)
        {
            GUIContent chance_label = new GUIContent("Chance", "What the chance of this event occuring is");
            script.CHANCE = EditorGUILayout.Slider(chance_label, script.CHANCE, 0.01f, 0.99f);
        }
        else
        {
            script.CHANCE = 1;
        }

        GUIContent weather_label = new GUIContent("Weather", "What weather should this event enforce");
        script.WEATHER = (EventClass.WEATHER_TYPE)EditorGUILayout.EnumPopup(weather_label.text, script.WEATHER);

        EditorGUI.indentLevel++;
        if(script.WEATHER != EventClass.WEATHER_TYPE.None)
        {
            GUIContent weatheroverride_label = new GUIContent("Weather Override", "Whether the weather it enforces can be overriden by other events");
            script.WEATHER_OVERRIDEABLE = (EventClass.WEATHER_OVERRIDEABLE_TYPE)EditorGUILayout.EnumPopup(weatheroverride_label, script.WEATHER_OVERRIDEABLE);
        }
        EditorGUI.indentLevel--;
    }

    public int DisplayDays(EventClass script, int field, Scheduler.Month month, GUIContent label)
    {
        int MAX = 31;
        switch (month)
        {
            case Scheduler.Month.None:
                MAX = 30;
                break;
            case Scheduler.Month.January:
                MAX = 31;
                break;
            case Scheduler.Month.February:
                MAX = 28;
                break;
            case Scheduler.Month.March:
                MAX = 31;
                break;
            case Scheduler.Month.April:
                MAX = 30;
                break;
            case Scheduler.Month.May:
                MAX = 31;
                break;
            case Scheduler.Month.June:
                MAX = 30;
                break;
            case Scheduler.Month.July:
                MAX = 31;
                break;
            case Scheduler.Month.August:
                MAX = 31;
                break;
            case Scheduler.Month.September:
                MAX = 30;
                break;
            case Scheduler.Month.October:
                MAX = 31;
                break;
            case Scheduler.Month.November:
                MAX = 30;
                break;
            case Scheduler.Month.December:
                MAX = 31;
                break;
            default:
                MAX = 30;
                Debug.Log("Error, not a proper month. " + month);
                break;
        }

        return EditorGUILayout.IntSlider(label, field, 1, MAX);
    }

}
#endif