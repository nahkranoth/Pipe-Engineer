using UnityEngine;
using System.Collections;

namespace nl.elleniaw.pipeBuilder{
	public class PipeGizmos{

		private Vector3[] gizmo_vertices;
		private Vector3 gizmo_position;
		private Quaternion gizmo_rotation;
		private Mesh pipe_mesh;
		public float size = 0.1f;

		public PipeGizmos(Vector3[] _vertices, Mesh _pipe_mesh, Vector3 _position, Quaternion _rotation){
			gizmo_vertices = _vertices;
			gizmo_position = _position;
			gizmo_rotation = _rotation;
			pipe_mesh = _pipe_mesh;
		}

		public void Update() {
			for (int i = 0; i < gizmo_vertices.Length; i++) {
				var v = gizmo_rotation * gizmo_vertices [i];
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (new Vector3 (v.x + gizmo_position.x, v.y + gizmo_position.y, v.z + gizmo_position.z), 0.1f * size);
			}
			Gizmos.color = new Color (1.0f, 1.0f, 0.0f, 0.3f);
			Gizmos.DrawMesh (pipe_mesh, gizmo_position, gizmo_rotation, Vector3.one);
		}
	}
}
