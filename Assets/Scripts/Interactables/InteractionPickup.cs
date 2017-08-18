using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

//Script is put onto objects we want to be able to pickup
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VelocityEstimator))]
public class InteractionPickup : InteractableCustom {

    private VelocityEstimator velocityEstimator;

    //Multiplier that affects how much of the velocity passes over onto the item when thrown.
    public float velocityMutiplier = 1;

    //Whether or not the object is held
    public bool isHeld = false;

    void Awake()
    {
        velocityEstimator = GetComponent<VelocityEstimator>();
    }
    //Write use function to pick up object
    public override void Use(Hand hand)
    {
        hand.AttachObject(gameObject);
    }

    //Happens whenever a hand is near the object
    public virtual void HandHoverUpdate(Hand hand)
    {
        //If we get the main button down
        if (hand.GetStandardInteractionButtonDown())
        {
            hand.AttachObject(gameObject);
        }
    }

    //Happens whenever this object is attached to a hand
    public virtual void OnAttachedToHand(Hand hand)
    {
        isHeld = true;
        GetComponent<Rigidbody>().isKinematic = true;
        if (hand.controller == null)
        {
            velocityEstimator.BeginEstimatingVelocity();
        }
    }

    //Happens every frame while held by a hand
    public virtual void HandAttachedUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButtonDown())
        {
            hand.DetachObject(gameObject);
        } 
    }

    //Happens when the object is detaches from the hand
    public virtual void OnDetachedFromHand(Hand hand)
    {
        isHeld = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        //GetComponent<Rigidbody>().isKinematic = false;
        rb.isKinematic = false;

        Vector3 position = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        Vector3 angularVelocity = Vector3.zero;

        if (hand.controller == null)
        {
            velocityEstimator.FinishEstimatingVelocity();
            velocity = velocityEstimator.GetVelocityEstimate();
            angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
            position = velocityEstimator.transform.position;
        }
        else
        {
            velocity = Player.instance.trackingOriginTransform.TransformVector(hand.controller.velocity) * velocityMutiplier;
            angularVelocity = Player.instance.trackingOriginTransform.TransformVector(hand.controller.angularVelocity) * velocityMutiplier;
            position = hand.transform.position;
        }

        Vector3 r = transform.TransformPoint(rb.centerOfMass) - position;
        rb.velocity = velocity + Vector3.Cross(angularVelocity, r);
        rb.angularVelocity = angularVelocity;

        float timeUntilFixedUpdate = (Time.fixedDeltaTime + Time.fixedTime) - Time.time;
        transform.position += timeUntilFixedUpdate * velocity;
        float angle = Mathf.Rad2Deg * angularVelocity.magnitude;
        Vector3 axis = angularVelocity.normalized;
        transform.rotation *= Quaternion.AngleAxis(angle * timeUntilFixedUpdate, axis);

    }
}
