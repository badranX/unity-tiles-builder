using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lib;
namespace Lib {
public static class Construct {



		
		public static GameObject source;
		
		private static Vector3 posM;
		private static int xValue,yValue;


		private static float tileHeight;

		private static float tileWidth;
		private static PlaneData plane;
		private static int index=0;
		private static bool addNewPlane = false;

		private static float angle;

		public static void addPlane (float angle){

			addNewPlane = true;
			Construct.angle = angle;
		}

		public static void setGameObject (GameObject source){
			

			Construct.source = source;

			Mesh mesh = source.GetComponent<MeshFilter>().sharedMesh;
			tileWidth = mesh.bounds.size.x;
			tileHeight = mesh.bounds.size.z;
		}

		public static void setPlane (PlaneData plane){
			Construct.plane = plane;
		}

		public static int getPlaneIndex (){
			return Construct.index;
		}


	public static void construct(){
		int id =GUIUtility.GetControlID (FocusType.Keyboard);
		
		
		
		
			
		HandleUtility.AddDefaultControl (id);
		Tools.current = Tool.None;
		
		
		
		
		
		
		posM = Event.current.mousePosition;	
		
		posM = new Vector3 (posM.x, Camera.current.pixelHeight -posM.y, 0);
		Ray ray = Camera.current.ScreenPointToRay(posM);
		
		Vector3 hitPoint;
		float dist;
			Quaternion currentPlaneRotation =plane.q  ;
			Plane currentPlane =  plane.p;
		
		if (currentPlane.Raycast (ray, out dist)) {
			hitPoint = ray.GetPoint (dist);
			
				Vector3 dv= hitPoint - plane.origin;
			Vector3 Px = plane.q* Vector3.right;
				Vector3 Py = plane.q *Vector3.forward;
			float dx = Vector3.Dot (dv,Px);
			float dy = Vector3.Dot(dv,Py);
			dx= Mathf.Round(dx/tileWidth) *tileWidth ;
			dy = Mathf.Round(dy/tileHeight)*tileHeight;
			hitPoint = dy * Py + dx*Px + plane.origin;
			
			xValue = (int)dx;
			yValue = (int)dy;
			
			
			
			
			
			Handles.color = Color.green;
			
			
			
			
			Handles.color = Color.blue;
			
			Quaternion newRotation = currentPlaneRotation;
			Vector3 newPosition = hitPoint;
			Vector3 tempVector = Vector3.zero;
			
			
				bool canAddNewPlane = false ;
			if (addNewPlane){
				Tile [] neighbors = new Tile[4];
				neighbors [0] = Tile.checkTile (plane.tileDictionary,xValue + 1, yValue);
				neighbors [1] = Tile.checkTile (plane.tileDictionary,xValue , yValue +1);
				neighbors [2] = Tile.checkTile (plane.tileDictionary,xValue, yValue - 1);
				neighbors [3] = Tile.checkTile (plane.tileDictionary,xValue - 1, yValue);
				canAddNewPlane = Tilt.tiltedTile(neighbors,hitPoint,plane,angle,tileWidth,out tempVector,out newPosition,out newRotation);
			}
			
			Handles.ArrowCap (GUIUtility.GetControlID (FocusType.Passive), newPosition, Quaternion.Inverse (newRotation), 10);
			Handles.DrawWireDisc(newPosition, newRotation*Vector3.up,tileWidth*0.5f);
			
			
			
			
			
			Event e = Event.current;
			
			
			
			if(e.type == EventType.KeyDown){
				Debug.Log ("KEY char : " + e.character + " : KEY CODE : " + e.keyCode );
			}
			
			if ( (e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
			{                
				
				PlaneData selectedPlane = plane;
				
				if(Tile.checkTile(selectedPlane.tileDictionary,xValue,yValue) == null){
					
						
						GameObject temp = (GameObject)MonoBehaviour.Instantiate(source,newPosition - newRotation*tempVector,newRotation);  /// here is the mistake,,, it doesn't map the position correctly
						
						Undo.RegisterCreatedObjectUndo (temp, "Created go");
						
						Tile tempTile = new Tile(xValue,yValue,temp);
						
						
						
						selectedPlane.tileDictionary.Add (new Tile.Key(xValue,yValue),tempTile);
						
					
					
				}
				


						if(canAddNewPlane){
							
							GameObject temp = (GameObject)MonoBehaviour.Instantiate(source,newPosition,newRotation);
							Undo.RegisterCreatedObjectUndo (temp, "Created go");
							
							Plane addedPlaneTemp = new Plane(newRotation*Vector3.up,newPosition );
							
							
							int index = PlaneData.addPlane(addedPlaneTemp,newRotation,newPosition);
							
							
							
							
							Tile tempTile = new Tile(xValue,yValue,temp);
							
							
							
							PlaneData.planes[index].tileDictionary.Add (new Tile.Key(xValue,yValue),tempTile);
							plane = PlaneData.planes[index];
							
							addNewPlane = false;
						}
						
						
					
			}
			
		}
		
		
		
		
		
		
		HandleUtility.Repaint();
		
		}


		public static void edit() {


		}
	}




}
