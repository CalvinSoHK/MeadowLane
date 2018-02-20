using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Top level script that knows all the plots here.
public class FarmManager : MonoBehaviour {

    //List of all plots under this manager
    public List<PlotManager> PlotList;

    //Calls day end for all plots under this farm manager.
    public void DayEndAll()
    {
        foreach(PlotManager plot in PlotList)
        {
            plot.DayEnd();
        }
    }

    //O(n^2).
    //
    public void SaveData()
    {
        string DATA = "";

        //For all our plot managers.
        for(int j = 0; j < PlotList.Count; j++)
        {
            //Add a denominator for this plot
            DATA += "Plot" + j + "\n";
            
            //For each block within this plot manager, get its string info and add it.
            for(int i = 0; i < PlotList[j].plotBlocks.Length; i++)
            {
                DATA += PlotList[j].GetSaveInfo(PlotList[j].plotBlocks[i]);
            }
        }

        SaveSystem.SaveTo(SaveSystem.SaveType.Farm, "/Farm\n" + DATA + "/");
    }

    //Loads our farm states
    public void LoadData(string DATA)
    {
        //Split the input by lines. Temp is for splitting each line within
        string[] INPUT = DATA.Split('\n'), TEMP;

        //First line is just Plot0.
        int PLOT_INDEX = 0, list_index = 0;

        //Length is minus one since the last index is empty because all of them end in \n
        for(int i = 1; i < INPUT.Length - 1; i++)
        {
            if (INPUT[i].Contains("Plot"))
            {
                PLOT_INDEX = int.Parse(INPUT[i].Substring(4));
                list_index = 0;
            }
            else if (!INPUT[i].Equals("EMPTY")) //Doens't account for tilled but empty land. Needs to be fixed.
            {
                //Set the block info of the given index
                PlotList[PLOT_INDEX].SetBlockTo(INPUT[i], list_index);

                //Increment through the grid in the same order it is loaded in from the game
                list_index++;
            }
            else if(INPUT[i].Equals("EMPTY"))
            {
                list_index++;
            }
        }
    }
}
