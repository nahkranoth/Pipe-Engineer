using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace nl.elleniaw.pipeBuilder{
	public class PipeLayoutToMesh
	{

		private bool doubledVertices;
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

		public void ExtrudeRing(PipeLayout pipe_layout){
			if (doubledVertices) {
				buildRingWithPhong (pipe_layout.amount_of_rings - 1);
			}
			buildTriangles ();
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
			triangles = new List<int> ();

			for (int i = 0; i < pipe_layout.amount_of_rings-1; i++) {
				int indexOfFirstRing = i;
				int indexOfSecondRing = i + 1;
				for (int j = 0; j < pipe_layout.amount_of_ring_vertices; j++) {
					BuildTriangleFromTheTop (j, indexOfFirstRing, indexOfSecondRing);
					BuildTriangleFromTheBottom (j, indexOfSecondRing, indexOfFirstRing);
				}
			}
		}

		void BuildTriangleFromTheBottom(int indexBottom, int currentRingIndex, int previousRingIndex){
			triangles.Add (indexBottom + pipe_layout.amount_of_ring_vertices * currentRingIndex);
			int next = (indexBottom + 1)%pipe_layout.amount_of_ring_vertices;
			triangles.Add (next + pipe_layout.amount_of_ring_vertices * currentRingIndex);
			triangles.Add (next + pipe_layout.amount_of_ring_vertices * previousRingIndex);
		}

		void BuildTriangleFromTheTop(int indexTop, int currentRingIndex, int previousRingIndex){
			triangles.Add (indexTop + currentRingIndex*pipe_layout.amount_of_ring_vertices);
			triangles.Add (indexTop + pipe_layout.amount_of_ring_vertices * previousRingIndex);
			int next = (indexTop + 1)%pipe_layout.amount_of_ring_vertices;
			triangles.Add (next + pipe_layout.amount_of_ring_vertices * currentRingIndex);
		}
	}
}


