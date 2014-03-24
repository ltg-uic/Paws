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
/// This file contains a class with static methods for working with Transforms.
/// </summary>
/// 
/// </file>

/// <summary>
/// A class for working with Transforms
/// </summary>
public static class TransformHelpers : System.Object
{
	/// <summary>
	/// Locate a Transform by name in a hierarchy starting from root
	/// </summary>
	/// <param name="root">
	/// A <see cref="Transform"/>
	/// </param>
	/// <param name="name">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="Transform"/>
	/// </returns>
	public static Transform GetTransformInHierarchy(Transform root, string name)
	{
		return GetTransformInHierarchy(root.GetComponentsInChildren<Transform>(), name);
	}
	
	/// <summary>
	/// Locate a Transform by name in a hierarchy defined by an array of components
	/// </summary>
	/// <param name="hierarchy">
	/// A <see cref="Component[]"/>
	/// </param>
	/// <param name="name">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="Transform"/>
	/// </returns>
	public static Transform GetTransformInHierarchy(Component[] hierarchy, string name)
	{
		foreach (Transform transform in hierarchy)
		{
			if (transform.name == name) return transform;
			// double check to ensure there is not a namespace prefix applied
			else if (transform.name.Contains(":") && 
				transform.name.Substring(transform.name.LastIndexOf(":")+1) == name)
				return transform;
		}
		return null;
	}
}