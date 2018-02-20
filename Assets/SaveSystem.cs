using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Saves the game on save
public static class SaveSystem {

    public enum SaveType { Date, Money, Messages, Inventory, Recipes, Farm, Decoration, Relationships, TownState };
    public enum TempType { Farm, HappyMart };

    static string PATH_TO_MAIN = Application.dataPath + "/SaveData/save-one.txt",
        PATH_TO_TEMP = Application.dataPath + "/SaveData/save-temp.txt";

    //Save data array
    /*  0 - Date
     *  1 - Money
     *  2 - Messages
     *  3 - Inventory
     *  4 - Recipes
     *  5 - Farm
     *  6 - Decoration
     *  7 - Relationships
     *  8 - Town State
     */
    static string[] SAVE_DATA = new string[9], TEMP_DATA = new string[2];
    static string[] LINES;

    //Clears all indexes in the array
    public static void ClearData()
    {
        for(int i = 0; i < SAVE_DATA.Length; i++)
        {
            SAVE_DATA[i] = "";
        }
    }

    public static void ClearTempData()
    {
        for(int i = 0; i < TEMP_DATA.Length; i++)
        {
            TEMP_DATA[i] = "";
        }
    }

    //Saves to the given index. Returns false if there is data there already.
    public static bool SaveTo(SaveType type, string data)
    {
        if(SAVE_DATA != null && SAVE_DATA[(int)type] != null)
        {
            //If the save doesn't exist
            if (SAVE_DATA[(int)type].Length == 0)
            {
                //Debug.Log(data);
                SAVE_DATA[(int)type] = data;
                //Debug.Log(SAVE_DATA[(int)type]);
                return true;
            }
        }
        return false;      
    }

    //Overload function to save temp type as well.
    //Saves to the given index, returns false if there is data there already.
    public static bool SaveTo(TempType type, string data)
    {
        if (TEMP_DATA != null && TEMP_DATA[(int)type] != null)
        {
            //If the save doesn't exist
            if (TEMP_DATA[(int)type].Length == 0)
            {
                //Debug.Log(data);
                TEMP_DATA[(int)type] = data;
                //Debug.Log(SAVE_DATA[(int)type]);
                return true;
            }
        }
        return false;
    }

    //Fills data for stuff that we haven't implemented yet.
    public static void Filler()
    {
        SaveTo(SaveType.Decoration, "/Decoration\n/");
        SaveTo(SaveType.Relationships, "/Relationships\n/");
        SaveTo(SaveType.TownState, "/Town\n/");
    }

    //Checks to see if we have save data in all indexes to write.
    public static bool AllDataPresent()
    {
        if(SAVE_DATA == null)
        { 
            return false;
        }

        for(int i = 0; i < SAVE_DATA.Length; i++)
        {
            if(SAVE_DATA[i].Length == 0)
            {
                return false;
            }
        }
        return true;
    }
	
    //Writes to the text file.
    public static void WriteData()
    {
        //File stream that will create a new file if path doesn't exist, or it will overwrite it.
        FileStream FS = File.Open(PATH_TO_MAIN, FileMode.Create);

        //Use stream writer at the given path.
        using (StreamWriter SW = new StreamWriter(FS))
        {
            foreach(string DATA in SAVE_DATA)
            {
                LINES = DATA.Split('\n');
                foreach(string LINE in LINES)
                {
                    SW.WriteLine(LINE);
                }             
            }
        }
    }

    //Writes temp save data
    public static void WriteTempData()
    {
        //File stream that will create a new file if path doesn't exist, or it will overwrite it.
        FileStream FS = File.Open(PATH_TO_TEMP, FileMode.Create);

        //Use stream writer at the given path.
        using (StreamWriter SW = new StreamWriter(FS))
        {
            foreach (string DATA in TEMP_DATA)
            {
                LINES = DATA.Split('\n');
                foreach (string LINE in LINES)
                {
                    SW.WriteLine(LINE);
                }
            }
        }
    }

    //Retrieves the given info and separates it into the necessary indexes.
    public static void LoadData()
    {
        //Read in the file
        using (StreamReader SR = new StreamReader(PATH_TO_MAIN))
        {
            string LINE, DATA = "";
            SaveType TYPE = SaveType.Date;
            while ((LINE = SR.ReadLine())!= null)
            {
                switch (TYPE)
                {
                    case SaveType.Date:
                        if (!LINE.Contains("/Date"))
                        {
                            if (!LINE.Contains("/"))
                            {
                                DATA += LINE;
                            }
                            else
                            {
                                GameManagerPointer.Instance.SCHEDULER.LoadData(DATA);
                                DATA = "";
                                TYPE = SaveType.Money;
                            }                         
                        }
                        break;
                    case SaveType.Money:
                        if (!LINE.Contains("/Money"))
                        {
                            if (!LINE.Contains("/"))
                            {
                                DATA += LINE;
                            }
                            else
                            {
                                GameManagerPointer.Instance.PLAYER_POINTER.PLAYER.GetComponent<PlayerStats>().LoadData(DATA);
                                DATA = "";
                                TYPE = SaveType.Messages;
                            }
                        }
                        break;
                    case SaveType.Messages:
                        if (!LINE.Contains("/Messages"))
                        {
                            if (!LINE.Equals("/"))
                            {
                                DATA += LINE + "\n";
                            }
                            else
                            {
                                TextMessageManager.LoadData(DATA);
                                DATA = "";
                                TYPE = SaveType.Inventory;
                            }
                        }
                        break;
                    case SaveType.Inventory:
                        if (!LINE.Contains("/Inventory"))
                        {
                            if (!LINE.Equals("/"))
                            {
                                DATA += LINE + "\n";
                            }
                            else
                            {
                                Inventory_Manager.LoadPlayerInventory(DATA);
                                DATA = "";
                                TYPE = SaveType.Recipes;
                            }
                        }
                        break;
                    case SaveType.Recipes:                     
                        if (!LINE.Contains("/Recipe"))
                        {
                            if (!LINE.Contains("/"))
                            {
                                DATA += LINE + "\n";
                            }
                            else
                            {
                                RecipeManager.LoadData(DATA);
                                DATA = "";
                                TYPE = SaveType.Farm;
                            }
                        }
                        break;
                    case SaveType.Farm:                   
                        if (!LINE.Contains("/Farm"))
                        {
                            if (!LINE.Equals("/"))
                            {
                                DATA += LINE + "\n";
                            }
                            else
                            {
                                GameManagerPointer.Instance.FARM_MANAGER_POINTER.FM.LoadData(DATA);
                                DATA = "";
                                TYPE = SaveType.Decoration;
                            }
                        }
                        break;
                    case SaveType.Decoration:
                        Debug.Log("Haven't written Decoration loading yet.");
                        TYPE = SaveType.Relationships;
                        break;
                    case SaveType.Relationships:
                        Debug.Log("Haven't written Relationships loading yet.");
                        TYPE = SaveType.TownState;
                        break;
                    case SaveType.TownState:
                        Debug.Log("Haven't written Townstate loading yet.");
                        break;
                    default: Debug.Log("Whoops");
                        break;
                }
            }
                
        }
    }

    //Retrieves the given info and separates it into the necessary indexes.
    public static void LoadTempData()
    {
        //Read in the file
        using (StreamReader SR = new StreamReader(PATH_TO_TEMP))
        {
            string LINE, DATA = "";
            TempType TYPE = TempType.Farm;
            while ((LINE = SR.ReadLine()) != null)
            {
                switch (TYPE)
                {
                    case TempType.Farm:
                        if (!LINE.Contains("/Farm"))
                        {
                            if (!LINE.Equals("/"))
                            {
                                DATA += LINE + "\n";
                            }
                            else
                            {
                                GameManagerPointer.Instance.FARM_MANAGER_POINTER.FM.LoadData(DATA);
                                DATA = "";
                                TYPE = TempType.HappyMart;
                            }
                        }
                        break;
                    default:
                        Debug.Log("Not written yet: " + TYPE);
                        break;
                }
            }

        }
    }

}
