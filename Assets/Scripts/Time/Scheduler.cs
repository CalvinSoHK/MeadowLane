using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scheduler function that schedules at the beginning of each day.
public class Scheduler : MonoBehaviour {
     
    //Enum for season
    public enum Season { None, Spring, Summer, Fall, Winter};

    //Enum for month
    public enum Month { None, January, February, March, April, May, June, July, August, September, October, November, December };

    //Enum for day of the week
    public enum Day { None, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday};

    //Enum for day cycle
    public enum Cycle { None, Day, Night, Midnight };

    //Array that saves the length of each month. Index is the month of the year. plugging in month enums should work one to one.
    [HideInInspector]
    int[] MonthLength = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

    //Class for a date
    public class Date
    {
        //The current season
        public Season season = Season.None;

        //The current month
        public Month month = Month.None;

        //The current day
        public Day day = Day.None;

        //The current day number
        public int dayNumber = 0;

        //The current year
        public int year = 0;

        //Constructor function
        public Date(Season inSeason, Month inMonth, Day inDay, int inNumber, int inYear)
        {
            season = inSeason;
            month = inMonth;
            day = inDay;
            dayNumber = inNumber;
            year = inYear;
        }

        //Setter function
        public void SetDate(Season inSeason, Month inMonth, Day inDay, int inNumber, int inYear)
        {
            season = inSeason;
            month = inMonth;
            day = inDay;
            dayNumber = inNumber;
            year = inYear;
        }

        //Print function
        public void Print()
        {
            Debug.Log(season + " | " + month + " " + dayNumber + " | " + day + " | Year " +  year);
        }

    }

    //The current date of the game
    public Date        date;

    //The current time for the game. 
    public float CLOCK = 0;

    //The cycle in game
    public Cycle CYCLE = Cycle.None;

    //The time in string
    public string time;

    //Use this for initialization
    void Start () {
        
        //Init a date.
        date = new Date(Season.Winter, Month.January, Day.Monday, 1, 1);
	}
	
	//Helper function to go next day
    public Date NextDay()
    {
        //Take input.
        Date var = date;

        //Day always just goes next.
        //If it isn't Saturday, just go next.
        if(var.day != (Day) 7)
        {
            var.day += 1;
        }
        else
        {
            var.day = Day.Sunday;
        }

        //Check to see if the day rolls over to next month.
        int index = (int)var.month - 1;
        
        //If the day is less than the length of the current month, just increment by one.
        if(var.dayNumber < MonthLength[index])
        {
            var.dayNumber++;
        }//If the day number is equal or greater to the month length.
        else
        {
            //Check for leap year.
            if(var.year % 4 == 1 && var.month == Month.February && var.dayNumber < MonthLength[index] + 1)
            {
                var.dayNumber++;
            }//If its not december
            else if(var.month != Month.December)
            {
                //Increment month, day to 1.
                var.month++;
                var.dayNumber = 1;
            }
            else
            {
                //Set month to January, day to 1, increment year by one.
                var.month = Month.January;
                var.dayNumber = 1;
                var.year++;
            }
        }

        //Set the season based on the month
        if(var.month == Month.December || var.month <= Month.February)
        {
            var.season = Season.Winter;
        }
        else if(var.month <= Month.May)
        {
            var.season = Season.Spring;
        }
        else if(var.month <= Month.August)
        {
            var.season = Season.Summer;
        }
        else if(var.month <= Month.November)
        {
            var.season = Season.Fall;
        }

        CLOCK = 28800;

        return var;
    }

    //Function that gives us the time in string
    public string GetTime()
    {
        string ret = "";

        int minutes = (int)(CLOCK / 60);
        int hours = minutes / 60;
        minutes -= hours * 60;
        int seconds = (int)(CLOCK % 60);

        if (hours < 10)
        {
            ret += 0 + "" + hours;
        }
        else
        {
            ret += hours;
        }
        ret += " : ";
        if(minutes < 10)
        {
            ret += 0 + "" + minutes;
        }
        else
        {
            ret += minutes;
        }
        ret += " : ";
        if(seconds < 10)
        {
            ret += 0 + "" + seconds;
        }
        else
        {
            ret += seconds;
        }

        return ret;
    }

    //Function that returns the cycle time depending on CLOCK
    public Cycle GetCycle()
    {
        if(CLOCK < 72000f)
        {
            return Cycle.Day;
        }
        else if(CLOCK < 86400)
        {
            return Cycle.Night;
        }
        else
        {
            return Cycle.Midnight;
        }
    }

    void Update()
    {

        //Manage time.
        if(CLOCK < 86400)
        {
            //Debug.Log("Tick");
            CLOCK += Time.deltaTime * 32f;
        }
        else
        {

            CLOCK = 86400;
        }
        time = GetTime();
        CYCLE = GetCycle();
    }
}


