using UnityEditor;
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
/// This file contains an AssetPostprocessor that searches an incoming file for
/// user properties indicating that it contains data for various Maya nodes to
/// be added to the incoming model as components.
/// 
/// Put this script in an Editor folder and it will automatically operate on
/// incoming files. Files must have correctly-formatted user properties and
/// must be in a source model folder.
/// 
/// This script expects properly-formatted user properties to define nodes. It
/// supports the following node types:
/// 	aimConstraint
/// 	orientConstraint
/// 	pointConstraint
/// 	am_exportTransform
/// 	am_hipConstraint
/// 	am_shoulderConstraint
/// For more detailed information, see adammechtley.com or refer to
/// documentation in the amTools Python package.
/// </summary>
/// 
/// </file>

/// <summary>
/// Import constraints that have been stored in the file
/// </summary>
public class ImportMayaNodes : ImportMayaRigs
{
	/// <summary>
	/// static accessor for other Maya AssetPostprocessors
	/// </summary>
	public static int postProcessOrder { get { return -100; } }
	
	/// <summary>
	/// Ensure this script executes before others by default
	/// </summary>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public override int GetPostprocessOrder ()
	{
		return postProcessOrder;
	}
	
	/// <summary>
	/// Link up components as needed
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	void OnPostprocessModel(GameObject go)
	{
		// early out if this is not a source model
		if (!isInSourceModelFolder) return;
		
		// get the whole DAG hierarchy
		Component[] allDagObjects = go.GetComponentsInChildren<Transform>();
		
		// find all the nodes that have been added as components
		Component[] allMayaNodes = go.GetComponentsInChildren<MayaNode>();
		
		// link references for nodes
		foreach (MayaNode node in allMayaNodes)
		{
			// see if the node is a constraint node
			if (node.GetType().IsSubclassOf(typeof(MayaConstraint)))
			{
				// link targets
				if ((node as MayaConstraint).targets != null)
				{
					foreach (MayaConstraint.Target target in (node as MayaConstraint).targets)
					{
						foreach (Transform dagObject in allDagObjects)
						{
							if (dagObject.name == target.targetName)
								target.target = dagObject;
						}
					}
				}
				
				// aim constraints have a further unique property
				if (node.GetType()==typeof(AimConstraint))
				{
					foreach (Transform dagObject in allDagObjects)
					{
						if (dagObject.name == (node as AimConstraint).worldUpObjectName)
							(node as AimConstraint).worldUpObject = dagObject;
					}
				}
			}
			// see if the node is an am_exposeTransform node
			else if (node.GetType()==typeof(ExposeTransform))
			{
				// link the object
				(node as ExposeTransform).t = node.transform;
				// link the reference
				foreach (Transform dagObject in allDagObjects)
				{
					if (dagObject.name == (node as ExposeTransform).referenceName)
						(node as ExposeTransform).reference = dagObject;
				}
			}
			// see if the node is an am_shoulderConstraint node
			else if (node.GetType()==typeof(ShoulderConstraint))
			{
				// link the shoulder and spine objects
				foreach (Transform dagObject in allDagObjects)
				{
					if (dagObject.name == (node as ShoulderConstraint).shoulderObjectName)
						(node as ShoulderConstraint).shoulderObject = dagObject;
					if (dagObject.name == (node as ShoulderConstraint).spineObjectName)
						(node as ShoulderConstraint).spineObject = dagObject;
				}
			}
			// see if the node is an am_hipConstraint node
			else if (node.GetType()==typeof(HipConstraint))
			{
				// link the shoulder and spine objects
				foreach (Transform dagObject in allDagObjects)
				{
					if (dagObject.name == (node as HipConstraint).hipObjectName)
						(node as HipConstraint).hipObject = dagObject;
					if (dagObject.name == (node as HipConstraint).pelvisObjectName)
						(node as HipConstraint).pelvisObject = dagObject;
				}
			}
		}
		
		// link up twist joints to their primary controllers to ensure proper evaluation order
		foreach (MayaNode node in allMayaNodes)
		{
			if (node.GetType()==typeof(OrientConstraint) && (node as OrientConstraint).targets.Length == 2)
			{
				OrientConstraint oc = node as OrientConstraint;
				
				// search for shoulder constraint target
				ShoulderConstraint sc = oc.targets[0].target.GetComponent<ShoulderConstraint>();
				short other = 1;
				if (sc == null)
				{
					sc = oc.targets[other].target.GetComponent<ShoulderConstraint>();
					other = 0;
				}
				if (sc!=null && oc.targets[other].target==sc.shoulderObject) sc.AppendTwistJoint(oc);
				
				// search for hip constraint target
				HipConstraint hc = oc.targets[0].target.GetComponent<HipConstraint>();
				other = 1;
				if (hc == null)
				{
					hc = oc.targets[other].target.GetComponent<HipConstraint>();
					other = 0;
				}
				if (hc!=null && oc.targets[other].target==hc.hipObject) hc.AppendTwistJoint(oc);
			}
		}
	}
	
	/// <summary>
	/// Read all custom user properties for constraints
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="propNames">
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <param name="values">
	/// A <see cref="System.Object[]"/>
	/// </param>
	void OnPostprocessGameObjectWithUserProperties(GameObject go, string[] propNames, System.Object[] values)
	{
		// early out if the file is not in a source model folder
		if (!isInSourceModelFolder) return;
		
		// parse the properties
		for (int i=0; i<propNames.Length; i++)
		{
			string[] tokens = propNames[i].Split(new string[1] {"__"}, System.StringSplitOptions.None);
			// see if the property is for a constraint
			switch (tokens[0])
			{
			case "aimConstraint":
				ProcessAimConstraintProperty(go, tokens[1], tokens[2], values[i]);
				break;
			case "orientConstraint":
				ProcessOrientConstraintProperty(go, tokens[1], tokens[2], values[i]);
				break;
			case "pointConstraint":
				ProcessPointConstraintProperty(go, tokens[1], tokens[2], values[i]);
				break;
			case "am_exposeTransform":
				ProcessExposeTransformProperty(go, tokens[1], tokens[2], values[i]);
				break;
			case "am_shoulderConstraint":
				ProcessShoulderConstraintProperty(go, tokens[1], tokens[2], values[i]);
				break;
			case "am_hipConstraint":
				ProcessHipConstraintProperty(go, tokens[1], tokens[2], values[i]);
				break;
			}
		}
	}
	
	/// <summary>
	/// Return a MayaNode with the specified name and type on the GameObject
	/// </summary>
	/// <param name="nodeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="nodeType">
	/// A <see cref="System.Type"/>
	/// </param>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <returns>
	/// A <see cref="MayaNode"/>
	/// </returns>
	MayaNode GetMayaNodeOnGameObject(string nodeName, System.Type nodeType, GameObject go)
	{
		MayaNode node = go.GetComponent(nodeType) as MayaNode;
		if (node == null)
		{
			node = go.AddComponent(nodeType) as MayaNode;
			node.nodeName = nodeName;
		}
		Component[] nodes = go.GetComponentsInChildren(nodeType);
		foreach (MayaNode n in nodes)
			if (n.nodeName == nodeName) node = n;
		return node;
	}
	
	/// <summary>
	/// Return whether or not the specified attributeName corresponds to a constraint weight
	/// </summary>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	bool IsConstraintWeightAttribute(string attributeName)
	{
		return new Regex(@".+W\d+$").IsMatch(attributeName);
	}
	
	/// <summary>
	/// Return whether or not the specified attributeName corresponds to a constraint target
	/// </summary>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	bool IsConstraintTargetAttribute(string attributeName)
	{
		return new Regex(@"target\d+targetParentMatrix$").IsMatch(attributeName);
	}
	
	/// <summary>
	/// Process a property for an aimConstraint
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="nodeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="val">
	/// A <see cref="System.Object"/>
	/// </param>
	void ProcessAimConstraintProperty(GameObject go, string nodeName, string attributeName, System.Object val)
	{
		AimConstraint node = GetMayaNodeOnGameObject(nodeName, typeof(AimConstraint), go) as AimConstraint;
		node.constrainedObject = go.transform;
		switch (attributeName)
		{
		case "offset":
			node.offset = (Vector4)val;
			break;
		case "aimVector":
			node.aimVector = (Vector4)val;
			break;
		case "upVector":
			node.upVector = (Vector4)val;
			break;
		case "worldUpVector":
			node.worldUpVector = (Vector4)val;
			break;
		case "worldUpMatrix":
			node.worldUpObjectName = val as string;
			break;
		case "worldUpType":
			node.worldUpType = (AimConstraint.WorldUpType)int.Parse(val as string);
			break;
		default:
			if (IsConstraintWeightAttribute(attributeName))
				node.InsertTargetUsingWeightAttribute(attributeName, (float)val);
			if (IsConstraintTargetAttribute(attributeName))
				node.InsertTargetUsingTargetAttribute(attributeName, val as string);
			break;
		}
	}
	
	/// <summary>
	/// Process a property for an orientConstraint node
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="nodeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="val">
	/// A <see cref="System.Object"/>
	/// </param>
	void ProcessOrientConstraintProperty(GameObject go, string nodeName, string attributeName, System.Object val)
	{
		OrientConstraint node = GetMayaNodeOnGameObject(nodeName, typeof(OrientConstraint), go) as OrientConstraint;
		node.constrainedObject = go.transform;
		switch (attributeName)
		{
		case "offset":
			node.offset = (Vector4)val;
			break;
		case "interpType":
			node.interpType = (QuaternionInterpolationMode)int.Parse(val as string);
			break;
		default:
			if (IsConstraintWeightAttribute(attributeName))
				node.InsertTargetUsingWeightAttribute(attributeName, (float)val);
			if (IsConstraintTargetAttribute(attributeName))
				node.InsertTargetUsingTargetAttribute(attributeName, val as string);
			break;
		}
	}
	
	/// <summary>
	/// Process a property for a pointConstraint node
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="nodeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="val">
	/// A <see cref="System.Object"/>
	/// </param>
	void ProcessPointConstraintProperty(GameObject go, string nodeName, string attributeName, System.Object val)
	{
		PointConstraint node = GetMayaNodeOnGameObject(nodeName, typeof(PointConstraint), go) as PointConstraint;
		node.constrainedObject = go.transform;
		switch (attributeName)
		{
		case "offset":
			node.offset = (Vector4)val;
			break;
		case "constraintOffsetPolarity":
			node.constraintOffsetPolarity = (float)val;
			break;
		default:
			if (IsConstraintWeightAttribute(attributeName))
				node.InsertTargetUsingWeightAttribute(attributeName, (float)val);
			if (IsConstraintTargetAttribute(attributeName))
				node.InsertTargetUsingTargetAttribute(attributeName, val as string);
			break;
		}
	}
	
	/// <summary>
	/// Process a property for an am_exposeTransform node
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="nodeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="val">
	/// A <see cref="System.Object"/>
	/// </param>
	void ProcessExposeTransformProperty(GameObject go, string nodeName, string attributeName, System.Object val)
	{
		ExposeTransform node = GetMayaNodeOnGameObject(nodeName, typeof(ExposeTransform), go) as ExposeTransform;
		switch (attributeName)
		{
		case "reference":
			node.referenceName = val as string;
			break;
		case "rotateOrder":
			node.rotateOrder = (EulerRotationOrder)int.Parse(val as string);
			break;
		case "objectAxis":
			node.objectAxis = (Vector4)val;
			break;
		case "referenceAxis":
			node.referenceAxis = (Vector4)val;
			break;
		}
	}
	
	/// <summary>
	/// Process a property for an am_shoulderConstraint node
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="nodeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="val">
	/// A <see cref="System.Object"/>
	/// </param>
	void ProcessShoulderConstraintProperty(GameObject go, string nodeName, string attributeName, System.Object val)
	{
		ShoulderConstraint node = GetMayaNodeOnGameObject(nodeName, typeof(ShoulderConstraint), go) as ShoulderConstraint;
		switch (attributeName)
		{
		case "raisedAngleOffset":
			node.raisedAngleOffset = (float)val;
			break;
		case "shoulder":
			node.shoulderObjectName = val as string;
			break;
		case "shoulderAimAxis":
			node.shoulderAimAxis = (Vector4)val;
			break;
		case "shoulderFrontAxis":
			node.shoulderFrontAxis = (Vector4)val;
			break;
		case "spine":
			node.spineObjectName = val as string;
			break;
		case "spineAimAxis":
			node.spineAimAxis = (Vector4)val;
			break;
		case "spineFrontAxis":
			node.spineFrontAxis = (Vector4)val;
			break;
		}
	}
	
	/// <summary>
	/// Process a property for an am_hipConstraint node
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	/// <param name="nodeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="attributeName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="val">
	/// A <see cref="System.Object"/>
	/// </param>
	void ProcessHipConstraintProperty(GameObject go, string nodeName, string attributeName, System.Object val)
	{
		HipConstraint node = GetMayaNodeOnGameObject(nodeName, typeof(HipConstraint), go) as HipConstraint;
		switch (attributeName)
		{
		case "hip":
			node.hipObjectName = val as string;
			break;
		case "hipAimAxis":
			node.hipAimAxis = (Vector4)val;
			break;
		case "hipFrontAxis":
			node.hipFrontAxis = (Vector4)val;
			break;
		case "pelvis":
			node.pelvisObjectName = val as string;
			break;
		case "pelvisAimAxis":
			node.pelvisAimAxis = (Vector4)val;
			break;
		case "pelvisFrontAxis":
			node.pelvisFrontAxis = (Vector4)val;
			break;
		}
	}
}