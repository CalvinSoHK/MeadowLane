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


	
	// Update is called once per frame
	void Update () {
		
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

        }

	}

    /// <summary>
    /// Init's the recipe mini game. Called as an event elsewhere.
    /// </summary>
    public void InitGame()
    {
        SetState(GAMESTATE_RECIPE.Init);
    }

    void LoadRandomRecipe()
    {

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
