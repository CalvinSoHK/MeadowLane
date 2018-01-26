using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Controls our baskets and lets us spawn things from them
public class Produce_Basket_Controller : InteractableCustom {

    //Produce to hold
    public BaseItem PRODUCE;

    //Count of how many of the produce we have
    public int COUNT = 0;

	//Function to init the basket DON'T OPEN PLEASE
    public void InitBasket()
    {
        //Get the category index for our produce
        int CATEGORY_INDEX = Inventory_Manager.checkItemCategoryIndex(PRODUCE.CATEGORY);

        //Get the total number from its inventory index.
        COUNT = Inventory_Manager.CategorySlots[CATEGORY_INDEX][Inventory_Manager.checkItemInvetorySlot(PRODUCE.KEY, CATEGORY_INDEX)].TotalNum;
    }

    //Function to call on spawning on an object
    public override void Use(Hand hand)
    {
        if(COUNT > 0)
        {
            //Spawn the object, add to hand, disable on trigger raycast.
            GameObject TEMP = Instantiate(PRODUCE.gameObject);
            TEMP.GetComponent<InteractionPickup>().Use(hand);
            COUNT--;
        }
        else
        {
            //Some sort of feedback to tell the player it failed.
        }
    }

    //On entering the collider, check if its the same item as ours, if it is, reaccept it back into the basket.
    void OnCollisionEnter(Collision col)
    {
        if(col.collider.GetComponent<BaseItem>() != null)
        {
            if(col.collider.GetComponent<BaseItem>().KEY == PRODUCE.KEY)
            {
                COUNT++;
                Destroy(col.collider.gameObject);
            }
        }
    }
}
