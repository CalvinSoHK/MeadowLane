using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {
    public Image currentCategory_Image, currentItem_Image; //Category and Item UI image ref
    bool isInventoryOn = false; //ref to whether the inventory UI is on
    int totalCategory; //total number of categories
    int TotalItemForCategory; //total number of items within the category
	// Use this for initialization
	void Start () {
        totalCategory = Inventory_Manager.Category.Count; //get the total number of categorries in the inventory manager
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space) && !isInventoryOn) //if player presses space and the inventory is off
        {
            int CategoryIndex = CheckIfOpeningCategoryContainsItem(Inventory_Manager.currentCategoryIndex); //we check to see which, from the last selected category, contains an item
            if (CategoryIndex != -1) //if the category index is not equal to -1 (meaning all categories are empty)
            {
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
        }

        if (isInventoryOn) //if the inventory is on
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)){ //if we press left on the D-Pad
                Inventory_Manager.currentCategorySlotsIndex -= 1; //move the inventory item to the left
                if (Inventory_Manager.currentCategorySlotsIndex <= 0) // if you go beyond the left most item rotate back to the right
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
                if(Inventory_Manager.currentCategoryIndex >= totalCategory) //if you go beyond the top most category roatate back to the bottom
                {
                    Inventory_Manager.currentCategoryIndex = 0;
                }
                TotalItemForCategory = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex].Count; //get the total number of items in the new category
                Inventory_Manager.currentCategorySlotsIndex = 0; //go to the first object in that category
                UpdateImage(true, true); //update the images of both the category and the item
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) //if we press Down on the D-Pad
            {
                Inventory_Manager.currentCategoryIndex -= 1;
                if (Inventory_Manager.currentCategoryIndex >= 0)
                {
                    Inventory_Manager.currentCategoryIndex = totalCategory - 1;
                }
                TotalItemForCategory = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex].Count; //get the total number of items in the new category
                Inventory_Manager.currentCategorySlotsIndex = 0;
                UpdateImage(true, true);
            }else if (Input.GetKeyDown(KeyCode.P))
            {
                CheckInventoryUI(false); //turn off the Inventory UI
                InventorySlot tempSlot = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex]; // get a ref to the item selected by the player
                if(Inventory_Manager.currentCategoryIndex != 0) //If we are not in the produce section
                {
                    if(tempSlot.TotalNum <= 0) //if the object is already in the scene
                    {
                        //get reference to the object in the scene through the inventory manager                        
                        GameObject tempGameObject = Inventory_Manager.MoveItemToHandOfPlayer(tempSlot.Key);
                        //attach the object to the hand of the player
                    }
                    else //if the object has not been put in the scene yet
                    {
                        SpawnItemFromInventory(tempSlot, true); //spawn the item into the scene
                    }
                }else //If we are in the produce section
                {
                    SpawnItemFromInventory(tempSlot, true); //spawn the produce into the scene
                }
                /*GameObject prefabRef = tempSlot.PrefabRef;
                Instantiate(prefabRef, new Vector3(2.85f, 1.31f, 0.42f), Quaternion.identity);
                Inventory_Manager.RemoveItemFromInventory(tempSlot);*/
            }            
        }        
	}

    public void SpawnItemFromInventory(InventorySlot tempSlot, bool removeFromInventory)
    {
        GameObject prefabRef = tempSlot.PrefabRef;
        //ADD THE INSTANTIATED OBJECT TO THE DICTIONARY OF THE INVENTORY MANAGER
        Instantiate(prefabRef, new Vector3(2.85f, 1.31f, 0.42f), Quaternion.identity);
        if (removeFromInventory)
        {
            Inventory_Manager.RemoveItemFromInventory(tempSlot);
        }
    }

    public void CheckInventoryUI(bool isOn)
    {
        isInventoryOn = isOn;
        if (!isInventoryOn)
        {
            currentCategory_Image.gameObject.SetActive(false);
            currentItem_Image.gameObject.SetActive(false);
        }else
        {
            currentCategory_Image.gameObject.SetActive(true);
            currentItem_Image.gameObject.SetActive(true);
        }
    }

    public int CheckIfOpeningCategoryContainsItem(int CategoryIndex)
    {
        if (Inventory_Manager.CategorySlots[CategoryIndex].Count != 0)
        {
            return CategoryIndex;
        }
        return CheckIfCategoryContainsItem(CategoryIndex);
    }

    public int CheckIfCategoryContainsItem(int CategoryIndex)
    {
        CategoryIndex += 1;
        for (int i = 0; i < totalCategory; i++)
        {
            if (CategoryIndex >= totalCategory)
            {
                CategoryIndex = 0;
            }
            if(Inventory_Manager.CategorySlots[CategoryIndex].Count != 0)
            {
                return CategoryIndex;
            }
            CategoryIndex += 1;

        }
        return -1;
    }    

    public void UpdateImage(bool category, bool item)
    {
        if (category)
        {            
            currentCategory_Image = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex].cIcon;
        }
        if (item)
        {
            currentItem_Image = Inventory_Manager.CategorySlots[Inventory_Manager.currentCategoryIndex][Inventory_Manager.currentCategorySlotsIndex].Icon;
        }
    }
}
