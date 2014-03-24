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
/// This file contains a class for a HipConstraint component designed to
/// replicate the behavior of the am_hipConstraint node in Maya.
/// 
/// Add this component to the first joint in a chain of helper joints to
/// correct twist deformation for a hip. Link up the OrientConstraint
/// components of the remaining twist joints to this component using
/// AppendTwistJoint() from an editor script.
/// </summary>
/// 
/// </file>

/// <summary>
/// A component to drive helper joints for hip twisting
/// </summary>
[AddComponentMenu("Maya/Constraints/Hip Constraint")]
public class HipConstraint : MayaNode
{
	/// <summary>
	/// the object to have the constraint applied - defaults to this GameObject if it is not supplied
	/// </summary>
	[HideInInspector][SerializeField]
	public Transform constrainedObject;
	
	/// <summary>
	/// axis on the hip joint that points to the knee
	/// </summary>
	public Vector3 hipAimAxis
	{
		get { return _hipAimAxis; }
		set
		{
			_hipAimAxis = value.normalized;
			OrthonormalizeHipAxes();
			ComputeFromNormalRotation();
		}
	}
	[SerializeField]
	private Vector3 _hipAimAxis = Vector3.forward;
	/// <summary>
	/// axis on the hip joint that points to the front of the character
	/// </summary>
	public Vector3 hipFrontAxis
	{
		get { return _hipFrontAxis; }
		set
		{
			_hipFrontAxis = value.normalized;
			OrthonormalizeHipAxes();
			ComputeFromNormalRotation();
		}
	}
	[SerializeField]
	private Vector3 _hipFrontAxis = Vector3.up;
	/// <summary>
	/// the hip object down which the twist is applied
	/// </summary>
	public Transform hipObject;
	/// <summary>
	/// cache for hip object name when configuring from an AssetPostprocessor
	/// </summary>
	[HideInInspector][SerializeField]
	public string hipObjectName;
	
	/// <summary>
	/// axis on the pelvis joint that points to the top of the character
	/// </summary>
	public Vector3 pelvisAimAxis
	{
		get { return _pelvisAimAxis; }
		set
		{
			_pelvisAimAxis = value.normalized;
			OrthonormalizePelvisAxes();
		}
	}
	[SerializeField]
	private Vector3 _pelvisAimAxis = Vector3.up;
	/// <summary>
	/// axis on the pelvis joint that points to the front of the character
	/// </summary>
	public Vector3 pelvisFrontAxis
	{
		get { return _pelvisFrontAxis; }
		set
		{
			_pelvisFrontAxis = value.normalized;
			OrthonormalizePelvisAxes();
		}
		
	}
	[SerializeField]
	private Vector3 _pelvisFrontAxis = Vector3.forward;
	/// <summary>
	/// the pelvis object in whose coordinate frame the hip's elevation angle is computed
	/// </summary>
	public Transform pelvisObject;
	/// <summary>
	/// cache for pelvis object name when configuring from an AssetPostprocessor
	/// </summary>
	[HideInInspector][SerializeField]
	public string pelvisObjectName;
	
	/// <summary>
	/// the intermediate helper joints
	/// </summary>
	public OrientConstraint[] twistJoints = new OrientConstraint[0];
	
	/// <summary>
	/// the elevation angle of the hip
	/// </summary>
	public float elevationAngle { get { return 180f-Vector3.Angle(_pelvisAimRotated, _hipAimRotated); } }
	/// <summary>
	/// elevation dot-product of the hip
	/// </summary>
	public float elevationDot { get { return Vector3.Dot(_pelvisAimRotated, _hipAimRotated); } }
	
	/// <summary>
	/// the right axis of the hip
	/// </summary>
	private Vector3 _hipRightAxis;
	/// <summary>
	/// the right axis of the pelvis
	/// </summary>
	private Vector3 _pelvisRightAxis;
	
	/// <summary>
	/// the hip aim axis in world space
	/// </summary>
	private Vector3 _hipAimRotated;
	/// <summary>
	/// the pelvis aim axis in world space
	/// </summary>
	private Vector3 _pelvisAimRotated;
	
	/// <summary>
	/// the desired up-vector for the hip
	/// </summary>
	private Vector3 _targetUpVector;
	
	/// <summary>
	/// precompute the angle to rotate the object from z-forward, y-up into its own custom orientation
	/// </summary>
	private Quaternion _fromNormalRotation;
	
	/// <summary>
	/// Initialize
	/// </summary>
	void Awake()
	{
		if (constrainedObject == null) constrainedObject = transform;
	}
	void Start()
	{
		// compute unknowns from known values
		OrthonormalizeHipAxes();
		OrthonormalizePelvisAxes();
		ComputeFromNormalRotation();
	}
	
	/// <summary>
	/// Apply the constraint automatically if requested
	/// </summary>
	void LateUpdate()
	{
		if (isInvokedExternally) return;
		Compute();
	}

	/// <summary>
	/// Compute the result of the constraint
	/// </summary>
	public override void Compute()
	{
		_hipAimRotated = hipObject.rotation*hipAimAxis;
		_pelvisAimRotated = pelvisObject.rotation*pelvisAimAxis;
		
		float pitchDot = elevationDot;
		float yawDot = Vector3.Dot(pelvisObject.rotation*pelvisFrontAxis, (_hipAimRotated - pelvisObject.rotation*pelvisAimAxis * pitchDot).normalized);
		float interpAmt = -(1f + elevationDot) * Mathf.Abs(yawDot);

		_targetUpVector = Quaternion.AngleAxis(Mathf.Sign(yawDot)*interpAmt*90f, _pelvisRightAxis)*pelvisFrontAxis;
		
		constrainedObject.rotation =  Quaternion.LookRotation(_hipAimRotated, pelvisObject.rotation*_targetUpVector) * _fromNormalRotation;
		
		foreach (OrientConstraint twist in twistJoints) twist.Compute();
	}
	
	/// <summary>
	/// Store the angle from normal rotation (z-forward, y-up) into a custom configuration
	/// </summary>
	void ComputeFromNormalRotation()
	{
		_fromNormalRotation = Quaternion.Inverse(Quaternion.LookRotation(hipAimAxis, hipFrontAxis));
	}
	
	/// <summary>
	/// Compute the up axis for the hip using the known axes
	/// </summary>
	void OrthonormalizeHipAxes()
	{
		Vector3.OrthoNormalize(ref _hipAimAxis, ref _hipFrontAxis, ref _hipRightAxis);
	}
	
	/// <summary>
	/// Compute the right axis on the pelvis using the known axes
	/// </summary>
	void OrthonormalizePelvisAxes()
	{
		Vector3.OrthoNormalize(ref _pelvisAimAxis, ref _pelvisFrontAxis, ref _pelvisRightAxis);
	}
	
	/// <summary>
	/// Add a new twist joint
	/// </summary>
	/// <param name="twistJoint">
	/// A <see cref="OrientConstraint"/>
	/// </param>
	public void AppendTwistJoint(OrientConstraint twistJoint)
	{
		OrientConstraint[] newTwist = new OrientConstraint[twistJoints.Length+1];
		for (int i=0; i<twistJoints.Length; i++)
			newTwist[i] = twistJoints[i];
		newTwist[newTwist.Length-1] = twistJoint;
		twistJoints = newTwist;
		foreach (OrientConstraint twist in twistJoints) twist.isInvokedExternally = true;
	}
}
