using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class CamScript : MonoBehaviour 
{
	//float 	camMinZoom 	= Screen.height/8, camMaxZoom = (27*Screen.height)/50,//--->(x/2)+(x/25)
	//private bool 	androidOS = false, winOS = false, reset = false;
	private float 	senseTime = 0.05f, speed = 0;
	private Vector3 mousePt1, mousePt2;
	private bool reset = false;

	public GameObject virtualCam;


	public float camMinZoom, camMaxZoom, minY, maxY, minX, maxX;
//	float 	camMinZoom 	= Screen.height/8, camMaxZoom = 0.49f*Screen.height,
//	minY = -0.35f*Screen.width, 		maxY = 0.35f*Screen.width,
//	minX = -Screen.width/2, 			maxX = Screen.width/2;
	
	private Vector3 clicked_mPos, curr_mPos, camPos;


	public static CamScript instance;


	void Start () 
	{
		instance = gameObject.GetComponent<CamScript>();

		virtualCam = Instantiate(virtualCam, new Vector3(0, 0.5f*Screen.width, -0.5f*Screen.width), Quaternion.identity) as GameObject;

		GetComponent<Camera>().orthographicSize = 0.49f*Screen.height;
		GetComponent<Camera>().farClipPlane = Screen.width*2.5f;
		transform.position = virtualCam.transform.position;
		camPos = transform.position;

		camMinZoom = GameManager.mapHeight/8;
		camMaxZoom = Screen.height/2;

		minY = -0.75f*GameManager.mapHeight/2;
		maxY = 0.75f*GameManager.mapHeight/2;
		minX = -GameManager.mapWidth/2;
		maxX =  GameManager.mapWidth/2;

		minX = camPos.x + minX;
		maxX = camPos.x + maxX;
		minY = camPos.y + minY;
		maxY = camPos.y + maxY;
	}	







	void Update()
	{
		if(senseTime > 0.0f) //------------------------------------> Sensor Time
			senseTime -= Time.deltaTime;
		
		//if(GameManager.gameStatus == GameStatus.VillageMode)
		{
			if( Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				if(GetComponent<Camera>().orthographicSize-5 >= camMinZoom)
					GetComponent<Camera>().orthographicSize -= 5;
				else
					GetComponent<Camera>().orthographicSize = camMinZoom;
				
				AdjustCamPosition();
			}
			if( Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				if(GetComponent<Camera>().orthographicSize+5 <= camMaxZoom)
					GetComponent<Camera>().orthographicSize += 5;
				else
					GetComponent<Camera>().orthographicSize = camMaxZoom;
				
				AdjustCamPosition();
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				speed = 0;
				
				clicked_mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				mousePt2 = clicked_mPos;
			}
			
			if(reset)
			{
				clicked_mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				reset = false;
			}
			
			if(Input.GetMouseButton(0))// && GameManager.buttonClicked == false && GameManager.panning)
			{
				camPos = transform.position;
				curr_mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				
				camPos.x += clicked_mPos.x - curr_mPos.x;
				camPos.y += clicked_mPos.y - curr_mPos.y;
				
				if(senseTime <= 0)
				{
					mousePt1 = mousePt2;
					mousePt2 = curr_mPos;
					senseTime = 0.05f;
				}
				
				AdjustCamPosition();
			}
			else if(GameManager.buttonClicked && GameManager.panning)
			{
				mousePt2 = mousePt1;
			}
			
			if(Input.GetMouseButtonUp(0))// && GameManager.panning)
			{
				virtualCam.transform.position = camPos;
				
				float 	time 	 = 0.05f,
				distance = Mathf.Sqrt(Mathf.Pow(mousePt2.x-mousePt1.x,2) + Mathf.Pow(mousePt2.y-mousePt1.y,2));
				
				speed = distance/time;
				
				//Debug.Log (speed);
				
				if(speed > 0 )
				{
					float rot = Mathf.Atan2(mousePt1.y-mousePt2.y, mousePt1.x-mousePt2.x) * (180/Mathf.PI);
					virtualCam.transform.rotation = Quaternion.Euler(0, 0, rot);
				}
				
				GameManager.panning = false;
			}
			
			if(speed > 0)
			{
				virtualCam.transform.Translate(Vector3.right * speed * Time.deltaTime);
				speed -= 10;
				
				camPos = virtualCam.transform.position;
				AdjustCamPosition();
			}
		}
	}
	
	void AdjustCamPosition()
	{
		float orthoY = GetComponent<Camera>().orthographicSize, orthoX = (orthoY * Screen.width)/Screen.height;

		if(camPos.x + orthoX > maxX)
		{
			camPos.x = maxX - orthoX;
			reset = true;
		}
		else if(camPos.x - orthoX < minX)
		{
			camPos.x = minX + orthoX;
			reset = true;
		}
		if(camPos.y + orthoY > maxY)
		{
			camPos.y = maxY - orthoY;
			reset = true;
		}
		else if(camPos.y - orthoY < minY)
		{
			camPos.y = minY + orthoY;
			reset = true;
		}

		transform.position = camPos;
	}
}
