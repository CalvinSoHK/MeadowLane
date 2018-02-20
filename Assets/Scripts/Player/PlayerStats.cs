using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player stats.
public class PlayerStats : MonoBehaviour {

    //How much money they have.
    public int MONEY;

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

    //Get how much money the player has
    public int GetMoney()
    {
        return MONEY;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Inventory_Manager.AddItemToInventory((Resources.Load("Produce/Wheat", typeof(GameObject)) as GameObject).GetComponent<BaseItem>());
            Inventory_Manager.AddItemToInventory((Resources.Load("Produce/Tomato", typeof(GameObject)) as GameObject).GetComponent<BaseItem>());
        }
    }

    public void SaveData()
    {
        SaveSystem.SaveTo(SaveSystem.SaveType.Money, "/Money\n" + MONEY.ToString() + "\n/");
    }

    public void LoadData(string DATA)
    {
        MONEY = int.Parse(DATA);
    }
}
