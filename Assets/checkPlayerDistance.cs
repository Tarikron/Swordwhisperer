using UnityEngine;
using System.Collections;

public class checkPlayerDistance : MonoBehaviour {

	public float triggerDistance = 2.0f;
	public bool endPoint = false;
	public string clipname = "";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.FindGameObjectWithTag("player");
		Vector3 playerPos = player.transform.position;

		float xLeft = transform.position.x + GetComponent<BoxCollider2D>().offset.x - GetComponent<BoxCollider2D>().size.x/2;
		float xRight = xLeft + GetComponent<BoxCollider2D>().size.x;
		
		bool case1 = playerPos.x <= xRight &&  playerPos.x >= xLeft;
		if (case1)
		{
			Camera mainCam = Camera.main;
			mainCam.gameObject.GetComponent<GameManager>().changeMusic(clipname);
		}
	}
}
