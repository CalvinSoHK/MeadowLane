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

    //The prefab we add for new entries. Temp is used for instantiation
    public GameObject ENTRY_PREFAB, TEMP;

    //The location we are at
    public string Location;

    //The bus location on arrival
    public Transform BUS_ARRIVAL;


    public void Start()
    {
        BSM = GameManagerPointer.Instance.BUS_STOP_MANAGER;
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
        //Remove everything on the list
        for(int i = 0; i < ENTRY_LIST.Count; i++)
        {
            Destroy(ENTRY_LIST[i].gameObject);
        }
        ENTRY_LIST.Clear();

        //For every stop in the stop list
        foreach(BusStopInfo STOP in BSM.STOP_LIST)
        {
            //If we aren't that location, add it to our entry list.
             if(!Location.Equals(STOP.SCENE_NAME))
             {
                SetEntry(STOP.SCENE_NAME, STOP.PRICE);
             }
        }
    }

    //Puts in the earliest empty entry
    public void SetEntry(string NAME_T, int PRICE_T)
    {
        TEMP = Instantiate(ENTRY_PREFAB, transform);
        TEMP.GetComponent<SingleEntryManager>().NAME = NAME_T;
        TEMP.GetComponent<SingleEntryManager>().PRICE = PRICE_T;
        ENTRY_LIST.Add(TEMP.GetComponent<SingleEntryManager>());
    }

    //Invoke the bus and give it our selected. Then clear.
    public void GiveToBus()
    {
        //Debug.Log(SELECTED.name);
        BSM.BUS.MoveTo(SELECTED.NAME);
        ClearSelected();
    }

    //Clear selected
    public void ClearSelected()
    {
        //Check for a selected entry.
        foreach (SingleEntryManager ENTRY in ENTRY_LIST)
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
       foreach(SingleEntryManager ENTRY in ENTRY_LIST)
       {
            if(ENTRY.GetComponent<InteractSelectEntry>() != null)
            {
                //If this one is selected.
                if (ENTRY.GetComponent<InteractSelectEntry>().SELECTED)
                {
                    //if we already have something selected, set that one to false.
                    if(SELECTED != null)
                    {
                        ENTRY_LIST[GetIndexOf(SELECTED)].GetComponent<InteractSelectEntry>().SELECTED = false;
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
        foreach(SingleEntryManager ENTRY in ENTRY_LIST)
        {
            if(ENTRY.NAME == OBJ.NAME)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

}
