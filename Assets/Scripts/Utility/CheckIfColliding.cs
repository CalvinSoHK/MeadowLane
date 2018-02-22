using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CheckIfColliding : MonoBehaviour {

    public bool IS_VALID = true;

    Color VALID_COLOR = Color.green, INVALID_COLOR = Color.red;

    private void Update()
    {
        if (IS_VALID)
        {
            Debug.Log(GetComponent<MeshRenderer>().materials[1].GetColor("g_vOutlineColor"));
            GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", VALID_COLOR);
            GetComponent<MeshRenderer>().materials[1].SetColor("g_vOutlineColor", VALID_COLOR);
        }
        else
        {
            Debug.Log(GetComponent<MeshRenderer>().materials[1].GetColor("g_vOutlineColor"));
            GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", INVALID_COLOR);
            GetComponent<MeshRenderer>().materials[1].SetColor("g_vOutlineColor", INVALID_COLOR);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("Collision!");
        IS_VALID = false;
    }

    private void OnTriggerExit(Collider other)
    {
        IS_VALID = true;
    }
}
