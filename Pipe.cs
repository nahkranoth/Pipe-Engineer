﻿using System;
using UnityEngine;


/*TODO:
	-> Extrude from new position instead of previous extruded point
	-> Hide handle when no selection
	-> Stop multiple gizmoMeshes created

	-> Bolts (Add Objects?)
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

		public bool drawMesh = true;//set
		public bool drawGizmos = true;
		public bool hasPhong = true;

		public Vector3 handle_position = Vector3.zero;
		public Vector3 handle_rotation = Vector3.zero;

		public delegate void HandleSelected (Vector3 vertice_position);
		protected HandleSelected handleSelectCallback;

		public void OnValidate(){
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

		public Pipe ()
		{
			MeshCallback meshCallback = new MeshCallback (setMesh);
			pipe_manager = new PipeManager (meshCallback);
			pipe_manager.root_position = transform.position;
			pipe_manager.root_rotation = transform.rotation;
			setMesh (pipe_manager.pipe_mesh.mesh);
		}

		public void OnMoveHandle(Vector3 _handle_position){
			Vector3 delta_handle_position = handle_position - _handle_position;
			handle_position = _handle_position;
			pipe_manager.OnMoveHandle (delta_handle_position/2, handleSelectCallback);
		}

		public void OnMouseSelection(Rect selection){
			handleSelectCallback = setHandle;
			pipe_manager.OnMouseSelection (selection, handleSelectCallback);
		}

		public void setHandle(Vector3 position){
			Debug.Log("Handle Callback: "+position);
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