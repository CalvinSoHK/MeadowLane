using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Just calls an update to the sun.
public class InteractionSun : InteractableCustom{

    public GameObject sun;

    public override void Use(Hand hand)
    {
        GameManagerPointer.Instance.LIGHTING_MANAGER.UpdateLight();
    }
}
