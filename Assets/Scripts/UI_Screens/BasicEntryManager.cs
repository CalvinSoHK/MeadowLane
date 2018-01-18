using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Basic entry manager for any screens requiring purchase. 
/// </summary>
public class BasicEntryManager : MonoBehaviour {

    //Cardreader we are attached to
    public CardreaderManager CM;

    //Manages all children entries
    [HideInInspector]
    public SingleEntryManager[] ENTRYU_UI_ARRAY;

    //List of information
    public List<Entry> ENTRY_LIST = new List<Entry>();

    //The total value of everything in the list
    public int TOTAL;

    public virtual void Start()
    {
        ENTRYU_UI_ARRAY = gameObject.GetComponentsInChildren<SingleEntryManager>();
    }

    //Function that returns the total within our entrymanager
    public void CMGetTotal()
    {
        CM.TOTAL = CalculateTotal();
    }

    //Function that calculates our total
    public virtual int CalculateTotal()
    {
        return 0; 
    }

    /// <summary>
    /// Function that updates all entries if need be. Useful for dynamic screens like shops
    /// </summary>
    public virtual void UpdateEntries()
    {
        return;
    }

}
