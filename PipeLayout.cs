using System;
using UnityEngine;
using System.Collections.Generic;

namespace nl.elleniaw.pipeBuilder{
	public class PipeLayout
	{
		public List<Vector3> ring_origins = new List<Vector3>();
		public List<Quaternion> ring_rotation = new List<Quaternion>();
		public List<float> ring_diameters = new List<float>();

		public int amount_of_rings = 2;
		public float diameter = 0.1f;
		public int amount_of_ring_vertices = 9;

		public Vector3 heading_deg = Vector3.zero;
		public Vector3 heading_pos = Vector3.zero;
		public Vector3 rotate_root_position = Vector3.zero;

		private PipeManager parent;
		//List with positions and rotations of each rings

		public PipeLayout (PipeManager _parent)
		{
			parent = _parent;
			AddRing (new Vector3 (0, 0, 0f), new Vector3 (0, 0, 0), 0.2f);
			AddRing (new Vector3 (0, 0, 0.2f), new Vector3 (0, 0, 0), 0.2f);
			AddRing (new Vector3 (0, 0, 0.2f), new Vector3 (0, 0, 0), 0.1f);
			AddRing (new Vector3 (0, 0, 1.0f), new Vector3 (0, 0, 0), 0.1f);
		}

		public void ExtrudeRing(float offset, Vector3 rotation, float diameter){

			Vector3 origin = new Vector3 (0,0,1.0f);
			Vector3 addition = new Vector3 (0,0,offset);

			heading_deg += rotation;
			heading_pos += Quaternion.Euler (heading_deg.x, heading_deg.y, heading_deg.z) * addition;

			AddRing (heading_pos, heading_deg, diameter);
			parent.UpdateLayout ();
		}

		public void UpdateLastRing(){
			RemoveRing ();
			parent.ApplyLayoutChanges ();
		}

		public void RemoveRing(){

			if(ring_origins.Count > 1){
				ring_origins.RemoveRange (ring_origins.Count -1, 1);
				ring_rotation.RemoveRange (ring_rotation.Count -1, 1);
				ring_diameters.RemoveRange (ring_diameters.Count -1, 1);

				heading_deg = ring_rotation [ring_rotation.Count - 1].eulerAngles;
				heading_pos = ring_origins [ring_origins.Count - 1];
				amount_of_rings = ring_origins.Count;

				parent.ApplyLayoutChanges ();
			}

		}

		private void AddRing(Vector3 position, Vector3 euler_angle, float diameter){
			ring_origins.Add (position);
			ring_rotation.Add (Quaternion.Euler(euler_angle));
			ring_diameters.Add (diameter);
			amount_of_rings = ring_origins.Count;
			heading_pos = position;
		}


		private void Reset(){
			ring_origins.Clear ();
			ring_rotation.Clear ();
			ring_diameters.Clear ();
			amount_of_rings = 0;
			parent.ApplyLayoutChanges ();
		}

	}
}


