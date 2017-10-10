using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Just retrieve the money we have and display it properly
public class RetrieveMoney : MonoBehaviour {

    //PhoneLinker
    public PhoneLinker LINK;

    //Reference to PlayerStats
    PlayerStats PS;

    private void Update()
    {
        if(PS == null)
        {
            if(LINK.PHONE.PLAYER_STATS != null)
            {
                PS = LINK.PHONE.PLAYER_STATS;
            }
        }
        else
        {
            int value = PS.GetMoney(); 
            if(value > 1000)
            {
                value /= 1000;
                GetComponent<Text>().text = "$ " + value + "K";
            }
            else
            {
                GetComponent<Text>().text = "$ " + value;
            }
           
        }
    }

}
