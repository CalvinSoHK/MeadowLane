using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticket_Animator_Controller : MonoBehaviour {

    Animator ANIM;

    private void Awake()
    {
        ANIM = GetComponent<Animator>();
    }

    public void MoveTicket()
    {
        //If we were interactable
        if(gameObject.layer == 8)
        {
            //Set to default
            gameObject.layer = 0;
        }
        else
        {
            //Set to interactable
            gameObject.layer = 8;
        }
        ANIM.SetTrigger("TICKET_MOVE");
    }
}
