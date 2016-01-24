using UnityEngine;
using System.Collections;
using System;

namespace nl.elleniaw.pipeBuilder{
	public class PipeManager {

		public PipeLayout pipe_layout;
		public PipeMesh pipe_mesh;
		private PipeGizmos pipe_gizmos;
		private PipeLayoutToMesh pipe_layout_to_mesh;

		public int amount_of_ring_vertices = 9;

		public bool drawMesh = true;
		public bool drawGizmos = false;
		public bool hasPhong = true;

		private Pipe.MeshCallback meshCallback;

		private Quaternion rotation;
		public Quaternion root_rotation{
			set { 
				rotation = value;
				ApplyLayoutChanges ();
			}
			get {
				return rotation;
			}
		}

		private Vector3 position;
		public Vector3 root_position {
			set { 
				position = value;
				ApplyLayoutChanges ();
			}
			get {
				return position;
			}
		}
			
		public PipeManager(Pipe.MeshCallback _meshCallback){
			meshCallback = _meshCallback;
			pipe_layout = new PipeLayout (this);
			root_position = new Vector3 (0,0,0);
			root_rotation = new Quaternion ();
			ApplyLayoutChanges();
		}

		public void ApplyLayoutChanges(){
			pipe_layout_to_mesh = new PipeLayoutToMesh (pipe_layout, hasPhong);
			if (drawMesh) {
				pipe_mesh = new PipeMesh (pipe_layout_to_mesh.vertices, pipe_layout_to_mesh.triangles);
			}
			if(drawGizmos){
				pipe_gizmos = new PipeGizmos (pipe_layout_to_mesh.vertices, pipe_mesh.mesh, root_position, root_rotation);
			}

			meshCallback (pipe_mesh.mesh);
		}
			
		public void UpdateGizmos(){
			if (drawGizmos) {
				pipe_gizmos.Update ();
			}
		}

	}
}