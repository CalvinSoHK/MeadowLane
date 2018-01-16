//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Collider dangling from the player's head
//
//=============================================================================

//Edited by Studio Outis
//Added. Rotates with the player head

using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( CapsuleCollider ) )]
	public class BodyCollider : MonoBehaviour
	{
		public Transform head;

		private CapsuleCollider capsuleCollider;

		//-------------------------------------------------
		void Awake()
		{
			capsuleCollider = GetComponent<CapsuleCollider>();
		}


		//-------------------------------------------------
		void FixedUpdate()
		{
            //Calculate the distance from the floor
			float distanceFromFloor = Vector3.Dot( head.localPosition, Vector3.up );

            //Set the height equal to the max
			capsuleCollider.height = Mathf.Max( capsuleCollider.radius, distanceFromFloor );

            //Set the collider's position so the camera is at the top
			transform.localPosition = head.localPosition -  distanceFromFloor * Vector3.up;

            //Rotate the collider to match the camera's Y rotation
            //transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, head.localEulerAngles.y, transform.localEulerAngles.z);
		}
	}
}
