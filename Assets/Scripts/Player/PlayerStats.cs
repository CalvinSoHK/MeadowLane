using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player stats.
public class PlayerStats : MonoBehaviour {

    //How much money they have.
    int MONEY;


    //Check to see if player has enough money
    public bool HasMoney(int i)
    {
        return MONEY >= i;
    }

    //Used to add or subtract money.
    //Bounds money to lowest being 0.
	public void AddMoney(int i)
    {
        MONEY += i;
        if(MONEY < 0)
        {
            MONEY = 0;
        }
    }
}
