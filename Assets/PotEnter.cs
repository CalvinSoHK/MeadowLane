using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotEnter : MonoBehaviour {

    public RecipeGameManager MANAGER;

    private void OnTriggerEnter(Collider other)
    {
        //If it is a base item
        if(other.GetComponent<BaseItem>() != null)
        {
            //If it is a produce AND it is NPC owned
            if (other.GetComponent<BaseItem>().CATEGORY.Trim().Equals("Produce") && other.GetComponent<BaseItem>()._OWNER == BaseItem.Owner.NPC)
            {
                MANAGER.ChooseItem(other.GetComponent<BaseItem>());
                transform.parent.GetComponent<Animator>().SetTrigger("Receive");
                Destroy(other.gameObject);
            }
        }
    }
}
