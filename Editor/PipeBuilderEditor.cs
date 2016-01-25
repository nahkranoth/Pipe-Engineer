using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(pipeBuilder))]
public class PipeBuilderEditor : Editor {
	void OnSceneGUI() {
		pipeBuilder pipeBuildTarget = (pipeBuilder) target;
		Undo.RecordObject (pipeBuildTarget, "Moved pipe build target");//Ctrl+Z

		int controlID = GUIUtility.GetControlID (FocusType.Passive);
		Vector3 screenPosition = Handles.matrix.MultiplyPoint(pipeBuildTarget.targetPos);

		Handles.color = Color.white;
		//pipeBuildTarget.handlePos = Handles.PositionHandle (pipeBuildTarget.targetPos , Quaternion.identity);
		Handles.ArrowCap(controlID,
			pipeBuildTarget.targetPos,
			Quaternion.Euler(pipeBuildTarget.transform.localEulerAngles),
			pipeBuildTarget.diameter
		);

		//HandleUtility.GetHandleSize (new Vector3(pipeBuildTarget.diameter, pipeBuildTarget.diameter, pipeBuildTarget.diameter));// (for dynamic size)

		switch (Event.current.GetTypeForControl (controlID)) {
		case EventType.Layout:
			HandleUtility.AddControl (
				controlID,
				HandleUtility.DistanceToCircle (screenPosition, 0.1f)
			);
			break;
		
		case EventType.MouseDown:
			if (HandleUtility.nearestControl == controlID && Event.current.button == 0) {
				GUIUtility.hotControl = controlID;
				Event.current.Use ();
			}
			break;
		case EventType.MouseUp:
			if (HandleUtility.nearestControl == controlID) {
				GUIUtility.hotControl = 0;
				Event.current.Use();
			}
			break;
		case EventType.MouseDrag:
			if (GUIUtility.hotControl == controlID && Event.current.button == 0) {
				pipeBuildTarget.handleDistance += (HandleUtility.niceMouseDelta/50);//division is slowing down dragspeed
				pipeBuildTarget.Update ();
				GUI.changed = true;
				Event.current.Use ();
			}
			break;
		}

		//pipeBuildTarget.handleDistance = 1;
		//Vector3.Distance (pipeBuildTarget.transform.position, pipeBuildTarget.targetPos);
	}
}
