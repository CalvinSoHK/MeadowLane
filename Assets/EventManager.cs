using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

//Manages events. Loads in new ones for the day and keeps track of what time thing sshould happen
public class EventManager : MonoBehaviour {

    string PATH_TO_WEATHER_EVENTS = "Events/Weather",
        PATH_TO_NAMED_EVENTS = "Events/Repeat",
        PATH_TO_NUMBERED_EVENTS = "Events/Numbered";

    Scheduler SCHEDULER;
    WeatherManager WEATHER_MANAGER;
    TextAsset WEATHER, NAMED, NUMBERED;
    string[] INPUT, LINE;

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

    //The events for today
    public List<EventInfo> EVENT_LIST, WAKE_LIST, SLEEP_LIST, MIDNIGHT_LIST;

    public void ClearLists()
    {
        EVENT_LIST.Clear();
        WAKE_LIST.Clear();
        SLEEP_LIST.Clear();
        MIDNIGHT_LIST.Clear();
    }

    //Loads in events for today
    public void LoadEvents()
    {
        //LoadWeather();
        ClearLists();
        LoadNamed();
        LoadNumbered();
        OrganizeList();
    }

    //Read in weather events
    public void LoadWeather()
    {   
        //Load in text asset if we haven't already done so
        if (WEATHER == null)
        {
            WEATHER = Resources.Load(PATH_TO_WEATHER_EVENTS, typeof(TextAsset)) as TextAsset;
        }

        if(WEATHER_MANAGER == null)
        {
            WEATHER_MANAGER = GetComponent<WeatherManager>();
        }

        //Each index in input should be a line in the text file
        INPUT = WEATHER.text.Split('\n');
        int index = 0;
        bool FOUND = true;

        //Keep going through the lines till we get to the one with the weather we're looking for.
        while (!INPUT[index].Contains(WEATHER_MANAGER.WEATHER.ToString()))
        {
            index++;
            if (index >= INPUT.Length)
            {
                //Debug.Log("Error: Couldn't find that weather type. " + GetComponent<WeatherManager>().WEATHER.ToString());
                FOUND = false;
                break;
            }
        }

        index++;

        if (FOUND)
        {
            while (!INPUT[index].Contains("/End"))
            {
                //Split the line into each word
                LINE = INPUT[index].Split(' ');

                //Name Type ImportantWeight Chance YearSetting Overrideable [Time] [Weather] 
                AddLine(LINE);

                index++;
                if (index >= INPUT.Length)
                {
                    //Debug.Log("Weather reached the end.");
                    break;
                }
            }
        }    
    }

    //Read in named events
    public void LoadNamed()
    {
        //Load in text asset if it is null
        if(NAMED == null)
        {
            NAMED = Resources.Load(PATH_TO_NAMED_EVENTS, typeof(TextAsset)) as TextAsset;
        }

        if(SCHEDULER == null)
        {
            SCHEDULER = GetComponent<Scheduler>();
        }

        //Each index in input should be a line in the text file
        INPUT = NAMED.text.Split('\n');
        int index = 0;
        bool FOUND = true;

        //Keep going through the lines till we get to the one with the named day we're looking for
        Debug.Log(SCHEDULER.date.day.ToString());
        while (!INPUT[index].Contains(SCHEDULER.date.day.ToString()))
        {
            index++;
            if (index >= INPUT.Length)
            {
                Debug.Log("Error: Couldn't find that named day type. " + GetComponent<Scheduler>().date.day.ToString());
                FOUND = false;
                break;
            }
        }

        index++;


        //Line should read something like:
        //  ValentinesDay 0.5 February 14 Sunny true Default true
        //  Name DayName Weather WeatherOverrideable ImportanceWeight YearSetting Overrideable
        if (FOUND)
        {
            while (!INPUT[index].Contains("/End"))
            {
                //Split the line into each word
                Debug.Log(INPUT[index]);
                LINE = INPUT[index].Split(' ');

                //Name Type ImportantWeight Chance YearSetting Overrideable [Time] [Weather] 
                AddLine(LINE);

                index++;
                if (index >= INPUT.Length)
                {
                    //Debug.Log("Named reached the end.");
                    break;
                }
            }
        }
   
    }

    //Read in the numbered events
    public void LoadNumbered()
    {
        //Load in text asset if it is null
        if (NUMBERED == null)
        {
            NUMBERED = Resources.Load(PATH_TO_NUMBERED_EVENTS, typeof(TextAsset)) as TextAsset;
        }

        if(SCHEDULER == null)
        {
            SCHEDULER = GetComponent<Scheduler>();
        }

        //Each index in input should be a line in the text file
        INPUT = NUMBERED.text.Split('\n');
        int index = 0;
        bool FOUND = true;

 
        //Keep going through the lines till we get to the one with the named day we're looking for
        while (!INPUT[index].Contains(GetComponent<Scheduler>().date.month + "_" + GetComponent<Scheduler>().date.dayNumber.ToString()))
        {
            index++;
            if (index >= INPUT.Length)
            {
                Debug.Log("Error: Couldn't find that numbered day type. " + GetComponent<Scheduler>().date.month + " " + GetComponent<Scheduler>().date.dayNumber.ToString());
                FOUND = false;
                break;
            }
        }

        index++;

        if (FOUND)
        {
            //Line should read something like:
            //  ValentinesDay 0.5 February 14 Sunny trueDefault true
            //  Name Month DayNumber Weather WeatherOverrideable ImportanceWeight YearSetting Overrideable
            while (!INPUT[index].Contains("/End"))
            {
                Debug.Log(INPUT[index]);
                //Split the line into each word
                LINE = INPUT[index].Split(' ');

                //Name Type ImportantWeight Chance YearSetting Overrideable [Time] [Weather] 
                //If LINE[8] is 0, 1, or 2, they are signifiers for special timed events. Sleep, Wake, Midnight, in that order.
                
                AddLine(LINE);


                index++;
                if (index >= INPUT.Length)
                {
                    //Debug.Log("Named reached the end.");
                    break;
                }
            }
        }   
    }

    /// <summary>
    /// Adds an info line for an event as an event
    /// </summary>
    /// <param name="LINE"></param>
    public void AddLine(string[] LINE)
    {
        //Name Type ImportantWeight Chance YearSetting Overrideable Weather WeatherOverride TimeStart TimeEnd Scene
        EventInfo TEMP = new EventInfo(LINE[0], (EventClass.EVENT_TYPE)Enum.Parse(typeof(EventClass.EVENT_TYPE), LINE[1]),
            float.Parse(LINE[2]), float.Parse(LINE[3]), bool.Parse(LINE[4]), bool.Parse(LINE[5]),
            (EventClass.WEATHER_TYPE)Enum.Parse(typeof(EventClass.WEATHER_TYPE), LINE[6]),
            bool.Parse(LINE[7]), int.Parse(LINE[8]), int.Parse(LINE[9]), (EventClass.SCENES)Enum.Parse(typeof(EventClass.SCENES), LINE[10]));

        //0, 1, and 2, are special signifiers for Sleep, Wake, and Midnight events.
        if (LINE[8].Equals("0"))
        {
            SLEEP_LIST.Add(TEMP);
        }
        else if (LINE[8].Equals("1"))
        {
            WAKE_LIST.Add(TEMP);
        }
        else if (LINE[8].Equals("2"))
        {
            MIDNIGHT_LIST.Add(TEMP);
        }
        else
        {
            EVENT_LIST.Add(TEMP);
        }


    }

    //Filters list. Flips coins for the chance events and removes them if they fail. Removes yearsetting ones as well.
    public void FilterList()
    {
        for(int i = 0; i < EVENT_LIST.Count; i++)
        {
            if (!isValid(EVENT_LIST[i]))
            {
                EVENT_LIST.RemoveAt(i);
                i--;
            }
        }
    }

    //Tells us if the given event is a valid event for today, by flipping coins for chances and if it is a newgameplus only event
    public bool isValid(EventInfo INFO)
    {
        //Check the yearsetting
        if (GetComponent<Scheduler>().date.year == 1)
        {
            if (INFO.NEW_GAME_PLUS)
            {
                return false;
            }
        }

        //Flip a coin for the given event
        //If we are not 1 AND we flip a coin that is greater than the chance it should occur, return false
        if(INFO.CHANCE != 1 && UnityEngine.Random.Range(0f, 1f) > INFO.CHANCE)
        {
            Debug.Log("Calculating chances.");
            return false;
        }

        return true;
    }

    //Goes through the list and for every override, remove the overrided events.
    public void OverrideList()
    {
        for(int i = 0; i < EVENT_LIST.Count - 1; i++)
        {
            //If it is a wake or sleep event, just don't check this event.
            if (EVENT_LIST[i].TIME_START != 0 || EVENT_LIST[i].TIME_START != 1)
            {
                //Compare all events that are after E1.
                for (int j = i + 1; j < EVENT_LIST.Count; j++)
                {
                    Debug.Log(EVENT_LIST[i].NAME + " " + EVENT_LIST[j].NAME);
                    //Ignore itself
                    if (i != j && EVENT_LIST[j].TIME_START != 0 && EVENT_LIST[j].TIME_START != 1)
                    {
                        //If the same scene and type
                        if (EVENT_LIST[i].SCENE == EVENT_LIST[j].SCENE && EVENT_LIST[i].TYPE == EVENT_LIST[j].TYPE)
                        {
                            //If the time of E2 is completely eclipsed by E1, remove E2.
                            if (EVENT_LIST[j].TIME_START >= EVENT_LIST[i].TIME_START && EVENT_LIST[j].TIME_END <= EVENT_LIST[i].TIME_END)
                            {
                                //If this event is overrideable, remove it
                                if (EVENT_LIST[j].OVERRIDEABLE)
                                {
                                    //Remove the event
                                    EVENT_LIST.RemoveAt(j);

                                    //If i is an index
                                    if (i > j)
                                    {
                                        i--;
                                    }
                                    j--;
                                }//If this event isnt overrideable but the original is, remove the original, and exit this for loop check
                                else if (EVENT_LIST[i].OVERRIDEABLE)
                                {
                                    EVENT_LIST.RemoveAt(i);
                                    i--;
                                    break;
                                }
                                else
                                {
                                    Debug.Log("Non-overrideable event " + EVENT_LIST[i].NAME + " conflicts with " + EVENT_LIST[j].NAME);
                                }
                            }
                            //If E2 ends during E1, make it end before E1
                            else if (EVENT_LIST[j].TIME_END >= EVENT_LIST[i].TIME_START && EVENT_LIST[j].TIME_END <= EVENT_LIST[i].TIME_END)
                            {

                                //If this event is overrideable, we can shift its time around
                                if (EVENT_LIST[j].OVERRIDEABLE)
                                {
                                    //Allow j to keep its normal time and shift i.
                                    //NOTE: Since the list is sorted by priority, j is more important than i.
                                    EVENT_LIST[j].TIME_END = EVENT_LIST[i].TIME_START - 1;
                                }//If this event is not overrideable, but the original one is, shift its start so the end for the second event is okay.
                                else if (EVENT_LIST[i].OVERRIDEABLE)
                                {
                                    EVENT_LIST[i].TIME_START = EVENT_LIST[j].TIME_END;
                                }
                                else
                                {
                                    Debug.Log("Non-overrideable event " + EVENT_LIST[i].NAME + " conflicts with " + EVENT_LIST[j].NAME);
                                }

                            }//IF E2 starts during E1
                            else if (EVENT_LIST[j].TIME_START >= EVENT_LIST[i].TIME_START && EVENT_LIST[j].TIME_START <= EVENT_LIST[i].TIME_END)
                            {
                                //If the event is overrideable, make it start right after
                                if (EVENT_LIST[j].OVERRIDEABLE)
                                {
                                    EVENT_LIST[j].TIME_START = EVENT_LIST[i].TIME_END + 1;
                                }//If the event is non-overrideable, but the original is, make the second event end as this one starts
                                else if (EVENT_LIST[i].OVERRIDEABLE)
                                {
                                    EVENT_LIST[i].TIME_END = EVENT_LIST[j].TIME_START - 1;
                                }
                                else
                                {
                                    Debug.Log("Non-overrideable event " + EVENT_LIST[i].NAME + " conflicts with " + EVENT_LIST[j].NAME);
                                }
                            }//If E2 eclipses E1 completely, shift E2's time end so that they can both appear
                            else if(EVENT_LIST[j].TIME_START < EVENT_LIST[i].TIME_START && EVENT_LIST[j].TIME_END >= EVENT_LIST[i].TIME_END)
                            {
                                //If the event is overrideable, make it start right after
                                if (EVENT_LIST[j].OVERRIDEABLE)
                                {
                                    EVENT_LIST[j].TIME_END = EVENT_LIST[i].TIME_START - 1;
                                }
                                else if (EVENT_LIST[i].OVERRIDEABLE)
                                {
                                    EVENT_LIST.RemoveAt(i);
                                    i--;
                                }
                                else
                                {
                                    Debug.Log("Non-overrideable event " + EVENT_LIST[i].NAME + " conflicts with " + EVENT_LIST[j].NAME);
                                }
                            }
                            else if(EVENT_LIST[j].TIME_START <= EVENT_LIST[i].TIME_START && EVENT_LIST[j].TIME_END > EVENT_LIST[i].TIME_END)
                            {
                                if (EVENT_LIST[j].OVERRIDEABLE)
                                {
                                    EVENT_LIST[j].TIME_START = EVENT_LIST[i].TIME_END + 1;
                                }
                                else if (EVENT_LIST[i].OVERRIDEABLE)
                                {
                                    EVENT_LIST.RemoveAt(i);
                                    i--;
                                }
                                else
                                {
                                    Debug.Log("Non-overrideable event " + EVENT_LIST[i].NAME + " conflicts with " + EVENT_LIST[j].NAME);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //Tells us if E2 is overlapping E1.
    public bool isOverlapping(EventClass E1, EventClass E2)
    {
        //If E2's end is contained with E1
        if(E2.TIME_END >= E1.TIME_START && E2.TIME_END <= E1.TIME_END)
        {
            return false;
        }//If E2's start is contained with E1
        else if(E2.TIME_START >= E1.TIME_START && E2.TIME_START <= E1.TIME_END)
        {
            return false;
        }
        return true;
    }

    //Sorts the event list, from least to greatest
    public void SortList()
    {
        EVENT_LIST.Sort(SortByWeight);
    }

    //Sort function
    int SortByWeight(EventInfo E1, EventInfo E2)
    {
        return E1.WEIGHT.CompareTo(E2.WEIGHT);
    }

    //Select our initial weather, which is the highest priority weather on the list
    //NOTE: This will give us 
    public EventClass.WEATHER_TYPE GetWeather()
    {
        EventClass.WEATHER_TYPE RETURN_WEATHER = EventClass.WEATHER_TYPE.None;
        
        for(int i = 0; i < EVENT_LIST.Count; i--)
        {
            //If we have an event with a non-overrideable weeather type, always use that one as long as its not none
            if (!EVENT_LIST[i].WEATHER_OVERRIDEABLE && EVENT_LIST[i].WEATHER != EventClass.WEATHER_TYPE.None)
            {
                return EVENT_LIST[i].WEATHER;
            }
            //If we have an event with a weather type but it is overrideable, AND the weather type is actually a weather type...
            else if(EVENT_LIST[i].WEATHER != EventClass.WEATHER_TYPE.None &&
                RETURN_WEATHER == EventClass.WEATHER_TYPE.None)
            {
                RETURN_WEATHER = EVENT_LIST[i].WEATHER;
            }
        }

        return RETURN_WEATHER;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadEvents();
        }
    }

    //Do all the organizing
    public void OrganizeList()
    {
        FilterList();
        SortList();
        OverrideList();
    }

    //Get the event info for the given scene and time
    public EventInfo GetEvent(EventClass.SCENES SCENE, float CURRENT_TIME)
    {
        //For loop
        for(int i = 0; i < EVENT_LIST.Count; i++)
        {
            //If the event is in the right scene and the event starts before this time and ends after this time.
            if(EVENT_LIST[i].SCENE == SCENE && EVENT_LIST[i].TIME_START < CURRENT_TIME &&
                EVENT_LIST[i].TIME_END > CURRENT_TIME)
            {
                return EVENT_LIST[i];
            }
        }
        Debug.Log("No event for the given scene and time: " + SCENE + " " + CURRENT_TIME);
        return null;
    }

}

[System.Serializable]
public class EventInfo
{
    public string NAME;
    public float WEIGHT,
         CHANCE;

    public bool OVERRIDEABLE = true, NEW_GAME_PLUS = false, WEATHER_OVERRIDEABLE = false;
    public EventClass.EVENT_TYPE TYPE;
    public EventClass.WEATHER_TYPE WEATHER;
    public EventClass.SCENES SCENE;

    public int TIME_START, TIME_END;

    public EventInfo(string TEMP_NAME, EventClass.EVENT_TYPE TYPE_TEMP, 
        float TEMP_WEIGHT, float TEMP_CHANCE, bool NEW_GAME_TEMP,
        bool OVERRIDEABLE_TEMP, EventClass.WEATHER_TYPE WEATHER_TEMP, bool WEATHER_OVERRIDE_TEMP,
        int TIME_START_TEMP, int TIME_END_TEMP, EventClass.SCENES SCENE_TEMP)
    {
        NAME = TEMP_NAME;
        WEIGHT = TEMP_WEIGHT;
        CHANCE = TEMP_CHANCE;
        NEW_GAME_PLUS = NEW_GAME_TEMP;
        OVERRIDEABLE = OVERRIDEABLE_TEMP;
        TYPE = TYPE_TEMP;
        WEATHER = WEATHER_TEMP;
        WEATHER_OVERRIDEABLE = WEATHER_OVERRIDE_TEMP;
        TIME_START = TIME_START_TEMP;
        TIME_END = TIME_END_TEMP;
        SCENE = SCENE_TEMP;
    }

}

