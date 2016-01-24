using UnityEngine;
using System.Collections;

namespace nl.elleniaw.pipeBuilder{
	
	public class PipeMesh {

		public Vector3[] vertices;
		public Vector2[] uvs;
		public Vector3[] normals;
		public int[] triangles;

		public Mesh mesh;

		public PipeMesh(Vector3[] _vertices, int[] _triangles){

			mesh = new Mesh ();
			vertices = _vertices;
			triangles = _triangles;
			uvs = new Vector2[vertices.Length];
			normals = new Vector3[vertices.Length];

			allUVs ();
			allNormals ();
			BuildMesh ();
		}

		private void BuildMesh(){
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();

		}

		private void allUVs(){
			//for every vertice build uv position
			for (int i = 0; i < vertices.Length; i++) {
				if (i % 4 == 0) {
					uvs [i] = Vector2.zero;
				} else if (i % 4 == 1) {
					uvs [i] = Vector2.right;
				} else if (i % 4 == 2) {
					uvs [i] = Vector2.up;
				} else {
					uvs[i] = Vector2.one;
				}
			}
		}

		private void allNormals(){
			//for every vertice build normal vector
			for (int i = 0; i < vertices.Length; i++) {
				normals [i] = new Vector3 (0,0,0);
			}
		}

	}
}