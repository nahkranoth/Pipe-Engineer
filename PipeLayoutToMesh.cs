using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace nl.elleniaw.pipeBuilder{
	public class PipeLayoutToMesh
	{

		public bool doubledVertices;
		public PipeLayout pipe_layout;

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
			vertices = new List<Vector3> ();
			triangles = new List<int>();
			for (int i = 0; i < pipe_layout.amount_of_rings; i++) {
				if (doubledVertices) {
					buildRingWithPhong (i);
				} else {
					buildRing (i);
				}
			}
			selected_vertices = Enumerable.Repeat(Vector3.zero, vertices.Count).ToList();
			if (doubledVertices) {
				buildTrianglesWithPhong ();
			} else {
				buildTriangles ();
				//buildTriangles2 ();
			}
		}

		public void ExtrudeRing(PipeLayout pipe_layout){
			if (doubledVertices) {
				buildRingWithPhong (pipe_layout.amount_of_rings - 1);
				buildTrianglesWithPhong ();
			} else {
				buildRing (pipe_layout.amount_of_rings - 1);
				buildTriangles ();
				//buildTriangles2 ();
			}
		}

		public void OnMoveHandle(Vector3 delta_pos, Pipe.HandleSelected handleCallback){
			for (int i = 0; i < selected_vertices.Count; i++) {
				if(selected_vertices[i] != Vector3.zero){
					vertices [i] -= delta_pos;
					selected_vertices [i] = vertices [i];
				}
			}
			pipe_layout.heading_pos -= (delta_pos*2);
		}

		public void OnMouseSelection(Rect selection, Pipe.HandleSelected handleCallback){
			selected_vertices = Enumerable.Repeat(Vector3.zero, vertices.Count).ToList();
			List<Vector3> only_selected = new List<Vector3> ();
			for (int i = 0; i < vertices.Count; i++) {
				if(Camera.current != null){
					Vector2 screen_position = Camera.current.WorldToScreenPoint (vertices [i]);
					if(selection.Contains(new Vector2(screen_position.x, Screen.height - screen_position.y), true)){
						selected_vertices [i] = vertices [i];
						only_selected.Add (vertices [i]);
					}
				}
			}
			setHandleToAverage (only_selected, handleCallback);
		}

		private void setHandleToAverage(List<Vector3> vector_list, Pipe.HandleSelected handleCallback){
			if(vector_list.Count > 0 && handleCallback != null){
				//not Average but the min and max's halfway point
				Vector3 average_vector = new Vector3 (
					vector_list.Average(x=>x.x),
					vector_list.Average(x=>x.y),
					vector_list.Average(x=>x.z)
				);

				setHandlePosition (average_vector, handleCallback);
			}
		}

		private void setHandlePosition(Vector3 position, Pipe.HandleSelected handleCallback){
			handleCallback (position);
		}

		private void buildRingWithPhong(int ring_index){
			float angle = 360.0f/(pipe_layout.amount_of_ring_vertices-1);
			int slice_root = ring_index * pipe_layout.amount_of_ring_vertices;
			Vector3 vertice_position = new Vector3();
			vertice_position = new Vector3 ( 
				Mathf.Sin(0) * pipe_layout.ring_diameters[ring_index],
				Mathf.Cos (0) * pipe_layout.ring_diameters[ring_index],
				0
			);
			vertice_position = pipe_layout.ring_rotation [ring_index] * vertice_position;
			vertice_position += pipe_layout.ring_origins[ring_index];

			setVertice(slice_root, vertice_position);

			for (int v = 0, i = 1; v < pipe_layout.amount_of_ring_vertices-1; v++, i++) {
				var rad = (angle * v) * Mathf.Deg2Rad;
				vertice_position = new Vector3();
				vertice_position = new Vector3 ( 
					Mathf.Sin(rad) * pipe_layout.ring_diameters[ring_index],
					Mathf.Cos (rad) * pipe_layout.ring_diameters[ring_index],
					0
				);
				vertice_position = pipe_layout.ring_rotation [ring_index] * vertice_position;
				vertice_position += pipe_layout.ring_origins[ring_index];

				setVertice(i + slice_root, vertice_position);
			}
		}

		private void buildRing(int ring_index){
			float angle = 360.0f/(pipe_layout.amount_of_ring_vertices-1);
			//we duplicate the first vertex, needed for the texture to wrap around nicely.
			Vector3 vertice_position = new Vector3();
			vertice_position = new Vector3 ( 
				Mathf.Sin(0) * pipe_layout.ring_diameters[ring_index],
				Mathf.Cos (0) * pipe_layout.ring_diameters[ring_index],
				0
			);
			vertice_position = pipe_layout.ring_rotation [ring_index] * vertice_position;
			vertice_position += pipe_layout.ring_origins[ring_index];

			int slice_root = (ring_index*2) * pipe_layout.amount_of_ring_vertices;
			setVertice(slice_root, vertice_position);
			setVertice(slice_root + 1, vertice_position);

			for (int v = 0, i = 2; v < pipe_layout.amount_of_ring_vertices-1; v++, i += 2) {
				var rad = (angle * v) * Mathf.Deg2Rad;
				vertice_position = new Vector3();
				vertice_position = new Vector3 ( 
					Mathf.Sin(rad) * pipe_layout.ring_diameters[ring_index],
					Mathf.Cos (rad) * pipe_layout.ring_diameters[ring_index],
					0
				);
				vertice_position = pipe_layout.ring_rotation [ring_index] * vertice_position;
				vertice_position += pipe_layout.ring_origins[ring_index];
				setVertice(i + slice_root, vertice_position);
				setVertice(i + slice_root + 1, vertice_position);// we double the vertices
			}
		}

		private void setVertice(int index, Vector3 position){
			vertices.Insert (index, position);
		}

		void buildTriangles(){
			triangles = new List<int> ();
			int indexOfFirstRing;
			int indexOfSecondRing;
			for (int i = 0; i < pipe_layout.amount_of_rings-1; i++) {
				indexOfFirstRing = i * 2;
				indexOfSecondRing = i * 2 + 2;
				for (int j = 2; j < 2 * pipe_layout.amount_of_ring_vertices; j += 2) {
					//special biuld triangle from the top
					triangles.Add (j + indexOfFirstRing * pipe_layout.amount_of_ring_vertices);
					triangles.Add (j + pipe_layout.amount_of_ring_vertices * indexOfSecondRing);
					int next = (j + 3) % (pipe_layout.amount_of_ring_vertices * 2);
					triangles.Add (next + pipe_layout.amount_of_ring_vertices * indexOfFirstRing);
					//special build triangle from the bottom
					next = (j + 3) % (pipe_layout.amount_of_ring_vertices * 2);
					triangles.Add (next + pipe_layout.amount_of_ring_vertices * indexOfSecondRing);
					triangles.Add (next + pipe_layout.amount_of_ring_vertices * indexOfFirstRing);
					triangles.Add (j + pipe_layout.amount_of_ring_vertices * indexOfSecondRing);
				}
			}
		}

		//another version of building triangles without phong, which would lead to stripes instead of
		//cloud-like mess on the texture for everything but the last face
		void buildTriangles2(){
			triangles = new List<int> ();
			int indexOfFirstRing;
			int indexOfSecondRing;
			bool sw = true;
			for (int i = 0; i < pipe_layout.amount_of_rings - 1; i++) {
				indexOfFirstRing = i * 2;
				indexOfSecondRing = i * 2 + 2;
				for (int j = 2; j < 2 * pipe_layout.amount_of_ring_vertices - 2; j += 2) {
					if (sw) {
						BuildTriangleFromTheTop (j, indexOfFirstRing, indexOfSecondRing, 2);
						BuildTriangleFromTheBottom (j, indexOfSecondRing, indexOfFirstRing, 2);
					} else {
						BuildTriangleFromTheTop (j + 1, indexOfFirstRing, indexOfSecondRing, 2);
						BuildTriangleFromTheBottom (j + 1, indexOfSecondRing, indexOfFirstRing, 2);
					}
					sw = !sw;
				}
				//if the number of vertices is even, just proceed with the last one
				if (pipe_layout.amount_of_ring_vertices % 2 == 0) {
					BuildTriangleFromTheTop (2 * pipe_layout.amount_of_ring_vertices - 1, indexOfFirstRing, indexOfSecondRing, 2);
					BuildTriangleFromTheBottom (2 * pipe_layout.amount_of_ring_vertices - 1, indexOfSecondRing, indexOfFirstRing, 2);
				}
				//if the number of vertices is odd, we need to consider a special case- this creates a cloud-like texture on that face
				else {
					int j = 2 * pipe_layout.amount_of_ring_vertices - 2;
					//special biuld triangle from the top
					triangles.Add (j + indexOfFirstRing*pipe_layout.amount_of_ring_vertices);
					triangles.Add (j + pipe_layout.amount_of_ring_vertices * indexOfSecondRing);
					int next = (j + 3)%(pipe_layout.amount_of_ring_vertices*2);
					triangles.Add (next + pipe_layout.amount_of_ring_vertices * indexOfFirstRing);
					//special build triangle from the bottom
					next = (j + 3)%(pipe_layout.amount_of_ring_vertices*2);
					triangles.Add (next + pipe_layout.amount_of_ring_vertices * indexOfSecondRing);
					triangles.Add (next + pipe_layout.amount_of_ring_vertices * indexOfFirstRing);
					triangles.Add (j + pipe_layout.amount_of_ring_vertices * indexOfSecondRing);
				}
			}
		}

		void buildTrianglesWithPhong(){
			triangles = new List<int> ();

			for (int i = 0; i < pipe_layout.amount_of_rings-1; i++) {
				int indexOfFirstRing = i;
				int indexOfSecondRing = i + 1;
				for (int j = 1; j < pipe_layout.amount_of_ring_vertices; j++) {
					BuildTriangleFromTheTop (j, indexOfFirstRing, indexOfSecondRing,1);
					BuildTriangleFromTheBottom (j, indexOfSecondRing, indexOfFirstRing,1);
				}
			}
		}

		void BuildTriangleFromTheBottom(int indexBottom, int currentRingIndex, int previousRingIndex, int step){
			int next = (indexBottom + step)%(pipe_layout.amount_of_ring_vertices*step);
			triangles.Add (next + pipe_layout.amount_of_ring_vertices * currentRingIndex);
			triangles.Add (next + pipe_layout.amount_of_ring_vertices * previousRingIndex);
			triangles.Add (indexBottom + pipe_layout.amount_of_ring_vertices * currentRingIndex);
		}

		void BuildTriangleFromTheTop(int indexTop, int currentRingIndex, int previousRingIndex, int step){
			triangles.Add (indexTop + currentRingIndex*pipe_layout.amount_of_ring_vertices);
			triangles.Add (indexTop + pipe_layout.amount_of_ring_vertices * previousRingIndex);
			int next = (indexTop + step)%(pipe_layout.amount_of_ring_vertices*step);
			triangles.Add (next + pipe_layout.amount_of_ring_vertices * currentRingIndex);
		}
	}
}


