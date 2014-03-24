using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

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
/// This file contains a class with static methods for working with the asset
/// database.
/// </summary>
/// 
/// </file>

/// <summary>
/// A utility class for working with the asset database
/// </summary>
public static class AssetDatabaseUtilities : System.Object
{
	/// <summary>
	/// Return the name of the folder the supplied asset is in
	/// </summary>
	/// <param name="assetPath">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string GetFolderName(string assetPath)
	{
		return Path.GetFileNameWithoutExtension(GetDirectoryName(assetPath));
	}
	
	/// <summary>
	/// Return the project-relative directory path for the supplied asset
	/// </summary>
	/// <param name="assetPath">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string GetDirectoryName(string assetPath)
	{
		return Path.GetDirectoryName(assetPath);
	}
	
	/// <summary>
	/// Return the extension of the supplied asset
	/// </summary>
	/// <param name="assetPath">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string GetExtension(string assetPath)
	{
		return new FileInfo(assetPath).Extension;
	}
}