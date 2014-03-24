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
/// This file contains a base class for generated Maya expression components.
/// 
/// Inherit from this class when replicating Maya expressions as components.
/// </summary>
/// 
/// </file>

/// <summary>
/// A base class for a generated Maya expression script
/// </summary>
public abstract class MayaExpressions : MayaNode
{
	/// <summary>
	/// cache import scale
	/// </summary>
	public float importScale = 1f;
	/// <summary>
	/// cache division of import scale
	/// </summary>
	public float oneOverImportScale = 1f;
	
	/// <summary>
	/// any MayaNodes that need to be updated before performing an expression
	/// </summary>
	public MayaNode[] upstreamDependencies = new MayaNode[0];
	
	/// <summary>
	/// allocation for getting/setting local positions
	/// </summary>
	protected Vector3 localPosition;
	/// <summary>
	/// allocation for getting/setting local Euler angles
	/// </summary>
	protected Vector3 localEulerAngles;
	/// <summary>
	/// allocation for getting/setting local scale
	/// </summary>
	protected Vector3 localScale;
		
	/// <summary>
	/// Compute expressions if not manually invoked
	/// </summary>
	void LateUpdate()
	{
		if (isInvokedExternally) return;
		Compute();
	}
	
	/// <summary>
	/// Perform all of the expressions contained in the original Maya file
	/// </summary>
	public override void Compute()
	{
		// override in subclass
		foreach (MayaNode node in upstreamDependencies) node.Compute();
	}
}