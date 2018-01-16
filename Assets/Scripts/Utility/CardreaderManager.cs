using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cardreader script. Connects the sensor to the player.
public class CardreaderManager : MonoBehaviour {

    //Smart sensor that is connected to this card reader
    public EntriesManager ENTRIES_MANAGER;
    public PaymentBasketManager BASKET_MANAGER;

    //On trigger enter
    void OnTriggerEnter(Collider col)
    {
        //If it is the smart phone
        if (col.gameObject.GetComponent<PhoneLinker>() != null)
        {
            Debug.Log("Is the phone");
            //Set the phone
            PhoneLinker PHONE = col.gameObject.GetComponent<PhoneLinker>();

            //If the phone is in the money app.
            if(PHONE.RUNNING_APP != null)
            {
                if (PHONE.RUNNING_APP.name.Equals("Money"))
                {
                    Debug.Log("Money app running.");
                    //Money app reference
                    MoneyApp APP = PHONE.RUNNING_APP.GetComponent<MoneyApp>();

                    //If we are able to pay for the total
                    if (APP.PayMoney(ENTRIES_MANAGER.TOTAL))
                    {
                        //Deliver the items
                        ENTRIES_MANAGER.DeliverItems();
                        BASKET_MANAGER.PackageItems();
                    }
                    else
                    {
                        //Failed to buy. Do nothing.
                    }
                }
            }
        }
    }

}
