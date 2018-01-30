using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the customer
public class CustomerController : MonoBehaviour {

    //Recipe this customer
    public Recipe RECIPE;

    //List of items we have given this customer
    public List<BaseItem> INGREDIENTS;

    //Basket Case
    public GameObject BASKET;

    //Whether or not we completed the list of ingredients
    bool isDone = false;

    //Timer of how long the customer will wait
    //All customers will wait the same amount of time
    public float TIME = 10f;
    float INTERNAL_TIME, LERP_TIMER = 0;

    //State machine
    public enum CustomerState { Approaching, Waiting, Result, Leaving };
    public CustomerState STATE;

    //Animator controller of the person
    Animator ANIMATOR;

    //The person
    Transform CUSTOMER_MODEL;

    //Slot numbner
    public int SLOT_NUMBER;

    //The display dialogue script
    DisplayDialogue DD;

    void Start()
    {
        //Customer model
        CUSTOMER_MODEL = transform.GetChild(0);

        //Get the animator
        ANIMATOR = transform.GetChild(0).GetComponent<Animator>();

        //Set the state on start. This is so our internal time works.
        SetState(CustomerState.Approaching);

        //Get the DisplayDialogue Script
        DD = GetComponent<DisplayDialogue>();
    }

    // Update is called once per frame
    void Update() {

        Debug.Log("name of cc: " + this.gameObject.name + " count: " + INGREDIENTS.Count);

        //Handle each state.
        switch (STATE)
        {
            case CustomerState.Approaching:
                //Move the customer model
                CUSTOMER_MODEL.position = Vector3.Lerp(BASKET.transform.position - BASKET.transform.forward * 3, BASKET.transform.position - BASKET.transform.forward * 1f, LERP_TIMER);
                LERP_TIMER += Time.deltaTime / 3f;
                if (LERP_TIMER >= 1)
                {
                    DD.ActivateShopDialogue(RECIPE.NAME);
                    SetState(CustomerState.Waiting);
                    LERP_TIMER = 0;
                }
                break;
            case CustomerState.Waiting:
                //If we have the right combination

                if (CheckIngredients())
                {
                    isDone = true;
                    DD.DeActivateShop();
                }

                //If we reach the end of our waiting time, or we get the correct combination
                if (GetStateTimeElapsed() >= TIME || isDone)
                {
                    SetState(CustomerState.Result);
                }
                break;
            case CustomerState.Result:
                //By default, customer will wait here till the stall manager tells it to leave. This is to make sure we have done the necessary calculations before disappearing.
                //NOTE: Once you have models, in the animation for the result, set the state to leaving.
                //ANIMATOR.Play...

                break;
            case CustomerState.Leaving:
                //Move the model customer away
                CUSTOMER_MODEL.position = Vector3.Lerp(BASKET.transform.position - BASKET.transform.forward * 1f, BASKET.transform.position - BASKET.transform.forward * 3, LERP_TIMER);
                LERP_TIMER += Time.deltaTime / 3f;
                if (LERP_TIMER >= 1)
                {
                    Debug.Log("Leaving");
                    Destroy(gameObject);
                }
                break;
            default:
                Debug.Log("Error: Invalid state.");
                break;
        }
    }

    //Setter for the state machine
    public void SetState(CustomerState TEMP)
    {
        STATE = TEMP;
        INTERNAL_TIME = Time.time;
    }

    //Gets the time since we changed states
    public float GetStateTimeElapsed()
    {
        return Time.time - INTERNAL_TIME;
    }

    //On trigger enter
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("is in trigger");
        //If the object is a base item
        if (other.gameObject.GetComponent<BaseItem>() != null)
        {
            BaseItem ITEM = other.gameObject.GetComponent<BaseItem>();
            //If it is a produce item
            if (ITEM.CATEGORY.Equals("Produce"))
            {
                Debug.Log("is it actually a produce");
                INGREDIENTS.Add(ITEM);
            }
        }
    }

    //On trigger exit
    void OnTriggerExit(Collider other)
    {
        Debug.Log("On Trigger Exit");

        //If the object is a base item
        if (other.gameObject.GetComponent<BaseItem>() != null)
        {
            BaseItem ITEM = other.gameObject.GetComponent<BaseItem>();
            //If it is a produce item
            if (ITEM.CATEGORY.Equals("Produce"))
            {
                INGREDIENTS.Remove(ITEM);
            }
        }
    }

    //Checks to see if we have all the necessary ingredients.
    //Regardless of the extra it will know if it has what it needs.
    public bool CheckIngredients()
    {
        //Basket list
        List<BaseItem> TEMP_LIST = new List<BaseItem>();

        //Copy list to the temp list
        foreach(BaseItem INGREDIENT in INGREDIENTS)
        {
            TEMP_LIST.Add(INGREDIENT);
        }

        //Check we at least we have the same amount or more
        if(RECIPE.INGREDIENTS.Count > INGREDIENTS.Count)
        {
            return false;
        }

        //For each ingredient in our RECIPE
        foreach(BaseItem INGREDIENT in RECIPE.INGREDIENTS)
        {
            //Bool for checking this ingredient
            bool isHere = false;

            //For each ingredient in our BASKET
            foreach (BaseItem ITEM in TEMP_LIST)
            {
                //If it is the same 
                if(ITEM.KEY == INGREDIENT.KEY)
                {
                    isHere = true;
                    TEMP_LIST.Remove(ITEM);
                    break;
                }
            }
            //We didn't find that ingredient
            if (!isHere)
            {
                return false;
            }
        }
        //If we reached this, we have all the ingredients we need in our basket.
        return true;   
    }

    //Returns us a list of ingredients that are MISTAKES
    public List<BaseItem> GetRedundantIngredients()
    {
        //Basket list
        List<BaseItem> TEMP_LIST = new List<BaseItem>();

        //Copy list to the temp list
        foreach (BaseItem INGREDIENT in INGREDIENTS)
        {
            TEMP_LIST.Add(INGREDIENT);
        }


        //For all items in our recipe
        foreach (BaseItem ITEM in RECIPE.INGREDIENTS)
        {
            //For all items in our basket
            foreach(BaseItem INGREDIENT in TEMP_LIST)
            {
                if(ITEM.KEY == INGREDIENT.KEY)
                {
                    Debug.Log("am I removing the object?");
                    TEMP_LIST.Remove(INGREDIENT);
                    break;
                }
            }
        }

        //Return the list.
        return TEMP_LIST;
    }

    //Remove all items in our basket
    public void ClearBasket()
    {
        //Debug.Log("Number of Ingredients before clear: " + INGREDIENTS.Count);

        //Clears the basket.
        foreach(BaseItem ITEM in INGREDIENTS)
        {
            //Debug.Log("are the objects getting destroyed?");
            Destroy(ITEM.gameObject);
        }
    }

    //Pays the player the amount of money they deserve
    public void PayMoney()
    {
        if (isDone)
        {
            //Get the player
            PlayerStats STATS = PlayerPointer.Instance.PLAYER.GetComponent<PlayerStats>();
            STATS.AddMoney(RECIPE.PRICE);
        }
      
    }
}
