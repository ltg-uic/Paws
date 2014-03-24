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
/// This file contains an editor class that provides controls for visually
/// confirming BlendShape data.
/// 
/// Put this script in an Editor folder and it should automatically work when
/// you add a BlendShape component to an object.
/// 
/// There is some minor validation in this script, but the general expectation
/// is that the BlendShape component being debugged has been set up with an
/// editor script (see ImportBlendShapeData.cs).
/// </summary>
/// 
/// </file>

/// <summary>
/// A custom editor for BlendShape components
/// </summary>
[CustomEditor(typeof(BlendShape))]
public class BlendShapeEditor : Editor
{
	/// <summary>
	/// the target object
	/// </summary>
	static BlendShape blendShape;
	
	/// <summary>
	/// the color for the debug lines
	/// </summary>
	private static Color debugColor = Color.magenta;
	
	/// <summary>
	/// is debug drawing mode currently enabled?
	/// </summary>
	public static bool isDebugEnabled
	{
		get { return _isDebugEnabled; }
		set
		{
			EditorPrefs.SetInt("Editor - BlendShapeEditor - isDebugEnabled", (value)?1:0);
			_isDebugEnabled = value;
		}
	}
	private static bool _isDebugEnabled = EditorPrefs.GetInt("Editor - BlendShapeEditor - isDebugEnabled", 0) == 1;
	
	/// <summary>
	/// the index of the target to display
	/// </summary>
	int debugTargetIndex = 0;
	
	/// <summary>
	/// the width of the viewport controls group
	/// </summary>
	float viewportControlsWidth = 200f;
	
	/// <summary>
	/// Initialize
	/// </summary>
	void OnEnable()
	{
		blendShape = (BlendShape) target;
	}
	
	/// <summary>
	/// Draw stuff
	/// </summary>
	void OnSceneGUI()
	{
		// on-screen GUI
		Handles.BeginGUI();
		
		// compute height based on how many targets there are in order to pin to lower right
		int targetButtonColumns = 2;
		float viewportControlsHeight = 20f + ((blendShape.targets.Length-1)/2+1)*21f;
		GUILayout.BeginArea(new Rect(
			Screen.width-2f*ViewportControls.viewportPadding-viewportControlsWidth,
			Screen.height-viewportControlsHeight-40f-ViewportControls.viewportPadding, // BUG: Unity reports Screen.height at +40 pixels?
			viewportControlsWidth,
			viewportControlsHeight));
		{
			GUILayout.BeginVertical();
			{	
				if (blendShape.seamlessBaseMesh == null)
				{
					GUILayout.Label("Specify seamlessBaseMesh to preview data.");
				}
				else
				{
					// checkbox for debug mode
					bool bVal = GUILayout.Toggle(isDebugEnabled, "Draw BlendShape Debug Info");
					if (bVal != isDebugEnabled) isDebugEnabled = bVal;
					
					// selector for current shape
					if (blendShape.targets.Length>0)
					{
						debugTargetIndex = Mathf.Min(debugTargetIndex, blendShape.targets.Length-1);
						debugTargetIndex = GUILayout.SelectionGrid(
							debugTargetIndex,
							blendShape.targetNames,
							targetButtonColumns
						);
					}
					else
					{
						GUILayout.Label("No targets found!");
					}
				}
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
		
		// end of on-screen GUI
		Handles.EndGUI();
		
		// early out if debug is not enabled
		if (!isDebugEnabled) return;
						
		// visual representations of data
		Color oldCol = Handles.color;
		Matrix4x4 oldMatrix = Handles.matrix;
		Handles.color = debugColor;
		Handles.matrix = blendShape.transform.localToWorldMatrix;
		if (blendShape.seamlessBaseMesh != null)
		{
			// draw a line at each point to show displacement
			for (int i=0; i<blendShape.targets[debugTargetIndex].vertices.Length; i++)
			{
				Handles.DrawLine(
					blendShape.seamlessBaseMesh.vertices[blendShape.targets[debugTargetIndex].vertices[i]],
					blendShape.seamlessBaseMesh.vertices[blendShape.targets[debugTargetIndex].vertices[i]]+blendShape.targets[debugTargetIndex].deltaPositions[i]
				);
			}
		}
		Handles.matrix = oldMatrix;
		Handles.color = oldCol;
	}
	
	/// <summary>
	/// Add the rollout with custom handle controls to the inspector
	/// </summary>
	public override void OnInspectorGUI()
	{
		// draw default inspector
		DrawDefaultInspector();
	}
}