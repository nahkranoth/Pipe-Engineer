using System;
using UnityEngine;


/*TODO:
	-> Remove last extrusion

	-> Mesh texture bug (UV's?)
	-> Bolts (Add Objects?)
	-> Handle instead of Shift to select, check closes handles?
	-> PBS materials
*/
using System.Collections.Generic;

namespace nl.elleniaw.pipeBuilder{
	
	[ExecuteInEditMode]
	public class Pipe : MonoBehaviour 
	{
		public PipeManager pipe_manager;
		MeshFilter filter;

		public delegate void MeshCallback(Mesh mesh);

		[HideInInspector]
		public Vector3 rotation = Vector3.one;
		[HideInInspector]
		public float offset = 1.0f;
		[HideInInspector]
		public float diameter = 1.0f;
		[HideInInspector]
		public int repeater = 1;
		public int detail = 7;
		[HideInInspector]
		public bool handle_visible = false;
		[HideInInspector]
		public Vector3 handle_rotation = Vector3.zero;
		[HideInInspector]
		public Vector3 handle_position = Vector3.zero;

		private bool drawMesh = true;
		public bool drawGizmos = true;
		public bool hasPhong = true;
	
		MeshCallback meshCallback;
		public delegate void HandleSelected (Vector3 vertice_position);
		protected HandleSelected handleSelectCallback;

		public void OnValidate(){
			Validate ();
		}

		public void Validate ()
		{
			if (pipe_manager != null) {
				pipe_manager.hasPhong = hasPhong;
				pipe_manager.drawGizmos = drawGizmos;
				pipe_manager.drawMesh = drawMesh;
				pipe_manager.ResetLayout ();
			}

			if (!drawMesh) {
				filter.mesh = null;
			}
		}

		void OnEnable(){
			meshCallback = new MeshCallback (setMesh);
			pipe_manager = new PipeManager (meshCallback);
			pipe_manager.root_position = transform.position;
			pipe_manager.root_rotation = transform.rotation;
			setMesh (pipe_manager.pipe_mesh.mesh);
			Validate ();
		}

		public void OnMoveHandle(Vector3 _handle_position){
			if(pipe_manager != null){
				Vector3 delta_handle_position = handle_position - _handle_position;
				handle_position = _handle_position;
				pipe_manager.OnMoveHandle (delta_handle_position/2, handleSelectCallback);
			}
		}

		public void OnMouseSelection(Rect selection){
			if (pipe_manager != null) {
				handleSelectCallback = setHandle;
				pipe_manager.OnMouseSelection (selection, handleSelectCallback);
				handle_visible = pipe_manager.pipe_gizmos.vertices_selected;
			}
		}

		public void setHandle(Vector3 position){
			handle_position = position;
		}

		private void setMesh(Mesh mesh){
			filter = gameObject.GetComponent<MeshFilter>();
			if(filter == null){
				filter = gameObject.AddComponent<MeshFilter>();
			}
			if (drawMesh) {
				filter.mesh = mesh;
			}
		}

		void Update(){
			if (transform.hasChanged && pipe_manager != null) {
				pipe_manager.root_position = transform.position;
				pipe_manager.root_rotation = transform.rotation;
				transform.hasChanged = false;
			}
		}

		public void OnDrawGizmos(){
			if(pipe_manager != null){
				pipe_manager.UpdateGizmos ();
			}
		}

		public void OnExtrudeRing(){
			pipe_manager.ExtrudeRing (offset, rotation, diameter);
		}

		public void OnRemoveRing(){
			if (pipe_manager != null) {
				pipe_manager.pipe_layout.RemoveRing ();
			}
		}
	}
}