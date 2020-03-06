using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lib;

class EditWindow : EditorWindow {

	static List<PlaneData> planes;

	public static GameObject source;
	static Vector3 posM;
	static int xValue,yValue;
	static float tileHeight;
	static float tileWidth;
	static Vector3 planeEulers;
	static float angle;
	static bool s = false;
	
	delegate void ModeDelegate();
	private static ModeDelegate mode;

	[MenuItem ("Tools/LevelBuilder")]
	public static void  ShowWindow () {
		SceneView.onSceneGUIDelegate += OnScene;
		EditorWindow.GetWindow(typeof(EditWindow));
	}
	 







	public static void undoTilesList (){
		List<Tile.Key> tempList = new List<Tile.Key> ();

		for (int i = 0; i < PlaneData.planes.Count; i++) { 
			
			foreach (KeyValuePair<Tile.Key,Tile> t in PlaneData.planes[i].tileDictionary) {
				if (t.Value.gameObject == null){
					tempList.Add (t.Key);
					
				}
			}
			
			
			
			foreach (Tile.Key k in tempList) {
				PlaneData.planes[i].tileDictionary.Remove (k);
			}
			if(PlaneData.planes[i].tileDictionary.Count <=0 && (PlaneData.planes.Count > 1)) {

				if(selGridInt == i) selGridInt--;

				PlaneData.removePlane(i);

			}

		}
		
	}
	
	
	
	void OnEnable(){

		Undo.undoRedoPerformed += undoTilesList; // += for more methods
		//Undo.UndoRedoCallback = undoTilesList;

		planes = PlaneData.planes;
		if (planes.Count <= 0) PlaneData.addPlane (new Plane (Vector3.up,Vector3.right), Quaternion.identity,new Vector3(0,0,0));



		angle = 45;
		}


	static bool addNewPlane;
	static public int selGridInt = 0;


	static Mesh mesh;
	void OnGUI () {
		// The actual window code goes here


	EditorGUILayout.BeginHorizontal();
	source = (GameObject)EditorGUILayout.ObjectField(source, typeof(GameObject), true);

	EditorGUILayout.EndHorizontal();


	
		addNewPlane = GUILayout.Toggle ( addNewPlane,"addNewPlane");

		EditorGUILayout.Vector3Field ("Mouse position:",posM);

		EditorGUILayout.LabelField("Width : " + tileWidth );
		EditorGUILayout.LabelField("Height : " + tileHeight );

		angle =EditorGUILayout.FloatField ("Angle" ,angle);
		planeEulers = EditorGUILayout.Vector3Field ("normal", planeEulers);

		selGridInt = GUILayout.SelectionGrid( selGridInt, PlaneData.planesNames(),2);


		if(source != null){
			
			mesh = source.GetComponent<MeshFilter>().sharedMesh;
			tileWidth = mesh.bounds.size.x;
			tileHeight = mesh.bounds.size.z;
			mode = Construct;
		}
	}






	private static void Construct (){
		
		int id =GUIUtility.GetControlID (FocusType.Passive);
		
		
		

			
			HandleUtility.AddDefaultControl (id);
			Tools.current = Tool.None;
	
		
		
		
		
		
		posM = Event.current.mousePosition;	
		
		posM = new Vector3 (posM.x, Camera.current.pixelHeight -posM.y, 0);
		Ray ray = Camera.current.ScreenPointToRay(posM);
		
		Vector3 hitPoint;
		float dist;
		Quaternion currentPlaneRotation =planes[selGridInt].q  ;
		Plane currentPlane =  planes[selGridInt].p;
		
		if (currentPlane.Raycast (ray, out dist)) {
			hitPoint = ray.GetPoint (dist);
			
			Vector3 dv= hitPoint - planes[selGridInt].origin;
			Vector3 Px = planes[selGridInt].q* Vector3.right;
			Vector3 Py = planes[selGridInt].q *Vector3.forward;
			float dx = Vector3.Dot (dv,Px);
			float dy = Vector3.Dot(dv,Py);
			dx= Mathf.Round(dx/tileWidth) *tileWidth ;
			dy = Mathf.Round(dy/tileHeight)*tileHeight;
			hitPoint = dy * Py + dx*Px + planes[selGridInt].origin;
			
			xValue = (int)dx;
			yValue = (int)dy;
			
			
			
			
			
			Handles.color = Color.green;
			
			
			
			
			Handles.color = Color.blue;
			bool addPlanePossible = false;
			Quaternion newRotation = currentPlaneRotation;
			Vector3 newPosition = hitPoint;
			Vector3 tempVector = Vector3.zero;
			
			
			
			if (addNewPlane){
				Tile [] neighbors = new Tile[4];
				neighbors [0] = Tile.checkTile (planes[selGridInt].tileDictionary,xValue + 1, yValue);
				neighbors [1] = Tile.checkTile (planes[selGridInt].tileDictionary,xValue , yValue +1);
				neighbors [2] = Tile.checkTile (planes[selGridInt].tileDictionary,xValue, yValue - 1);
				neighbors [3] = Tile.checkTile (planes[selGridInt].tileDictionary,xValue - 1, yValue);
				addPlanePossible = Tilt.tiltedTile(neighbors,hitPoint,planes[selGridInt],angle,tileWidth,out tempVector,out newPosition,out newRotation);
			}
			
			Handles.ArrowCap (GUIUtility.GetControlID (FocusType.Passive), newPosition, Quaternion.Inverse (newRotation), 10);
			Handles.DrawWireDisc(newPosition, newRotation*Vector3.up,tileWidth*0.5f);
			
			
			
			
			
			Event e = Event.current;
			
			
			//If you want to activate Keybord events you need to use FocousType.keybord instead of FocousType.passive
//			if(e.type == EventType.KeyDown){
//				Debug.Log ("KEY char : " + e.character + " : KEY CODE : " + e.keyCode );
//			}





			if ( (e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
			{                
				
				PlaneData selectedPlane = planes[selGridInt];
				if(addNewPlane && addPlanePossible ){
					
					
					
					GameObject temp = (GameObject)MonoBehaviour.Instantiate(source,newPosition,newRotation);
					Undo.RegisterCreatedObjectUndo (temp, "Created go");
					
					Plane addedPlaneTemp = new Plane(newRotation*Vector3.up,newPosition );
					
					
					int x = PlaneData.addPlane(addedPlaneTemp,newRotation,newPosition);
					
					
					addNewPlane = false;
					
					Tile tempTile = new Tile(xValue,yValue,temp);
					
					
					
					planes[x].tileDictionary.Add (new Tile.Key(xValue,yValue),tempTile);
					selGridInt = x;

				} else if(Tile.checkTile(selectedPlane.tileDictionary,xValue,yValue) == null){
							
					GameObject temp = (GameObject)MonoBehaviour.Instantiate(source,newPosition - newRotation*tempVector,newRotation);  /// here is the mistake,,, it doesn't map the position correctly
					
					Undo.RegisterCreatedObjectUndo (temp, "Created go");
					
					Tile tempTile = new Tile(xValue,yValue,temp);
					
					selectedPlane.tileDictionary.Add (new Tile.Key(xValue,yValue),tempTile);
					
							
					}
						
				}


			}

				

			

		
		
		
		
		
		
		HandleUtility.Repaint();


	}


	private static void OnScene(SceneView sceneview)

	{
		if(mode != null)
			mode();
	}





	public void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= OnScene;
	}




	void OnInspectorUpdate(){

		//must be deleted finally
		if(s == false){
			SceneView.onSceneGUIDelegate += OnScene;
			s = true;
			
		}
		this.Repaint ();
	}
}
