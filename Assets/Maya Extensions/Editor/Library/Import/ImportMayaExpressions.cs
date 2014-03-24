using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

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
/// user properties indicating that it contains data for animation expressions.
/// It will then automatically add a new script for the expression to the
/// project, add the generated component to the incoming model, and link up all
/// fields as needed. The file also contains a class for an expression file
/// used to generate the formatted code form the user properties.
/// 
/// Put this script in an Editor folder and it will automatically operate on
/// incoming files. Files must have correctly-formatted user properties and
/// must be in a source model folder.
/// 
/// This script expects properly-formatted user properties to define the
/// expression code. For more detailed information, see adammechtley.com or 
/// refer to documentation in the amTools Python package.
/// </summary>
/// 
/// </file>

/// <summary>
/// A class containing all of the parts of a MayaExpression to output it to a file
/// </summary>
public class MayaExpressionFile : System.Object
{
	/// <summary>
	/// guid of the asset to which this belongs
	/// </summary>
	public string guid = "";
	/// <summary>
	/// the name of the class/script
	/// </summary>
	public string className { get { return _classPrefix+"MayaExpressions"; } }
	/// <summary>
	/// name of the file converted to capital case
	/// </summary>
	public string classPrefix
	{
		get { return _classPrefix; }
		set
		{
			string[] tokens = value.Split(new char[2] { " "[0], "_"[0] });
			StringBuilder sb = new StringBuilder();
			foreach (string tok in tokens)
				sb.Append(tok.ToUpper()[0].ToString()+tok.Substring(1));
			_classPrefix = sb.ToString();
		}
	}
	private string _classPrefix = "FileName"; 
	
	/// <summary>
	/// all of the class variable declarations
	/// </summary>
	public ArrayList declarations = new ArrayList();
	
	/// <summary>
	/// all of the methodBodies
	/// </summary>
	public Hashtable methods = new Hashtable();
	
	/// <summary>
	/// Build the contents of the cs file
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		
		// tag with guid
		sb.AppendLine(string.Format("// {0}", guid));
		
		// namespaces/assembly references
		sb.AppendLine("using UnityEngine;");
		sb.AppendLine("using System.Collections;");
		sb.AppendLine();
		
		// header
		sb.AppendLine("/*");
		sb.AppendLine(" * A generated class for Maya Expressions");
		sb.AppendLine(" * */");
		sb.AppendLine(string.Format("[AddComponentMenu(\"Maya/Expressions/{0} Maya Expressions\")]", classPrefix));
		sb.AppendLine(string.Format("public class {0} : MayaExpressions", className));
		sb.AppendLine("{");
		
		// add reference fields
		sb.AppendLine("\t// reference fields");
		foreach (string declaration in declarations)
		{
			// only add jointOrient variables that are actually used in order to avoid compiler warnings
			if (new Regex(@"private Quaternion \w+_jointOrient").Match(declaration).Success)
			{
				string varName = declaration.Split(" "[0])[2];
				bool isNeeded = false;
				foreach (string method in methods.Keys)
					if (new Regex(string.Format("\\W{0}\\W", varName)).Match(methods[method] as string).Success) isNeeded = true;
				if (isNeeded) sb.AppendLine(string.Format("\t{0}", declaration));
			}
			else
			{
				sb.AppendLine(string.Format("\t{0}", declaration));
			}
		}
		sb.AppendLine();
		
		// populate main function
		sb.AppendLine("\t/*");
		sb.AppendLine("\t * Perform all of the expressions contained in the original Maya file");
		sb.AppendLine("\t * */");
		sb.AppendLine("\tpublic override void Compute()");
		sb.AppendLine("\t{");
		sb.AppendLine("\t\tforeach (MayaNode node in upstreamDependencies) node.Compute();");
		foreach (string method in methods.Keys)
		{
			sb.AppendLine(string.Format("\t\t{0}{1}();", method.ToUpper()[0], method.Substring(1)));
		}
		sb.AppendLine("\t}");
		
		// add each of the expression methods
		foreach (string method in methods.Keys)
		{
			sb.AppendLine();
			sb.AppendLine("\t/*");
			sb.AppendLine("\t * Generated expression");
			sb.AppendLine("\t * */");
			sb.AppendLine(string.Format("\tpublic void {0}()", method));
			sb.AppendLine("\t{");
			string[] lines = (methods[method] as string).Split("\n"[0]);
			foreach (string line in lines)
				sb.AppendLine(string.Format("\t\t{0}", line));
			sb.AppendLine("\t}");
		}
		sb.AppendLine("}");
		
		return sb.ToString();
	}
	
	/// <summary>
	/// Write the expression contents to the specified path
	/// </summary>
	/// <param name="fullPathToScript">
	/// A <see cref="System.String"/>
	/// </param>
	public void WriteToPath(string fullPathToScript)
	{
		using (StreamWriter sw = new StreamWriter(fullPathToScript))
		{
			sw.Write(this.ToString());
		}
	}
	
	/// <summary>
	/// Determine whether there is a naming conflict with a script at the supplied path
	/// </summary>
	/// <param name="fullPathToScript">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool IsNamingConflict(string fullPathToScript)
	{
		return (File.Exists(fullPathToScript) && GetScriptGuidTag(fullPathToScript)!=guid);
	}
	
	/// <summary>
	/// Determine whether or not the contents of the script at the supplied path match this one
	/// </summary>
	/// <param name="fullPathToScript">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/>
	/// </returns>
	public bool DoScriptContentsMatch(string fullPathToScript)
	{
		using (StreamReader sr = new StreamReader(fullPathToScript))
		{
			if (sr.ReadToEnd() != this.ToString()) return false;
		}
		return true;
	}
	
	/// <summary>
	/// Get the Guid of the asset to which the script belongs
	/// </summary>
	/// <param name="fullPathToScript">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string GetScriptGuidTag(string fullPathToScript)
	{
		string ret;
		using (StreamReader sr = new StreamReader(fullPathToScript))
			ret = sr.ReadLine().Substring(3); // guid is comment on the first line
		return ret;
	}
}

/// <summary>
/// Import expressions that have been stored in the file
/// </summary>
public class ImportMayaExpressions : ImportMayaRigs
{
	// bool to specify whether or not the incoming asset has an expression
	private bool hasExpressions = false;
	
	// an object to store the contents of the expression file
	MayaExpressionFile expressionContents = new MayaExpressionFile();
	
	// key-value pairs for fieldName:referenceName
	private Hashtable expressionFields = new Hashtable();
	
	// the relative path where the expressions will be stored
	public static string expressionsFolder { get { return "_Scripts/~Generated Maya Expressions"; } }
	
	/// <summary>
	/// Ensure this script executes after nodes and blendShapes
	/// </summary>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public override int GetPostprocessOrder()
	{
		return Mathf.Max(ImportMayaNodes.postProcessOrder, ImportBlendShapeData.postProcessOrder)+1;
	}
	
	/// <summary>
	/// Link up fields as needed
	/// </summary>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	void OnPostprocessModel(GameObject go)
	{
		// early out if the file has no expressions
		if (!hasExpressions) return;
		
		// supply guid and class name for expression script
		expressionContents.guid = AssetDatabase.AssetPathToGUID(assetPath);
		expressionContents.classPrefix = assetPath.Substring(assetPath.LastIndexOf("/")+1, assetPath.LastIndexOf(".")-assetPath.LastIndexOf("/")-1);
		
		// create the expressions directory if it does not already exist
		string scriptDirectory = string.Format("{0}/{1}", Application.dataPath, expressionsFolder);
		if (!Directory.Exists(scriptDirectory))
			Directory.CreateDirectory(scriptDirectory);
		
		// absolute path for the new script plus file name
		string fullPathToScript = string.Format("{0}/{1}.cs", scriptDirectory, expressionContents.className);
		
		// first determine if there is a naming conflict
		bool isScriptNamingConflict = expressionContents.IsNamingConflict(fullPathToScript);
		
		// correct naming conflict if present
		if (isScriptNamingConflict)
		{
			int scriptNumber = 0;
			while (isScriptNamingConflict)
			{
				scriptNumber++;
				expressionContents.classPrefix += scriptNumber.ToString();
				fullPathToScript = string.Format("{0}/{1}.cs", scriptDirectory, expressionContents.className);
				isScriptNamingConflict = expressionContents.IsNamingConflict(fullPathToScript);
			}
		}
		
		// determine whether we are overwriting an existing file or creating a new one
		bool doesScriptAlreadyExist = File.Exists(fullPathToScript);
					
		// if the script's contents have changed, then it needs to be recompiled
		bool doesScriptNeedToBeRecompiled = false;
		if (doesScriptAlreadyExist)
			doesScriptNeedToBeRecompiled = !expressionContents.DoScriptContentsMatch(fullPathToScript);
		
		// reimport the script and start the postprocessor over if the script is new or needs be recompiled
		if (!doesScriptAlreadyExist || doesScriptNeedToBeRecompiled)
		{
			// write the script contents
			expressionContents.WriteToPath(fullPathToScript);
			
			// force the script to import and compile immediately
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			AssetDatabase.ImportAsset(string.Format("Assets/{0}/{1}.cs", expressionsFolder, expressionContents.className), ImportAssetOptions.ForceSynchronousImport);
			
			// reimport this asset so assemblies will rebuild and script contents should be identical on next import pass
			AssetDatabase.ImportAsset(assetImporter.assetPath, ImportAssetOptions.Default);
		}
		else
		{
			// add the expression component to the root gameObject
			MayaExpressions newExpressionComponent = go.AddComponent(expressionContents.className) as MayaExpressions;
			
			// set scale values
			newExpressionComponent.importScale = (assetImporter as ModelImporter).globalScale;
			newExpressionComponent.oneOverImportScale = 1f/(assetImporter as ModelImporter).globalScale;
			
			// get all of the possible object matches in the model
			Component[] dagHierarchy = go.GetComponentsInChildren<Transform>();
			Component[] mayaNodes = go.GetComponentsInChildren<MayaNode>();
			Component[] blendShapes = go.GetComponentsInChildren<BlendShape>();
									
			// set the field values based on the incoming data
	        foreach (FieldInfo fi in newExpressionComponent.GetType().GetFields())
			{
				// ignore default fields
				switch (fi.Name)
				{
				case "nodeName":
				case "isInvokedExternally":
				case "importScale":
				case "oneOverImportScale":
				case "upstreamDependencies":
				case "localPosition":
				case "localEulerAngles":
				case "localScale":
					continue;
				// process custom fields
				default:
					// try to find the matching object
					Component match = null;
					string referenceName = expressionFields[fi.Name] as string;
					if (fi.FieldType == typeof(BlendShape))
					{
						foreach (BlendShape b in blendShapes)
						{
							if (b.name == referenceName)
							{
								match = b;
							}
						}
					}
					else if (fi.FieldType == typeof(Transform))
					{
						foreach (Transform t in dagHierarchy)
						{
							if (t.name == referenceName)
							{
								match = t;
							}
						}
					}
					else if (fi.FieldType.IsSubclassOf(typeof(MayaNode)))
					{
						foreach (MayaNode n in mayaNodes)
						{
							if (n.nodeName == referenceName)
							{
								match = n;
							}
						}
					}
					if (match == null)
					{
						continue;
					}					
					// link up the match
					fi.SetValue(newExpressionComponent, match);
					break;
				}
			}
		}
	}
	
	/// <summary>
	/// Read all custom user properties for expressions
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
	void OnPostprocessGameObjectWithUserProperties(GameObject go, string[] propNames, object[] values)
	{
		// early out if the file is not in a source model folder
		if (!isInSourceModelFolder) return;
		
		// parse the properties
		for (int i=0; i<propNames.Length; i++)
		{
			string[] tokens = propNames[i].Split(new string[1] {"__"}, 3, System.StringSplitOptions.None);
			if (tokens[0] != "expression") continue;
			hasExpressions = true;
			
			switch (tokens[1])
			{
			case "field":
				expressionFields.Add(tokens[2], values[i] as string);
				break;
			case "declaration":
				expressionContents.declarations.Add(values[i] as string);
				break;
			case "methodBody":
				expressionContents.methods.Add(tokens[2], values[i] as string);
				break;
			}
		}
	}
}