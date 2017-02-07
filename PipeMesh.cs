using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace nl.elleniaw.pipeBuilder{
	
	public class PipeMesh {

		public List<Vector3> vertices;
		public List<Vector2> uvs;
		public List<Vector3> normals;
		public List<int> triangles;

		public Mesh mesh;

		public PipeMesh(List<Vector3> _vertices, List<int> _triangles){
			vertices = _vertices;
			triangles = _triangles;
			uvs = new List<Vector2> ();
			normals = new List<Vector3> ();

			allUVs ();
			allNormals ();
			BuildMesh ();
		}

		private void BuildMesh(){
			mesh = new Mesh ();
			mesh.vertices = vertices.ToArray ();
			mesh.normals = normals.ToArray ();
			mesh.uv = uvs.ToArray ();
			mesh.triangles = triangles.ToArray();
			mesh.RecalculateNormals();

		}

		private void allUVs(){
			//for every vertice build uv position
			for (int i = 0; i < vertices.Count; i++) {
				if (i % 4 == 0) {
					uvs.Insert(i ,Vector2.zero);
				} else if (i % 4 == 1) {
					uvs.Insert(i ,Vector2.up);
				} else if (i % 4 == 2) {
					uvs.Insert(i ,Vector2.right);
				} else {
					uvs.Insert(i ,Vector2.one);
				}
			}
		}

		private void allNormals(){
			//for every vertice build normal vector
			for (int i = 0; i < vertices.Count; i++) {
				normals.Insert(i, new Vector3 (0,0,0));
			}
		}

	}
}