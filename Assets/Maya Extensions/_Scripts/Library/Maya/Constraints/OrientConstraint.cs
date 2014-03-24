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
/// This file contains a class for an OrientConstraint component designed to
/// replicate the behavior of the orientConstraint node in Maya.
/// 
/// Add this component to an object that needs to lock its orientation to
/// another object, or that needs to perform a multiple weighted interpolation
/// of several objects' orientations.
/// 
/// See QuaternionHelpers.cs for other notes and limitations.
/// </summary>
/// 
/// </file>

/// <summary>
/// An orient constraint node
/// </summary>
[AddComponentMenu("Maya/Constraints/Orient Constraint")]
public class OrientConstraint : MayaConstraint
{
	/// <summary>
	/// specifies the interpolation method to use for more than 1 target
	/// </summary>
	public QuaternionInterpolationMode interpType = QuaternionInterpolationMode.Average;
	
	/// <summary>
	/// an array of the target rotations
	/// </summary>
	private QuaternionInterpolationTarget[] _targetRotations;
	
	/// <summary>
	/// the current target rotation
	/// </summary>
	private Quaternion _desiredRotation;
	
	/// <summary>
	/// a cache of previous rotations
	/// </summary>
	private Quaternion[] _cache = new Quaternion[16];
	
	/// <summary>
	/// Set the rotation on the object to a weighted average of the targets' rotations and apply offset
	/// </summary>
	public override void Compute()
	{
		// update normalized target weights
		ProcessTargetList();
		
		// store the constraint targets as QuaternionBlendTargets
		StoreQuaternionTargets();
		
		// determine the desired rotation based on the interpType
		_desiredRotation = QuaternionHelpers.Interpolate(_targetRotations, interpType, ref _cache, _oneOverTargetListLength);
		
		// set rotation
		constrainedObject.rotation = _desiredRotation * Quaternion.Euler(offset);
	}
	
	/// <summary>
	/// Convert the blend targets to quaternion targets
	/// </summary>
	void StoreQuaternionTargets()
	{
		_targetRotations = new QuaternionInterpolationTarget[targets.Length];
		for (int i=0; i<_targetRotations.Length; i++)
			_targetRotations[i] = targets[i].ToQuatInterpTarget(_normalizedWeights[i]);
	}
}