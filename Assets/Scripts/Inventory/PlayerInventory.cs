﻿using System.Collections;
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

    //Pos offset for the inventoryUI on being called
    public Vector3 POS_OFFSET;
    public Vector3 ROT_OFFSET;

    public Canvas UICanvas;
    public Image currentCategory_Image, currentItem_Image; //Category and Item UI image ref
    bool isInventoryOn = false; //ref to whether the inventory UI is on
    int totalCategory; //total number of categories
    int TotalItemForCategory; //total number of items within the category

    public GameObject[] DebugInventory;
	// Use this for initialization
	void Start () {
        Inventory_Manager.LoadPlayerInventory();
        totalCategory = Inventory_Manager.Category.Count; //get the total number of categorries in the inventory manager
        hand1 = transform.GetChild(0).Find("Hand1").GetComponent<Hand>();
        hand2 = transform.GetChild(0).Find("Hand2").GetComponent<Hand>();
    }
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("inventory is being debugged");
            debugInventory();
        }
        /*
        if (Input.GetKeyDown(KeyCode.Space) && !isInventoryOn) //if player presses space and the inventory is off
        {
            Debug.Log("is this getting accessed");
            int CategoryIndex = CheckIfOpeningCategoryContainsItem(Inventory_Manager.currentCategoryIndex, true); //we check to see which, from the last selected category, contains an item
            if (CategoryIndex != -1) //if the category index is not equal to -1 (meaning all categories are empty)
            {
                Debug.Log("-1 you is not");
                Inventory_Manager.currentCategoryIndex = CategoryIndex; //update the category index that we are in
                Inventory_Manager.currentCategorySlotsIndex = 0; //reset the item index
                TotalItemForCategory = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex].Count; //get the total number of items in category
                UpdateImage(true, true); //update both the category and item image
                CheckInventoryUI(true); //Inventory will only open if there is something in it
            }
        }

        else if (Input.GetKeyDown(KeyCode.Space) && isInventoryOn)// if we press space and the inventory is active
        {
            CheckInventoryUI(false); //then we turn off the inventory
        }*/

        if (isInventoryOn) //if the inventory is on
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)){ //if we press left on the D-Pad
                Inventory_Manager.currentCategorySlotsIndex -= 1; //move the inventory item to the left
                if (Inventory_Manager.currentCategorySlotsIndex < 0) // if you go beyond the left most item rotate back to the right
                {
                    Inventory_Manager.currentCategorySlotsIndex = TotalItemForCategory - 1;
                }
                UpdateImage(false, true); // update the image of the item
            }else if (Input.GetKeyDown(KeyCode.RightArrow)) //if we press Right on the D-Pad
            {
                Inventory_Manager.currentCategorySlotsIndex += 1; //move the inventory item to the right
                if(Inventory_Manager.currentCategorySlotsIndex >= TotalItemForCategory) //if you go beyond the right most item rotate back to the left
                {
                    Inventory_Manager.currentCategorySlotsIndex = 0;
                }
                UpdateImage(false, true); //update the image of the item
            }else if (Input.GetKeyDown(KeyCode.UpArrow)) //if we press Up on the D-Pad
            {
                Inventory_Manager.currentCategoryIndex += 1; //get the next category in the list
                
                if(Inventory_Manager.currentCategoryIndex >= totalCategory) //if you go beyond the top most category rotate back to the bottom
                {
                    Inventory_Manager.currentCategoryIndex = 0;
                }
                Inventory_Manager.currentCategoryIndex = CheckIfOpeningCategoryContainsItem(Inventory_Manager.currentCategoryIndex, true);
                TotalItemForCategory = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex].Count; //get the total number of items in the new category
                Inventory_Manager.currentCategorySlotsIndex = 0; //go to the first object in that category
                UpdateImage(true, true); //update the images of both the category and the item
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) //if we press Down on the D-Pad
            {
                Inventory_Manager.currentCategoryIndex -= 1; //get the previous category in the list
                if (Inventory_Manager.currentCategoryIndex < 0) //if you go beyond the bottom most category rotate back to the bottom
                {
                    Debug.Log(Inventory_Manager.currentCategoryIndex);
                    Inventory_Manager.currentCategoryIndex = totalCategory - 1;
                    Debug.Log(Inventory_Manager.currentCategoryIndex);
                }
                Inventory_Manager.currentCategoryIndex = CheckIfOpeningCategoryContainsItem(Inventory_Manager.currentCategoryIndex, false);
                Debug.Log(Inventory_Manager.currentCategoryIndex);
                TotalItemForCategory = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex].Count; //get the total number of items in the new category
                Inventory_Manager.currentCategorySlotsIndex = 0; //go to the first object in that category
                UpdateImage(true, true); //update the category and item image
            }else if (Input.GetKeyDown(KeyCode.P)) //If we press the center button of the D-Pad
            {
                CheckInventoryUI(false); //turn off the Inventory UI
                InventorySlot tempSlot = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex]; // get a ref to the item selected by the player
                if(Inventory_Manager.currentCategoryIndex != 0) //If we are not in the produce section
                {
                    if(tempSlot.TotalNum <= 0) //if the object is already in the scene
                    {
                        //get reference to the object in the scene through the inventory manager                        
                        GameObject tempGameObject = Inventory_Manager.MoveItemToHandOfPlayer(tempSlot.Key);
                        AttachObjectToHand(tempGameObject); //attach that object to the hand
                    }
                    else //if the object has not been put in the scene yet
                    {
                        SpawnItemFromInventory(tempSlot, true); //spawn the item into the scene
                    }
                }
                else //If we are in the produce section
                {
                    SpawnItemFromInventory(tempSlot, true); //spawn the produce into the scene
                }
                /*GameObject prefabRef = tempSlot.PrefabRef;
                Instantiate(prefabRef, new Vector3(2.85f, 1.31f, 0.42f), Quaternion.identity);
                Inventory_Manager.RemoveItemFromInventory(tempSlot);*/
            }            
        }        
	}

    //Only brings up the inventory on the given hand.
    public void CallInventory(Hand hand)
    {
        //Only allow it if the hand isn't holding anything.
        if(hand.currentAttachedObject.name == hand.controllerPrefab.name + "_" + hand.name)
        {
            //If we are in the NONE show state, just show the phone on this hand
            if (SHOW == ShowState.None)
            {
                ShowInventory();
                if (hand == hand1)
                {
                    SHOW = ShowState.Hand1;
                }
                else
                {
                    SHOW = ShowState.Hand2;
                }
            }
            else if (SHOW == ShowState.Hand1)
            {
                //If the hand that called it wasn't hand1, move the UI to the other
                if (hand != hand1)
                {
                    SHOW = ShowState.Hand2;
                }
            }
            else if (SHOW == ShowState.Hand2)
            {
                //If the hand that called it wasn't hand2, mvoe the UI to the other.
                if (hand != hand2)
                {
                    SHOW = ShowState.Hand1;
                }
            }
            hand.GetComponent<OnTriggerRaycast>().PickUpObj(UICanvas.gameObject);
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
                SHOW = ShowState.None;
                hand.GetComponent<OnTriggerRaycast>().DropObj(UICanvas.gameObject);
            }
        }
        else if (SHOW == ShowState.Hand2)
        {
            //If the hand that called it wasn't hand2, mvoe the UI to the other.
            if (hand == hand2)
            {
                CheckInventoryUI(false);
                SHOW = ShowState.None;
                hand.GetComponent<OnTriggerRaycast>().DropObj(UICanvas.gameObject);
            }
        }
    }

    /// <summary>
    /// Places inventory on the given hand
    /// </summary>
    /// <param name="hand"></param>
    public void MoveInventory(Hand hand)
    {
        UICanvas.transform.parent = hand.transform;
        UICanvas.transform.localPosition = POS_OFFSET;
        UICanvas.transform.localRotation = Quaternion.Euler(ROT_OFFSET);
    }

    /// <summary>
    ///Brings up inventory on given hand
    /// </summary>
    public void ShowInventory()
    {
        if (!isInventoryOn) //if player presses space and the inventory is off
        {
            Debug.Log("is this getting accessed");
            int CategoryIndex = CheckIfOpeningCategoryContainsItem(Inventory_Manager.currentCategoryIndex, true); //we check to see which, from the last selected category, contains an item
            if (CategoryIndex != -1) //if the category index is not equal to -1 (meaning all categories are empty)
            {
                Debug.Log("-1 you is not");
                Inventory_Manager.currentCategoryIndex = CategoryIndex; //update the category index that we are in
                Inventory_Manager.currentCategorySlotsIndex = 0; //reset the item index
                TotalItemForCategory = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex].Count; //get the total number of items in category
                UpdateImage(true, true); //update both the category and item image
                CheckInventoryUI(true); //Inventory will only open if there is something in it
            }

        }
    }

    public void debugInventory()
    {
        for(int i = 0; i < DebugInventory.Length; i++)
        {
            Inventory_Manager.AddItemToInventory(DebugInventory[i].GetComponent<BaseItem>());
        }
    }


    /// <summary>
    /// Instantiates item from inventory into 
    /// </summary>
    /// <param name="tempSlot"></param>
    /// <param name="removeFromInventory"></param>
    public void SpawnItemFromInventory(InventorySlot tempSlot, bool removeFromInventory)
    {
        GameObject prefabRef = tempSlot.PrefabRef; //get prefab reference of object
        GameObject ObjectRef = Instantiate(prefabRef, new Vector3(2.85f, 1.31f, 0.42f), Quaternion.identity); //Keep reference of instantiated object
        Inventory_Manager.AddItemToDictionary(tempSlot.Key, ObjectRef); //Add the instantiated object to the Dictionary in Inventory Manager that keeps track of inventory items in the scene
        AttachObjectToHand(ObjectRef);//Attach the instantiated object to the player's hand
        if (removeFromInventory) //does it need to be removed from inventory
        {
            Inventory_Manager.RemoveItemFromInventory(tempSlot); //remove it from inv
        }
    }

    /// <summary>
    /// Attaches object that has been instantiated from player inventory to the player's hand
    /// </summary>
    /// <param name="InstantiatedObject"></param>
    public void AttachObjectToHand(GameObject InstantiatedObject)
    {
        //Attach object to hand
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
            SHOW = ShowState.None;
        }
        else //if true
        {
            currentCategory_Image.gameObject.SetActive(true); //turn UI images for inventory on
            currentItem_Image.gameObject.SetActive(true);
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
        if (Inventory_Manager.CategorySlots[CategoryIndex].Count != 0) //if there is at least 1 item in the opening category
        {
            return CategoryIndex; //return that index
        }
        if (after) { 
            return CheckIfCategoryContainsItem(CategoryIndex); //either find the index for the next category that has an intem
        }else
        {
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
            if(Inventory_Manager.CategorySlots[CategoryIndex].Count != 0) // check if the category has at least one item in it
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
            if(CategoryIndex <= 0)
            {
                CategoryIndex = totalCategory - 1;
            }
            if (Inventory_Manager.CategorySlots[CategoryIndex].Count != 0) // check if the category has at least one item in it
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
    public void UpdateImage(bool category, bool item)
    {
        if (category) //if we have to change the category image
        {
            currentCategory_Image.GetComponent<Image>().sprite = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex].cIcon.GetComponent<Image>().sprite; //update to the current category index
        }
        if (item) // if we have to change the item image 
        {
            currentItem_Image.GetComponent<Image>().sprite = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex].Icon.GetComponent<Image>().sprite; //update to the current item index
        }
    }
}
