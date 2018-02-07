using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Cardreader script. Connects the sensor to the player.
public class CardreaderManager : MonoBehaviour {

    //Two events. One for getting the total we need to keep going. Second for if we have enough.
    public UnityEvent GET_TOTALPRICE, ORDER_VALID;

    //Total value of the thing we are trying to do
    public int TOTAL;

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
                if (PHONE.RUNNING_APP.name.Equals("MoneyApp(Clone)"))
                {
                    Debug.Log("Money app running.");
                    //Money app reference
                    MoneyApp APP = PHONE.RUNNING_APP.GetComponent<MoneyApp>();

                    //We call the event that will pass the price to this script.
                    GET_TOTALPRICE.Invoke();

                    //If we are able to pay for the total
                    if (APP.PayMoney(TOTAL))
                    {
                        ORDER_VALID.Invoke();
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
