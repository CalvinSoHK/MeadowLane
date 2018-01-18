using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Manages the bus location entries on screen.
public class BusEntryManager : BasicEntryManager {

    //The bus stop manager
    Bus_Stop_Manager BSM;

    //Keeps track of which entry we are keeping track of
    public SingleEntryManager SELECTED = null;

    //The outline image we are going to move around to select things.
    public GameObject UI_SELECT_OUTLINE;

    //The list of locations and prices
    public List<string> LOCATION_LIST = new List<string>();
    public List<int> PRICE_LIST = new List<int>();

    //The location we are at
    public string Location;

    //The type of transition we need to go here
    public enum TransitionType { Village, Outskirts, Beachside };
    public TransitionType TRANSITION_TO;

    public override void Start()
    {
        base.Start();
        BSM = Bus_Stop_Manager.Instance;
        SetEntries();

    }

    private void Update()
    {
        UpdateEntries();
        UpdateUIOutline();
    }


    //Puts in entries for the locations for us.
    public void SetEntries()
    {
        int index = 0;
        //For every entry in the stop list
        foreach(string ENTRY in BSM.STOP_LIST)
        {
            //If we aren't that location, add it to our entry list.
             if(Location != BSM.STOP_LIST[index])
             {
                SetEntry(BSM.STOP_LIST[index], BSM.STOP_PRICES[index]);
             }
            index++;
        }

        //For every entry that isn't set, set them inactive
        foreach(SingleEntryManager ENTRY in ENTRYU_UI_ARRAY)
        {
            if(ENTRY.NAME == "")
            {
                ENTRY.gameObject.SetActive(false);
            }
        }
    }

    //Puts in the earliest empty entry
    public void SetEntry(string NAME_T, int PRICE_T)
    {
        //Find the earliest one that is empty
        foreach(SingleEntryManager ENTRY in ENTRYU_UI_ARRAY)
        {
            if(ENTRY.NAME == "")
            {
                ENTRY.NAME = NAME_T;
                ENTRY.PRICE = PRICE_T;
                return;
            }     
        }
    }

    //Invoke the bus and give it our selected. Then clear.
    public void GiveToBus()
    {
        Bus_Stop_Manager.Instance.BUS.MoveTo(Location, SELECTED.NAME);
        ClearSelected();
    }

    //Clear selected
    public void ClearSelected()
    {
        //Check for a selected entry.
        foreach (SingleEntryManager ENTRY in ENTRYU_UI_ARRAY)
        {
            if (ENTRY.GetComponent<InteractSelectEntry>() != null)
            {
                 ENTRY.GetComponent<InteractSelectEntry>().SELECTED = false;
            }
        }

        SELECTED = null;
    }

    //Calculates the total. Is fed into the function that passes it to the cardreader.
    public override int CalculateTotal()
    {
        return SELECTED.PRICE;
    }

    //Manages the outline so its in the right place
    public void UpdateUIOutline()
    {
        //If its null, UI_SELECT_OUTLINE should be inactive
        if(SELECTED == null)
        {
            UI_SELECT_OUTLINE.SetActive(false);
        }
        else
        {
            //If we're not active
            if (!UI_SELECT_OUTLINE.activeSelf)
            {
                UI_SELECT_OUTLINE.SetActive(true);
            }
            UI_SELECT_OUTLINE.transform.position = SELECTED.transform.position;
           
        }
    }

    //Manage entries. Only allow for one selected.
    public override void UpdateEntries()
    {
        //Check for a selected entry.
       foreach(SingleEntryManager ENTRY in ENTRYU_UI_ARRAY)
       {
            if(ENTRY.GetComponent<InteractSelectEntry>() != null)
            {
                //If this one is selected.
                if (ENTRY.GetComponent<InteractSelectEntry>().SELECTED)
                {
                    //if we already have something selected, set that one to false.
                    if(SELECTED != null)
                    {
                        ENTRYU_UI_ARRAY[GetIndexOf(SELECTED)].GetComponent<InteractSelectEntry>().SELECTED = false;
                    }
                    SELECTED = ENTRY;            
                }
            }
       }
    }

    /// <summary>
    /// Get selected entries index. Return -1 if not a valid entry.
    /// </summary>
    /// <param name="OBJ"></param>
    /// <returns></returns>
    public int GetIndexOf(SingleEntryManager OBJ)
    {
        int index = 0;
        foreach(SingleEntryManager ENTRY in ENTRYU_UI_ARRAY)
        {
            if(ENTRY == OBJ)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

}
