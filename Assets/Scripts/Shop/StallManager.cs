using System.Collections.Generic;
using UnityEngine;

//Write a state machine
public class StallManager : MonoBehaviour {

    //State of the game
    public enum StallState {Closed, Return, Open };
    public StallState STATE = StallState.Closed;

    //Number of customers at the stall
    int CUSTOMER_COUNT;

    //Array of baskets to init
    public Produce_Basket_Controller[] PRODUCE_BASKET_LIST;

    //List of all produce we currently are managing
    List<string> ALL_PRODUCE = new List<string> ();

    //List of each current customer
    List<CustomerController> CUSTOMER_LIST = new List<CustomerController>();

    //List of all recipes valid for this game
    public List<Recipe> RECIPE_LIST = new List<Recipe>();

    //Dictionary of all base items and the number they have
    Dictionary<int, int> ITEM_COUNT = new Dictionary<int, int>();

    //Total earned
    int TOTAL_EARNED = 0;

    //Customer prefab
    public GameObject CUSTOMER;

    //Transform list of customer slots
    public Transform[] SLOT_LOCATIONS;

    //Whether or not a slot is open for a customer
    List<bool> CUSTOMER_SLOTS = new List<bool>();

    //Baskets for each customer
    public GameObject[] SHOPPING_BASKET_LIST = new GameObject[3];

    //Timer info for spawning intervals for customrs
    float SPAWN_TIMER = 0;
    public float MAX_TIME, COIN_VALUE;

    //Index for how far up the deck we have traveled
    int CARD_INDEX = 0;

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Inventory_Manager.AddItemToInventory((Resources.Load("Produce/Tomato") as GameObject).GetComponent<BaseItem>());
        }

        //We manage our customers regardless if its closed or not because some customers can show up right at the end.
        ManageCustomers();

        //If the stall is open
        if(STATE == StallState.Open)
        {
            //Start with spawn first. Timer will be 0 at the beginning
            if(SPAWN_TIMER <= 0)
            {
                //Spawn a customer
                SpawnCustomer();

                //Reset the timer
                SPAWN_TIMER = MAX_TIME;
            }
            else
            {
                //Reduce the timer
                SPAWN_TIMER -= Time.deltaTime;
            }

            //If we run out of produce
            if (ITEM_COUNT.Count == 0 && NoCustomersHere())
            {
                //Problem
                STATE = StallState.Return;
            }
        }
        else if(STATE == StallState.Return) // Return state where we return the inventory.
        {
            ReturnToInventory();
            STATE = StallState.Closed;
        }
        else if(STATE == StallState.Closed) //If the stall is closed
        {
            
        }
	}

    /// <summary>
    /// Helper function that checks tat we have no customers
    /// </summary>
    /// <param name="HERE"></param>
    /// <returns></returns>
    public bool NoCustomersHere()
    {
        //NOTE: True means empty
        foreach(bool TEMP in CUSTOMER_SLOTS)
        {
            if (!TEMP)
            {
                return false;
            }
        }
        return true;
    }

    //Function that manages our customers
    //Everything below here should be called by the animation OR called by the state machine. We can add a new state called ANIMATING.
    //Should be called by each individual customer.
    public void ManageCustomers()
    {
        //For all the customers we have.
        foreach(CustomerController CC in CUSTOMER_LIST)
        {
            if(CC.STATE == CustomerController.CustomerState.Result)
            {
                //If we actually completed the recipe, we need to remove extra items then process it.
                if (CC.isDone)
                {
                    //A list of all redundant items.
                    List<BaseItem> BASKET_LIST = CC.GetRedundantIngredients();

                    //If we have any at all
                    if (BASKET_LIST.Count > 0)
                    {
                        //for all the items in our basket list
                        foreach (BaseItem ITEM in BASKET_LIST)
                        {
                            RemoveItem(ITEM);
                        }
                    }

                    //Pay the money
                    CC.PayMoney();


                    //Clear the basket
                    CC.ClearBasket();

                }
                else //We failed to complete the recipe. Add the ingredients we failed to use back to our item count
                {
                    foreach(BaseItem ITEM in CC.RECIPE.INGREDIENTS)
                    {
                        AddItem(ITEM);
                    }
                }


                //Make the customer leave.
                CC.SetState(CustomerController.CustomerState.Leaving);

                //Set the bool for that customer's slot back to true
                CUSTOMER_SLOTS[CC.SLOT_NUMBER] = true;
            }
        }
    }

    //Get random valid recipe
    public Recipe GetRandom()
    {
        //Make sure the card index is within the length
        if (CARD_INDEX > RECIPE_LIST.Count - 1)
        {
            CARD_INDEX = RECIPE_LIST.Count - 1;
        }

        //Extract a recipe
        int index = Random.Range(0, CARD_INDEX + 1);
        Recipe RECIPE = RECIPE_LIST[index];

        //Increment CARD_INDEX as long as its less than the deck length.
        if(CARD_INDEX < RECIPE_LIST.Count - 1)
        {
            CARD_INDEX++;
        }

        //Return the random recipe
        return RECIPE;
    }

    //Comparison function for recipe
    public int COMPARE_RECIPE_WEIGHT(Recipe s1, Recipe s2)
    {
        if (s1.WEIGHT < s2.WEIGHT)
        {
            return -1;
        }
        else if (s1.WEIGHT > s2.WEIGHT)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    //Function that FYShuffles the list
    public void FYShuffle(List<Recipe> LIST)
    {
        //Counting down from the max index
        for (int i = LIST.Count - 1; i >= 1; i--)
        {
            //Pick a random index from 0 to i. Since it is exclusive on the end we have to do + 1.
            int RAND_INDEX = Random.Range(0, i + 1);

            //Swap cards
            Recipe TEMP = LIST[i];
            LIST[i] = LIST[RAND_INDEX];
            LIST[RAND_INDEX] = LIST[i];
        }
    }

    //Function that does a geometric weighted shuffles on our list
    public void GeoWeightedShuffle(List<Recipe> LIST, float COIN_WEIGHT)
    {
        //Shuffle then sort. Shuffle is needed for the sort to not prefer ties.
        FYShuffle(LIST);
        LIST.Sort(COMPARE_RECIPE_WEIGHT);

        //Use a second list to shuffle
        List<Recipe> SHUFFLED_LIST = new List<Recipe>();

        //While the list has stuff still it.
        while (LIST.Count > 0)
        {
            int INDEX = 0;
            //While the index is less than the LIST count, keep incrementing the index till the coin flip fails.
            while (INDEX < LIST.Count - 1)
            {
                //If we pass the coin weight check, increment index.
                if (Random.value <= COIN_WEIGHT)
                {
                    break;
                }
                INDEX++;
            }

            //Move the selected index to the shuffled list
            SHUFFLED_LIST.Add(LIST[INDEX]);
            LIST.RemoveAt(INDEX);
        }

        //Add all elements of the shuffled list to the real one
        foreach(Recipe TEMP in SHUFFLED_LIST)
        {
            //Add it to the temp
            LIST.Add(TEMP);
        }
    }

    //Function that spawns a customer
    public void SpawnCustomer()
    {
        Debug.Log("ITEM COUNT: " + ITEM_COUNT.Count);
        //If we have any items at all.
        if (ITEM_COUNT.Count > 0)
        {
            Debug.Log("Tomato count: " + ITEM_COUNT[0]);
            //Index of the bool if we want to change it
            int index = 0;

            //Filter the list for all ingredients
            RECIPE_LIST = FilterRecipeList(RECIPE_LIST);

            //Check recipe list length.
            if(RECIPE_LIST.Count > 0)
            {
                Debug.Log("RECIPE_LIST COUNT " + RECIPE_LIST.Count);    

                //Check if there is an available slot
                foreach (bool VALID in CUSTOMER_SLOTS)
                {
                    //Debug.Log(VALID);
                    //If the slot is open, spawn a customer
                    if (VALID)
                    {
                        //NOTE: Customer is two part object. The main object is a collider for the basket. Its child is the actual customer model.
                        //Spawn a customer, set the slot to taken. We spawn the BASKET on the baskte position, and move the customer to the right offset, then turn it on.
                        GameObject TEMP = Instantiate(CUSTOMER, SHOPPING_BASKET_LIST[index].transform.position, SHOPPING_BASKET_LIST[index].transform.rotation);
                        Transform TEMP_PERSON = CUSTOMER.transform.GetChild(0);

                        //Place the person a bit further back
                        TEMP_PERSON.position = SLOT_LOCATIONS[index].position;
                        TEMP_PERSON.rotation = SLOT_LOCATIONS[index].rotation;
                        TEMP_PERSON.gameObject.SetActive(true);
                        CUSTOMER_SLOTS[index] = false;

                        //Give the customer a basket and a recipe
                        Recipe TEMP_RECIPE = GetRandom();
                        TEMP.GetComponent<CustomerController>().BASKET = SHOPPING_BASKET_LIST[index];
                        TEMP.GetComponent<CustomerController>().RECIPE = TEMP_RECIPE;
                        TEMP.GetComponent<CustomerController>().SLOT_NUMBER = index;

                        //Account for the recipe in our item count
                        foreach (BaseItem ITEM in TEMP_RECIPE.INGREDIENTS)
                        {
                            RemoveItem(ITEM);
                        }

                        //Add the customer to our list of customers.
                        CUSTOMER_LIST.Add(TEMP.GetComponent<CustomerController>());

                        //Exit the foreach loop
                        break;
                    }
                    //Increment the index if we haven't spawned a customer
                    index++;
                }
            }       
        }
    }

    //Function to be called when we start the game
    public void StartGame()
    {
        //Get all the produce the player has
        GetProduce();

        //If we have any produce
        if(ITEM_COUNT.Count > 0)
        {
            //Init the list. The function will do nothing if already init.
            RecipeManager.InitList();

            //Get the list of recipes the player has already discovered.
            RECIPE_LIST = RecipeManager.GetDiscovered();


            //Init baskets
            InitBaskets();

            //Wipe out the produce
            Inventory_Manager.RemoveAllItemsFromCategory(Inventory_Manager.checkItemCategoryIndex("Produce"));

            //Further reduce the list to the recipes the player can actually make with their items.
            RECIPE_LIST = FilterRecipeList(RECIPE_LIST);

            //Shuffle the list.
            GeoWeightedShuffle(RECIPE_LIST, COIN_VALUE);

            //Clear the bool list in case we've done it before
            CUSTOMER_SLOTS.Clear();

            //Init the customer slot bools
            for (int i = 0; i < SHOPPING_BASKET_LIST.Length; i++)
            {
                CUSTOMER_SLOTS.Add(true);
            }

            //Reset the index for difficulty
            CARD_INDEX = 0;

            //Change the state
            STATE = StallState.Open;
        }
    }

    //Helper function that fills ITEM_COUNT from player inventory
    public void GetProduce()
    {
        //Get the produce
        List<InventorySlot> PRODUCE_LIST =  Inventory_Manager.GetCategory("Produce");

        //Check to see we have some produce of some kind
        if(PRODUCE_LIST != null)
        {
            //Add every slot onto our item count list.
            foreach (InventorySlot SLOT in PRODUCE_LIST)
            {
                for (int i = 0; i < SLOT.TotalNum; i++)
                {
                    AddItem(SLOT);
                    ALL_PRODUCE.Add(SLOT.Name);
                }
            }
        }
      
    }

    //Helper function that reduces a given list to the recipes the player can make
    public List<Recipe> FilterRecipeList(List<Recipe> LIST)
    {
        //Setup a return list.
        List<Recipe> RETURN_LIST = new List<Recipe>();

        //Copy of our item count so we can reset it
        Dictionary<int, int> TEMP_DICTIONARY = new Dictionary<int, int>();

        //Make a deep copy of the dictionary.
        foreach(KeyValuePair<int,int> ENTRY in ITEM_COUNT)
        {
            TEMP_DICTIONARY.Add(ENTRY.Key, ENTRY.Value);
        }

        //Each recipe...
        foreach (Recipe RECIPE in LIST)
        {

            bool isValid = true;

            //For each of its ingredients
            foreach(BaseItem INGREDIENT in RECIPE.INGREDIENTS)
            {
                //If the player has more than 
                if(TEMP_DICTIONARY[INGREDIENT.KEY] > 0)
                {
                    TEMP_DICTIONARY[INGREDIENT.KEY]--;
                }
                else { isValid = false; break; }
            }

            //If we are valid add to list.
            if (isValid)
            {
                RETURN_LIST.Add(RECIPE);
            }

            //Reset the Dictionary
            TEMP_DICTIONARY.Clear();

            //Make a deep copy of the dictionary.
            foreach (KeyValuePair<int, int> ENTRY in ITEM_COUNT)
            {
                TEMP_DICTIONARY.Add(ENTRY.Key, ENTRY.Value);
            }

        }
        return RETURN_LIST;
    }

    //Function for the dictinoary to add an item to the count. Overload method that takes baseitems
    public void AddItem(BaseItem ITEM)
    {
        if (ITEM_COUNT.ContainsKey(ITEM.KEY))
        {
            ITEM_COUNT[ITEM.KEY]++;
        }
        else
        {
            ITEM_COUNT.Add(ITEM.KEY, 1);
        }
    }

    //Function for the dictionary to add an item to the count
    public void AddItem(InventorySlot SLOT)
    {
        if (ITEM_COUNT.ContainsKey(SLOT.Key))
        {
            ITEM_COUNT[SLOT.Key]++;
        }
        else
        {
            ITEM_COUNT.Add(SLOT.Key, 1);
        }
    }

    //Function for the dictionary to remove an item from the dictionary
    public void RemoveItem(BaseItem ITEM)
    {
        //If we have that item in our dictinoary
        if (ITEM_COUNT.ContainsKey(ITEM.KEY))
        {
            if(ITEM_COUNT[ITEM.KEY] > 1)
            {
                //Debug.Log("Decrement");
                ITEM_COUNT[ITEM.KEY]--;
            }
            else
            {
                //Debug.Log("Remove Item");
                ITEM_COUNT.Remove(ITEM.KEY);
            }
        }
    }
    
    //Function that inits all our baskets for us
    public void InitBaskets()
    {
        for(int i = 0; i < PRODUCE_BASKET_LIST.Length; i++)
        {
            PRODUCE_BASKET_LIST[i].InitBasket();
        }
    }

    //Returns all produce back to inventory
    public void ReturnToInventory()
    {
        //Load in inventory
        foreach(string NAME in ALL_PRODUCE)
        {
            GameObject TEMP = Resources.Load("Produce/" + NAME) as GameObject;

            if (ITEM_COUNT.ContainsKey(TEMP.GetComponent<BaseItem>().KEY))
            {
                Inventory_Manager.AddItemToInventory(TEMP.GetComponent<BaseItem>());
                RemoveItem(TEMP.GetComponent<BaseItem>());
            }
        }
    }
}

