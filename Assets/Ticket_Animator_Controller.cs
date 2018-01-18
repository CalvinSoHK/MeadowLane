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
        ANIM.SetTrigger("TICKET_MOVE");
    }
}
