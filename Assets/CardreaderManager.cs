using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cardreader script. Connects the sensor to the player.
public class CardreaderManager : MonoBehaviour {

    //Smart sensor that is connected to this card reader
    public SmartSensor SENSOR;


    //On trigger enter
    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.name);
        //If it is the smart phone
        if (col.gameObject.GetComponent<PhoneLinker>() != null)
        {
            Debug.Log("Is the phone");
            //Set the phone
            PhoneLinker PHONE = col.gameObject.GetComponent<PhoneLinker>();

            //If the phone is in the money app.
            if(PHONE.RUNNING_APP.name.Equals("Money"))
            {
                Debug.Log("Money app running.");
                //Money app reference
                MoneyApp APP = PHONE.RUNNING_APP.GetComponent<MoneyApp>();

                //Check if we are attempting to buy something
                //The first statement is if we are attempting to buy anything
                if (SENSOR.GetTotal(BaseItem.Owner.NPC) > 0)
                {
                    int TOTALEARNED = 0;

                    //If we are also selling something
                    if (SENSOR.GetTotal(BaseItem.Owner.Player) > 0)
                    {
                        Debug.Log("Also selling");
                        TOTALEARNED = SENSOR.GetTotal(BaseItem.Owner.Player);
                        SENSOR.EmptyContainers(BaseItem.Owner.Player);
                    }

                    //Calculate how much money we lost/earned
                    TOTALEARNED -= SENSOR.GetTotal(BaseItem.Owner.NPC);

                    //Apply the right function depending on the value
                    //If after selling and buying we still made money...
                    if (TOTALEARNED > 0)
                    {
                        //Add the amount of money we want
                        APP.EarnMoney(TOTALEARNED);
                    }
                    //If we lost money AND we have enough to pay for it.
                    else if (TOTALEARNED < 0 && APP.PayMoney(-TOTALEARNED))
                    {
                        //Change ownership of the sold items.
                        SENSOR.ChangeOwnership(BaseItem.Owner.Player);
                    }
                }//Selling something ONLY.
                else if(SENSOR.GetTotal(BaseItem.Owner.Player) > 0)
                {
                    APP.EarnMoney(SENSOR.GetTotal(BaseItem.Owner.Player));
                    SENSOR.EmptyContainers(BaseItem.Owner.Player);
                }
            }            
        }
    }
}
