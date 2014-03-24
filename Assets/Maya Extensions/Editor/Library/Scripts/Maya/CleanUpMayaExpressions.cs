using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

/*
 * ----------------------------------------------------------------------------
 * Creation Info
 * ----------------------------------------------------------------------------
 * CleanUpMayaExpressions.cs
 * Donations: http://adammechtley.com/donations/
 * License: The MIT License
 * 
 * Copyright (c) 2011 Adam Mechtley (http://adammechtley.com)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 * 
 * ----------------------------------------------------------------------------
 * Description
 * ----------------------------------------------------------------------------
 * This file contains a class with a menu item to clean up generated
 * MayaExpressions scripts that are no longer in use.
 * 
 * ----------------------------------------------------------------------------
 * Usage
 * ----------------------------------------------------------------------------
 * Access the menu item.
 * 
 * ----------------------------------------------------------------------------
 * Notes and Limitations
 * ----------------------------------------------------------------------------
 * N/A
 * */

/*
 * Editor script to clean up unused files that were generated from Maya expressions
 * */
public class CleanUpMayaExpressions : ScriptableObject
{
	/*
	 * Delete expression scripts presumed to no longer be in use
	 * */
	[MenuItem ("Custom/Library/Maya/Clean Up Unused Maya Expressions in Project")]
	static void Clean()
	{
		// get the files in the expressions directory
		string expressionsDirectory = string.Format("{0}/{1}", Application.dataPath, ImportMayaExpressions.expressionsFolder);
		FileInfo[] allExpressionScripts = new DirectoryInfo(expressionsDirectory).GetFiles("*.cs");
		foreach(FileInfo fi in allExpressionScripts)
		{
			// get the guid of the asset to which the script belongs
			string guid = MayaExpressionFile.GetScriptGuidTag(fi.FullName);
			// if the guid is no longer valid, then assume the script is not being used
			bool isBeingUsed = (AssetDatabase.GUIDToAssetPath(guid) != "");
			if (isBeingUsed)
			{
				// if the asset indicated by the guid no longer has the component, then assume it is not being used
				GameObject go = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as GameObject;
				if (go==null || go.GetComponent(fi.Name.Substring(0, fi.Name.Length-3)) == null) isBeingUsed = false;
			}
			// if the script is not being used, then delete it
			if (!isBeingUsed) AssetDatabase.DeleteAsset(string.Format("Assets/{0}/{1}", ImportMayaExpressions.expressionsFolder, fi.Name));
		}
	}
}