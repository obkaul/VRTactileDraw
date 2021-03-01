//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Throwable that uses physics joints to attach instead of just
//			parenting
//
//=============================================================================

using UnityEngine;
using System.Collections.Generic;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	[RequireComponent( typeof( Interactable ) )]
	public class ComplexThrowable : MonoBehaviour
	{
		public enum AttachMode
		{
			FixedJoint,
			Force,
		}

		public float attachForce = 800.0f;
		public float attachForceDamper = 25.0f;

		public AttachMode attachMode = AttachMode.FixedJoint;

		[EnumFlags]
		public Hand.AttachmentFlags attachmentFlags = 0;

		private List<Hand> holdingHands = new List<Hand>();
		private List<Rigidbody> holdingBodies = new List<Rigidbody>();
		private List<Vector3> holdingPoints = new List<Vector3>();

		private List<Rigidbody> rigidBodies = new List<Rigidbody>();

		//-------------------------------------------------
		private void Awake()
		{
			GetComponentsInChildren<Rigidbody>( rigidBodies );
		}


		//-------------------------------------------------
		private void Update()
		{
			for ( var i = 0; i < holdingHands.Count; i++ )
			{
                if (holdingHands[i].IsGrabEnding(this.gameObject))
                {
					PhysicsDetach( holdingHands[i] );
				}
			}
		}


		//-------------------------------------------------
		private void OnHandHoverBegin( Hand hand )
		{
			if ( holdingHands.IndexOf( hand ) == -1 )
			{
				if ( hand.isActive )
				{
					hand.TriggerHapticPulse( 800 );
				}
			}
		}


		//-------------------------------------------------
		private void OnHandHoverEnd( Hand hand )
		{
			if ( holdingHands.IndexOf( hand ) == -1 )
			{
				if (hand.isActive)
				{
					hand.TriggerHapticPulse( 500 );
				}
			}
		}


		//-------------------------------------------------
		private void HandHoverUpdate( Hand hand )
		{
            var startingGrabType = hand.GetGrabStarting();

            if (startingGrabType != GrabTypes.None)
			{
				PhysicsAttach( hand, startingGrabType );
			}
		}


		//-------------------------------------------------
		private void PhysicsAttach( Hand hand, GrabTypes startingGrabType )
		{
			PhysicsDetach( hand );

			Rigidbody holdingBody = null;
			var holdingPoint = Vector3.zero;

			// The hand should grab onto the nearest rigid body
			var closestDistance = float.MaxValue;
			for ( var i = 0; i < rigidBodies.Count; i++ )
			{
				var distance = Vector3.Distance( rigidBodies[i].worldCenterOfMass, hand.transform.position );
				if ( distance < closestDistance )
				{
					holdingBody = rigidBodies[i];
					closestDistance = distance;
				}
			}

			// Couldn't grab onto a body
			if ( holdingBody == null )
				return;

			// Create a fixed joint from the hand to the holding body
			if ( attachMode == AttachMode.FixedJoint )
			{
				var handRigidbody = Util.FindOrAddComponent<Rigidbody>( hand.gameObject );
				handRigidbody.isKinematic = true;

				var handJoint = hand.gameObject.AddComponent<FixedJoint>();
				handJoint.connectedBody = holdingBody;
			}

			// Don't let the hand interact with other things while it's holding us
			hand.HoverLock( null );

			// Affix this point
			var offset = hand.transform.position - holdingBody.worldCenterOfMass;
			offset = Mathf.Min( offset.magnitude, 1.0f ) * offset.normalized;
			holdingPoint = holdingBody.transform.InverseTransformPoint( holdingBody.worldCenterOfMass + offset );

			hand.AttachObject( this.gameObject, startingGrabType, attachmentFlags );

			// Update holding list
			holdingHands.Add( hand );
			holdingBodies.Add( holdingBody );
			holdingPoints.Add( holdingPoint );
		}


		//-------------------------------------------------
		private bool PhysicsDetach( Hand hand )
		{
			var i = holdingHands.IndexOf( hand );

			if ( i != -1 )
			{
				// Detach this object from the hand
				holdingHands[i].DetachObject( this.gameObject, false );

				// Allow the hand to do other things
				holdingHands[i].HoverUnlock( null );

				// Delete any existing joints from the hand
				if ( attachMode == AttachMode.FixedJoint )
				{
					Destroy( holdingHands[i].GetComponent<FixedJoint>() );
				}

				Util.FastRemove( holdingHands, i );
				Util.FastRemove( holdingBodies, i );
				Util.FastRemove( holdingPoints, i );

				return true;
			}

			return false;
		}


		//-------------------------------------------------
		private void FixedUpdate()
		{
			if ( attachMode == AttachMode.Force )
			{
				for ( var i = 0; i < holdingHands.Count; i++ )
				{
					var targetPoint = holdingBodies[i].transform.TransformPoint( holdingPoints[i] );
					var vdisplacement = holdingHands[i].transform.position - targetPoint;

					holdingBodies[i].AddForceAtPosition( attachForce * vdisplacement, targetPoint, ForceMode.Acceleration );
					holdingBodies[i].AddForceAtPosition( -attachForceDamper * holdingBodies[i].GetPointVelocity( targetPoint ), targetPoint, ForceMode.Acceleration );
				}
			}
		}
	}
}
