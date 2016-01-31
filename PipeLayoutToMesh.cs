using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace nl.elleniaw.pipeBuilder{
	public class PipeLayoutToMesh
	{

		private bool doubledVertices;
		private float handleDistance = 1.0f; // remove and get from other place
		private PipeLayout pipe_layout;

		public List<Vector3> selected_vertices;
		public List<Vector3> vertices;
		public List<int> triangles;
		//List with vertices and positions of the vertices

		public PipeLayoutToMesh (PipeLayout _pipe_layout, bool _doubled_vertices)
		{
			//vertices = new Vector3[0];
			pipe_layout = _pipe_layout;
			doubledVertices = _doubled_vertices;
			Initiate ();
		}

		private void Initiate(){
			if (doubledVertices) {
				vertices = new List<Vector3> ();
			} else {
				vertices = new List<Vector3> ();
			}
			triangles = new List<int>();

			for (int i = 0; i < pipe_layout.amount_of_rings; i++) {
				if (doubledVertices) {
					buildRingWithPhong (i);
				} else {
					buildRing (i);
				}
			}
			selected_vertices = Enumerable.Repeat(Vector3.zero, vertices.Count).ToList();
			buildTriangles ();
		}

		public void UpdateLayout(PipeLayout _pipe_layout, bool _doubled_vertices){
			
			if (doubledVertices) {
				vertices = new List<Vector3> ();
			} else {
				vertices = new List<Vector3> ();
			}
			triangles = new List<int>();

			for (int i = 0; i < pipe_layout.amount_of_rings; i++) {
				if (doubledVertices) {
					buildRingWithPhong (i);
				} else {
					buildRing (i);
				}
			}
			selected_vertices = Enumerable.Repeat(Vector3.zero, vertices.Count).ToList();
			buildTriangles ();

		}

		public void OnMoveHandle(Vector3 delta_pos){
			for (int i = 0; i < selected_vertices.Count; i++) {
				if(selected_vertices[i] != Vector3.zero){
					vertices [i] -= delta_pos;
					selected_vertices [i] = vertices [i];
				}
			}
		}

		public void OnMouseSelection(Rect selection){
			selected_vertices = Enumerable.Repeat(Vector3.zero, vertices.Count).ToList();
			for (int i = 0; i < vertices.Count; i++) {
				if(Camera.current != null){
					Vector2 screen_position = Camera.current.WorldToScreenPoint (vertices [i]);
					if(selection.Contains(new Vector2(screen_position.x, Screen.height - screen_position.y), true)){
						selected_vertices [i] = vertices [i];
					}
				}
			}
		}

		private void buildRingWithPhong(int ring_index){
			float angle = 360.0f/pipe_layout.amount_of_ring_vertices;

			for (var i = 0; i < pipe_layout.amount_of_ring_vertices; i++) {
				var rad = (angle * i) * Mathf.Deg2Rad;
				Vector3 vertice_position = new Vector3();
				vertice_position = new Vector3 ( 
					Mathf.Sin(rad) * pipe_layout.ring_diameters[ring_index],
					Mathf.Cos (rad) * pipe_layout.ring_diameters[ring_index],
					0
				);
				vertice_position = pipe_layout.ring_rotation [ring_index] * vertice_position;
				vertice_position += pipe_layout.ring_origins[ring_index];

				int slice_root = ring_index * pipe_layout.amount_of_ring_vertices;
				setVertice(i + slice_root, vertice_position);
			}
		}

		private void buildRing(int ring_index){
			float angle = 360.0f/pipe_layout.amount_of_ring_vertices;

			for (int v = 0, i = 0; v < pipe_layout.amount_of_ring_vertices; v++, i += 2) {
				var rad = (angle * v) * Mathf.Deg2Rad;
				Vector3 vertice_position = new Vector3();
				vertice_position = new Vector3 ( 
					Mathf.Sin(rad) * pipe_layout.ring_diameters[ring_index],
					Mathf.Cos (rad) * pipe_layout.ring_diameters[ring_index],
					0
				);
				vertice_position = pipe_layout.ring_rotation [ring_index] * vertice_position;
				vertice_position += pipe_layout.ring_origins[ring_index];

				int slice_root = (ring_index*2) * pipe_layout.amount_of_ring_vertices;//slice * 2 because we doubled the vertices
				setVertice(i + slice_root, vertice_position);
				setVertice(i + slice_root + 1, vertice_position);// we double the vertices
			}

		}

		private void setVertice(int index, Vector3 position){
			vertices.Insert (index, position);
		}

		void buildTriangles(){
			for(var j=1;j<pipe_layout.amount_of_rings;j++){
				for(var i=1;i<=pipe_layout.amount_of_ring_vertices;i++){
					var index = i + ((j-1)*pipe_layout.amount_of_ring_vertices);
					if (doubledVertices) {
						buildQuadWithPhong ( index, j);
					} else {
						buildQuadNoPhong ( index, j);
					}

				}
			}
		}

		void buildQuadWithPhong(int vertice, int ring){
			var triangle = ((vertice - 1) * 6) + 1;
			var baseVert = vertice - 1;

			int baseVert2 = baseVert;
			if(baseVert + 1 == (pipe_layout.amount_of_ring_vertices * ring)){
				baseVert2 = (pipe_layout.amount_of_ring_vertices * ring) - (pipe_layout.amount_of_ring_vertices + 1);
			}

			triangles.Insert (triangle - 1, baseVert);
			triangles.Insert (triangle, baseVert + pipe_layout.amount_of_ring_vertices);
			triangles.Insert (triangle + 1, baseVert2 + 1);
			triangles.Insert (triangle + 2, baseVert2 + 1);
			triangles.Insert (triangle + 3, baseVert + pipe_layout.amount_of_ring_vertices);
			triangles.Insert (triangle + 4,  baseVert2 + pipe_layout.amount_of_ring_vertices + 1);

//			triangles [triangle - 1] = baseVert;
//			triangles [triangle] = triangles [triangle + 3] =  baseVert + pipe_layout.amount_of_ring_vertices;
//			triangles [triangle + 1] = triangles [triangle + 2] =baseVert + 1;
//			triangles [triangle + 4] = baseVert + pipe_layout.amount_of_ring_vertices + 1;
		}

		void buildQuadNoPhong(int vertice, int ring){
			var triangle = (vertice * 6) - 5;
			var baseVert = vertice + (vertice - 1);
			var faces = (pipe_layout.amount_of_ring_vertices * 2);
			int baseVert2 = baseVert;
			if(baseVert + 1 == (faces * ring)){
				baseVert2 = (faces * ring) - (faces + 1);
			}

			var opposideVert = baseVert + (pipe_layout.amount_of_ring_vertices * 2);

			triangles.Insert (triangle - 1, baseVert);
			triangles.Insert (triangle, opposideVert);
			triangles.Insert (triangle + 1, baseVert2 + 1);
			triangles.Insert (triangle + 2, baseVert2 + 1);
			triangles.Insert (triangle + 3, opposideVert);

//			triangles [triangle + 1] = triangles [triangle + 2] = baseVert + 1;

			if(opposideVert + 1 == (faces * ring) + faces){
				opposideVert = (faces * ring) - 1;
			}

			triangles.Insert (triangle + 4, opposideVert + 1);

//			triangles [triangle + 4] = opposideVert + 1;

		}

	}
}


