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
/// This script contains a component for applying linear per-vertex deformation
/// to points in a mesh. It works with both static and skinned meshes. The data
/// are sparse, so it only operates on points that actually produce
/// deformation.
/// 
/// It works by applying target deformation to a proxy mesh, seamlessBaseMesh,
/// and then mapping the results of this deformation back to _outMesh, the mesh
/// being rendered. Because the proxy mesh is presumed to have no seams from
/// uv, normal, or submesh separations, it allows for fast recalculation of
/// smooth normals using Mesh.RecalculateNormals(), and is consequently faster
/// since it has fewer points than _outMesh.
/// 
/// This component is intended to be set up entirely by editor scripts since it
/// requires proper mapping of sparse target information. For an example of
/// data coming from Maya, see ImportBlendShapeData.cs.
/// 
/// If you are planning to control this script externally, set the desired
/// weight values for all of the targets and then call Compute().
/// 
/// For best perormance, the component does not automatically recalculate
/// bounds for the mesh. If you know you will have dramatic changes, it is
/// advised that you call Compute() from another script and then call
/// RecalculateBounds().
/// 
/// The component assumes that the point count, triangle list, and winding
/// order for the mesh being rendered do not change at run-time.
/// 
/// Computing smooth normals relies on Mesh.RecalculateNormals(), which does
/// not produce identical results to 180-degree smoothing in the asset importer
/// or in a modeling application. Consequently, you may see some minor visual
/// artifacts, especially if using normal maps that were generated without this
/// assumption.
/// 
/// Computing smooth normals works in the space of the proxy seamlessBaseMesh.
/// As such, it presumes that any seams existing on the proxy are intentional.
/// 
/// Computing smooth normals will not preserve manually-specified normals.
/// 
/// The component cannot be meaningfully configured without editor scripts.
/// 
/// No support yet for in-between targets.
/// </summary>
/// 
/// </file>

/// <summary>
/// A component for deforming vertices on a model based on several input targets
/// </summary>
[AddComponentMenu("Maya/Deformers/BlendShape")]
[RequireComponent(typeof(Renderer))]
public class BlendShape : MayaNode
{
	/// <summary>
	/// A class to store sparse data for a target
	/// </summary>
	[System.Serializable]
	public class Target : System.Object
	{
		/// <summary>
		/// the name of the blend shape target
		/// </summary>
		public string name;
		
		/// <summary>
		/// the current and previous weight of the shape
		/// </summary>
		public float weight = 0f;
		public float previousWeight
		{
			get { return _prevWeight; }
		}
		private float _prevWeight = 0f;
		
		/// <summary>
		/// array of vertex indices for the shape
		/// </summary>
		public int[] vertices = new int[0];
		/// <summary>
		/// array of delta values for the shape
		/// </summary>
		public Vector3[] deltaPositions = new Vector3[0];
		
		/// <summary>
		/// Cache the weight value, to be invoked at the end of a frame
		/// </summary>
		public void RecordWeightForNextFrame()
		{
			_prevWeight = weight;
		}
	}
	
	/// <summary>
	/// the seamless base mesh to deform, which corresponds to the mesh on this GameObject's MeshFilter or SkinnedMeshRenderer
	/// </summary>
	public Mesh seamlessBaseMesh;
	
	/// <summary>
	/// the renderer attached to this object
	/// </summary>
	public Renderer meshRenderer;
	
	/// <summary>
	/// the mesh attached to this object's renderer
	/// </summary>
	[SerializeField][HideInInspector] // allow this to be overriden via an editor script
	private Mesh _outMesh;
	public Mesh outMesh { get { return _outMesh; } }
	
	/// <summary>
	/// an array mapping indices on _outMesh to indices on seamlessBaseMesh
	/// </summary>
	public int[] indexMap;
	
	/// <summary>
	/// an array of all of the targets
	/// </summary>
	public BlendShape.Target[] targets = new BlendShape.Target[0];
	
	/// <summary>
	/// indicate whether or not to smooth normals
	/// </summary>
	public bool isSmoothingNormals = true;
	
	// cached arrays
	private Vector3[] _defaultNormals;
	private Vector3[] _seamlessNormals;
	private Vector3[] _seamlessVertices;
	private Vector3[] _outNormals;
	private Vector3[] _outVertices;
	
	/// <summary>
	/// get an array of the target names
	/// </summary>
	public string[] targetNames
	{
		get
		{
			string[] tl = new string[targets.Length];
			for (int i=0; i<targets.Length; i++) tl[i] = targets[i].name;
			return tl;
		}
	}
	
	/// <summary>
	/// index operator overload to get a target by name
	/// </summary>
	/// <param name="s">
	/// A <see cref="System.String"/>
	/// </param>
	public BlendShape.Target this[string s]
	{
		get { return _targetsByName[s] as BlendShape.Target; }
		set { _targetsByName[s] = value; }
	}
	private Hashtable _targetsByName = new Hashtable();
	
	/// <summary>
	/// Initialize
	/// </summary>
	public void Awake()
	{
		// build the hashtable of targets
		foreach (BlendShape.Target t in targets)
			_targetsByName.Add(t.name, t);
		
		// acquire the _outMesh based on the type of renderer
		bool isSkin = GetComponent<Renderer>().GetType() == typeof(SkinnedMeshRenderer);
		if (_outMesh == null)
		{
			if (isSkin)
				_outMesh = (meshRenderer as SkinnedMeshRenderer).sharedMesh;
			else
				_outMesh = GetComponent<MeshFilter>().mesh;
		}
		
		// duplicate _outMesh to avoid modifying source asset
		_outMesh = MeshHelpers.DuplicateMesh(_outMesh, string.Format("{0} (Duplicate)", _outMesh.name));
		if (isSkin)
			(meshRenderer as SkinnedMeshRenderer).sharedMesh = _outMesh;
		else
			GetComponent<MeshFilter>().mesh = _outMesh;
		// duplicate seamlessBaseMesh to avoid modifying source asset
		seamlessBaseMesh = MeshHelpers.DuplicateMesh(seamlessBaseMesh, string.Format("{0} (Duplicate)", seamlessBaseMesh.name));

		// store necessary vertex and normal arrays
		_defaultNormals = _outMesh.normals;
		_seamlessNormals = seamlessBaseMesh.normals;
		_seamlessVertices = seamlessBaseMesh.vertices;
		_outNormals = _outMesh.normals;
		_outVertices = _outMesh.vertices;
	}
	
	/// <summary>
	/// Deform the mesh automatically if requested
	/// </summary>
	void Update()
	{
		if (isInvokedExternally) return;
		Compute();
	}
	
	/// <summary>
	/// Apply the current blend result
	/// </summary>
	public override void Compute()
	{
		_seamlessVertices = seamlessBaseMesh.vertices;
		
		// iterate points in target
		for (int t=0; t<targets.Length; t++)
		{
			float deltaWeight = targets[t].weight-targets[t].previousWeight;
			for (int i=0; i<targets[t].vertices.Length; i++)
				_seamlessVertices[targets[t].vertices[i]] += targets[t].deltaPositions[i]*deltaWeight;
			// cache the previous weight for use in the next Compute() call
			targets[t].RecordWeightForNextFrame();
		}
		
		// apply the results to the seamlessBaseMesh
		// NOTE: calling Clear() is unnecessary since the triangle list is unaffected
		seamlessBaseMesh.vertices = _seamlessVertices;
		if (isSmoothingNormals)
		{
			seamlessBaseMesh.RecalculateNormals();
			_seamlessNormals = seamlessBaseMesh.normals;
			for (int i=0; i<indexMap.Length; i++)
			{
				_outVertices[i] = _seamlessVertices[indexMap[i]];
				_outNormals[i] = _seamlessNormals[indexMap[i]];
			}
		}
		else
		{
			for (int i=0; i<indexMap.Length; i++)
			{
				_outVertices[i] = _seamlessVertices[indexMap[i]];
			}
			_defaultNormals.CopyTo(_outNormals, 0);
		}
		
		// copy results to _outMesh
		_outMesh.vertices = _outVertices;
		_outMesh.normals = _outNormals;
	}
	
	/// <summary>
	/// Recalculate the bounding box for _outMesh on demand
	/// </summary>
	public void RecalculateBounds()
	{
		_outMesh.RecalculateBounds();
	}
	
	/// <summary>
	/// Override _outMesh, e.g., via an editor script
	/// </summary>
	/// <param name="mesh">
	/// A <see cref="Mesh"/>
	/// </param>
	public void OverrideOutMesh(Mesh mesh)
	{
		_outMesh = mesh;
		_outNormals = mesh.normals;
		_outVertices = mesh.vertices;
	}
}