using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the recipe mini game states
/// </summary>
public class RecipeGameManager : MonoBehaviour {

    //Potential game states
    public enum GAMESTATE_RECIPE { None, Waiting, Init, Choices, Choosing, Chosen, Finishing };
    [SerializeField]
    private GAMESTATE_RECIPE STATE;

    //Round we are on, and the round we end on
    int ROUND = 0, END_ROUND = 1;

    //The recipe that the player is currently trying to unlock
    [SerializeField]
    private Recipe CURRENT_RECIPE = null;

    //Ingredients that can show up in the game
    public List<BaseItem> POSSIBLE_INGREDIENTS;

    //List of ingredients the player has 
    List<string> PLAYER_INGREDIENTS = new List<string>();

    //List of choices on display
    List<BaseItem> DISPLAYED_INGREDIENTS = new List<BaseItem>();
    BaseItem CHOSEN_INGREDIENT = null, RIGHT_INGREDIENT = null;

    //The dish prefab used to show choices
    public GameObject DISH_PREFAB;

    //The slots the dish can appear in
    public List<Transform> DISH_POSITIONS;

    //Bool success or failure finish
    bool isSuccessful = false, choices_spawned = false;

    //Used when spawning things
    GameObject TEMP; BaseItem TEMP_ITEM; Transform TEMP_TRANSFORM;

    //The pot
    public GameObject POT;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //We have to load everything for this game to work. In-game, known recipes would already have been loaded by the time the player reaches the tavern
            SaveSystem.LoadData();

            //RecipeManager.LoadData(Application.dataPath + "/SaveData/save-one.txt");
            //RecipeManager.LoadItems(RecipeManager.MASTER_TXT_LOCATION);

            //Start the game
            //InitGame();
        } 
		
        //For this state machine, we want many of the changes to occur after animations from the waitress and the like, so s tate changes won't be in this script.
        //Do stuff while waiting
        if(STATE == GAMESTATE_RECIPE.Waiting)
        {

        }
        else if(STATE == GAMESTATE_RECIPE.Init)
        {
            isSuccessful = false;
            RIGHT_INGREDIENT = CURRENT_RECIPE.INGREDIENTS[0];

            Debug.Log("Moving to next state. Replace with state change from animation.");
            SetState(GAMESTATE_RECIPE.Choices);
            //Switches to choices after an animation from the waitress
        }
        else if(STATE == GAMESTATE_RECIPE.Choices)
        {
            //Present the player with their choices
            if (!choices_spawned)
            {
                Debug.Log("Spawning dishes.");
                //Stop this from happening more than once.
                choices_spawned = true;

                //Randomly choose how many options will be shown. Between 3 and 5
                int DISHES = Random.Range(3, 6);
                int START_INDEX = 0;
                if(DISHES == 3)
                {
                    START_INDEX = 1;
                }
                else if(DISHES == 4)
                {
                    START_INDEX = 0;
                }
                else if(DISHES == 5)
                {
                    START_INDEX = 0;
                }

                //Spawn all the dishes and open them
                //Populate the dishes with ingredients
                //Picks the index that will have the right ingredient
                int CHOSEN = Random.Range(START_INDEX, START_INDEX + DISHES);
                for(int i = START_INDEX; i < START_INDEX + DISHES; i++)
                {
                    //Spawn dish
                    TEMP = Instantiate(DISH_PREFAB, DISH_POSITIONS[i].position, DISH_POSITIONS[i].rotation, DISH_POSITIONS[i]);

                    //Reveals the ingredients
                    TEMP.GetComponent<Animator>().SetTrigger("Reveal");

                    //Spawn incorrect ingredient
                    if (i != CHOSEN)
                    {                      
                        POSSIBLE_INGREDIENTS.Remove(RIGHT_INGREDIENT);
                        int RANDOM_INDEX = Random.Range(0, POSSIBLE_INGREDIENTS.Count);
                        TEMP_TRANSFORM = TEMP.transform.Find("ProduceSpawnPoint");


                        TEMP = Instantiate(Resources.Load(POSSIBLE_INGREDIENTS[RANDOM_INDEX].CATEGORY + "/" + POSSIBLE_INGREDIENTS[RANDOM_INDEX]._NAME, typeof(GameObject)) as GameObject, TEMP_TRANSFORM);
                        TEMP.transform.position = TEMP_TRANSFORM.position;
                        //TEMP.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                        TEMP.GetComponent<BaseItem>()._OWNER = BaseItem.Owner.NPC;

                        POSSIBLE_INGREDIENTS.Add(RIGHT_INGREDIENT);
                    }
                    else //Spawn the right ingredient in that index
                    {
                        TEMP_TRANSFORM = TEMP.transform.Find("ProduceSpawnPoint");
                        TEMP = Instantiate(Resources.Load(RIGHT_INGREDIENT.CATEGORY + "/" + RIGHT_INGREDIENT._NAME, typeof(GameObject)) as GameObject, TEMP_TRANSFORM);
                        TEMP.transform.position = TEMP_TRANSFORM.position;
                        //TEMP.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                        TEMP.GetComponent<BaseItem>()._OWNER = BaseItem.Owner.NPC;
                    }
                }
               
                SetState(GAMESTATE_RECIPE.Choosing);
            }
        }
        else if(STATE == GAMESTATE_RECIPE.Choosing)
        {
            //Idle while the player is making choices.

        }
        else if(STATE == GAMESTATE_RECIPE.Chosen)
        {
            //Do something with the chosen item
            //If it is the right ingredient
            if (CHOSEN_INGREDIENT._NAME.Trim().Equals(RIGHT_INGREDIENT._NAME.Trim()))
            {
                //End the game if we were on the final round. Else increment round number and go back to choices state.
                if (ROUND == END_ROUND)
                {
                    isSuccessful = true;
                    POT.GetComponent<Animator>().SetTrigger("Cooking");
                    SetState(GAMESTATE_RECIPE.Finishing);
                }
                else
                {
                    //Increment the round
                    ROUND++;

                    //Set the next right ingredient
                    RIGHT_INGREDIENT = CURRENT_RECIPE.INGREDIENTS[ROUND];

                    //Set the gamestate
                    SetState(GAMESTATE_RECIPE.Choices);
                }
            }
            else
            {
                //End the game if the wrong ingredient is used.
                SetState(GAMESTATE_RECIPE.Finishing);
            }

            //No matter what happens reset the choices spawned boolean
            choices_spawned = false;
            ClearDishes();
        }
        else if(STATE == GAMESTATE_RECIPE.Finishing)
        {
            //If we were successful
            if (isSuccessful)
            {
                //Discover the recipe then set it back to null;
                RecipeManager.DiscoverRecipe(CURRENT_RECIPE);
            }
            
            CURRENT_RECIPE = null;
            SetState(GAMESTATE_RECIPE.Waiting);
        }

	}

    /// <summary>
    /// Init's the recipe mini game. Called as an event elsewhere.
    /// </summary>
    public void InitGame()
    {
        //Set the current recipe to null
        CURRENT_RECIPE = null;

        //Set round to 0
        ROUND = 0;

        //Set to init state
        SetState(GAMESTATE_RECIPE.Init);

        //Load player ingredients. Need them to pick a valid recipe.
        LoadPlayerIngredients();

        //Pick a recipe for this round
        LoadRecipe(); 
    }

    /// <summary>
    /// Restarts the game without reloading new ingredients or recipe. Used if the chef sees you meddling with his recipe.
    /// </summary>
    public void RestartGame()
    {
        SetState(GAMESTATE_RECIPE.Init);

        ROUND = 0;
    }

    public void ClearDishes()
    {
        for(int i = 0; i < DISH_POSITIONS.Count; i++)
        {
            if(DISH_POSITIONS[i].childCount > 0)
            {
                Destroy(DISH_POSITIONS[i].GetChild(0).gameObject);
            }
        }
    }

    /// <summary>
    /// Chooses a given item.
    /// </summary>
    public void ChooseItem(BaseItem ITEM)
    {
        CHOSEN_INGREDIENT = ITEM;
        SetState(GAMESTATE_RECIPE.Chosen);
    }

    /// <summary>
    /// Loads all the player ingredients that he could use. Number is not important
    /// </summary>
    void LoadPlayerIngredients()
    {
        int CATEGORY_INDEX = Inventory_Manager.checkItemCategoryIndex("Produce", Inventory_Manager.Category);

        //List of produce
        for(int i = 0; i < Inventory_Manager.CategorySlots[CATEGORY_INDEX].Count; i++)
        {
            if(Inventory_Manager.CategorySlots[CATEGORY_INDEX][i].TotalNum > 0)
            {
                PLAYER_INGREDIENTS.Add(Inventory_Manager.CategorySlots[CATEGORY_INDEX][i].Name);
            }
        }      
    }

    /// <summary>
    /// Loads a recipe for this round of minigame. Only loads a recipe that the player has the ingredients for.
    /// </summary>
    void LoadRecipe()
    {
        //Always take the first recipe that the player can fulfill in the list.
        if (RecipeManager.UNKNOWN_LIST.Count > 0)
        {
            //Debug.Log("Something to discover.");
            int index = 0;
            while(CURRENT_RECIPE == null)
            {
                //Debug.Log("Trying to find a new recipe.");
                //If the given recipe only has ingredients which the player has
                if (HasIngredientsFor(RecipeManager.UNKNOWN_LIST[index]))
                {
                    //Debug.Log("Found a new one: " + RecipeManager.UNKNOWN_LIST[index].NAME);
                    CURRENT_RECIPE = RecipeManager.UNKNOWN_LIST[index];
                }
                else
                {
                    //Debug.Log("Incrementing index.");
                    index++;
                    if(index >= RecipeManager.UNKNOWN_LIST.Count)
                    {
                        //Debug.Log("Couldn't find anything to discover given player ingredients.");
                        SetState(GAMESTATE_RECIPE.Waiting);
                        break;
                    }
                }
            }
        }
        else
        {
            //Debug.Log("No recipes left to discover. Ending game.");
            SetState(GAMESTATE_RECIPE.Waiting);
        }
    }

    /// <summary>
    /// Returns true if the given recipe is fulfillable given player inventory.
    /// </summary>
    /// <param name="RECIPE"></param>
    /// <returns></returns>
    bool HasIngredientsFor(Recipe RECIPE)
    {
        foreach(BaseItem INGREDIENT in RECIPE.INGREDIENTS)
        {
            if (!PLAYER_INGREDIENTS.Contains(INGREDIENT._NAME))
            {
                return false;
            }
        }
        return true;
    }

    public GAMESTATE_RECIPE GetState()
    {
        return STATE;
    }

    public void SetState(GAMESTATE_RECIPE TEMP)
    {
        STATE = TEMP;
    }
}
