using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PaymentBasketManager : MonoBehaviour {

    //All objects in this basket
    List<GameObject> OBJECT_LIST = new List<GameObject>();

    //Entry manager this basket is linked to
    public ShopEntryManager ENTRIES_MANAGER;

    //Events to fire when thing enters the container.
    public UnityEvent ON_OBJ_ENTER, ON_OBJ_EXIT;

    //The object we are manipulating currently
    public BaseItem CURRENT;

    void OnTriggerEnter(Collider other)
    {
        //If we are a base item.
        if(other.GetComponent<BaseItem>() != null)
        {
            //If we aren't owned by the player
            if(other.GetComponent<BaseItem>()._OWNER != BaseItem.Owner.Player)
            {
                ENTRIES_MANAGER.AddItem(other.GetComponent<BaseItem>());
                OBJECT_LIST.Add(other.gameObject);
                ON_OBJ_ENTER.Invoke();
            }
        }
           
    }

    void OnTriggerExit(Collider other)
    {
        //If we are a base item
        if(other.GetComponent<BaseItem>() != null)
        {
            //If we aren't owned by the player
            if (other.GetComponent<BaseItem>()._OWNER != BaseItem.Owner.Player)
            {
                ENTRIES_MANAGER.RemoveItem(other.GetComponent<BaseItem>());
                OBJECT_LIST.Remove(other.gameObject);
                ON_OBJ_EXIT.Invoke();
            }
        }
    }

    //Gives us the list
    public List<GameObject> GetList()
    {
        return OBJECT_LIST;
    }

    /// <summary>
    /// Function that will package our items with a flourish. Juice primarily.
    /// Destroys the objects as well.
    /// </summary>
    public void PackageItems()
    {
       for(int i = OBJECT_LIST.Count - 1; i > -1; i--)
        {
            Destroy(OBJECT_LIST[i]);
        }
    }
}
