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
/// This file contains a class for a PointConstraint component designed to
/// replicate the behavior of the pointConstraint node in Maya.
/// 
/// Add this component to an object that needs to lock its position to another
/// object, or that needs to perform a multiple weighted interpolation of
/// several objects' positions.
/// </summary>
/// 
/// </file>

/// <summary>
/// A point constraint node
/// </summary>
[AddComponentMenu("Maya/Constraints/Point Constraint")]
public class PointConstraint : MayaConstraint
{
	/// <summary>
	/// the weight of the offset value
	/// </summary>
	public float constraintOffsetPolarity = 1f;
	
	/// <summary>
	/// the current target position
	/// </summary>
	private Vector3 _desiredPosition;
	
	/// <summary>
	/// Perform weighted average of target transforms and apply offset
	/// </summary>
	public override void Compute()
	{
		// update normalized target weights
		ProcessTargetList();
		
		// get target position as normalized weighted average of target positions
		_desiredPosition = Vector3.zero;
		for (int i=0; i<targets.Length; i++)
		{
			_desiredPosition += _normalizedWeights[i]*(targets[i].position);
		}
		
		// set position
		constrainedObject.position = _desiredPosition + constrainedObject.TransformDirection(offset);
	}
}
