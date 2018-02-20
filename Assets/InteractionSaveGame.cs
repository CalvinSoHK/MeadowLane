using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Call all systems that need to save as well as clear the array
public class InteractionSaveGame : InteractableCustom {

    //FarmManager reference
    public FarmManager FM;

    //Whether or not we're attempting to save
    private bool ATTEMPT_SAVE = false, ATTEMPT_LOAD_DATA;
    public bool EYES_CLOSED = false;

    public override void Use(Hand hand)
    {
        ATTEMPT_LOAD_DATA = true;
    }

    private void Update()
    {
        if (ATTEMPT_LOAD_DATA && EYES_CLOSED)
        {
            //Clear the data.
            SaveSystem.ClearData();

            //Call save on everything.
            GameManagerPointer.Instance.SCHEDULER.SaveData(); //Date
            GameManagerPointer.Instance.PLAYER_POINTER.PLAYER.GetComponent<PlayerStats>().SaveData(); // Money
            TextMessageManager.SaveData(); //Messages
            Inventory_Manager.SavePlayerInventory(); //Inventory
            RecipeManager.SaveData(); //Recipes
            FM.SaveData(); //Farm
                           //Decoration
                           //Relationships
                           //Town States

            //Placeholder function that will fill the other data.
            SaveSystem.Filler();

            ATTEMPT_SAVE = true;
            ATTEMPT_LOAD_DATA = false;
            EYES_CLOSED = false;
        }
        else if(ATTEMPT_SAVE && SaveSystem.AllDataPresent())
        {
             SaveSystem.WriteData();
             ATTEMPT_SAVE = false;
        }
    }


}
