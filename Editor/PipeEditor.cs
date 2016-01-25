using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace nl.elleniaw.pipeBuilder{

	[CustomEditor(typeof(Pipe))]

	public class PipeEditor : Editor {

		Pipe pipeBuildTarget;

		string rotationX = "0";
		string rotationY = "0";
		string rotationZ = "0";
		string offset = "1";
		string diameter = "0.1";
		string repeater = "0";

		void OnSceneGUI() {
			pipeBuildTarget = (Pipe)target;
			Undo.RecordObject (pipeBuildTarget, "Moved pipe build target");//Ctrl+Z
			SetGUIMenu ();
//			SetHandle ();
		}


		void SetHandle() {
			pipeBuildTarget = (Pipe)target;
			int controlID = GUIUtility.GetControlID (FocusType.Passive);
			Vector3 screenPosition = Handles.matrix.MultiplyPoint(pipeBuildTarget.pipe_manager.pipe_layout.handle_position);
			Handles.color = Color.white;

			pipeBuildTarget.pipe_manager.pipe_layout.handle_position = Handles.PositionHandle (pipeBuildTarget.pipe_manager.pipe_layout.handle_position, Quaternion.Euler(pipeBuildTarget.pipe_manager.pipe_layout.handle_rotation));

			if (GUI.changed)
			{
//				Check if Extrude
//				EditorUtility.SetDirty (target);
//				pipeBuildTarget.pipe_manager.pipe_layout.UpdateLastRing ();

			}
		}

		void OnEnable(){
			UpdateHandlePosition ();
			loadParams ();
		}

		void UpdateHandlePosition(){
			pipeBuildTarget = (Pipe)target;
			pipeBuildTarget.pipe_manager.pipe_layout.handle_position = pipeBuildTarget.pipe_manager.pipe_layout.heading_pos;
			pipeBuildTarget.pipe_manager.pipe_layout.handle_rotation = pipeBuildTarget.pipe_manager.pipe_layout.heading_deg;
		}

		void OnDisable(){
			saveParams ();
		}

		void loadParams(){
			pipeBuildTarget = (Pipe)target;
			rotationX = pipeBuildTarget.rotation.x.ToString();
			rotationY = pipeBuildTarget.rotation.y.ToString();
			rotationZ = pipeBuildTarget.rotation.z.ToString();
			offset =  pipeBuildTarget.offset.ToString();
			diameter =  pipeBuildTarget.diameter.ToString();
			repeater = pipeBuildTarget.repeater.ToString();
		}
			
		void saveParams(){
			pipeBuildTarget.repeater = int.Parse (repeater);
			pipeBuildTarget.offset = float.Parse (offset);
			pipeBuildTarget.diameter = float.Parse (diameter);
			pipeBuildTarget.rotation.x = float.Parse (rotationX);
			pipeBuildTarget.rotation.y = float.Parse (rotationY);
			pipeBuildTarget.rotation.z = float.Parse (rotationZ);
		}

		void checkValidInput(){
			rotationX = IsFloatOnly(rotationX) ? rotationX : pipeBuildTarget.rotation.x.ToString();
			rotationY = IsFloatOnly(rotationY) ? rotationY : pipeBuildTarget.rotation.y.ToString();
			rotationZ = IsFloatOnly(rotationZ) ? rotationZ : pipeBuildTarget.rotation.z.ToString();
			offset = IsFloatOnly(offset) ? offset : pipeBuildTarget.offset.ToString();
			diameter = IsFloatOnly(diameter) ? diameter : pipeBuildTarget.diameter.ToString();
			repeater = IsDigitOnly(repeater) ? repeater : pipeBuildTarget.repeater.ToString();
		}

		//werkt nog niet precies

		bool IsDigitOnly(string str){
			Regex r = new Regex("[0-9]");
			if (r.IsMatch(str)) {
				return true;
			}
			return false;
		}

		bool IsFloatOnly(string str)
		{
			Regex r = new Regex("[0-9|.|-]");
			if (r.IsMatch(str)) {
				return true;
			}
			return false;
		}

//		Main Extruder Menu
		private void SetGUIMenu(){
			pipeBuildTarget = (Pipe)target;
			Handles.BeginGUI();
			Rect size = new Rect (0, 0, 310, 150);

			GUI.BeginGroup (new Rect (0, 0, size.width, size.height));
			GUI.color = getColor("#ABA688");
			GUI.Box (new Rect (4, 4, 300, 128), "Pipe Machine");
			GUI.color = getColor("#DAEFF6F");
			buildPositionTextFields ();
			buildRotationTextFields ();

			GUI.Label(new Rect(19, 77, 100, 20), "Diameter");
			diameter = GUI.TextField(new Rect(102, 77, 50, 17), diameter, 25);

			GUI.Label(new Rect(19, 102, 100, 20), "Repeat");
			repeater = GUI.TextField(new Rect(102, 102, 50, 17), repeater, 25);

			if (GUI.Button (new Rect (225, 102, 70, 20), "Extrude")) {
				saveParams ();
				for (int i = 0; i < int.Parse(repeater); i++) {
					pipeBuildTarget.OnExtrudeRing ();
				}
			}

			if (GUI.Button (new Rect (170, 102, 50, 20), "Del")) {
				for (int i = 0; i < int.Parse(repeater); i++) {
					pipeBuildTarget.OnRemoveRing ();
				}
			}
			if (GUI.changed) {
				checkValidInput ();
			}

			GUI.EndGroup ();
			Handles.EndGUI ();
		}
			
		private void buildPositionTextFields(){
			var root = 89;
			var margin = 15;
			var width = 50;
			var count = 0;
			var top = 52;
			var height = 17;
			var textWidth = 12;

			GUI.Label(new Rect(19, top, 100, 20), "Forwards");

			var pos1 = root + (count * (width + margin));
			offset = GUI.TextField(new Rect(pos1 + textWidth, top, width, height), offset, 25);
		}

		private void buildRotationTextFields(){
			var root = 89;
			var margin = 15;
			var width = 50;
			var count = 0;
			var top = 28;
			var height = 17;
			var textWidth = 12;

			GUI.Label(new Rect(19, top, 100, 20), "Rotation");

			var pos1 = root + (count * (width + margin));
			GUI.Label(new Rect(pos1, top, 100, 20), "X");
			rotationX = GUI.TextField(new Rect(pos1 + textWidth, top, width, height), rotationX, 25);

			count++;
			var pos2 = root + count * (width + margin);
			GUI.Label(new Rect(pos2, top, 100, 20), "Y");
			rotationY = GUI.TextField(new Rect(pos2 + textWidth, top, width, height), rotationY, 25);

			count++;
			var pos3 = root + count * (width + margin);
			GUI.Label(new Rect(pos3, top, 100, 20), "Z");
			rotationZ = GUI.TextField(new Rect(pos3 + textWidth, top, width, height), rotationZ, 25);
		}

		private Color getColor(string hex){
			Color bg_clr = new Color ();
			ColorUtility.TryParseHtmlString (hex, out bg_clr);
			return bg_clr;
		}
	}

}

