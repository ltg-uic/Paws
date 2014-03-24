using UnityEditor;
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
/// This file contains a class with static methods for common viewport controls.
/// </summary>
/// 
/// </file>

/// <summary>
/// A class containing helpers for common viewport controls
/// </summary>
public static class ViewportControls : System.Object
{
	/// <summary>
	/// a pixel amount to describe padding in from an edge
	/// </summary>
	public static float viewportPadding = 5f;
	/// <summary>
	/// array of string labels for on/off switches
	/// </summary>
	public static string[] onOffLabels = new string[2] {"Off", "On"};
	
	/// <summary>
	/// A wrapper for GUILayout.BeginArea that automatically handles sizing
	/// </summary>
	/// <param name="width">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <param name="height">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <param name="anchor">
	/// A <see cref="GUIAnchor"/>
	/// </param>
	public static void BeginArea(float width, float height, GUIAnchor anchor)
	{
		// ensure width does not exceed screen area
		width = Mathf.Min(width, Screen.width-2f*viewportPadding);
		
		// begin the area
		GUILayout.BeginArea(new Rect (
			(anchor==GUIAnchor.LowerLeft||anchor==GUIAnchor.TopLeft)?viewportPadding:Screen.width-viewportPadding-width, 
			(anchor==GUIAnchor.TopLeft||anchor==GUIAnchor.TopRight)?viewportPadding:Screen.height-viewportPadding-height, 
			width, 
			height));
	}
	/// <summary>
	/// A wrapper for GUILayout.BeginArea that automatically handles sizing
	/// </summary>
	/// <param name="width">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <param name="anchor">
	/// A <see cref="GUIAnchor"/>
	/// </param>
	public static void BeginArea(float width, GUIAnchor anchor)
	{
		BeginArea(width, Screen.height-2f*viewportPadding, anchor);
	}
	
	/// <summary>
	/// A wrapper for GUILayout.EndArea()
	/// </summary>
	public static void EndArea()
	{
		GUILayout.EndArea();
	}
	
	/// <summary>
	/// Return the value of an on/off switch
	/// </summary>
	/// <param name="label">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="val">
	/// A <see cref="System.Boolean"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public static bool OnOffToggle(string label, bool val)
	{
		GUILayout.BeginHorizontal();
			GUILayout.Label(label);
			val = GUILayout.SelectionGrid((!val)?0:1, onOffLabels, onOffLabels.Length, GUILayout.Width(80f))==1;
		GUILayout.EndHorizontal();
		return val;
	}
}