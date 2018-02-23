using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {

    //Both hands
    Hand hand1, hand2;

    //Where the inventory is showing
    public enum ShowState { None, Hand1, Hand2 };
    public ShowState SHOW = ShowState.None;

    public enum FadeState { FadeOut, FadeIn, Wait};
    public FadeState currentFadeState;

    public enum InventoryState { Item, Furniture};
    public InventoryState currentInventoryState = InventoryState.Item;

    //Temp state for the next frame. We change this tate, which is applied at the end of the frame.
    //Prevents inputs from being taken on the first frame.
    private ShowState TEMP_STATE = ShowState.None;

    //Pos offset for the inventoryUI on being called
    public Vector3 POS_OFFSET;
    public Vector3 ROT_OFFSET;

    public Canvas UICanvas;
    public Image currentCategory_Image, currentItem_Image, upArrow, downArrow, leftArrow, rightArrow; //Category and Item UI image ref
    public Text currentCount; //UI text placement for count
    bool isInventoryOn = false; //ref to whether the inventory UI is on
    int totalCategory; //total number of categories
    int TotalItemForCategory; //total number of items within the category

    //Bools for the four directions on the UI
    public bool LEFT = false, RIGHT = false, UP = false, DOWN = false, PRESS_DOWN = false, PRESS_UP = false, TRIGGER_DOWN = false;

    bool changeCategoryImage, changeItemImage, fadeIn, fadeOut;
    float imageTimer = 0.0f, imageWaitTime = 3.0f, alphaTime = 0.0f, alphaValueItem = 0.0f, alphaValueCategory = 0.0f;

    //Opacity for visible and invisilbe
    public float VISIBLE = 1.0f, INVISIBLE = 0f;

    public GameObject[] DebugInventory;
    List<InventorySlot>[] currentInventory;

    // Use this for initialization
    void Start () {
        Inventory_Manager.InitPlayerInventory(); //Initialize the static inventory categories
        totalCategory = Inventory_Manager.Category.Count; //get the total number of categorries in the inventory manager
        hand1 = transform.GetChild(0).Find("Hand1").GetComponent<Hand>(); //get a reference to the two player hands
        hand2 = transform.GetChild(0).Find("Hand2").GetComponent<Hand>();
    }
	
	// Update is called once per frame
	void Update () {


        switch (currentFadeState) //swtich statement used to keep track of what fade state we are currently in fo rthe inventory
        {
            case FadeState.Wait: //nothing needs to happen in terms of fading
                changeCategoryImage = false; //we are neither changing the category or item image
                changeItemImage = false;
                break;

            case FadeState.FadeOut: //we are fading out one of the inventory images
                if (changeItemImage) //we are changing the item image
                {
                    Color tmp = currentCategory_Image.color; //get a reference to the category image
                    alphaValueCategory = INVISIBLE; //alpha value for the category should be 0
                    tmp.a = alphaValueCategory; //set it as so for the temp color
                    currentCategory_Image.color = tmp; //update the category image and the up and down arrows to have a 0 alpha value
                    upArrow.color = tmp;
                    downArrow.color = tmp;

                }
                else if (changeCategoryImage) //we are changing the category image
                {
                    Color tmp = currentItem_Image.color; //get a reference to the item image
                    alphaValueItem = INVISIBLE; // alpha value for the category should be 0
                    tmp.a = alphaValueItem; //set is as such for the temp color
                    currentItem_Image.color = tmp; //update the category image and the up and down arrows and the item number count to have a 0 alpha value
                    rightArrow.color = tmp;
                    leftArrow.color = tmp;
                    currentCount.color = tmp;
                }

                
                //if enough time has elapsed and either the alpha value of the category or image item is equal to 0
                if (elapsedTime() > imageWaitTime && (alphaValueCategory == INVISIBLE || alphaValueItem == INVISIBLE))
                {
                    fadeOut = false; currentFadeState = FadeState.FadeIn; imageTimer = Time.time; alphaTime = 0f; //we need to fade in the category or item image
                }
                break;

            case FadeState.FadeIn: //we are now fading in the category or item image
                if (changeItemImage) //we are changing the item imgae
                {
                    alphaTime += Time.deltaTime; //we are lerping the alpha value of the category image back to 1
                    Color tmp = currentCategory_Image.color;
                    alphaValueCategory = Mathf.Lerp(INVISIBLE, VISIBLE, alphaTime);
                    tmp.a = alphaValueCategory;
                    currentCategory_Image.color = tmp;
                    upArrow.color = tmp;
                    downArrow.color = tmp;

                }
                else if (changeCategoryImage) //if we are changing the category image
                {
                    alphaTime += Time.deltaTime; //we are lerping the alpha value of the item image back to 1
                    Color tmp = currentItem_Image.color;
                    alphaValueItem = Mathf.Lerp(INVISIBLE, VISIBLE, alphaTime);
                    tmp.a = alphaValueItem;
                    currentItem_Image.color = tmp;
                    rightArrow.color = tmp;
                    leftArrow.color = tmp;
                    currentCount.color = tmp;
                }

                //if enough time has elapsed and either the alpha value of the category or image item is equal to 0
                if (elapsedTime() > imageWaitTime && (alphaValueCategory == VISIBLE || alphaValueItem == VISIBLE))
                {
                    fadeIn = false; currentFadeState = FadeState.Wait; imageTimer = Time.time; changeItemImage = false; turnOnArrows(true); alphaTime = 0f; //now we wait for the player to do something else
                }
                break;

            
        }


        //OLD DEBUG STUFF. DOES NOT WORK ANYMORE
        /*if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("inventory is being debugged");
            debugInventory();
        }*/
      

        //If we're on AND we're not showing nothing.
        if (SHOW != ShowState.None && isInventoryOn)
        {
            //Saves the count of the item we're on
            int COUNT = -1;

            //Update the directional bools based off of the current hand's trackpad
            if (SHOW == ShowState.Hand1)
            {
                PRESS_DOWN = hand1.GetTrackpadDown();
                PRESS_UP = hand1.GetTrackpadUp();
                LEFT = hand1.GetTrackpadPressLeft();
                RIGHT = hand1.GetTrackpadPressRight();
                UP = hand1.GetTrackpadPressUp();
                DOWN = hand1.GetTrackpadPressDown();
                TRIGGER_DOWN = hand1.GetStandardInteractionButtonDown();
                if (TRIGGER_DOWN)
                {
                    HideInventory(hand1);
                }

            }
            else if (SHOW == ShowState.Hand2)
            {
                PRESS_DOWN = hand2.GetTrackpadDown();
                PRESS_UP = hand2.GetTrackpadUp();
                LEFT = hand2.GetTrackpadPressLeft();
                RIGHT = hand2.GetTrackpadPressRight();
                UP = hand2.GetTrackpadPressUp();
                DOWN = hand2.GetTrackpadPressDown();
                TRIGGER_DOWN = hand2.GetStandardInteractionButtonDown();
                if (TRIGGER_DOWN)
                {
                    HideInventory(hand2);
                }
            }

            if (LEFT && PRESS_DOWN) { //if we press left on the D-Pad
                Inventory_Manager.changeIndexSlots(currentInventoryState, -1); //move the inventory item to the left
                if (Inventory_Manager.getCurrentCategorySlotIndex(currentInventoryState) < 0) // if you go beyond the left most item rotate back to the right
                {
                    Inventory_Manager.setIndexSlots(currentInventoryState, TotalItemForCategory - 1);
                }
                UpdateImage(false, true, false); // update the image of the item
            } else if (RIGHT && PRESS_DOWN) //if we press Right on the D-Pad
            {
                Inventory_Manager.changeIndexSlots(currentInventoryState, 1); //move the inventory item to the right
                if (Inventory_Manager.getCurrentCategorySlotIndex(currentInventoryState) >= TotalItemForCategory) //if you go beyond the right most item rotate back to the left
                {
                    Inventory_Manager.setIndexSlots(currentInventoryState, 0);
                }
                UpdateImage(false, true, false); //update the image of the item
            } else if (UP && PRESS_DOWN) //if we press Up on the D-Pad
            {
                Inventory_Manager.changeIndex(currentInventoryState, 1); //get the next category in the list

                if (Inventory_Manager.getCurrentCategoryIndex(currentInventoryState) >= totalCategory) //if you go beyond the top most category rotate back to the bottom
                {
                    Inventory_Manager.setIndex(currentInventoryState, 0);
                }
                //Inventory_Manager.currentCategoryIndex = 
                Inventory_Manager.setIndex(currentInventoryState, CheckIfOpeningCategoryContainsItem(Inventory_Manager.getCurrentCategoryIndex(currentInventoryState), true));
                TotalItemForCategory = Inventory_Manager.getCurrentInventory(currentInventoryState)[Inventory_Manager.getCurrentCategoryIndex(currentInventoryState)].Count; //get the total number of items in the new category
                Inventory_Manager.setIndexSlots(currentInventoryState, 0); //go to the first object in that category
                UpdateImage(true, true, false); //update the images of both the category and the item
            }
            else if (DOWN && PRESS_DOWN) //if we press Down on the D-Pad
            {
                Inventory_Manager.changeIndex(currentInventoryState, -1); //get the previous category in the list
                if (Inventory_Manager.getCurrentCategoryIndex(currentInventoryState) < 0) //if you go beyond the bottom most category rotate back to the bottom
                {
                    //Debug.Log(Inventory_Manager.currentCategoryIndex);
                    Inventory_Manager.setIndex(currentInventoryState, totalCategory - 1);
                    //Debug.Log(Inventory_Manager.currentCategoryIndex);
                }
                Inventory_Manager.setIndex(currentInventoryState, CheckIfOpeningCategoryContainsItem(Inventory_Manager.getCurrentCategoryIndex(currentInventoryState), false));
                //Debug.Log(Inventory_Manager.currentCategoryIndex);
                TotalItemForCategory = Inventory_Manager.getCurrentInventory(currentInventoryState)[Inventory_Manager.getCurrentCategoryIndex(currentInventoryState)].Count; //get the total number of items in the new category
                Inventory_Manager.setIndexSlots(currentInventoryState, 0); //go to the first object in that category
                UpdateImage(true, true, false); //update the category and item image
            } else if (PRESS_DOWN) //If we press the center button of the D-Pad
            {
                InventorySlot tempSlot = Inventory_Manager.getCurrentInventory(currentInventoryState)[Inventory_Manager.getCurrentCategoryIndex(currentInventoryState)][Inventory_Manager.getCurrentCategorySlotIndex(currentInventoryState)]; // get a ref to the item selected by the player
                switch (currentInventoryState)
                {
                    case InventoryState.Item:
                        if (Inventory_Manager.currentCategoryIndex != 0) //If we are not in the produce section
                        {
                            if (tempSlot.TotalNum <= 0) //if the object is already in the scene
                            {
                                //get reference to the object in the scene through the inventory manager                        
                                GameObject tempGameObject = Inventory_Manager.MoveItemToHandOfPlayer(tempSlot.Key);
                                AttachObjectToHand(tempGameObject); //attach that object to the hand
                            }
                            else //if the object has not been put in the scene yet
                            {
                                GameObject ObjectRef = SpawnItemFromInventory(tempSlot, true); //spawn the item into the scene
                                Inventory_Manager.AddItemToDictionary(tempSlot.Key, ObjectRef); //Add the instantiated object to the Dictionary in Inventory Manager that keeps track of inventory items in the scene

                            }
                        }
                        else //If we are in the produce section
                        {
                            COUNT = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex].TotalNum - 1;
                            SpawnItemFromInventory(tempSlot, true); //spawn the produce into the scene
                        }
                        break;
                    case InventoryState.Furniture:
                        COUNT = Inventory_Manager.FurnitureCategorySlots[Inventory_Manager.currentFurnitureCategoryIndex][Inventory_Manager.currentFurnitureCategorySlotsIndex].TotalNum - 1;
                        SpawnItemFromInventory(tempSlot, true); //spawn the produce into the scene
                        break;
                }
                
                
                CheckInventoryUI(false); //turn off the Inventory UI                
            }

            //Update the item count
            if (COUNT != 0)
            {
                switch (currentInventoryState)
                {
                    case InventoryState.Item:
                        //if the item beign displayed is a seed container
                        if (Inventory_Manager.InventorySeedCount.ContainsKey(Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex].Key))
                        {
                            //curretn count should be displaying the number of seeds
                            currentCount.text = Inventory_Manager.getSeeds(Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex].Key) + "";
                        }
                        else //otherwise it will display the number of the current object
                        {
                            currentCount.text = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex].TotalNum + "";
                        }
                        break;
                    case InventoryState.Furniture:
                        currentCount.text = Inventory_Manager.FurnitureCategorySlots[Inventory_Manager.currentFurnitureCategoryIndex][Inventory_Manager.currentFurnitureCategorySlotsIndex].TotalNum + "";
                        break;
                }
                
            }

            COUNT = 1;
            //currentCount.text = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex].TotalNum + "";
        }

        //Change the state machine at the end of a frame. Prevents same input when opening the inventory up.
        SHOW = TEMP_STATE;        
    }

    //Return the right hand
    //Only called when UI is up, which means show state is either hand. Should never give us a null.
    public Hand GetHand()
    {
        if (SHOW == ShowState.Hand1)
        {
            return hand1;

        }
        else if (SHOW == ShowState.Hand2)
        {
            return hand2;
        }
        Debug.Log("Error: For some reason spawning item with no hand.");
        return null;
    }

    //Only brings up the inventory on the given hand.
    public void CallInventory(Hand hand)
    {
        //Only allow it if the hand isn't holding anything.
        if(hand.currentAttachedObject.name == hand.controllerPrefab.name + "_" + hand.name)
        {
            //If we are in the NONE show state, just show the phone on this hand
            if (SHOW == ShowState.None && ShowInventory())
            {
                hand.GetComponent<OnTriggerRaycast>().PickUpObj(UICanvas.gameObject);
                if (hand == hand1)
                {
                    UICanvas.gameObject.SetActive(true);
                    TEMP_STATE = ShowState.Hand1;
                }
                else
                {
                    UICanvas.gameObject.SetActive(true);
                    TEMP_STATE = ShowState.Hand2;
                }
            }
            else if (SHOW == ShowState.Hand1)
            {
                //If the hand that called it wasn't hand1, move the UI to the other
                if (hand != hand1)
                {
                    hand.GetComponent<OnTriggerRaycast>().PickUpObj(UICanvas.gameObject);
                    TEMP_STATE = ShowState.Hand2;
                }
            }
            else if (SHOW == ShowState.Hand2)
            {
                //If the hand that called it wasn't hand2, mvoe the UI to the other.
                if (hand != hand2)
                {
                    hand.GetComponent<OnTriggerRaycast>().PickUpObj(UICanvas.gameObject);
                    TEMP_STATE = ShowState.Hand1;
                }
            }
            
            MoveInventory(hand);
        }
     
    }

    //hides the phone if the input hand is the correct one.
    public void HideInventory(Hand hand)
    {
        if (SHOW == ShowState.Hand1)
        {
            //If the hand that called it wasn't hand1, move the UI to the other
            if (hand == hand1)
            {
                CheckInventoryUI(false);
            }
        }
        else if (SHOW == ShowState.Hand2)
        {
            //If the hand that called it wasn't hand2, mvoe the UI to the other.
            if (hand == hand2)
            {
                CheckInventoryUI(false);
            }
        }
    }

    /// <summary>
    /// Places inventory on the given hand
    /// </summary>
    /// <param name="hand"></param>
    public void MoveInventory(Hand hand)
    {
        UICanvas.transform.SetParent(hand.transform);
        UICanvas.transform.localPosition = POS_OFFSET;
        UICanvas.transform.localRotation = Quaternion.Euler(ROT_OFFSET);
    }

    /// <summary>
    ///Brings up inventory on given hand. Returns whether or not it worked.
    /// </summary>
    public bool ShowInventory()
    {
        if (!isInventoryOn) //if player presses space and the inventory is off
        {
            int currentCategoryIndex;
            if (GetComponent<HomeCustomizationManager>().CustomizingHome())
            {
                currentInventoryState = InventoryState.Furniture;
                currentCategoryIndex = Inventory_Manager.currentFurnitureCategoryIndex;
                totalCategory = Inventory_Manager.FurnitureCategory.Count;
            }else
            {
                currentInventoryState = InventoryState.Item;
                currentCategoryIndex = Inventory_Manager.currentCategoryIndex;
                totalCategory = Inventory_Manager.Category.Count;
            }
            //Debug.Log("is this getting accessed");
            int CategoryIndex = CheckIfOpeningCategoryContainsItem(currentCategoryIndex, true); //we check to see which, from the last selected category, contains an item
            if (CategoryIndex != -1) //if the category index is not equal to -1 (meaning all categories are empty)
            {
                //Debug.Log("-1 you is not: " + CategoryIndex);
                Inventory_Manager.currentCategoryIndex = CategoryIndex; //update the category index that we are in
                Inventory_Manager.currentCategorySlotsIndex = 0; //reset the item index
                //DO i need to get the number of categories here?
                TotalItemForCategory = Inventory_Manager.getCurrentInventory(currentInventoryState)[Inventory_Manager.currentCategoryIndex].Count; //get the total number of items in category
                //Debug.Log("Count: " + TotalItemForCategory);
                UpdateImage(true, true, true); //update both the category and item image
                CheckInventoryUI(true); //Inventory will only open if there is something in it
                return true;
            }

        }
        return false;
    }


    /// <summary>
    /// I THINK THIS IS OLD AND OBSELETE. TREAD CAREFULLY
    /// </summary>
    /*public void debugInventory()
    {
        for(int i = 0; i < DebugInventory.Length; i++)
        {
            Inventory_Manager.AddItemToInventory(DebugInventory[i].GetComponent<BaseItem>());
        }
    }*/


    /// <summary>
    /// Instantiates item from inventory into 
    /// </summary>
    /// <param name="tempSlot"></param>
    /// <param name="removeFromInventory"></param>
    public GameObject SpawnItemFromInventory(InventorySlot tempSlot, bool removeFromInventory)
    {
        GameObject prefabRef = Resources.Load(tempSlot.Category + "/" + tempSlot.Name, typeof(GameObject)) as GameObject; //get a ref to the object from the resources folder
        GameObject ObjectRef = Instantiate(prefabRef, new Vector3(2.85f, 1.31f, 0.42f), Quaternion.identity); //Keep reference of instantiated object
        if (ObjectRef.GetComponent<BaseItem>().hasTag(BaseItem.ItemTags.Container)) //if the object is a container
        {
            ObjectRef.GetComponent<PourObject>().COUNT = Inventory_Manager.getSeeds(ObjectRef.GetComponent<BaseItem>()); //then we need to get the number of seeds it needs
        }
        AttachObjectToHand(ObjectRef);//Attach the instantiated object to the player's hand
        if (removeFromInventory) //does it need to be removed from inventory
        {
            Inventory_Manager.RemoveItemFromInventory(tempSlot, Inventory_Manager.getCurrentInventory(currentInventoryState), ObjectRef.GetComponent<BaseItem>()); //remove it from inv
        }
        return ObjectRef;
    }

    /// <summary>
    /// Attaches object that has been instantiated from player inventory to the player's hand
    /// </summary>
    /// <param name="InstantiatedObject"></param>
    public void AttachObjectToHand(GameObject InstantiatedObject)
    {
        //Attach object to hand
        Hand hand = GetHand();

        hand.GetComponent<OnTriggerRaycast>().PickUpObj(InstantiatedObject);
    }

    /// <summary>
    /// Turns Inventory on/off
    /// </summary>
    /// <param name="isOn"></param>
    public void CheckInventoryUI(bool isOn)
    {
        isInventoryOn = isOn; //updates inventory bool
        if (!isInventoryOn) //if false
        {
            currentCategory_Image.gameObject.SetActive(false); //turn UI images for inventory off
            currentItem_Image.gameObject.SetActive(false);
            currentCount.gameObject.SetActive(false);
            turnOnArrows(false);
            if (SHOW == ShowState.Hand1) //turn off the inventory for the various hands
            {
                UICanvas.gameObject.SetActive(false);
                hand1.GetComponent<OnTriggerRaycast>().DropObj(UICanvas.gameObject);
                //hand1.GetComponent<OnTriggerRaycast>().ENABLED = false;
            }
            else if (SHOW == ShowState.Hand2)
            {
                UICanvas.gameObject.SetActive(false);
                hand2.GetComponent<OnTriggerRaycast>().DropObj(UICanvas.gameObject);
                //hand2.GetComponent<OnTriggerRaycast>().ENABLED = false;
            }

            TEMP_STATE = ShowState.None; //we need to reset all the alpha values for all the components of the inventory.
            Color tmp = currentItem_Image.color;
            alphaValueItem = 1.0f;
            tmp.a = alphaValueItem;
            currentItem_Image.color = tmp;
            tmp = currentCategory_Image.color;
            alphaValueCategory = 1.0f;
            tmp.a = alphaValueCategory;
            currentCategory_Image.color = tmp;
            upArrow.color = tmp;
            downArrow.color = tmp;
            leftArrow.color = tmp;
            rightArrow.color = tmp;
            currentCount.color = tmp;
            turnOnArrows(false);
            currentFadeState = FadeState.Wait;
        }
        else //if true
        {
            currentCategory_Image.gameObject.SetActive(true); //turn UI images for inventory on
            currentItem_Image.gameObject.SetActive(true); //turn the rest of the inventory UI components on
            currentCount.gameObject.SetActive(true);
            turnOnArrows(true);
        }
    }

    /// <summary>
    /// When opening the inventory, check if last category the player was on contains an item. 
    /// If not moves to the next category that has an item.
    /// </summary>
    /// <param name="CategoryIndex"></param>
    /// <returns></returns>
    public int CheckIfOpeningCategoryContainsItem(int CategoryIndex, bool after)
    {
        if (Inventory_Manager.getCurrentInventory(currentInventoryState)[CategoryIndex].Count != 0) //if there is at least 1 item in the opening category
        {
            return CategoryIndex; //return that index
        }
        if (after) { 
            return CheckIfCategoryContainsItem(CategoryIndex); //either find the index for the next category that has an intem
        }else
        {
            Debug.Log("Goes to previous.");
            return CheckIfPreviousCategoryContainsItem(CategoryIndex);
        }
    }

    /// <summary>
    /// Finds which category from the starting index onward has an item in it.
    /// </summary>
    /// <param name="CategoryIndex"></param>
    /// <returns></returns>
    public int CheckIfCategoryContainsItem(int CategoryIndex)
    {
        CategoryIndex += 1; //start off from the next index
        for (int i = 0; i < totalCategory; i++) //go through each possible category
        {
            if (CategoryIndex >= totalCategory) //if we reach the right most category
            {
                CategoryIndex = 0; //loop back to the beginning of index
            }
            if(Inventory_Manager.getCurrentInventory(currentInventoryState)[CategoryIndex].Count != 0) // check if the category has at least one item in it
            {
                return CategoryIndex; //if it does return that index
            }
            CategoryIndex += 1;
        }
        return -1; //all categories are empty
    }

    /// <summary>
    /// Finds which category from the starting index backward has an item in it.
    /// </summary>
    /// <param name="CategoryIndex"></param>
    /// <returns></returns>
    public int CheckIfPreviousCategoryContainsItem (int CategoryIndex)
    {
        CategoryIndex -= 1;
        Debug.Log("Category Index: " + CategoryIndex);
        for (int i = 0; i < totalCategory; i++) //go through each possible category
        {
            if(CategoryIndex < 0)
            {
                CategoryIndex = totalCategory - 1;
            }
            if (Inventory_Manager.getCurrentInventory(currentInventoryState)[CategoryIndex].Count != 0) // check if the category has at least one item in it
            {
                Debug.Log("Category Index before return: " + CategoryIndex);
                return CategoryIndex; //if it does return that index
            }
            CategoryIndex -= 1;

        }
        return -1;
    } 


    /// <summary>
    /// Update the cateogry and item images for the inventory UI
    /// </summary>
    /// <param name="category"></param>
    /// <param name="item"></param>
    public void UpdateImage(bool category, bool item, bool open)
    {
        if (category) //if we have to change the category image
        {
            currentCategory_Image.GetComponent<Image>().sprite = Inventory_Manager.getCurrentInventory(currentInventoryState)[Inventory_Manager.getCurrentCategoryIndex(currentInventoryState)][Inventory_Manager.getCurrentCategorySlotIndex(currentInventoryState)].cIcon; //update to the current category index
            currentItem_Image.GetComponent<Image>().sprite = Inventory_Manager.getCurrentInventory(currentInventoryState)[Inventory_Manager.getCurrentCategoryIndex(currentInventoryState)][Inventory_Manager.getCurrentCategorySlotIndex(currentInventoryState)].Icon; //update to the current item index
            imageTimer = Time.time;            
            
            if (!changeItemImage) //we were not currently changing the item image
            {
                changeCategoryImage = true; //we are now changing the category image
            }
            else //we were changing the item image
            {
                changeItemImage = false; //we are no longer changing the item image
                changeCategoryImage = true; //we are now changing the category image
                Color tmp = currentCategory_Image.color; //we need to set the alpha value of the category to 1 (same for up and down arrows)
                Color tmp2 = currentItem_Image.color; //while the alpha value of the image is set to 0 and the rest of the relevant UI objects 
                alphaValueCategory = 1.0f;
                alphaValueItem = 0.0f;
                tmp.a = alphaValueCategory;
                tmp2.a = alphaValueItem;
                currentCategory_Image.color = tmp;
                currentItem_Image.color = tmp2;
                upArrow.color = tmp;
                downArrow.color = tmp;
            }
        }
        else if (item) // if we have to change the item image 
        {
            currentItem_Image.GetComponent<Image>().sprite = Inventory_Manager.getCurrentInventory(currentInventoryState)[Inventory_Manager.getCurrentCategoryIndex(currentInventoryState)][Inventory_Manager.getCurrentCategorySlotIndex(currentInventoryState)].Icon;//Update to the current index
            imageTimer = Time.time;
            if (!changeCategoryImage) //if we are not changing the category and we are trying to change the item.
            {
                //Debug.Log("Getting accessed.");
                changeItemImage = true; //we can now change the item
            }
            else
            {
                changeCategoryImage = false; // we are no longer changing the category image
                changeItemImage = true; //we can now change the item image
                Color tmp = currentItem_Image.color; //we need to set the alpha values of the inventory UI elements
                Color tmp2 = currentCategory_Image.color;
                alphaValueItem = 1.0f;
                alphaValueCategory = 0.0f;
                tmp.a = alphaValueItem;
                tmp2.a = alphaValueCategory;
                currentItem_Image.color = tmp;
                currentCategory_Image.color = tmp2;
                rightArrow.color = tmp;
                leftArrow.color = tmp;
                currentCount.color = tmp;
            }           
        }
        if (open) // if we are opeing the inventory 
        {
            changeCategoryImage = false; //we are neither changing the category or item image
            changeItemImage = false;
            
            turnOnArrows(true); //arrows should be turned on 
            currentFadeState = FadeState.Wait; // we wait for the player to do something (in regards to the inventory )
            return;    
        }
        currentFadeState = FadeState.FadeOut; //fade out one of the images
    }

    /// <summary>
    /// get the elapsed time
    /// </summary>
    /// <returns></returns>
    public float elapsedTime()
    {
        return Time.time - imageTimer;
    }

    /// <summary>
    /// set the UI arrowns on or off
    /// </summary>
    /// <param name="on"></param>
    public void turnOnArrows(bool on)
    {
        if (on)
        {
            rightArrow.gameObject.SetActive(true);
            leftArrow.gameObject.SetActive(true);
            upArrow.gameObject.SetActive(true);
            downArrow.gameObject.SetActive(true);
        }
        else
        {
            rightArrow.gameObject.SetActive(false);
            leftArrow.gameObject.SetActive(false);
            upArrow.gameObject.SetActive(false);
            downArrow.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// set individual arrows on or off
    /// </summary>
    /// <param name="arrow1"></param>
    /// <param name="arrow2"></param>
    /// <param name="arrow3"></param>
    /// <param name="arrow4"></param>
    public void setArrows(bool arrow1, bool arrow2, bool arrow3, bool arrow4)
    {
        rightArrow.gameObject.SetActive(arrow1);
        leftArrow.gameObject.SetActive(arrow2);
        upArrow.gameObject.SetActive(arrow3);
        downArrow.gameObject.SetActive(arrow4);
    }
}   
