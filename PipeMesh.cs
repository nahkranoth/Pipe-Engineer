using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace nl.elleniaw.pipeBuilder{
	
	public class PipeMesh {

		private PipeLayoutToMesh pipeLayoutToMesh;
		public List<Vector2> uvs;
		public List<Vector3> normals;

		public Mesh mesh;

		public PipeMesh(PipeLayoutToMesh pipeLayoutToMesh){
			this.pipeLayoutToMesh = pipeLayoutToMesh;
			uvs = new List<Vector2> ();
			normals = new List<Vector3> ();
			allUVs ();
			BuildMesh ();
		}

		private void BuildMesh(){
			mesh = new Mesh ();
			mesh.vertices = pipeLayoutToMesh.vertices.ToArray();
			mesh.uv = uvs.ToArray ();
			mesh.triangles = pipeLayoutToMesh.triangles.ToArray();
			mesh.RecalculateNormals();
		    var norms = mesh.normals.ToList ();
			for (int i = 0; i < mesh.normals.Length; i+=pipeLayoutToMesh.pipe_layout.amount_of_ring_vertices) {
				var n = (mesh.normals [i] + mesh.normals [i+1]) * 0.5f;
				norms [i] = n;
				norms [i+1] = n;
			}
			mesh.normals = norms.ToArray ();
		}

		private void allUVs(){
			//for every vertice build uv position
			float y = 0;
			int i = 0;
			int repetitionsOfRing = 2;
			if (pipeLayoutToMesh.doubledVertices) {
				repetitionsOfRing = 1;
			}
			while(i<pipeLayoutToMesh.vertices.Count) {
				for (int k = 0; k < repetitionsOfRing; k++) {
					uvs.Insert (i, new Vector2 (1.0f, y));
					i++;
					for (float x = 0; x < pipeLayoutToMesh.pipe_layout.amount_of_ring_vertices - 1; x++) {
						uvs.Insert (i, new Vector2 (x / (pipeLayoutToMesh.pipe_layout.amount_of_ring_vertices - 1), y));
						i++;
					}
				}
				y = (y + 1) % 2;
			}
		}
	}
}