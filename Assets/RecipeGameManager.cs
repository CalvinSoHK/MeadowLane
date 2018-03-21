using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the recipe mini game states
/// </summary>
public class RecipeGameManager : MonoBehaviour {

    //Potential game states
    public enum GAMESTATE_RECIPE { None, Waiting, Init, Running, Finishing };
    private GAMESTATE_RECIPE STATE;

    //The recipe that the player is currently trying to unlock
    public Recipe CURRENT_RECIPE;

    //List of ingredients the player has 
    List<string>  PLAYER_INGREDIENTS;

    //Used ingredients
    List<BaseItem> INPUT_INGREDIENTS;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            RecipeManager.LoadItems(RecipeManager.MASTER_TXT_LOCATION);
        } 
		
        //Do stuff while waiting
        if(STATE == GAMESTATE_RECIPE.Waiting)
        {

        }
        else if(STATE == GAMESTATE_RECIPE.Init)
        {

        }
        else if(STATE == GAMESTATE_RECIPE.Running)
        {

        }
        else if(STATE == GAMESTATE_RECIPE.Finishing)
        {
            //Discover the recipe then set it back to null;
            RecipeManager.DiscoverRecipe(CURRENT_RECIPE);
            CURRENT_RECIPE = null;
        }

	}

    /// <summary>
    /// Init's the recipe mini game. Called as an event elsewhere.
    /// </summary>
    public void InitGame()
    {
        //Set to init state
        SetState(GAMESTATE_RECIPE.Init);

        //Load player ingredients. Need them to pick a valid recipe.
        LoadPlayerIngredients();

        //Pick a recipe for this round
        LoadRecipe(); 
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
            int index = 0;
            while(CURRENT_RECIPE == null)
            {
                //If the given recipe only has ingredients which the player has
                if (HasIngredientsFor(RecipeManager.UNKNOWN_LIST[index]))
                {
                    CURRENT_RECIPE = RecipeManager.UNKNOWN_LIST[index];
                }
                else
                {
                    index++;
                    if(index >= RecipeManager.UNKNOWN_LIST.Count)
                    {
                        Debug.Log("Couldn't find anything to discover given player ingredients.");
                        SetState(GAMESTATE_RECIPE.None);
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.Log("No recipes left to discover. Ending game.");
            SetState(GAMESTATE_RECIPE.None);
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
