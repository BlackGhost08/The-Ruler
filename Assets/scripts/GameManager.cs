using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum GameMode
{
	VillageMode,
	Inventory,
	BattleMode,
}

public enum MouseAction
{
	None,
	MovingBiulding,
	Panning,
	UsingHiker,
}

public class GameManager : MonoBehaviour
{
	public GameObject Map, tileObj, tilesBag, testobj;

	public Camera subCam;
	
	public Vector3 mouse1, mouse2;
	public int mapSize;
	public MouseAction mouse_action;

	//                                      _______________________
	/*-----------------------------------   Globally used variables -----------------------------------*/
	//                                      ***********************
	public static float terrainWidth = Screen.width, terrainHeight = Screen.width, unitWidth, unitHeight;
	public static float mapWidth = (87.5f/100)*terrainWidth, mapHeight = (87.5f/100)*terrainHeight;
	public static GameObject[,] tile;
	public static GameManager instance; //----> To access GameManager

	public static bool buttonClicked = false, panning = false;
	public static GameMode gamemode;
	//==================================================================================================\\





	 
	private Ray ray 		= new Ray();
	private RaycastHit hit 	= new RaycastHit();




	void Awake()
	{
		tile = new GameObject[2*mapSize, 2*mapSize];		
		instance = gameObject.GetComponent<GameManager>();
	}

	void Start()
	{
		gamemode = GameMode.VillageMode;
		mouse_action = MouseAction.None;

		GameObject m = Instantiate(Map, new Vector3(0, -0.5f*Screen.width, 0.5f*Screen.width), Quaternion.Euler(90,0,0))as GameObject;
		m.transform.localScale = new Vector3(terrainWidth, terrainHeight, 1);//----> x-(x/4)


		//Size of the map is 100%. 87.5% space can be used to place buildings
		/*  Width and Height of single tile building  */
		unitWidth = mapWidth / (mapSize * 2);  //->GLOBAL
		unitHeight = mapHeight / (mapSize * 2);//  VARIABLES
		//==================================================================\\


		//                                      ________________
		//-----------------------------------   Tiles drawn here    -----------------------------------\\
		//                                      ****************
		Vector3 beginPos = Vector3.zero, nextPos;
		beginPos.z = -(unitHeight / 2) * ((2 * mapSize) - 1);

		for (int i=0; i<(2*mapSize); i++)
		{
			nextPos = beginPos;
			
			for (int j=0; j<(2*mapSize); j++)
			{
				//gridPos.Add(nextPos);
				//GameObject objj = Instantiate(testobj, nextPos, Quaternion.identity)as GameObject;
				GameObject obj = Instantiate(tileObj, nextPos, Quaternion.identity) as GameObject;
				tile[i,j] = obj;
				obj.transform.parent = tilesBag.transform;
				obj.name = i + "," + j;
				//obj.name = tileno.ToString();
				nextPos.x += unitWidth / 2;
				nextPos.z += unitHeight / 2;
			}
			beginPos.x -= unitWidth / 2;
			beginPos.z += unitHeight / 2;
		}
		//=============================================================================================\\

//		testObj2 = Instantiate(testObj2, Vector3.zero, Quaternion.Euler(90,0,0))as GameObject;
//		testObj2.transform.localScale = new Vector3(unitWidth, unitHeight, 1);
	}


	void Update()
	{
//		if (Input.GetMouseButton(0))
//		{
//
//			ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//
//			if (Physics.Raycast (ray, out hit, 2000))
//			{
//				testObj2.transform.position = hit.point;
//			}
//		}
	}

	public int[] GetNearestPoint(Vector3 point1)
	{
		point1.y = 0;
		float distance = 1000;
		int[] location = new int[2];
		int length = (2 * mapSize);
		for (int i=0; i<length; i++)
		{
			for (int j=0; j<length; j++)
			{
				Vector3 point2 = tile [i,j].transform.position;
				if (Vector3.Distance(point1, point2) < distance)
				{
					distance = Vector3.Distance(point1, point2);
					location[0] = i;
					location[1] = j;
				}
			}
		}
		//        if (selectedBuilding != null)
		//            selectedBuilding.GetComponent<Building>().movedTile = tile_no;
		
		return location;
	}
}