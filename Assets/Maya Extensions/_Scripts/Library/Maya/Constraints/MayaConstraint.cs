using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

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
/// This file contains a base class for components based on constraint nodes in
/// Maya, as well as a class for constraint targets.
/// 
/// Inherit from this class when replicating Maya constraints as components.
/// </summary>
/// 
/// </file>

/// <summary>
/// A base class for a Maya constraint node
/// </summary>
public abstract class MayaConstraint : MayaNode
{
	/// <summary>
	/// A target for a MayaConstraint
	/// </summary>
	[System.Serializable]
	public class Target : System.Object
	{
		/// <summary>
		/// the name of the target transform, in order to search for it later as in e.g. ImportMayaNodes
		/// </summary>
		[HideInInspector]
		public string targetName;
		/// <summary>
		/// the target transform
		/// </summary>
		public Transform target;
		/// <summary>
		/// the influence of the target on the constraint
		/// </summary>
		public float weight = 1f;
		
		/// <summary>
		/// accessors to get target properties
		/// </summary>
		public Vector3 position { get { return target.position; } }
		public Quaternion rotation { get { return target.rotation; } }
		
		/// <summary>
		/// Convert the target to a QuaternionInterpolationTarget for QuaternionHelper functions
		/// </summary>
		/// <param name="normalizedWeight">
		/// A <see cref="System.Single"/>
		/// </param>
		/// <returns>
		/// A <see cref="QuaternionInterpolationTarget"/>
		/// </returns>
		public QuaternionInterpolationTarget ToQuatInterpTarget(float normalizedWeight)
		{
			QuaternionInterpolationTarget t = new QuaternionInterpolationTarget();
			t.quaternion = target.rotation;
			t.weight = normalizedWeight;
			return t;
		}
	}
	
	/// <summary>
	/// the constrained object's transform
	/// </summary>
	public Transform constrainedObject;
	
	/// <summary>
	/// an array of targets influencing the constraint
	/// </summary>
	public MayaConstraint.Target[] targets = new MayaConstraint.Target[1];
	
	/// <summary>
	/// a premultiplied value to avoid doing division constantly
	/// </summary>
	protected float _oneOverTargetListLength;
	/// <summary>
	/// an array of all of the target weights with their sum normalized to 1
	/// </summary>
	protected float[] _normalizedWeights;
	protected float _sumOfAllWeights;
	protected float _oneOverSumOfAllWeights;
	
	/// <summary>
	/// offset may either be a position or rotation value depending upon the constraint
	/// </summary>
	public Vector3 offset = Vector3.zero;
	
	/// <summary>
	/// Initialize
	/// </summary>
	void Awake()
	{
		// grab the transform if none is specified
		if (constrainedObject == null) constrainedObject = transform;
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
	/// Store a normalized array of all target weights and cache a division for the target list
	/// </summary>
	public virtual void ProcessTargetList()
	{
		_sumOfAllWeights = 0f;
		for (int i=0; i<targets.Length; i++)
			_sumOfAllWeights += targets[i].weight;
		if (Mathf.Approximately(_sumOfAllWeights, 0f)) // reflect Maya divide-by-zero behavior
			_oneOverSumOfAllWeights = 0f;
		else _oneOverSumOfAllWeights = 1f/_sumOfAllWeights;
		
		_normalizedWeights = new float[targets.Length];
		for (int i=0; i<targets.Length; i++)
			_normalizedWeights[i] = targets[i].weight*_oneOverSumOfAllWeights;
		
		_oneOverTargetListLength = 1f/targets.Length;
	}
		
	/// <summary>
	/// Insert a MayaConstraint.Target placeholder into targets using its attribute name
	/// </summary>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="weight">
	/// A <see cref="System.Single"/>
	/// </param>
	public void InsertTargetUsingWeightAttribute(string attributeName, float weight)
	{
		// attribute names from Maya have the format targetObjectNameW##
		int targetIndex = int.Parse(new Regex(@"(?<=W)\d+$").Match(attributeName).Value);
		
		if (targets == null) targets = new MayaConstraint.Target[targetIndex+1];
		if (targets.Length <= targetIndex)
		{
			MayaConstraint.Target[] newTargetList = new MayaConstraint.Target[targetIndex+1];
			for (int i=0; i<targets.Length; i++)
				newTargetList[i] = targets[i];
			targets = newTargetList;
		}
		if (targets[targetIndex] == null)
			targets[targetIndex] = new MayaConstraint.Target();
		targets[targetIndex].weight = weight;
	}
	
	/// <summary>
	/// Insert a MayaConstraint.Target placeholder into targets using its attribute name
	/// </summary>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="targetName">
	/// A <see cref="System.String"/>
	/// </param>
	public void InsertTargetUsingTargetAttribute(string attributeName, string targetName)
	{
		// attribute names from Maya have the format target##targetParentMatrix
		int targetIndex = int.Parse(new Regex(@"(?<=target)\d+").Match(attributeName).Value);
		
		if (targets == null) targets = new MayaConstraint.Target[targetIndex+1];
		if (targets.Length <= targetIndex)
		{
			MayaConstraint.Target[] newTargetList = new MayaConstraint.Target[targetIndex+1];
			for (int i=0; i<targets.Length; i++)
				newTargetList[i] = targets[i];
			targets = newTargetList;
		}
		if (targets[targetIndex] == null)
			targets[targetIndex] = new MayaConstraint.Target();
		targets[targetIndex].targetName = targetName;
	}
}