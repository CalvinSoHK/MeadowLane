﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Linq;


/// <summary>
/// Compiles all events in our events folder into a series of text files.
/// </summary>
public class EventCompiler : MonoBehaviour {

    GameObject[] allPrefabEvents;
    EventClass[] allEventClass;
    //Dictionary<int, List<EventClass>> SceneSpecificEventRef = new Dictionary<int, List<EventClass>>();
    //Dictionary<int, EventClass> MainEventsForScenes = new Dictionary<int, EventClass>();

    Dictionary<int, Dictionary<int, List<EventClass>>> DATE_EventOrguanizer = new Dictionary<int, Dictionary<int, List<EventClass>>>();
    Dictionary<int, List<EventClass>> REPEAT_EventOrguanizer = new Dictionary<int, List<EventClass>>();
    Dictionary<string, List<EventClass>> Weather_Orguanizer = new Dictionary<string, List<EventClass>>();

    string EVENT_TXT_PATH, REPEAT_TXT_PATH, WEATHER_TXT_PATH;

    public void setTheEventTextFile()
    {
        DATE_EventOrguanizer.Clear();
        REPEAT_EventOrguanizer.Clear();
        Weather_Orguanizer.Clear();
        EVENT_TXT_PATH = Application.dataPath + "/Resources/Events/Numbered.txt";
        REPEAT_TXT_PATH = Application.dataPath + "/Resources/Events/Repeat.txt";
        WEATHER_TXT_PATH = Application.dataPath + "/Resources/Events/Weather.txt";

        allPrefabEvents = Resources.LoadAll("Events".Trim(), typeof(GameObject)).Cast<GameObject>().ToArray();//get all the prefab event game objects
        //Debug.Log(allPrefabEvents.Count());
        Debug.Log("Started Compiling Events");
        InitEventClass();
        OrguanizeByDay();
        SetRelevantEventsPerDay();
        SetRelevantRepeatEvents();
        WriteAllEventsIntoTheTextFile();
        Debug.Log("Done Compiling Events");
    }

    /// <summary>
    /// Initialize the event class array to assign all of the prefabs' event class script into the event class array
    /// </summary>
    public void InitEventClass()
    {
        allEventClass = new EventClass[allPrefabEvents.Length]; //initialize the event class array
        for (int i = 0; i < allPrefabEvents.Length; i++) //assign all of the prefabs' event class script into the event class array
        {
            allEventClass[i] = allPrefabEvents[i].GetComponent<EventClass>();
        }
    }

    /// <summary>
    /// Looks at what occurence type the event is (singular, multi, or repeat) and will then add the event in the date_orguanizer in the appropriate month/date key value pair
    /// </summary>
    public void OrguanizeByDay()
    {
        for(int i = 0; i < allEventClass.Length; i++) //for loop to go through all the events
        {
            if(allEventClass[i].TIME == EventClass.OCCURENCE_TYPE.Singular) //check the occurence type, whether it is singular
            {
                addEvent(allEventClass[i], (int) allEventClass[i].START_MONTH, allEventClass[i].DAY_NUMBER_START); //add it to the dictionary (months/key) of dictionaries (day/key, eventList/value)
            }else if(allEventClass[i].TIME == EventClass.OCCURENCE_TYPE.Multi) //is it of occurence type multi
            {
                int SM = (int)allEventClass[i].START_MONTH; //get the starting month value
                int SD = allEventClass[i].DAY_NUMBER_START; //get the start day value
                int EM = (int)allEventClass[i].END_MONTH; //get the end date value
                int ED = allEventClass[i].DAY_NUMBER_END; //get the end date value
                int numberOfMonths = EM - SM; //get the number of month between the start and end month
                int daysInTheMonth; //how many days will we need to go through for this month.

                while (numberOfMonths != -1) //while there are still months for us to go through
                {

                    if(numberOfMonths == 0) //if we are on the last month
                    {
                        daysInTheMonth = ED; //we only need to go through the days until the end day
                    }else //otherwise
                    {
                        daysInTheMonth = GameManagerPointer.Instance.SCHEDULER.MonthLength[SM - 1]; //we need to go through all the days in the month
                    }

                    for (int j = SD; j <= daysInTheMonth; j ++) //we go through all the days
                    {
                        addEvent(allEventClass[i], SM, j); //and we add them to the Date_Orguanizer
                    }
                    SM += 1; //we need to move on to the next month
                    SD = 1; //we reset the start day to 1 (the start of the month)
                    numberOfMonths -= 1; //we have on less month to go through now
                }
            }
            else if(allEventClass[i].TIME == EventClass.OCCURENCE_TYPE.Repeat)
            {
                int weekDayEnumStartingIndex = 3; //weekdays index in the Event class repeat enum
                int totalWeekDay = 5; //how many weekdays to account for
                int weekendEnumStartingIndex = 8; //weekends index in the Event class repeat enum
                int totalWeekend = 2; //how many weekend days to account for
                switch (allEventClass[i].REPEAT) //switch, correct repeat event
                {
                    case EventClass.REPEAT_TYPE.Week:
                        for(int j = weekDayEnumStartingIndex; j < weekDayEnumStartingIndex + totalWeekDay; j++)
                        {
                            addEventRepeat(allEventClass[i], j); //add the event to all the weekday keys in dict
                        }
                        break;
                    case EventClass.REPEAT_TYPE.Weekend:
                        for(int j = weekendEnumStartingIndex; j < weekendEnumStartingIndex + totalWeekend; j++)
                        {
                            addEventRepeat(allEventClass[i], j); //add all events to weekend keys in dict
                        }
                        break;

                    case EventClass.REPEAT_TYPE.Mondays:
                    case EventClass.REPEAT_TYPE.Tuesdays:
                    case EventClass.REPEAT_TYPE.Wednesdays:
                    case EventClass.REPEAT_TYPE.Thursdays:
                    case EventClass.REPEAT_TYPE.Fridays:
                    case EventClass.REPEAT_TYPE.Saturdays:
                    case EventClass.REPEAT_TYPE.Sundays:
                    case EventClass.REPEAT_TYPE.Rain:
                    case EventClass.REPEAT_TYPE.Sunny:
                    case EventClass.REPEAT_TYPE.Snow:
                        addEventRepeat(allEventClass[i]); //add event to its corresponding 
                        break;
                }               
            }
        }
    }

    /// <summary>
    /// we need to add the currentEvent in the appropriate month and day whithin the DATE_Organizer
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <param name="theMonth"></param>
    /// <param name="theDay"></param>
    public void addEvent(EventClass currentEvent, int theMonth, int theDay)
    {
        if (!DATE_EventOrguanizer.ContainsKey(theMonth)) //if the organizer does not have a key for the specific month
        {
            DATE_EventOrguanizer.Add(theMonth, new Dictionary<int, List<EventClass>>()); //add the month
            DATE_EventOrguanizer[theMonth].Add(theDay, new List<EventClass>()); //create a key for the current day 
            DATE_EventOrguanizer[theMonth][theDay].Add(currentEvent); //add the event to the orguanizer on that month/day
        }else //we already have a key for that month
        {
            if (!DATE_EventOrguanizer[theMonth].ContainsKey(theDay)) //we don't have a key for that day
            {
                DATE_EventOrguanizer[theMonth].Add(theDay, new List<EventClass>()); //create a key for the current day 
                DATE_EventOrguanizer[theMonth][theDay].Add(currentEvent); //add the event to the orguanizer on that month/day
            }
            else //we also have a key for that day
            {
                DATE_EventOrguanizer[theMonth][theDay].Add(currentEvent); //add the event to the orguanizer on that month/day
            }
        }
    }

    /// <summary>
    /// works the same as the add Event function but is specific to events that have an occurence type of repeat.
    /// we need to add the currentEvent in the appropriate repeat type whithin the REPEAT_ORGANIZER
    /// </summary>
    /// <param name="currenEvent"></param>
    public void addEventRepeat(EventClass currenEvent) {
        if (!REPEAT_EventOrguanizer.ContainsKey((int)currenEvent.REPEAT)){
            REPEAT_EventOrguanizer.Add((int)currenEvent.REPEAT, new List<EventClass>());
            REPEAT_EventOrguanizer[(int)currenEvent.REPEAT].Add(currenEvent);
        }else
        {
            REPEAT_EventOrguanizer[(int)currenEvent.REPEAT].Add(currenEvent);
        }
    }

    public void addEventRepeat(EventClass currenEvent, int REPEAT)
    {
        if (!REPEAT_EventOrguanizer.ContainsKey(REPEAT))
        {
            REPEAT_EventOrguanizer.Add(REPEAT, new List<EventClass>());
            REPEAT_EventOrguanizer[REPEAT].Add(currenEvent);
        }
        else
        {
            REPEAT_EventOrguanizer[REPEAT].Add(currenEvent);
        }
    }

    /// <summary>
    /// goes through all of the events and figures out which ones overlap and are not relevant to this day
    /// </summary>
    public void SetRelevantEventsPerDay()
    {
        List<int> MonthKeys = new List<int>(this.DATE_EventOrguanizer.Keys); //get the month keys from the dictionary 
        for(int i = 0; i < MonthKeys.Count; i++) //goes through the keys
        {
            List<int> DayKeys = new List<int>(this.DATE_EventOrguanizer[MonthKeys[i]].Keys); //gets the days keys
            for(int j = 0; j < DayKeys.Count; j++) //goes through ech day for each month
            {
                SortList(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]]); //sorts the events in the day by importance
                for( int k = 0; k<DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Count - 1; k++) //goes through each event in this day (nested for loop to compare the events)
                {
                    if(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Count == 1) //if there is  no two items to compare
                    {
                        break; //no need to keep going on this day
                    }
                    int currentEventType = (int) DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].TYPE; //get the event type
                    int currentTimeOfDay = (int)DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].DAY; //get the time of day
                    int currentLocation = (int)DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].SCENE; //get the location
                    float currentChance = DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].CHANCE; //get the chance                   

                    for (int l = k + 1; l < DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Count; l++)
                    {
                        if (currentEventType == (int)EventClass.EVENT_TYPE.None) //if its of type none, move on to the next event
                        {
                            break;
                        }
                        else if (currentChance != 1) //if there is a percent chance of this event to happen, move on to the next event
                        {
                            
                            break;
                        }
                        
                        if (  currentLocation == (int)DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][l].SCENE) //if the current location of both events are the same
                        {
                            if (currentEventType == (int)DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][l].TYPE) //if the type are the same
                            {
                                if(checkIfEventTimeOverLap(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k], DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][l])) //if one of the two events is completely encapsulated by the other
                                {
                                    Debug.Log("we should remove one...");
                                    if((int)DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].OVERRIDEABLE == 1 && (int) DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][l].OVERRIDEABLE == 1) //if both are none overidable, error
                                    {
                                        Debug.Log("ERROR!! We are trying to add two events that are on the same day, same type, same TOD, and same Location  "
                                            + MonthKeys[i] + " Month " + DayKeys[DayKeys[j]] + "Days");
                                    }else if((int)DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][l].OVERRIDEABLE == 1) //if second event is non overidable
                                    {
                                        //remove the first one
                                        Debug.Log("objectWasRemoved_1");
                                        DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Remove(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]); //remove the main event 
                                        k -= 1; //go back one
                                        break; //break out to move to the next event
                                    }
                                    else //if second is overidable
                                    {
                                        //remove the current
                                        Debug.Log("objectWasRemoved_2");
                                        DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Remove(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][l]); //remove the current event
                                        l -= 1; //go back one                                                                             
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetRelevantRepeatEvents()
    {
        List<int> RepeatKeys = new List<int>(this.REPEAT_EventOrguanizer.Keys); //get the REPEAT keys from the dictionary 
        for(int i = 0; i < RepeatKeys.Count; i++)
        {
            for(int j = 0; j < REPEAT_EventOrguanizer[RepeatKeys[i]].Count - 1; j++)
            {
                int currentEventType = (int)REPEAT_EventOrguanizer[RepeatKeys[i]][j].TYPE; //get the event type
                int currentTimeOfDay = (int)REPEAT_EventOrguanizer[RepeatKeys[i]][j].DAY; //get the time of day
                int currentLocation = (int)REPEAT_EventOrguanizer[RepeatKeys[i]][j].SCENE; //get the location
                float currentChance = REPEAT_EventOrguanizer[RepeatKeys[i]][j].CHANCE; //get the chance
                for (int k = j+1; k< REPEAT_EventOrguanizer[RepeatKeys[i]].Count; k++)
                {
                    if (currentEventType == (int)EventClass.EVENT_TYPE.None) //if its of type none, move on to the next event
                    {
                        break;
                    }
                    else if (currentChance != 1) //if there is a percent chance of this event to happen, move on to the next event
                    {

                        break;
                    }

                    if (currentLocation == (int)REPEAT_EventOrguanizer[RepeatKeys[i]][k].SCENE) //if the current location of both events are the same
                    {
                        if (currentEventType == (int)REPEAT_EventOrguanizer[RepeatKeys[i]][k].TYPE) //if the type are the same
                        {
                            if (checkIfEventTimeOverLap(REPEAT_EventOrguanizer[RepeatKeys[i]][j], REPEAT_EventOrguanizer[RepeatKeys[i]][k])) //if one of the two events is completely encapsulated by the other
                            {
                                if ((int)REPEAT_EventOrguanizer[RepeatKeys[i]][j].OVERRIDEABLE == 1 && (int)REPEAT_EventOrguanizer[RepeatKeys[i]][k].OVERRIDEABLE == 1) //if both are none overidable, error
                                {
                                    Debug.Log("ERROR!! We are trying to add two events that are on the same day, same type, same TOD, and same Location  ");
                                }
                                else if ((int)REPEAT_EventOrguanizer[RepeatKeys[i]][k].OVERRIDEABLE == 1) //if second event is non overidable
                                {
                                    //remove the first one
                                    Debug.Log("objectWasRemoved_1");
                                    REPEAT_EventOrguanizer[RepeatKeys[i]].Remove(REPEAT_EventOrguanizer[RepeatKeys[i]][j]); //remove the main event 
                                    j -= 1; //go back one
                                    break; //break out to move to the next event
                                }
                                else //if second is overidable
                                {
                                    //remove the current
                                    Debug.Log("objectWasRemoved_2");
                                    REPEAT_EventOrguanizer[RepeatKeys[i]].Remove(REPEAT_EventOrguanizer[RepeatKeys[i]][k]); //remove the current event
                                    k -= 1; //go back one                                                                             
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// check if the times of two events overlap
    /// </summary>
    /// <param name="firstEvent"></param>
    /// <param name="secondEvent"></param>
    /// <returns></returns>
    public bool checkIfEventTimeOverLap(EventClass firstEvent, EventClass secondEvent)
    {
        firstEvent.TIME_START = int.Parse(getTimeStart(firstEvent.DAY, firstEvent));
        Debug.Log(firstEvent.TIME_START);
        firstEvent.TIME_END = int.Parse(getTimeEnd(firstEvent.DAY, firstEvent));
        secondEvent.TIME_START = int.Parse(getTimeStart(secondEvent.DAY, secondEvent));
        secondEvent.TIME_END = int.Parse(getTimeEnd(secondEvent.DAY, secondEvent));
         //Debug.Log("check overlap is happening");
        bool ev2_Ov = false; //event 2 non overidable?
        
        if((int)firstEvent.OVERRIDEABLE == 1)
        {
            ev2_Ov = true;
        }

        if (secondEvent.TIME_START >= firstEvent.TIME_START && secondEvent.TIME_END <= firstEvent.TIME_END) //if it is encapsulated
        {
            return true;
        }
        else if (secondEvent.TIME_END >= firstEvent.TIME_START && secondEvent.TIME_END <= firstEvent.TIME_END) //if it overlapse on one side
        {
            Debug.Log(firstEvent.NAME + "    " + secondEvent.NAME);
            if (ev2_Ov) //and event2 is non overidable
            {
                firstEvent.TIME_START = secondEvent.TIME_END + 1; //update event's 1 time
                return false;
            }
            Debug.Log(secondEvent.TIME_END);
            secondEvent.TIME_END = firstEvent.TIME_START - 1;// otherwise update event 2's time
            Debug.Log(secondEvent.TIME_END);
            return false;
        }
        else if (secondEvent.TIME_START >= firstEvent.TIME_START && secondEvent.TIME_START <= firstEvent.TIME_END) //do the same as above
        {
            
            if (ev2_Ov)
            {
                firstEvent.TIME_END = secondEvent.TIME_START - 1;
                return false;
            }
            secondEvent.TIME_START = firstEvent.TIME_END + 1;
            return false;
        }
        else
        {
            
            return false;
        }

    }

    /// <summary>
    /// will go through all of our events, and write all relevant information into a new text file
    /// </summary>
    public void WriteAllEventsIntoTheTextFile()
    {       
        //Note: Text file should read in this order
        //[] : Elements in [] have variable length depending on what type of event it is. As in singular, multi, or repeat
        //Chance : From 0.1 to 1.00. 1.00 means the event happens everytime
        //Time : Variable length depending on what type of event it is
        //Weather : Variable length. If none its just none, else it will have weather and overrideableweather
        //YearSetting : Whether this should happen in the first year or not
        //Overrideable : Whether this event can be overriden
        //ImportantWeight : Helps us sort it for priorities in dialogue and such
        //Name : Given name. Also helps us load the event text file as it is part of the path.
        //Type : Event type, like Stock, or other.
        //TimeStart : Time the event will start
        //TimeEnd : Time the event will end. NOTE: All our options are converted into time int so it should be good.
        //Name Type ImportantWeight Chance YearSetting Overrideable Weather WeatherOverride TimeStart TimeEnd

        FileStream FS = File.Open(EVENT_TXT_PATH, FileMode.Create);

        using (StreamWriter SW = new StreamWriter(FS))
        {
            List<int> MonthKeys = new List<int>(this.DATE_EventOrguanizer.Keys); //get the month keys from the dictionary 
            for (int i = 0; i < MonthKeys.Count; i++) //goes through the keys
            {
                List<int> DayKeys = new List<int>(this.DATE_EventOrguanizer[MonthKeys[i]].Keys); //gets the days keys
                for (int j = 0; j < DayKeys.Count; j++) //goes through ech day for each month
                {
                    string currentDate = getCurrentEventDate(MonthKeys[i], DayKeys[j]);
                    string eventEntry = "";
                    for (int k = 0; k < DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Count; k++) //goes through each event on this day 
                    {
                        /*if((int)DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].WEATHER == 0)
                        {
                            if (DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Count == 1)
                            {
                                eventEntry = currentDate + "\n" + getEventInfo(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]);

                            }
                            else if (k == 0)
                            {
                                eventEntry = currentDate + "\n" + getEventInfo(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]) + "\n";
                            }
                            else if (k == DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Count - 1)
                            {
                                eventEntry += getEventInfo(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]);
                            }
                            else
                            {
                                eventEntry += getEventInfo(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]) + "\n";
                            }
                        }else
                        {
                            if (!Weather_Orguanizer.ContainsKey(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].WEATHER.ToString()))
                            {
                                Weather_Orguanizer.Add(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].WEATHER.ToString().Trim(), new List<EventClass>());
                                Weather_Orguanizer[DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].WEATHER.ToString().Trim()].Add(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]);
                            }
                            else
                            {
                                Weather_Orguanizer[DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k].WEATHER.ToString().Trim()].Add
                                    (DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]);
                            }
                        }  */
                        if (DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Count == 1)
                        {
                            eventEntry = currentDate + "\n" + getEventInfo(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]);

                        }
                        else if (k == 0)
                        {
                            eventEntry = currentDate + "\n" + getEventInfo(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]) + "\n";
                        }
                        else if (k == DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]].Count - 1)
                        {
                            eventEntry += getEventInfo(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]);
                        }
                        else
                        {
                            eventEntry += getEventInfo(DATE_EventOrguanizer[MonthKeys[i]][DayKeys[j]][k]) + "\n";
                        }

                    }
                    eventEntry += "\n/End";
                    SW.WriteLine(eventEntry);
                }
            }

        }


        FileStream FS_REPEAT = File.Open(REPEAT_TXT_PATH, FileMode.Create);
        using (StreamWriter SW = new StreamWriter(FS_REPEAT))
        {
            List<int> RepeatKeys = new List<int>(this.REPEAT_EventOrguanizer.Keys); //get the REPEAT keys from the dictionary 
            for (int i = 0; i < RepeatKeys.Count; i++)
            {
                if (RepeatKeys[i] == (int)EventClass.REPEAT_TYPE.Rain || RepeatKeys[i] == (int)EventClass.REPEAT_TYPE.Sunny || RepeatKeys[i] == (int)EventClass.REPEAT_TYPE.Snow)
                {
                    for (int j = 0; j < REPEAT_EventOrguanizer[RepeatKeys[i]].Count; j++)
                    {
                        if (!Weather_Orguanizer.ContainsKey(REPEAT_EventOrguanizer[RepeatKeys[i]][j].WEATHER.ToString()))
                        {
                            Weather_Orguanizer[REPEAT_EventOrguanizer[RepeatKeys[i]][j].WEATHER.ToString()] = new List<EventClass>();
                            Weather_Orguanizer[REPEAT_EventOrguanizer[RepeatKeys[i]][j].WEATHER.ToString()].Add(REPEAT_EventOrguanizer[RepeatKeys[i]][j]);
                        }
                        else
                        {
                            Weather_Orguanizer[REPEAT_EventOrguanizer[RepeatKeys[i]][j].WEATHER.ToString()].Add(REPEAT_EventOrguanizer[RepeatKeys[i]][j]);
                        }
                    }
                }
                else
                {
                    string currentEvent = ((EventClass.REPEAT_TYPE)RepeatKeys[i]).ToString() + "\n";
                    for (int j = 0; j < REPEAT_EventOrguanizer[RepeatKeys[i]].Count; j++)
                    {
                        if(j == REPEAT_EventOrguanizer[RepeatKeys[i]].Count - 1)
                        {
                            currentEvent += getEventInfo(REPEAT_EventOrguanizer[RepeatKeys[i]][j]);
                        }else
                        {
                            currentEvent += getEventInfo(REPEAT_EventOrguanizer[RepeatKeys[i]][j]) + "\n";
                        }
                    }
                    currentEvent += "\n/End";
                    SW.WriteLine(currentEvent);
                }

            }
        }

        FileStream FS_WEATHER = File.Open(WEATHER_TXT_PATH, FileMode.Create);
        using (StreamWriter SW = new StreamWriter(FS_WEATHER))
        {
            List<string> WeatherKeys = new List<string>(this.Weather_Orguanizer.Keys); //get the REPEAT keys from the dictionary 
            for (int i = 0; i < WeatherKeys.Count; i++)
            {
                string currentEvent = WeatherKeys[i] + "\n";
                for (int j = 0; j < Weather_Orguanizer[WeatherKeys[i]].Count; j++)
                {
                    if (j == Weather_Orguanizer[WeatherKeys[i]].Count - 1)
                    {
                        currentEvent += getEventInfo(Weather_Orguanizer[WeatherKeys[i]][j]);
                    }
                    else
                    {
                        currentEvent += getEventInfo(Weather_Orguanizer[WeatherKeys[i]][j]) + "\n";
                    }
                }
                currentEvent += "\n/End";
                SW.WriteLine(currentEvent);
            }
        }
    }

    public string getEventInfo(EventClass currentEvent)
    {

        string EVENT_INFO = "";
        EVENT_INFO += currentEvent.NAME + " " +
                            currentEvent.TYPE + " " +
                            currentEvent.IMPORTANCE_WEIGHT + " " +
                            currentEvent.CHANCE + " ";

        if (currentEvent.YEAR == EventClass.YEAR_TYPE.Default)
        {
            EVENT_INFO += false + " ";
        }
        else
        {
            EVENT_INFO += true + " ";
        }

        if (currentEvent.OVERRIDEABLE == EventClass.OVERRIDEABLE_TYPE.Overrideable)
        {
            EVENT_INFO += true + " ";
        }
        else
        {
            EVENT_INFO += false + " ";
        }
        
        EVENT_INFO += currentEvent.WEATHER + " ";


        if (currentEvent.WEATHER_OVERRIDEABLE == EventClass.WEATHER_OVERRIDEABLE_TYPE.Overrideable)
        {
            EVENT_INFO += true + " ";
        }
        else
        {
            EVENT_INFO += false + " ";
        }

        EVENT_INFO += currentEvent.TIME_START + " " + currentEvent.TIME_END + " ";

        if((int)currentEvent.SCENE_TYPE == 1)
        {
           EVENT_INFO += currentEvent.SCENE.ToString();
        }
        else
        {
            EVENT_INFO += EventClass.SCENES.None.ToString();
        }
                            

        return EVENT_INFO;
    }

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
        }
        return currentEvent.TIME_START.ToString();
    }

    /// <summary>
    /// returns the int/string value of the time end (based on the type given by the event)
    /// </summary>
    /// <param name="currentType"></param>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    public string getTimeEnd(EventClass.TIME_TYPE currentType, EventClass currentEvent)
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
        }
        return currentEvent.TIME_END.ToString();
    }

    /// <summary>
    /// returns the date based on the current day and month of the events we are going through.
    /// </summary>
    /// <param name="month"></param>
    /// <param name="day"></param>
    /// <returns></returns>
    public string getCurrentEventDate(int month, int day)
    {
        return ((Scheduler.Month)month).ToString() + "_" + day;
    }

    /// <summary>
    /// Sorts the event list, from least to greatest
    /// </summary>
    /// <param name="EventsForTheDay"></param>
    public void SortList(List<EventClass> EventsForTheDay)
    {
        EventsForTheDay.Sort(SortByWeight);
    }

    
    /// <summary>
    /// Sort function
    /// </summary>
    /// <param name="E1"></param>
    /// <param name="E2"></param>
    /// <returns></returns>
    int SortByWeight(EventClass E1, EventClass E2)
    {
        return E1.IMPORTANCE_WEIGHT.CompareTo(E2.IMPORTANCE_WEIGHT);
    }




    //OLD STUFF THAT DOES NOT MATTER 
    /*
    /// <summary>
    /// USED ONLY FOR REPEAT EVENTS!
    /// Adds the event in the RepeatEvent_Orguanizer based on the repeat type (ex: every monday). 
    /// the repeat type (enum) is used as a key.
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <param name="repeatKey"></param>
    public void addEvent(EventClass currentEvent, int repeatKey)
    {
        if (!REPEAT_EventOrguanizer.ContainsKey(repeatKey)) //check if the key is not already present in the dictionary 
        {
            REPEAT_EventOrguanizer.Add(repeatKey, new List<EventClass>()); //add the key to the class
            REPEAT_EventOrguanizer[repeatKey].Add(currentEvent); //add the event to the list associated to that key in the dictionary.
        }else //the key already exists 
        {
            REPEAT_EventOrguanizer[repeatKey].Add(currentEvent); //add the event to the list associated to that key in the dictionary.
        }
    }

    /// <summary>
    /// Finds all the events and orguanizes them by scene
    /// </summary>
    public void OrguanizeSceneSpecificDictionary()
    {
        for (int i = 0; i < allEventClass.Length; i++) //goes through all the events
        {
            if (SceneSpecificEventRef.ContainsKey((int)allEventClass[i].SCENE)) //checks if this the scene in which the current event happens has been accounted for
            {
                SceneSpecificEventRef.Add((int) allEventClass[i].SCENE, new List<EventClass>()); //create a new list within the dictionary with that scene as a key
                SceneSpecificEventRef[(int) allEventClass[i].SCENE].Add(allEventClass[i]); //add the even to the list
            }
            else
            {
                SceneSpecificEventRef[(int) allEventClass[i].SCENE].Add(allEventClass[i]); //just add the event to the already existing list for that scene
            }
        }
    }

    /// <summary>
    /// gets the most important event for each type within all individual locations
    /// </summary>
    public void GetMainEventsForScenes()
    {
        List<int> keyList = new List<int>(this.SceneSpecificEventRef.Keys); //gets a ref to all the keys from the dict that has all the events orguanized by scene
        for(int i = 0; i < keyList.Count; i++) //go through all the keys we have
        {
            for(int j = 0; j < SceneSpecificEventRef[keyList[i]].Count; j++) //nested loop to go through all the list for those dict keys
            {
                if (MainEventsForScenes.ContainsKey((int)SceneSpecificEventRef[keyList[i]][j].TYPE)) //if the this event is not in the impo dict then add it
                {
                    MainEventsForScenes[(int)SceneSpecificEventRef[keyList[i]][j].TYPE] = SceneSpecificEventRef[keyList[i]][j];
                }else //if there is already an item in the dict, we need to check if this new item is more important than the current one
                {
                    if(SceneSpecificEventRef[keyList[i]][j].OVERRIDEABLE == EventClass.OVERRIDEABLE_TYPE.NonOverrideable && MainEventsForScenes[(int)SceneSpecificEventRef[keyList[i]][j].TYPE].OVERRIDEABLE == EventClass.OVERRIDEABLE_TYPE.NonOverrideable) //if both events are not overridable
                    {
                       // Debug.Log("ERROR, two non overridable events of the same type in same location: " + SceneSpecificEventRef[keyList[i]][j].DAY + " Day " + SceneSpecificEventRef[keyList[i]][j].STARTMONTH + " Month " + SceneSpecificEventRef[keyList[i]][j].SCENE + " SCENE"); //that should not happen, debug the error.
                    }
                }
            }
        }
    }*/
}

/// <summary>
/// Custom inspector for the event compiler so that we can press a button to set up the event text file.
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(EventCompiler))]
public class EventCompilerCustomInspectorButton: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EventCompiler eventCompilerScript = (EventCompiler)target;
        if (GUILayout.Button("Convert_Events"))
        {
            
            eventCompilerScript.setTheEventTextFile();
        }
        //base.OnInspectorGUI();
    }
}
#endif
