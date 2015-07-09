using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class CamScript : MonoBehaviour 
{
	//float 	camMinZoom 	= Screen.height/8, camMaxZoom = (27*Screen.height)/50,//--->(x/2)+(x/25)
	//private bool 	androidOS = false, winOS = false, reset = false;
	private float 	senseTime = 0.05f, speed = 0, prevMagnifyDist;
	private Vector3 mousePt1, mousePt2;
	private bool reset = false, magnification = false;

	public GameObject virtualCam;


	public float camMinZoom, camMaxZoom, minY, maxY, minX, maxX;
//	float 	camMinZoom 	= Screen.height/8, camMaxZoom = 0.49f*Screen.height,
//	minY = -0.35f*Screen.width, 		maxY = 0.35f*Screen.width,
//	minX = -Screen.width/2, 			maxX = Screen.width/2;
	
	private Vector3 clicked_mPos, curr_mPos, camPos, defaultCamPos;


	public static CamScript instance;

	private Touch touch1, touch2;


	void Start () 
	{//Debug.Log("SW: "+Screen.width+" SH: "+Screen.height);
		instance = gameObject.GetComponent<CamScript>();

		virtualCam = Instantiate(virtualCam, new Vector3(0, 0.5f*Screen.width, -0.5f*Screen.width), Quaternion.identity) as GameObject;

		GetComponent<Camera>().orthographicSize = 0.3f*Screen.height;
		GetComponent<Camera>().farClipPlane = Screen.width*2.5f;
		transform.position = virtualCam.transform.position;
		defaultCamPos = transform.position;


		camMinZoom = Screen.height/6;
		camMaxZoom = Screen.height/2;
//		camMinZoom = GameManager.mapHeight/6f;
//		camMaxZoom = (GameManager.mapHeight/Mathf.Sqrt(2f))/2f;

		//minY = -(GameManager.mapHeight/Mathf.Sqrt(2f))/2f;
		//maxY = (GameManager.mapHeight/Mathf.Sqrt(2f))/2f;
		//maxY = 0.75f*GameManager.mapHeight/2;
		minX = -GameManager.terrainHeight/2;
		maxX =  GameManager.terrainHeight/2;


		minX = defaultCamPos.x + minX;
		maxX = defaultCamPos.x + maxX;
		minY = defaultCamPos.y - (GameManager.terrainHeight/2 - (0.5f * Mathf.Sqrt(2.0f) * GetComponent<Camera>().orthographicSize));
		maxY = defaultCamPos.y + (GameManager.terrainHeight/2 - (0.5f * Mathf.Sqrt(2.0f) * GetComponent<Camera>().orthographicSize));

//		minX = camPos.x + minX;
//		maxX = camPos.x + maxX;
//		minY = camPos.y + minY;
//		maxY = camPos.y + maxY;


		camPos = transform.position;
	}	







	void Update()
	{
		if(senseTime > 0.0f) //------------------------------------> Sensor Time
			senseTime -= Time.deltaTime;
		
		//if(GameManager.gameStatus == GameStatus.VillageMode)
		{






			if(Input.touchCount == 2)
			{
				touch1 = Input.GetTouch(0);
				touch2 = Input.GetTouch(1);
				float magnifyDist = Vector3.Distance(touch1.position, touch2.position);
				
				if(!magnification)
				{
					magnification = true;
					prevMagnifyDist = Vector3.Distance(touch1.position, touch2.position);
				}
				
				if(senseTime <= 0)
				{
					senseTime = 0.05f;
					
					if(magnifyDist > prevMagnifyDist)
					{
						if(GetComponent<Camera>().orthographicSize+prevMagnifyDist-magnifyDist >= camMinZoom)
							GetComponent<Camera>().orthographicSize += prevMagnifyDist-magnifyDist;
						else
							GetComponent<Camera>().orthographicSize = camMinZoom;
						prevMagnifyDist = magnifyDist;
						
						AdjustCamPosition();
					}
					else
					{
						if(GetComponent<Camera>().orthographicSize+prevMagnifyDist-magnifyDist <= camMaxZoom)
							GetComponent<Camera>().orthographicSize += prevMagnifyDist-magnifyDist;
						else
							GetComponent<Camera>().orthographicSize = camMaxZoom-1;
						prevMagnifyDist = magnifyDist;
						
						AdjustCamPosition();
					}
				}
			}
			else
				magnification = false;







			if( Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				if(GetComponent<Camera>().orthographicSize-5 >= camMinZoom)
					GetComponent<Camera>().orthographicSize -= 5;
				else
					GetComponent<Camera>().orthographicSize = camMinZoom;

				minY = defaultCamPos.y - (GameManager.terrainHeight/2 - (0.5f * Mathf.Sqrt(2.0f) * GetComponent<Camera>().orthographicSize));
				maxY = defaultCamPos.y + (GameManager.terrainHeight/2 - (0.5f * Mathf.Sqrt(2.0f) * GetComponent<Camera>().orthographicSize));

				AdjustCamPosition();
			}
			if( Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				if(GetComponent<Camera>().orthographicSize+5 <= camMaxZoom)
					GetComponent<Camera>().orthographicSize += 5;
				else
					GetComponent<Camera>().orthographicSize = camMaxZoom;

				minY = defaultCamPos.y - (GameManager.terrainHeight/2 - (0.5f * Mathf.Sqrt(2.0f) * GetComponent<Camera>().orthographicSize));
				maxY = defaultCamPos.y + (GameManager.terrainHeight/2 - (0.5f * Mathf.Sqrt(2.0f) * GetComponent<Camera>().orthographicSize));
				
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
			
			if(Input.GetMouseButton(0) && Input.touchCount < 2)// && GameManager.buttonClicked == false && GameManager.panning)
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
		
		orthoY = orthoY / Mathf.Sqrt(2.0f);
		
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
			camPos.y = maxY - orthoY-0.1f;
			reset = true;
		}
		else if(camPos.y - orthoY < minY)
		{
			camPos.y = minY + orthoY+0.1f;
			reset = true;
		}
		
		transform.position = camPos;
	}


//	void AdjustCamPosition()
//	{
//		float orthoY = GetComponent<Camera>().orthographicSize, orthoX = (orthoY * Screen.width)/Screen.height;
//
//		orthoY = orthoY / Mathf.Sqrt(2.0f);
//
//		if(camPos.x + orthoX > maxX)
//		{
//			camPos.x = maxX - orthoX;
//			reset = true;
//		}
//		else if(camPos.x - orthoX < minX)
//		{
//			camPos.x = minX + orthoX;
//			reset = true;
//		}
//		if(camPos.y + orthoY > maxY)
//		{
//			camPos.y = maxY - orthoY;
//			reset = true;
//		}
//		else if(camPos.y - orthoY < minY)
//		{
//			camPos.y = minY + orthoY;
//			reset = true;
//		}
//
//		transform.position = camPos;
//	}
}
