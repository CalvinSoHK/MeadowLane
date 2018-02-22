using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CheckIfColliding : MonoBehaviour {

    public bool IS_VALID = true, TOO_FAR = false;

    Color VALID_COLOR = Color.green, INVALID_COLOR = Color.red, FAR_COLOR = Color.blue;

    private void Update()
    {
        if (TOO_FAR)
        {
            GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", FAR_COLOR);
            GetComponent<MeshRenderer>().materials[1].SetColor("g_vOutlineColor", FAR_COLOR);
        }
        else if (IS_VALID)
        {     
            GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", VALID_COLOR);
            GetComponent<MeshRenderer>().materials[1].SetColor("g_vOutlineColor", VALID_COLOR);
        }
        else
        {
            GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", INVALID_COLOR);
            GetComponent<MeshRenderer>().materials[1].SetColor("g_vOutlineColor", INVALID_COLOR);
        }
    }

    public void IgnoreCollision(GameObject OBJ)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), OBJ.GetComponent<Collider>());
    }

    private void OnTriggerStay(Collider other)
    {
        IS_VALID = false;
    }

    private void OnTriggerExit(Collider other)
    {
        IS_VALID = true;
    }
}
