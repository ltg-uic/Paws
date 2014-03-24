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
/// This file contains a class for an ExposeTransform component designed to
/// replicate the behavior of the am_exposeTransform node in Maya.
/// 
/// Add this component to an object you want to know about in the space of some
/// other transfom, and use its various properties from other scripts as
/// needed.
/// 
/// If no reference object is supplied, component performs all computations
/// using world matrix.
/// </summary>
/// 
/// </file>

/// <summary>
/// A component to get information about one transform in the space of another
/// </summary>
[AddComponentMenu("Maya/Utilities/Expose Transform")]
public class ExposeTransform : MayaNode
{
	/// <summary>
	/// the object being exposed
	/// </summary>
	[HideInInspector][SerializeField]
	public Transform t;
	/// <summary>
	/// cache name of frame of reference when configuring via an AssetPostprocessor
	/// </summary>
	[HideInInspector][SerializeField]
	public string referenceName;
	/// <summary>
	/// frame of reference
	/// </summary>
	public Transform reference;
	/// <summary>
	/// Euler angle order for decomposition
	/// </summary>
	public EulerRotationOrder rotateOrder;
	/// <summary>
	/// axis on the exposed object
	/// </summary>
	public Vector3 objectAxis = Vector3.forward;
	/// <summary>
	/// axis on the reference object
	/// </summary>
	public Vector3 referenceAxis = Vector3.forward;
	/// <summary>
	/// should the axes be normalized during computation?
	/// </summary>
	public bool normalizeAxes = true;
	
	/// <summary>
	/// position of transform relative to reference
	/// </summary>
	public Vector3 position
	{
		get
		{
			if (reference == null) return t.position;
			else return reference.InverseTransformPoint(t.position);
		}
	}
	
	/// <summary>
	/// distance from reference to transform
	/// </summary>
	public float distance
	{
		get
		{
			if (reference == null) return t.position.magnitude;
			else return Vector3.Magnitude(reference.InverseTransformPoint(t.position));
		}
	}
	
	/// <summary>
	/// Euler rotation of transform with respect to reference
	/// </summary>
	public Vector3 rotation
	{
		get
		{
			if (reference == null) return QuaternionHelpers.ToEulerAngles(t.rotation, rotateOrder);
			else return QuaternionHelpers.ToEulerAngles(Quaternion.Inverse(reference.rotation) * t.rotation, rotateOrder);
		}
	}
	
	/// <summary>
	/// dot product of specified axes on the transform and the reference
	/// </summary>
	public float dot
	{
		get
		{
			if (reference == null)
			{
				if (normalizeAxes) return Vector3.Dot(referenceAxis.normalized, t.TransformDirection(objectAxis));
				else return Vector3.Dot(referenceAxis, t.TransformPoint(objectAxis));
			}
			else
			{
				if (normalizeAxes) return Vector3.Dot(reference.TransformDirection(referenceAxis), t.TransformDirection(objectAxis));
				else return Vector3.Dot(reference.TransformPoint(referenceAxis), t.TransformPoint(objectAxis));
			}
		}
	}
	
	/// <summary>
	/// angle between the specified axes on the transform and the reference
	/// </summary>
	public float angle
	{
		get
		{
			if (reference == null) return Vector3.Angle(referenceAxis, t.TransformDirection(objectAxis));
			else return Vector3.Angle(reference.TransformDirection(referenceAxis), t.TransformDirection(objectAxis));
		}
	}
	
	/// <summary>
	/// dot product from the axis on the transform to the position of the reference
	/// </summary>
	public float dotToTarget
	{
		get
		{
			if (reference == null)
			{
				if (normalizeAxes) return Vector3.Dot(-t.position.normalized, t.TransformDirection(objectAxis));
				else return Vector3.Dot(-t.position, t.TransformPoint(objectAxis));
			}
			else
			{
				if (normalizeAxes) return Vector3.Dot((reference.position-t.position).normalized, t.TransformDirection(objectAxis));
				else return Vector3.Dot(reference.position-t.position, t.TransformPoint(objectAxis));
			}
		}
	}
	
	/// <summary>
	/// angle from the axis on the transform to the position of the reference
	/// </summary>
	public float angleToTarget
	{
		get
		{
			if (reference == null) return Vector3.Angle(-t.position, t.TransformDirection(objectAxis));
			else return Vector3.Angle(reference.position-t.position, t.TransformDirection(objectAxis));
		}
	}
	
	/// <summary>
	/// Initialize
	/// </summary>
	void Awake()
	{
		if (t==null) t = transform;
	}
}