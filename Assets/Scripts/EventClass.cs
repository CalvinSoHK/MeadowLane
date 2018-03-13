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
    public enum EVENT_TYPE { None, Stock, Message, Deco};
    public EVENT_TYPE TYPE;

    //Whether or not this event should fire off in a specific scene
    public enum SCENE_SPECIFIC { NonSpecific, Specific };
    public SCENE_SPECIFIC SCENE_TYPE;
    //Scene that this event fires off in
    public enum SCENES { None, PlayerHome, HappyMart, TownSquare };
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

    /// <summary>
    /// returns the int/string value of the time start (based on the type given by the event)
    /// Sleeping is 0
    /// Waking is 1
    /// Both need numbers because we need to parse int later.
    /// </summary>
    /// <param name="currentType"></param>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    public string getTimeStart(EventClass.TIME_TYPE currentType, EventClass currentEvent)
    {
        string RET = getTimeStartNoEvent(currentType);
        if (RET.Length <= 0)
        {
            return currentEvent.TIME_START.ToString();
        }
        else
        {
            return RET;
        }
    }

    /// <summary>
    /// Returns a start time as a string for given presets.
    /// </summary>
    /// <param name="currentType"></param>
    /// <returns></returns>
    public string getTimeStartNoEvent(EventClass.TIME_TYPE currentType)
    {
        switch (currentType)
        {
            case EventClass.TIME_TYPE.AllDay:
                return "28800";
            case EventClass.TIME_TYPE.Evening:
                return "57600";
            case EventClass.TIME_TYPE.Morning:
                return "28800";
            case EventClass.TIME_TYPE.Night:
                return "72000";
            case EventClass.TIME_TYPE.Noon:
                return "43200";
            case EventClass.TIME_TYPE.Sleep:
                return "0";
            case EventClass.TIME_TYPE.Wake:
                return "1";
            case EventClass.TIME_TYPE.Midnight:
                return "2";
        }
        return "";
    }

    /// <summary>
    /// returns the int/string value of the time end (based on the type given by the event)
    /// </summary>
    /// <param name="currentType"></param>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    public string getTimeEnd(EventClass.TIME_TYPE currentType, EventClass currentEvent)
    {
        string RET = getTimeEndNoEvent(currentType);
        if (RET.Length <= 0)
        {
            return currentEvent.TIME_END.ToString();
        }
        else
        {
            return RET;
        }
    }

    /// <summary>
    /// Returns a string end time for a given preset
    /// </summary>
    /// <param name="currentType"></param>
    /// <returns></returns>
    public string getTimeEndNoEvent(EventClass.TIME_TYPE currentType)
    {
        switch (currentType)
        {
            case EventClass.TIME_TYPE.AllDay:
                return "79200";
            case EventClass.TIME_TYPE.Evening:
                return "72000";
            case EventClass.TIME_TYPE.Morning:
                return "43200";
            case EventClass.TIME_TYPE.Night:
                return "79200";
            case EventClass.TIME_TYPE.Noon:
                return "57600";
            case EventClass.TIME_TYPE.Sleep:
                return "0";
            case EventClass.TIME_TYPE.Wake:
                return "1";
            case EventClass.TIME_TYPE.Midnight:
                return "2";
        }
        return "";
    }
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

        obj.Update();

        //Weather property. Need it for different if statements.
        SerializedProperty WEATHER = obj.FindProperty("WEATHER");

        //REpeat property. Need it for different if statements.
        SerializedProperty REPEAT = obj.FindProperty("REPEAT");

        //Display name
        GUIContent name_label = new GUIContent("Name", "The given name for the event");

        //Get the values we need
        SerializedProperty NAME = obj.FindProperty("NAME");
        NAME.stringValue = EditorGUILayout.TextField(name_label, NAME.stringValue);

        //Display importance weight
        GUIContent weight_label = new GUIContent("Weight", "The priority of this event against others");
        SerializedProperty IMPORTANCE_WEIGHT = obj.FindProperty("IMPORTANCE_WEIGHT");
        IMPORTANCE_WEIGHT.floatValue = EditorGUILayout.Slider(weight_label, IMPORTANCE_WEIGHT.floatValue, 0.01f, 1.0f);

        //Display the type of event this is
        GUIContent eventtype_label = new GUIContent("Event Type", "The category we use to filter events");
        SerializedProperty TYPE = obj.FindProperty("TYPE");
        TYPE.enumValueIndex = (int)(EventClass.EVENT_TYPE)EditorGUILayout.EnumPopup(eventtype_label, (EventClass.EVENT_TYPE)(TYPE.enumValueIndex));

        EditorGUI.indentLevel++;
        if (TYPE.enumValueIndex != (int)EventClass.EVENT_TYPE.None)
        {
            GUIContent overrideable_label = new GUIContent("Overrideable", "Whether or not this event can be overridden");
            SerializedProperty OVERRIDEABLE = obj.FindProperty("OVERRIDEABLE");
            OVERRIDEABLE.enumValueIndex = (int)(EventClass.OVERRIDEABLE_TYPE)EditorGUILayout.EnumPopup(overrideable_label, (EventClass.OVERRIDEABLE_TYPE)(OVERRIDEABLE.enumValueIndex));
        }
        EditorGUI.indentLevel--;
        
        //Display the scene type the event is on
        GUIContent scenetype_label = new GUIContent("Scene Type", "The scene type this event is");
        SerializedProperty SCENE_TYPE = obj.FindProperty("SCENE_TYPE");
        SCENE_TYPE.enumValueIndex = (int)(EventClass.SCENE_SPECIFIC)EditorGUILayout.EnumPopup(scenetype_label, (EventClass.SCENE_SPECIFIC)(SCENE_TYPE.enumValueIndex));

        EditorGUI.indentLevel += 1;
        if(SCENE_TYPE.enumValueIndex == (int)EventClass.SCENE_SPECIFIC.Specific)
        {
            GUIContent scenespecific_label = new GUIContent("Specific Scene", "The specific scene this event will occur on");
            SerializedProperty SCENE = obj.FindProperty("SCENE");
            SCENE.enumValueIndex = (int)(EventClass.SCENES)EditorGUILayout.EnumPopup(scenespecific_label, (EventClass.SCENES)(SCENE.enumValueIndex));
        }
        else if (SCENE_TYPE.enumValueIndex == (int)EventClass.SCENE_SPECIFIC.NonSpecific)
        {
            SerializedProperty SCENE = obj.FindProperty("SCENE");
            SCENE_TYPE.enumValueIndex = (int)(EventClass.SCENES.None);
        }
        EditorGUI.indentLevel -= 1;

        //Display time choice
        GUIContent time_label = new GUIContent("Time Occurence", "When on the given days the event will occur on");
        SerializedProperty DAY = obj.FindProperty("DAY");
        SerializedProperty TIME_START_HOUR = obj.FindProperty("TIME_START_HOUR");
        SerializedProperty TIME_END_HOUR = obj.FindProperty("TIME_END_HOUR");
        SerializedProperty TIME_START = obj.FindProperty("TIME_START");
        SerializedProperty TIME_END = obj.FindProperty("TIME_END");

        DAY.enumValueIndex = (int)(EventClass.TIME_TYPE)EditorGUILayout.EnumPopup(time_label, (EventClass.TIME_TYPE)(DAY.enumValueIndex));

        if(DAY.enumValueIndex == (int)EventClass.TIME_TYPE.Specific)
        {
            GUIContent timestart_label = new GUIContent("Time Start", "The time when this event will start");
            GUIContent timeend_label = new GUIContent("Time End", "The time when this event will end");
            TIME_START_HOUR.intValue = EditorGUILayout.IntSlider(timestart_label, TIME_START_HOUR.intValue, 8, 24);
            TIME_END_HOUR.intValue = EditorGUILayout.IntSlider(timeend_label, TIME_END_HOUR.intValue, TIME_START_HOUR.intValue, 24);
            TIME_START.intValue = TIME_START_HOUR.intValue * 3600;
            TIME_END.intValue = TIME_END_HOUR.intValue * 3600;
        }
        else
        {
            TIME_START.intValue = int.Parse(((EventClass)target).getTimeStartNoEvent((EventClass.TIME_TYPE)DAY.enumValueIndex));
            TIME_END.intValue = int.Parse(((EventClass)target).getTimeEndNoEvent((EventClass.TIME_TYPE)DAY.enumValueIndex));
        }

        //Convert inputs into the seconds for scheduler.
   

        //Display day choice
        GUIContent day_label = new GUIContent("Day Occurence", "The type of days this event will occur on");
        SerializedProperty TIME = obj.FindProperty("TIME");
        TIME.enumValueIndex = (int)(EventClass.OCCURENCE_TYPE)EditorGUILayout.EnumPopup(day_label, (EventClass.OCCURENCE_TYPE)(TIME.enumValueIndex));

        EditorGUI.indentLevel += 1;

        //Depending on the time type of the event we have to show different time opt ions
        if (TIME.enumValueIndex == (int)EventClass.OCCURENCE_TYPE.Singular)
        {
            //Display just the time_start field
            GUIContent startmonth_label = new GUIContent("Month", "The month that the event will occur in");
            GUIContent daytype_label = new GUIContent("Day Type", "Whether to use a named day or a numbered day");
            GUIContent startday_label = new GUIContent("Day", "The day that the event will occur in");
            SerializedProperty START_MONTH = obj.FindProperty("START_MONTH");
            SerializedProperty DATE_DAY_START = obj.FindProperty("DATE_DAY_START");
            START_MONTH.enumValueIndex = (int)(Scheduler.Month)EditorGUILayout.EnumPopup(startmonth_label, (Scheduler.Month)(START_MONTH.enumValueIndex));
            DATE_DAY_START.enumValueIndex = (int)(EventClass.DATE_DAY_TYPE)EditorGUILayout.EnumPopup(daytype_label, (EventClass.DATE_DAY_TYPE)DATE_DAY_START.enumValueIndex);
            if (DATE_DAY_START.enumValueIndex == (int)EventClass.DATE_DAY_TYPE.Named)
            {
                GUIContent startname_label = new GUIContent("Name", "The day of the week we occur on");
                GUIContent startnamednumber_label = new GUIContent("Numbered of Name", "The numbered occurence of the given name in a month the event should be on. i.e. The first Monday of January");
                SerializedProperty DAY_NAME_START = obj.FindProperty("DAY_NAME_START");
                SerializedProperty DAY_NAMED_NUMBER_START = obj.FindProperty("DAY_NAMED_NUMBER_START");
                DAY_NAME_START.enumValueIndex = (int)(Scheduler.Day)EditorGUILayout.EnumPopup(startname_label, (Scheduler.Day)DAY_NAME_START.enumValueIndex);
                DAY_NAMED_NUMBER_START.intValue = EditorGUILayout.IntSlider(startnamednumber_label, DAY_NAMED_NUMBER_START.intValue, 1, 4);
            }
            else if (DATE_DAY_START.enumValueIndex == (int)EventClass.DATE_DAY_TYPE.Number)
            {
                GUIContent startnumber_label = new GUIContent("Number Day", "The numbered day this event should occur on");
                SerializedProperty DAY_NUMBER_START = obj.FindProperty("DAY_NUMBER_START");
                DAY_NUMBER_START.intValue = DisplayDays(DAY_NUMBER_START.intValue, (Scheduler.Month)START_MONTH.enumValueIndex, startday_label);
            }
            REPEAT.enumValueIndex = (int)EventClass.REPEAT_TYPE.None;
        }
        else if (TIME.enumValueIndex == (int)EventClass.OCCURENCE_TYPE.Repeat)
        {
            //Display the choices for repeat
            GUIContent repeat_label = new GUIContent("Repeat", "The days this event will repeat on");
            //GUIContent repeatdaytype_label = new GUIContent("Repeat Day Type", "Whether to use a named day or a numbered day to repeat");
           
            REPEAT.enumValueIndex = (int)(EventClass.REPEAT_TYPE)EditorGUILayout.EnumPopup(repeat_label, (EventClass.REPEAT_TYPE)REPEAT.enumValueIndex);
            if (REPEAT.enumValueIndex == (int)EventClass.REPEAT_TYPE.SpecificDays)
            {
                GUIContent daytype_label = new GUIContent("Day Type", "Whether to use a named day or a numbered day");
                //GUIContent startday_label = new GUIContent("Day", "The day that the event will repoeat on");
                SerializedProperty DATE_DAY_START = obj.FindProperty("DATE_DAY_START");
                DATE_DAY_START.enumValueIndex = (int)(EventClass.DATE_DAY_TYPE)EditorGUILayout.EnumPopup(daytype_label, (EventClass.DATE_DAY_TYPE)DATE_DAY_START.enumValueIndex);
                if (DATE_DAY_START.enumValueIndex == (int)EventClass.DATE_DAY_TYPE.Named)
                {
                    GUIContent startname_label = new GUIContent("Name", "The day of the week we occur on");
                    GUIContent startnamednumber_label = new GUIContent("Numbered of Name", "The numbered occurence of the given name in a month the event should be on. i.e. The first Monday of January");
                    SerializedProperty DAY_NAME_START = obj.FindProperty("DAY_NAME_START");
                    SerializedProperty DAY_NAMED_NUMBER_START = obj.FindProperty("DAY_NAMED_NUMBER_START");
                    DAY_NAME_START.enumValueIndex = (int)(Scheduler.Day)EditorGUILayout.EnumPopup(startname_label, (Scheduler.Day)DAY_NAME_START.enumValueIndex);
                    DAY_NAMED_NUMBER_START.intValue = EditorGUILayout.IntSlider(startnamednumber_label, DAY_NAMED_NUMBER_START.intValue, 1, 4);
                }
                else if (DATE_DAY_START.enumValueIndex == (int)EventClass.DATE_DAY_TYPE.Number)
                {
                    GUIContent startnumber_label = new GUIContent("Number Day", "The numbered day this event should occur on. If the number doesn't occur in that month it won't occur.");
                    SerializedProperty DAY_NUMBER_START = obj.FindProperty("DAY_NUMBER_START");
                    DAY_NUMBER_START.intValue = DisplayDays(DAY_NUMBER_START.intValue, Scheduler.Month.December, startnumber_label);
                }
            }
            else if(REPEAT.enumValueIndex == (int)EventClass.REPEAT_TYPE.Rain)
            {
                WEATHER.enumValueIndex = (int)EventClass.WEATHER_TYPE.Rain;
            }
            else if(REPEAT.enumValueIndex == (int)EventClass.REPEAT_TYPE.Cloudy)
            {
                WEATHER.enumValueIndex = (int)EventClass.WEATHER_TYPE.Cloudy;
            }
            else if(REPEAT.enumValueIndex == (int)EventClass.REPEAT_TYPE.Hail)
            {
                WEATHER.enumValueIndex = (int)EventClass.WEATHER_TYPE.Hail;
            }
            else if(REPEAT.enumValueIndex == (int)EventClass.REPEAT_TYPE.Misty)
            {
                WEATHER.enumValueIndex = (int)EventClass.WEATHER_TYPE.Misty;
            }
            else if(REPEAT.enumValueIndex == (int)EventClass.REPEAT_TYPE.Snow)
            {
                WEATHER.enumValueIndex = (int)EventClass.REPEAT_TYPE.Snow;
            }
            else if(REPEAT.enumValueIndex == (int)EventClass.WEATHER_TYPE.Sunny)
            {
                WEATHER.enumValueIndex = (int)EventClass.REPEAT_TYPE.Sunny;
            }
            else if(REPEAT.enumValueIndex == (int)EventClass.WEATHER_TYPE.Thunderstorm)
            {
                WEATHER.enumValueIndex = (int)EventClass.WEATHER_TYPE.Thunderstorm;
            }
        }
        else if (TIME.enumValueIndex == (int)EventClass.OCCURENCE_TYPE.Multi)
        {
            //Display just the time_start field
            GUIContent startmonth_label = new GUIContent("Start Month", "The month that the event will start in");
            GUIContent startdaytype_label = new GUIContent("Start Day Type", "Whether to use a named day or a numbered day");
            //GUIContent startday_label = new GUIContent("Start Day", "The day that the event will start in");
           
            SerializedProperty START_MONTH = obj.FindProperty("START_MONTH");
            SerializedProperty DATE_DAY_START = obj.FindProperty("DATE_DAY_START");
            START_MONTH.enumValueIndex = (int)(Scheduler.Month)EditorGUILayout.EnumPopup(startmonth_label, (Scheduler.Month)START_MONTH.enumValueIndex);
            DATE_DAY_START.enumValueIndex = (int)(EventClass.DATE_DAY_TYPE)EditorGUILayout.EnumPopup(startdaytype_label, (EventClass.DATE_DAY_TYPE)DATE_DAY_START.enumValueIndex);
            if (DATE_DAY_START.enumValueIndex == (int)EventClass.DATE_DAY_TYPE.Named)
            {
                GUIContent startname_label = new GUIContent("Name", "The day of the week we begin on");
                GUIContent startnamednumber_label = new GUIContent("Numbered of Name", "The numbered occurence of the given name in a month the event should begin on. i.e. The first Monday of January");
                SerializedProperty DAY_NAME_START = obj.FindProperty("DAY_NAME_START");
                SerializedProperty DAY_NAMED_NUMBER_START = obj.FindProperty("DAY_NAMED_NUMBER_START");
                DAY_NAME_START.enumValueIndex = (int)(Scheduler.Day)EditorGUILayout.EnumPopup(startname_label, (Scheduler.Day)DAY_NAME_START.enumValueIndex);
                DAY_NAMED_NUMBER_START.intValue = EditorGUILayout.IntSlider(startnamednumber_label, DAY_NAMED_NUMBER_START.intValue, 1, 4);
            }
            else if (DATE_DAY_START.enumValueIndex == (int)EventClass.DATE_DAY_TYPE.Number)
            {
                GUIContent startnumber_label = new GUIContent("Number Day", "The numbered day this event should bein on. If the number doesn't occur in that month it won't occur.");
                SerializedProperty DAY_NUMBER_START = obj.FindProperty("DAY_NUMBER_START");
                DAY_NUMBER_START.intValue = DisplayDays(DAY_NUMBER_START.intValue, Scheduler.Month.December, startnumber_label);
            }

            GUIContent endmonth_label = new GUIContent("End Month", "The month that the event will end on");
            GUIContent enddaytype_label = new GUIContent("End Day Type", "Whether to use a named or a numbered day");
            SerializedProperty END_MONTH = obj.FindProperty("END_MONTH");
            SerializedProperty DATE_DAY_END = obj.FindProperty("DATE_DAY_END");
            END_MONTH.enumValueIndex = (int)(Scheduler.Month)EditorGUILayout.EnumPopup(endmonth_label, (Scheduler.Month)END_MONTH.enumValueIndex);
            DATE_DAY_END.enumValueIndex = (int)(EventClass.DATE_DAY_TYPE)EditorGUILayout.EnumPopup(enddaytype_label, (EventClass.DATE_DAY_TYPE)DATE_DAY_END.enumValueIndex);
            if (DATE_DAY_END.enumValueIndex == (int)EventClass.DATE_DAY_TYPE.Named)
            {
                GUIContent daynameend_label = new GUIContent("End Day Name", "The final day's name for this event.");
                GUIContent daynamenumberend_label = new GUIContent("End Day Number", "Which numbered occurence of the name in the given month our event ends on. i.e. The fourth Friday of January");
                SerializedProperty DAY_NAME_END = obj.FindProperty("DAY_NAME_END");
                SerializedProperty DAY_NAMED_NUMBER_END = obj.FindProperty("DAY_NAMED_NUMBER_END");
                DAY_NAME_END.enumValueIndex = (int)(Scheduler.Day)EditorGUILayout.EnumPopup(daynameend_label, (Scheduler.Day)DAY_NAME_END.enumValueIndex);
                DAY_NAMED_NUMBER_END.intValue = EditorGUILayout.IntSlider(daynamenumberend_label, DAY_NAMED_NUMBER_END.intValue, 1, 4);
            }
            else if (DATE_DAY_END.enumValueIndex == (int)EventClass.DATE_DAY_TYPE.Number)
            {
                GUIContent endday_label = new GUIContent("End Day", "The day that the event will end on");
                SerializedProperty DAY_NUMBER_END = obj.FindProperty("DAY_NUMBER_END");
                DAY_NUMBER_END.intValue = DisplayDays(DAY_NUMBER_END.intValue, (Scheduler.Month)END_MONTH.enumValueIndex, endday_label);
            }
            REPEAT.enumValueIndex = (int)EventClass.REPEAT_TYPE.None;
        }
        EditorGUI.indentLevel -= 1;

        GUIContent year_label = new GUIContent("Year", "Whether or not it should appear in year one or only after");
        SerializedProperty YEAR = obj.FindProperty("YEAR");
        YEAR.enumValueIndex = (int)(EventClass.YEAR_TYPE)EditorGUILayout.EnumPopup(year_label, (EventClass.YEAR_TYPE)YEAR.enumValueIndex);

        GUIContent consistency_label = new GUIContent("Consistency", "Whether or not the event should have a chance of occuring or not");
        SerializedProperty CONSISTENCY = obj.FindProperty("CONSISTENCY");
        CONSISTENCY.enumValueIndex = (int)(EventClass.CONSISTENCY_TYPE)EditorGUILayout.EnumPopup(consistency_label, (EventClass.CONSISTENCY_TYPE)CONSISTENCY.enumValueIndex);

        SerializedProperty CHANCE = obj.FindProperty("CHANCE");
        if (CONSISTENCY.enumValueIndex == (int)EventClass.CONSISTENCY_TYPE.Chance)
        {
            GUIContent chance_label = new GUIContent("Chance", "What the chance of this event occuring is");
            CHANCE.floatValue = EditorGUILayout.Slider(chance_label, CHANCE.floatValue, 0.01f, 0.99f);
        }
        else
        {
            CHANCE.floatValue = 1;
        }

        GUIContent weather_label = new GUIContent("Weather", "What weather should this event enforce");
        //SerializedProperty WEATHER = obj.FindProperty("WEATHER");

        //Show weather choices if we aren't repeating based on weather.
        if(REPEAT.enumValueIndex <= 11 || REPEAT.enumValueIndex >= 19)
        {
            WEATHER.enumValueIndex = (int)(EventClass.WEATHER_TYPE)EditorGUILayout.EnumPopup(weather_label, (EventClass.WEATHER_TYPE)WEATHER.enumValueIndex);
        }    

        EditorGUI.indentLevel++;
        if(WEATHER.enumValueIndex != (int)EventClass.WEATHER_TYPE.None)
        {
            GUIContent weatheroverride_label = new GUIContent("Weather Override", "Whether the weather it enforces can be overriden by other events");
            SerializedProperty WEATHER_OVERRIDEABLE = obj.FindProperty("WEATHER_OVERRIDEABLE");
            WEATHER_OVERRIDEABLE.enumValueIndex = (int)(EventClass.WEATHER_OVERRIDEABLE_TYPE)EditorGUILayout.EnumPopup(weatheroverride_label, (EventClass.WEATHER_OVERRIDEABLE_TYPE)WEATHER_OVERRIDEABLE.enumValueIndex);
        }
        EditorGUI.indentLevel--;
        obj.ApplyModifiedProperties();
    }

    public int DisplayDays( int field, Scheduler.Month month, GUIContent label)
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