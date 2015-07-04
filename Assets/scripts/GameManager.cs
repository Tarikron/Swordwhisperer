using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {

	public GameObject player;
	private GameCamera cam;
	private float minY = float.PositiveInfinity;
	private float maxY = -1.0f;
	private bool offsetSet = false;
	private float distancePlayerCamBottom = 0.0f;
	private float oldYOffset = 0.0f;

	public GameObject[] prefab_levels;
	private GameObject[] levels;
	private int currentIndex = 0;

	private int[] indexToRemove;

	void Start () 
	{
		cam = GetComponent<GameCamera>();
		cPlayer_c cPlayer = player.GetComponent<cPlayer_c>();

		cam.SetTrackSpeed(cPlayer.accelerationX,cPlayer.accelerationY);
		cam.SetTarget(player.transform);
		cam.SetOffset(new Vector3(0.0f,player.transform.position.y*-1.0f ,0.0f));
		oldYOffset = player.transform.position.y*-1.0f;
		offsetSet = false;
		
		Camera c = GetComponent<Camera>();
		Vector3 p2 = c.ViewportToWorldPoint(new Vector3(0, 0, Mathf.Abs(transform.position.z)));
		distancePlayerCamBottom = Mathf.Abs (p2.y) - Mathf.Abs (player.transform.position.y);


		//load all levels

		Vector3 p = c.ViewportToWorldPoint(new Vector3(1, 1, Mathf.Abs(transform.position.z)));
		levels = new GameObject[32];
		indexToRemove = new int[32];

		for (int i =0; i < prefab_levels.Length; i++)
		{
			GameObject prefab = prefab_levels[i];


			float xLeft = prefab.transform.position.x + prefab.GetComponent<BoxCollider2D>().offset.x - prefab.GetComponent<BoxCollider2D>().size.x/2;
			float xRight = xLeft + prefab.GetComponent<BoxCollider2D>().size.x;

			bool case1 = p.x >= xRight &&  p2.x <= xRight || p.x >= xLeft && p2.x <= xLeft;
			bool case2 = xLeft <= p2.x && xRight >= p2.x && xLeft <= p.x && xRight >= p.x;

			indexToRemove[i] = 1;
			if (cFunction.xor(case1,case2))
			{
				GameObject go = GameObject.Instantiate(prefab);
				levels[i] = go;
				indexToRemove[i] = -1;
			}
		}

	}
	
	void Update()
	{
		//handle cam for player, if player is in air
		cPlayer_c cPlayer = player.GetComponent<cPlayer_c>();
		Vector2 currentSpeed = cPlayer.GetCurrentSpeed();
		Camera c = GetComponent<Camera>();
		Vector3 p = c.ViewportToWorldPoint(new Vector3(1, 1, Mathf.Abs(transform.position.z)));
		Vector3 p2 = c.ViewportToWorldPoint(new Vector3(0, 0, Mathf.Abs(transform.position.z)));

		for (int i =0; i < prefab_levels.Length; i++)
		{
			GameObject prefab = prefab_levels[i];
			
			
			float xLeft = prefab.transform.position.x + prefab.GetComponent<BoxCollider2D>().offset.x - prefab.GetComponent<BoxCollider2D>().size.x/2;
			float xRight = xLeft + prefab.GetComponent<BoxCollider2D>().size.x;
			
			bool case1 = p.x >= xRight &&  p2.x <= xRight || p.x >= xLeft && p2.x <= xLeft;
			bool case2 = xLeft <= p2.x && xRight >= p2.x && xLeft <= p.x && xRight >= p.x;

			if (cFunction.xor(case1,case2))
			{
				if (indexToRemove[i] == 1)
				{
					GameObject go = GameObject.Instantiate(prefab);
					levels[i] = go;
					indexToRemove[i] = -1;
				}
			}
			else
				indexToRemove[i] = 1;
		}
		for (int i = 0; i < indexToRemove.Length; i++)
		{
			if (indexToRemove[i] == 1)
			{
				GameObject go = levels[i];
				if (go != null)
				{
					Destroy (go);
					levels[i] = null;
				}
			}
		}


		//Get max cam height
		//first calculate how height we will jump(physics), then take current player y position  + current top y position
		if (currentSpeed.y > 0.0f && maxY < 0.0f)
		{
			float height = player.GetComponent<MeshRenderer>().bounds.size.y;
			maxY = GetDistanceWithVelocityZero(cPlayer)+Mathf.Abs (player.transform.position.y+height) + Mathf.Abs (p.y);
			minY = p2.y;
		}
		else if (currentSpeed.y < 0.0f)
		{
			float distance = GetDistanceToGround(cPlayer);
			if (minY != float.PositiveInfinity && distance != float.PositiveInfinity)
			{
				minY = player.transform.position.y - (distance + distancePlayerCamBottom);
			}

			if (cam.stopCam)
				cam.SetOffset(new Vector3(0.0f,0.0f,0.0f));
			if (offsetSet == false)
			{
				maxY = -1.0f;
				cam.stopCam = false;
			}
			if (offsetSet && cPlayer.playerPhysics.grounded && minY == float.PositiveInfinity)
			{
				cam.SetTarget(player.transform);
				cam.SetOffset(new Vector3(0.0f,oldYOffset ,0.0f));
				offsetSet = false;
				cam.stopCam = false;
			}

			if (minY != float.PositiveInfinity && p2.y <= minY)
			{
				cam.stopCam = true;
				minY = float.PositiveInfinity;
				offsetSet = true;
			}
		}
		if (maxY > 0.0f && p.y >= maxY)
			cam.stopCam = true;


		//dialog
		//if position for event reached 
		// then set eventTrigger for canvas gameobject (send message)
	}

	private void SpawnPlayer()
	{
		cam.SetTarget((Instantiate(player, Vector3.zero,Quaternion.identity) as GameObject).transform);
	}

	private float GetDistanceWithVelocityZero(cPlayer_c cPlayer)
	{
		float v = cPlayer.jumpHeight/cPlayer.jumpTime;
		return ((-cPlayer.accelerationY/2)* (Mathf.Pow (-v/-cPlayer.accelerationY,2)))+(v*(Mathf.Pow (-v/-cPlayer.accelerationY,2)))+(player.transform.position.y);
	}

	private float GetDistanceToGround(cPlayer_c cPlayer)
	{
		return cPlayer.playerPhysics.GetDistanceToGround();
	}
}
