using UnityEngine;
using System.Collections;

/// <summary>
/// a struct for packing mesh data
/// </summary>
public struct MeshParameters
{
	public Vector3[] vertices;
	
	public Matrix4x4[] bindPoses;
	public BoneWeight[] boneWeights;
	public Color[] colors;
	public Vector3[] normals;
	public Vector4[] tangents;
	public int[] triangles;
	public Vector2[] uv;
	public Vector2[] uv2;
}

/// <summary>
/// a struct for packing skinning data
/// </summary>
public struct SkinParameters
{
	public Matrix4x4[] bindPoses;
	public Transform[] bones;
	public BoneWeight[] weights;
}

/// <summary>
/// A utility class for operating on meshes
/// </summary>
public static class MeshHelpers : System.Object
{
	/// <summary>
	/// Duplicate a supplied mesh and return the duplicate
	/// </summary>
	/// <param name="mesh">
	/// A <see cref="Mesh"/>
	/// </param>
	/// <returns>
	/// A <see cref="Mesh"/>
	/// </returns>
	public static Mesh DuplicateMesh(Mesh mesh)
	{
		return DuplicateMesh(mesh, string.Format("{0} (Duplicate)", mesh.name));
	}
	/// <summary>
	/// Duplicate a supplied mesh and return the duplicate
	/// </summary>
	/// <param name="mesh">
	/// A <see cref="Mesh"/>
	/// </param>
	/// <param name="newName">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="Mesh"/>
	/// </returns>
	public static Mesh DuplicateMesh(Mesh mesh, string newName)
	{
		Mesh newMesh = new Mesh();
		
		newMesh.vertices = mesh.vertices;
		newMesh.colors = mesh.colors;
		newMesh.normals = mesh.normals;
		newMesh.subMeshCount = mesh.subMeshCount;
		for (int i=0; i<mesh.subMeshCount; i++) newMesh.SetTriangles(mesh.GetTriangles(i), i);
		newMesh.tangents = mesh.tangents;
		newMesh.uv = mesh.uv;
		newMesh.uv2 = mesh.uv2;
		
		newMesh.bindposes = mesh.bindposes;
		newMesh.boneWeights = mesh.boneWeights;
		
		newMesh.name = newName;
		
		return newMesh;
	}
	
	/// <summary>
	/// Set the vertex colors in a mesh to all one color
	/// </summary>
	/// <param name="meshFilter">
	/// A <see cref="MeshFilter"/>
	/// </param>
	/// <param name="color">
	/// A <see cref="Color"/>
	/// </param>
	public static void FloodVertexColors(MeshFilter meshFilter, Color color)
	{
		Mesh mesh = meshFilter.mesh;
		FloodVertexColors(mesh, color);
		meshFilter.mesh = mesh;
		return;
	}
	/// <summary>
	/// Set the vertex colors in a mesh to all one color
	/// </summary>
	/// <param name="skin">
	/// A <see cref="SkinnedMeshRenderer"/>
	/// </param>
	/// <param name="color">
	/// A <see cref="Color"/>
	/// </param>
	public static void FloodVertexColors(SkinnedMeshRenderer skin, Color color)
	{
		Mesh mesh = skin.sharedMesh;
		FloodVertexColors(mesh, color);
		skin.sharedMesh = mesh;
		return;
	}
	/// <summary>
	/// Set the vertex colors in a mesh to all one color
	/// </summary>
	/// <param name="mesh">
	/// A <see cref="Mesh"/>
	/// </param>
	/// <param name="color">
	/// A <see cref="Color"/>
	/// </param>
	public static void FloodVertexColors(Mesh mesh, Color color)
	{
		Color[] colors = new Color[mesh.vertices.Length];
		for (int i=0; i<colors.Length; i++) colors[i] = color;
		mesh.colors = colors;
		return;
	}
	
	/// <summary>
	/// Return an array where mesh's colors are tinted by tint
	/// </summary>
	/// <param name="mesh">
	/// A <see cref="Mesh"/>
	/// </param>
	/// <param name="tint">
	/// A <see cref="Color"/>
	/// </param>
	/// <returns>
	/// A <see cref="Color[]"/>
	/// </returns>
	public static Color[] GetTintedColors(Mesh mesh, Color tint)
	{
		Color[] col = mesh.colors;
		col = TintColors(col, tint);
		return col;
	}
	/// <summary>
	/// Return a copy
	/// </summary>
	/// <param name="col">
	/// A <see cref="Color[]"/>
	/// </param>
	/// <param name="tint">
	/// A <see cref="Color"/>
	/// </param>
	/// <returns>
	/// A <see cref="Color[]"/>
	/// </returns>
	public static Color[] TintColors(Color[] col, Color tint)
	{
		Color[] ret = new Color[col.Length];
		for (int i=0; i<col.Length; i++) ret[i] = col[i] * tint;
		return ret;
	}
	
	/// <summary>
	/// Create a new mesh from a MeshParameters object
	/// </summary>
	/// <param name="parameters">
	/// A <see cref="MeshParameters"/>
	/// </param>
	/// <returns>
	/// A <see cref="Mesh"/>
	/// </returns>
	public static Mesh CreateMeshFromParameters(MeshParameters parameters)
	{
		Mesh mesh = new Mesh();
		mesh.vertices = parameters.vertices;
		if (parameters.bindPoses!=null && parameters.bindPoses.Length>0)
			mesh.bindposes = parameters.bindPoses;
		if (parameters.boneWeights!=null && parameters.boneWeights.Length>0)
			mesh.boneWeights = parameters.boneWeights;
		if (parameters.colors!=null && parameters.colors.Length>0)
			mesh.colors = parameters.colors;
		mesh.normals = parameters.normals;
		if (parameters.tangents!=null && parameters.tangents.Length>0)
			mesh.tangents = parameters.tangents;
		mesh.triangles = parameters.triangles;
		mesh.uv = parameters.uv;
		if (parameters.uv2!=null && parameters.uv2.Length>0)
			mesh.uv2 = parameters.uv2;
		return mesh;
	}
	
	/// <summary>
	/// Returns a copy of inMesh that has been flipped over the x-axis (TODO: support flipping using any axis)
	/// </summary>
	/// <param name="inMesh">
	/// A <see cref="Mesh"/>
	/// </param>
	/// <returns>
	/// A <see cref="Mesh"/>
	/// </returns>
	public static Mesh CreateMirrorMesh(Mesh inMesh)
	{
		Mesh mesh = DuplicateMesh(inMesh, string.Format("{0} (Mirrored)", inMesh.name));
		
		// flip vertices and normals
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		for (int i=0; i<mesh.vertices.Length; i++)
		{
			vertices[i] = mesh.vertices[i];
			vertices[i].x *= -1f;
			normals[i] = mesh.normals[i];
			normals[i].x *= -1f;
		}
		
		// reverse triangle winding order
		int[] triangles = mesh.triangles;
		for (int i=0; i<triangles.Length-2; i+=3)
		{
			int temp = triangles[i];
			triangles[i] = triangles[i+2];
			triangles[i+2] = temp;
		}
		
		// set all the new properties
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.triangles = triangles;
		
		// return the result
		return mesh;
	}
}