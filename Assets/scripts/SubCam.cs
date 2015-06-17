using UnityEngine;
using System.Collections;

public class SubCam : MonoBehaviour 
{

	public Camera cam;

	public static SubCam instance;

	void Awake()
	{
		instance = gameObject.GetComponent<SubCam>();
		gameObject.SetActive(false);
	}

	public Vector3 GetMousePosition()
	{
		float orthoSize = Camera.main.orthographicSize;
		//cam.orthographicSize = Mathf.Sqrt(2) * orthoSize;
		cam.orthographicSize = orthoSize;

		float x = CamScript.instance.transform.position.x;
		float z = (CamScript.instance.transform.position.y*GameManager.mapHeight)/CamScript.instance.maxY;

		//transform.position = new Vector3(x, transform.position.y, z);

		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.y=0;

		return mousePos;
	}
}
