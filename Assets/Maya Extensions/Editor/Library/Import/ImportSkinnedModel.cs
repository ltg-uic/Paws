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
/// This file contains an AssetPostprocessor that searches an incoming model
/// for a SkinnedMeshRenderer component. If one is found, it searches for an
/// empty transform corresponding to the mesh in the source file, and removes
/// it.
/// 
/// Put this script in an Editor folder and it will automatically operate on
/// incoming files
/// </summary>
/// 
/// </file>

/// <summary>
/// Clean up the importation of a model with a skinned mesh
/// </summary>
public class ImportSkinnedModel : AssetPostprocessor
{
	/// <summary>
	/// static accessor for other AssetPostprocessors
	/// </summary>
	public static int postProcessOrder { get { return -1; } }
	
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
	/// Clean up the incoming model if there's a bunk transform
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	void OnPostprocessModel(GameObject go)
	{
		Component[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>();
		Component[] transforms = go.GetComponentsInChildren<Transform>();
		
		// check for an empty transform corresponding to each skinned mesh name and remove it
		foreach (SkinnedMeshRenderer skin in skins)
		{
			string name = skin.sharedMesh.name;
			Transform candidate = TransformHelpers.GetTransformInHierarchy(transforms, name);
			if (candidate == null) continue;
			if (candidate.GetComponent<SkinnedMeshRenderer>() != null) continue;
			GameObject.DestroyImmediate(candidate.gameObject, true);
		}
	}
}