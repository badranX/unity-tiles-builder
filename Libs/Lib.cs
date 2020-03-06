using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lib {


public class Tilt  {



		static private void tilt (Tile neighbor,Vector3 hitPoint,PlaneData plane,float angle,float tileWidth, out Vector3 tempVector, out Vector3 newPosition,out Quaternion newRotation){
			Vector3 neighborTile = neighbor.gameObject.transform.position;
			Vector3 newTileDirection = -1*(neighborTile - hitPoint).normalized;
			Vector3 rotator = Vector3.Cross(newTileDirection,plane.p.normal);
			Quaternion tempRotation = Quaternion.AngleAxis (angle, rotator);
			newRotation =tempRotation* plane.q;
			
			tempVector =  newTileDirection*  tileWidth*0.5f;
			
			newPosition = tempRotation*tempVector + neighborTile+tempVector;
		}
		static public bool tiltedTile (Tile [] neighbors,Vector3 hitPoint,PlaneData plane,float angle,float tileWidth, out Vector3 tempVector, out Vector3 newPosition,out Quaternion newRotation){
			newRotation = plane.q;
			newPosition = hitPoint;
			tempVector = Vector3.zero;
			if (neighbors [0] != null && neighbors [1] == null && neighbors [2] == null && neighbors [3] == null) {
				
				
				tilt (neighbors[0],hitPoint,plane,angle,tileWidth,out tempVector,out newPosition,out newRotation);
				
			} else if (neighbors [0] == null && neighbors [1] != null && neighbors [2] == null && neighbors [3] == null) { 
				tilt (neighbors[1],hitPoint,plane,angle,tileWidth,out tempVector,out newPosition,out newRotation);
				
				
				
			} else if (neighbors [0] == null && neighbors [1] == null && neighbors [2] != null && neighbors [3] == null) {
				tilt (neighbors[2],hitPoint,plane,angle,tileWidth,out tempVector,out newPosition,out newRotation);
				
			} else if (neighbors [0] == null && neighbors [1] == null && neighbors [2] == null && neighbors [3] != null) {
				tilt (neighbors[3],hitPoint,plane,angle,tileWidth,out tempVector,out newPosition,out newRotation);
				
				
			}else return false;
			
			return true;
		}
	
}


	 public class Tile {
		public Tile(int x, int z ,GameObject gameObject){
			this.x = x;
			this.z = z;
			this.gameObject = gameObject;
			this.width = 1;
			this.height = 1;


		}
		public Tile(int x, int z ,float height,float width,GameObject gameObject){
			this.x = x;
			this.z = z;
			this.gameObject = gameObject;
			this.height = height;	
			this.width = width;
			
		}
		public float height;
		public float width;
		public int x;
		public int z;
		public GameObject gameObject; 

		public struct Key {
			public Key(int x,int y){
				this.x = x;
				this.y = y;
			}
			public int x;
			public int y;
		}

		public static Dictionary<Key,Tile> dictionary = new Dictionary<Key,Tile >();

		public static Tile checkTile(Dictionary<Key,Tile> dictionary,int x, int z){

			
			if (dictionary.ContainsKey (new Key (x, z))) {
				
				return dictionary [new Key (x, z)];
			} else {
				return null;
			}
		}


	}



	public class PlaneData{
		public PlaneData(Plane p,Quaternion q,Vector3 origin){
			this.p = p;
			this.q = q;
			this.origin = origin;
			this.tileDictionary= new Dictionary<Tile.Key,Tile >();
			
		}
		static public int addPlane(Plane p,Quaternion q,Vector3 originShift){
			
			planes.Add (new PlaneData (p, q,originShift));

			return planes.Count -1;
		}

		static public string [] planesNames () {
			List <string> temp = new List<string>();

			foreach(PlaneData p in planes ){
				Vector3 vectTemp ;
				float angTemp  ;
				p.q.ToAngleAxis(out angTemp, out vectTemp);
				temp.Add (vectTemp.ToString () + " : " + angTemp.ToString ());
			}
			return temp.ToArray();
		}
		public Vector3 origin ;
		public Plane p;
		public Quaternion q;
		public Dictionary<Tile.Key,Tile> tileDictionary;


		public static List<PlaneData> planes = new List<PlaneData>();

		public static void removePlane(int id){
			planes.RemoveAt (id);
		}


	}


}