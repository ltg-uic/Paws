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
/// This file contains a class for a ShoulderConstraint component designed to
/// replicate the behavior of the am_shoulderConstraint node in Maya.
/// 
/// Add this component to the first joint in a chain of helper joints to
/// correct twist deformation for a shoulder. Link up the OrientConstraint
/// components of the remaining twist joints to this component using
/// AppendTwistJoint() from an editor script.
/// </summary>
/// 
/// </file>

/// <summary>
/// A component to drive helper joints for shoulder twisting
/// </summary>
[AddComponentMenu("Maya/Constraints/Shoulder Constraint")]
public class ShoulderConstraint : MayaNode
{
	/// <summary>
	/// the object to have the constraint applied - defaults to this GameObject if it is not supplied
	/// </summary>
	[HideInInspector][SerializeField]
	public Transform constrainedObject;
	
	/// <summary>
	/// axis on the shoulder joint that points to the elbow
	/// </summary>
	public Vector3 shoulderAimAxis
	{
		get { return _shoulderAimAxis; }
		set
		{
			_shoulderAimAxis = value.normalized;
			OrthonormalizeShoulderAxes();
			ComputeFromNormalRotation();
		}
	}
	[SerializeField]
	private Vector3 _shoulderAimAxis = Vector3.forward;
	/// <summary>
	/// axis on the shoulder joint that points to the front of the character
	/// </summary>
	public Vector3 shoulderFrontAxis
	{
		get { return _shoulderFrontAxis; }
		set
		{
			_shoulderFrontAxis = value.normalized;
			OrthonormalizeShoulderAxes();
			ComputeFromNormalRotation();
		}
	}
	[SerializeField]
	private Vector3 _shoulderFrontAxis = Vector3.right;
	/// <summary>
	/// the shoulder object down which the twist is applied
	/// </summary>
	public Transform shoulderObject;
	/// <summary>
	/// cache for the shoulder object's name when configuring from an AssetPostprocessor
	/// </summary>
	[HideInInspector][SerializeField]
	public string shoulderObjectName;
	
	/// <summary>
	/// axis on the spine joint that points to the top of the character
	/// </summary>
	public Vector3 spineAimAxis
	{
		get { return _spineAimAxis; }
		set
		{
			_spineAimAxis = value.normalized;
			OrthonormalizeSpineAxes();
			DetermineBodySide();
			ComputeTargetVectors();
		}
	}
	[SerializeField]
	private Vector3 _spineAimAxis = Vector3.forward;
	/// <summary>
	/// axis on the spine joint that points to the front of the character
	/// </summary>
	public Vector3 spineFrontAxis
	{
		get { return _spineFrontAxis; }
		set
		{
			_spineFrontAxis = value.normalized;
			OrthonormalizeSpineAxes();
			DetermineBodySide();
			ComputeTargetVectors();
		}
		
	}
	[SerializeField]
	private Vector3 _spineFrontAxis = Vector3.up;
	/// <summary>
	/// the spine object in whose coordinate frame the shoulder's elevation angle is computed
	/// </summary>
	public Transform spineObject;
	/// <summary>
	/// cache for the spine object's name when configuring from an AssetPostprocessor
	/// </summary>
	[HideInInspector][SerializeField]
	public string spineObjectName;
	
	/// <summary>
	/// how far back should the helper joint's up-vector rotate when the arm is raised?
	/// </summary>
	public float raisedAngleOffset = 45f;
	
	/// <summary>
	/// the intermediate helper joints
	/// </summary>
	public OrientConstraint[] twistJoints = new OrientConstraint[0];
	
	/// <summary>
	/// the elevation angle of the shoulder
	/// </summary>
	public float elevationAngle { get { return 180f-Vector3.Angle(_spineAimRotated, _shoulderAimRotated); } }
	/// <summary>
	/// the elevation dot-product of the shoulder
	/// </summary>
	public float elevationDot { get { return Vector3.Dot(_spineAimRotated, _shoulderAimRotated); } }
	
	/// <summary>
	/// the up axis of the shoulder
	/// </summary>
	private Vector3 _shoulderUpAxis;
	/// <summary>
	/// the right axis of the spine
	/// </summary>
	private Vector3 _spineRightAxis;
	
	/// <summary>
	/// the shoulder aim axis in world space
	/// </summary>
	private Vector3 _shoulderAimRotated;
	/// <summary>
	/// the spine aim axis in world space
	/// </summary>
	private Vector3 _spineAimRotated;
	
	/// <summary>
	/// the desired up-vector for the shoulder
	/// </summary>
	private Vector3 _targetUpVector;
	/// <summary>
	/// the interpolation targets for the helper's up-vector when the arm is raised
	/// </summary>
	private Vector3 _targetUpRaised;
	/// <summary>
	/// the interpolation targets for the helper's up-vector when the arm is at rest
	/// </summary>
	private Vector3 _targetUpRest;
	/// <summary>
	/// the interpolation targets for the helper's up-vector when the arm is lowered
	/// </summary>
	private Vector3 _targetUpLowered;
	
	/// <summary>
	/// the side of the body the helper joint is on will determine rotation order
	/// </summary>
	private float _bodySideScalar;
	
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
		OrthonormalizeShoulderAxes();
		OrthonormalizeSpineAxes();
		ComputeFromNormalRotation();
		
		// store the body side to know if this is a left or right shoulder
		DetermineBodySide();
		
		// compute orientation of each of the target vectors
		ComputeTargetVectors();
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
		_shoulderAimRotated = shoulderObject.rotation*shoulderAimAxis;
		_spineAimRotated = spineObject.rotation*spineAimAxis;
		
		float interpAmt = elevationDot;

		if (interpAmt < 0f) _targetUpVector = Vector3.Lerp(_targetUpRest, _targetUpLowered, -interpAmt);
		else _targetUpVector = Vector3.Lerp(_targetUpRest, _targetUpRaised, interpAmt);
		
		constrainedObject.rotation =  Quaternion.LookRotation(_shoulderAimRotated, spineObject.rotation*_targetUpVector) * _fromNormalRotation;
		
		foreach (OrientConstraint twist in twistJoints) twist.Compute();
	}
		
	/// <summary>
	/// Determine which side of the body this shoulder is on
	/// </summary>
	void DetermineBodySide()
	{
		if (shoulderObject==null || spineObject==null) return;
		_bodySideScalar = Mathf.Sign(Vector3.Dot((shoulderObject.position-spineObject.position), spineObject.rotation*_spineRightAxis));
	}
	
	/// <summary>
	/// Store the angle from normal rotation (z-forward, y-up) into a custom configuration
	/// </summary>
	void ComputeFromNormalRotation()
	{
		_fromNormalRotation = Quaternion.Inverse(Quaternion.LookRotation(shoulderAimAxis, _shoulderUpAxis));
	}
	
	/// <summary>
	/// Compute the up axis for the shoulder using the known axes
	/// </summary>
	void OrthonormalizeShoulderAxes()
	{
		_shoulderUpAxis = Vector3.Cross(shoulderAimAxis, shoulderFrontAxis);
	}
	
	/// <summary>
	/// Compute the right axis on the spine using the known axes
	/// </summary>
	void OrthonormalizeSpineAxes()
	{
		_spineRightAxis = Vector3.Cross(spineFrontAxis, spineAimAxis);
	}
	
	/// <summary>
	/// Determine the current orientation of the target up-vectors
	/// </summary>
	void ComputeTargetVectors()
	{
		_targetUpRest = spineAimAxis * _bodySideScalar;
		_targetUpLowered = -Vector3.Cross(spineAimAxis, spineFrontAxis);
		_targetUpRaised = Quaternion.AngleAxis(raisedAngleOffset, spineAimAxis*_bodySideScalar)*-_spineRightAxis;
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
