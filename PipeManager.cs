using UnityEngine;
using System.Collections;
using System;
using System.Linq;

namespace nl.elleniaw.pipeBuilder{
	public class PipeManager {

		public PipeLayout pipe_layout;
		public PipeMesh pipe_mesh;
		public PipeGizmos pipe_gizmos;
		public PipeLayoutToMesh pipe_layout_to_mesh;

		public bool drawMesh = true;
		public bool drawGizmos = false;
		public bool hasPhong = true;

		private Pipe.MeshCallback meshCallback;

		private Quaternion rotation;
		public Quaternion root_rotation{
			set { 
				rotation = value;
				ResetLayout ();
			}
			get {
				return rotation;
			}
		}

		private Vector3 position;
		public Vector3 root_position {
			set { 
				position = value;
				ResetLayout ();
			}
			get {
				return position;
			}
		}

		public void ExtrudeRing(float offset, Vector3 rotation, float diameter){
			pipe_layout.AddRing (offset, rotation, diameter);
			pipe_layout_to_mesh.ExtrudeRing (pipe_layout);
			UpdateLayout ();
		}

		public void OnMouseSelection(Rect selection, Pipe.HandleSelected handleCallback){
			if(pipe_gizmos != null && drawGizmos){
				pipe_gizmos.OnMouseSelection (selection);
				pipe_layout_to_mesh.OnMouseSelection (selection, handleCallback);
			}
		}
		public void OnMoveHandle(Vector3 delta_pos, Pipe.HandleSelected handleCallback){
			pipe_gizmos.OnMoveHandle (delta_pos);
			pipe_layout_to_mesh.OnMoveHandle (delta_pos, handleCallback);
			UpdateLayout ();
		}

		public PipeManager(Pipe.MeshCallback _meshCallback){
			meshCallback = _meshCallback;
			pipe_layout = new PipeLayout (this);
			root_position = new Vector3 (0,0,0);
			root_rotation = new Quaternion ();
			ResetLayout();
		}
			
		//Update Mesh
		public void UpdateLayout(){
			CreateMesh ();
			if(drawGizmos){
				pipe_gizmos.gizmo_vertices = pipe_layout_to_mesh.vertices;
				pipe_gizmos.selected_gizmos = Enumerable.Repeat(Vector3.zero, pipe_gizmos.gizmo_vertices.Count).ToList();
				for (int i = 0; i <  pipe_layout_to_mesh.selected_vertices.Count; i++) {
					pipe_gizmos.selected_gizmos.Insert (i, pipe_layout_to_mesh.selected_vertices [i]);
				}
			}
			meshCallback (pipe_mesh.mesh);
		}

		//totally redraw Mesh and Gizmo Layout
		public void ResetLayout(){
			pipe_layout_to_mesh = new PipeLayoutToMesh (pipe_layout, hasPhong);
			CreateMesh ();
			if(drawGizmos){
				pipe_gizmos = new PipeGizmos (pipe_layout_to_mesh.vertices, pipe_mesh.mesh, root_position, root_rotation);
			}
			meshCallback (pipe_mesh.mesh);
		}
			
		private void CreateMesh(){
			if (drawMesh) {
				pipe_mesh = new PipeMesh (pipe_layout_to_mesh.vertices, pipe_layout_to_mesh.triangles);
			}
		}

		public void UpdateGizmos(){
			if (drawGizmos) {
				pipe_gizmos.Update ();
			}
		}

	}
}