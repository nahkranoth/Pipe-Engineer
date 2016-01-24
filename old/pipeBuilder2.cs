using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]

/*TODO:
	-> Fix vertices and triangles (every quad own verts?)
	-> Handle control tweaking (dragSpeed)
	-> Thicker pieces & Bolts
	-> Corners, EndCaps
	
	-> UV's did I set them correctly?
	-> PBS materials (and other materials)
	-> Expose functionality
*/


public class pipeBuilder2 : MonoBehaviour {

	public int faces_amount = 10;
	public int amount_of_rings = 2;
	public float diameter = 2.0f;
	public bool drawMesh = false;
	public bool drawGizmos = true;

	private Vector3[] vertices;
	private Vector3[] gizmo_vertices;
	private Vector2[] uvs;
	private Vector3[] normals;
	private int[] triangles;
	public Vector3 targetPos = new Vector3(0, 0, 0);

	private Mesh mesh;

	public Vector3 originAngle;
	public float handleDistance = 0;

	public void Update() {
		float slide_distance = handleDistance/(amount_of_rings-1);
		mesh = new Mesh ();
		vertices = new Vector3[faces_amount * amount_of_rings];
		gizmo_vertices = new Vector3[vertices.Length];
		uvs = new Vector2[vertices.Length];
		normals = new Vector3[vertices.Length];
		triangles = new int[(vertices.Length - faces_amount) * 6];

		originAngle = transform.eulerAngles;
		targetPos = Quaternion.Euler (originAngle) * (Vector3.forward * handleDistance);//set rotational heading on target from origin
		targetPos += transform.position;//add origin position to target

		for (int i = 0; i < amount_of_rings; i++) {
			buildRing (i * slide_distance, i);
		}

		buildTriangles ();
		buildUVS ();
		buildNormals ();
		DrawMesh ();
	}
		
	void buildRing(float xRoot, int slice){
		float angle = 360/faces_amount;

		for (var i = 0; i < faces_amount; i++) {
			var rad = (angle * i) * Mathf.Deg2Rad;
			Vector3 vertice_position = new Vector3();
			vertice_position = new Vector3 ( 
				Mathf.Sin(rad) * diameter,
				Mathf.Cos (rad) * diameter,
				0
			);
			vertice_position += Vector3.forward * ((handleDistance/2) * slice); //offset each ring forward
			int slice_root = slice * faces_amount;//slice * 2 because we doubled the vertices
			buildVertice(i + slice_root, vertice_position);
		}

	}

	void buildVertice(int index, Vector3 position){
		vertices [index] = position;
		gizmo_vertices [index] = transform.rotation * position;
	}

	void buildTriangles(){

		for(var j=1;j<amount_of_rings;j++){
			for(var i=1;i<=faces_amount;i++){
				var index = i + ((j-1)*faces_amount);
				buildQuad ( index, j);
			}
		}
	}

	void buildQuad(int vertice, int ring){
		var triangle = ((vertice - 1) * 6) + 1;
		var baseVert = vertice - 1;
	
		triangles [triangle - 1] = baseVert;
		triangles [triangle] = triangles [triangle + 3] =  baseVert + faces_amount;

		if(baseVert + 1 == (faces_amount * ring)){
			baseVert = (faces_amount * ring) - (faces_amount + 1);
		}
		triangles [triangle + 1] = triangles [triangle + 2] =baseVert + 1;
		triangles [triangle + 4] = baseVert + faces_amount + 1;
	}
		
	void buildUVS() {
		for (int i = 0; i < vertices.Length; i++) {
			if (i % 4 == 0) {
				uvs [i] = Vector2.zero;
			} else if (i % 4 == 1) {
				uvs [i] = Vector2.right;
			} else if (i % 4 == 2) {
				uvs [i] = Vector2.up;
			} else {
				uvs[i] = Vector2.one;
			}
		}
	}

	void buildNormals(){
		for (int i = 0; i < vertices.Length; i++) {
			normals [i] = new Vector3 (0,0,0);
		}
	}

	void OnDrawGizmos() {
		if (drawGizmos) {
			for (int i = 0; i < gizmo_vertices.Length; i++) {
				var v = gizmo_vertices [i];
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (new Vector3 (v.x + transform.position.x, v.y + transform.position.y, v.z + transform.position.z), 0.1f * diameter);
			}
			Gizmos.color = new Color (1.0f, 1.0f, 0.0f, 0.3f);
			Gizmos.DrawMesh (mesh, transform.position,transform.rotation, Vector3.one);
		}
	}

	void DrawMesh (){
		MeshFilter filter = gameObject.GetComponent<MeshFilter>();
		if(filter == null){
			filter = gameObject.AddComponent<MeshFilter>();
		}
		if (!drawMesh) {
			filter.mesh = null;
		} else {
			filter.mesh = mesh;
		}
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		//mesh.RecalculateBounds();
		//mesh.Optimize();

	}

}