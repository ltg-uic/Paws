using UnityEngine;
using System.Collections;

/// <file>
/// 
/// <author>
/// Adam Mechtley
/// http://adammechtley.com/donations
/// </author>
/// 
/// <copyright>
/// Copyright (c) 2011,  Adam Mechtley.
/// All rights reserved.
/// 
/// Redistribution and use in source and binary forms, with or without
/// modification, are permitted provided that the following conditions are met:
/// 
/// 1. Redistributions of source code must retain the above copyright notice,
/// this list of conditions and the following disclaimer.
/// 
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
/// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
/// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
/// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
/// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
/// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
/// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
/// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
/// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
/// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
/// POSSIBILITY OF SUCH DAMAGE.
/// </copyright>
/// 
/// <summary>
/// This file contains a class for an AimConstraint component designed to
/// replicate the behavior of the aimConstraint node in Maya.
/// 
/// Add this component to an object that needs to aim at another object, or
/// that needs to aim at a point between the multiple weighted interpolation of
/// several objects' positions.
/// 
/// When copying data in from Maya, offset is only correct for rotateOrder XYZ
/// (this is a Maya bug).
/// </summary>
/// 
/// </file>

/// <summary>
/// An aim constraint node
/// </summary>
[AddComponentMenu("Maya/Constraints/Aim Constraint")]
public class AimConstraint : MayaConstraint
{
	/// <summary>
	/// an enum to describe different modes of computing an up-vector
	/// </summary>
	public enum WorldUpType { SceneUp, ObjectUp, ObjectRotationUp, Vector, None }
	
	/// <summary>
	/// axis on the constrained object to look at target
	/// </summary>
	public Vector3 aimVector = Vector3.forward;
	/// <summary>
	/// axis on the constrained object to act as up vector
	/// </summary>
	public Vector3 upVector = Vector3.up;
	
	/// <summary>
	/// the object to use as the up-vector constraint
	/// </summary>
	public Transform worldUpObject;
	/// <summary>
	/// cache for the name of the worldUpObject when configuring from AssetPostprocessor
	/// </summary>
	[HideInInspector]
	public string worldUpObjectName;
	
	/// <summary>
	/// the type for the world up vector computation
	/// </summary>
	public AimConstraint.WorldUpType worldUpType;
	/// <summary>
	/// the axis to use as the up-vector constraint
	/// </summary>
	public Vector3 worldUpVector = Vector3.up;
	
	/// <summary>
	/// the target vector
	/// </summary>
	private Vector3 _desiredAimVector;
	/// <summary>
	/// the world up vector rotated into place
	/// </summary>
	private Vector3 _desiredUpVector;
	/// <summary>
	/// the current position of the constrained object
	/// </summary>
	private Vector3 _constrainedObjectPosition;
	
	/// <summary>
	/// Cache a division for the target list; weights don't need to be normalized
	/// </summary>
	public override void ProcessTargetList()
	{
		_oneOverTargetListLength = 1f/targets.Length;
	}
	
	/// <summary>
	/// Aim the aimVector on the object at the weighted average position of the targets and apply offset
	/// </summary>
	public override void Compute()
	{
		// update target list length
		ProcessTargetList();
		
		// determine the desired up-vector based on the worldUpType
		switch (worldUpType)
		{
		case AimConstraint.WorldUpType.SceneUp:
			_desiredUpVector = Vector3.up;
			break;
		case AimConstraint.WorldUpType.ObjectUp:
			_desiredUpVector = worldUpObject.position - constrainedObject.position;
			break;
		case AimConstraint.WorldUpType.ObjectRotationUp:
			_desiredUpVector = worldUpObject.TransformDirection(worldUpVector);
			break;
		case AimConstraint.WorldUpType.Vector:
			_desiredUpVector = worldUpVector;
			break;
		case AimConstraint.WorldUpType.None:
			// TODO: This presently just does the same thing as SceneUp
			_desiredUpVector = Vector3.up;
			break;
		}
		
		// get target aim vector using weighted average of target vectors
		_desiredAimVector = Vector3.zero;
		_constrainedObjectPosition = constrainedObject.position;
		for (int i=0; i<targets.Length; i++)
		{
			_desiredAimVector += targets[i].weight*(targets[i].position - _constrainedObjectPosition);
		}
		_desiredAimVector *= _oneOverTargetListLength;
		
		// set rotation
		constrainedObject.rotation = QuaternionHelpers.CustomLookRotation(_desiredAimVector, _desiredUpVector, aimVector, upVector) * Quaternion.Euler(offset);
	}
}
