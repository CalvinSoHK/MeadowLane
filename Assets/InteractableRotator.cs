using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class InteractableRotator : InteractableCustom {

    float yRotation = 0;

    private void Update()
    {
        Debug.Log(transform.localEulerAngles.y);
    }
}
