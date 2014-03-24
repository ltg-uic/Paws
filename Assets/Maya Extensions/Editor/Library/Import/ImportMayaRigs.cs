using UnityEngine;
using UnityEditor;

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
/// This file contains an AssetPostprocessor that is a base class for importing
/// Maya rigs for run-time evaluation. Its primary purpose is to identify
/// models that should have rigs imported, while subclasses handle technical
/// details.
/// 
/// Put this script in an Editor folder and it will automatically operate on
/// incoming files in the specified source models folder.
/// 
/// See adammechtley.com for mode information, or refer to documentation in the
/// amTools Python package for more detailed limitations.
/// </summary>
/// 
/// </file>

/// <summary>
/// Base class to import Maya rigs
/// </summary>
public class ImportMayaRigs : AssetPostprocessor
{
	/// <summary>
	/// if a model is not in a folder matching this name, it is not processed according to these rules
	/// </summary>
	protected static string sourceModelFolder = "Source Models";
	/// <summary>
	/// store whether the model is in a source model folder
	/// </summary>
	protected bool isInSourceModelFolder = false;
	
	/// <summary>
	/// Confirm the current model's folder to determine whether or not user properties should be parsed
	/// </summary>
	protected void OnPreprocessModel()
	{
		isInSourceModelFolder = (AssetDatabaseUtilities.GetFolderName(assetPath) == sourceModelFolder);
	}
}