using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace nl.elleniaw.pipeBuilder{
	public class PipeGizmos{

		public List<Vector3> gizmo_vertices;
		public List<Vector3> selected_gizmos;
		private Vector3 gizmo_position;
		private Quaternion gizmo_rotation;
		public float size = 0.1f;

		public bool vertices_selected = false;

		public PipeGizmos(List<Vector3> _vertices, Mesh _pipe_mesh, Vector3 _position, Quaternion _rotation){
			gizmo_vertices = _vertices;
			selected_gizmos = Enumerable.Repeat(Vector3.zero, gizmo_vertices.Count).ToList();
			gizmo_position = _position;
			gizmo_rotation = _rotation;
		}

		public void OnMoveHandle(Vector3 delta_pos){
			for (int i = 0; i < selected_gizmos.Count; i++) {
				if(selected_gizmos[i] != Vector3.zero){
					gizmo_vertices [i] -= delta_pos;
					selected_gizmos [i] = gizmo_vertices [i];
				}
			}
		}
			
		public void OnMouseSelection(Rect selection){
			selected_gizmos = Enumerable.Repeat(Vector3.zero, gizmo_vertices.Count).ToList();
			vertices_selected = false;
			for (int i = 0; i < gizmo_vertices.Count; i++) {
				if(Camera.current != null){
					Vector2 screen_position = Camera.current.WorldToScreenPoint (gizmo_vertices [i]);
					if(selection.Contains(new Vector2(screen_position.x, Screen.height - screen_position.y), true)){
						selected_gizmos [i] = gizmo_vertices [i];
						vertices_selected = true;
					}
				}
			}
		}

		public void Update() {
			for (int i = 0; i < gizmo_vertices.Count; i++) {
				var v = gizmo_rotation * gizmo_vertices [i];
				if (selected_gizmos != null && selected_gizmos [i] == gizmo_vertices [i]) {
					Gizmos.color = Color.yellow;
				} else {
					Gizmos.color = Color.black;
				}
				Gizmos.DrawSphere (new Vector3 (v.x + gizmo_position.x, v.y + gizmo_position.y, v.z + gizmo_position.z), 0.1f * size);
			}
		}
	}
}
