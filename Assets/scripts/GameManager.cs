using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour {

	public GameObject player;
	//public GameObject playerPrefab;
	private GameCamera cam;
	private float minY = float.PositiveInfinity;
	private float maxY = -1.0f;
	private bool offsetSet = false;
	private float distancePlayerCamBottom = 0.0f;
	private float oldYOffset = 0.0f;

	public GameObject[] clouds;
	public GameObject[] prefab_levels;
	private GameObject[] levels;
	private int currentIndex = 0;

	private int[] indexToRemove;

	private float cloudSpawnTimer = 0.0f;
	private float cloudSpawnTime = 1.0f;

	private float first = 0.0f;

	public AudioClip cave;
	public AudioClip field;
	public AudioClip woods;
	public AudioClip darkwoods;
	public AudioClip boss;
	private float clipFadeOutTime = 0.2f;

	private bool musicChange = false;
	private string clipToChange = "";
	private float volumeDirection = -1.0f;
	private float volumeFade = 0.2f;

	private bool clipChanged = false;
	private bool decrease = false;

	private bool soundInit = false;

	void Start () 
	{
		//player = GameObject.Instantiate(playerPrefab);

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

	public void changeMusic(string area)
	{
		GameObject _boss = GameObject.FindGameObjectWithTag("boss1");

		AudioClip clip = null;
		if (_boss != null && area == "boss")
			clip = boss;
		else if (area == "field")
			clip = field;
		else if (area == "woods")
			clip = woods;
		else if (area == "darkwoods")
			clip = darkwoods;
		else if (area == "cave")
			clip = cave;


	    if (_boss != null && _boss.GetComponent<cEnemyBoss1>().bossActive)
			clip = boss;

		if (clip == null)
			return;

		if (GetComponent<AudioSource>().clip.name != clip.name)
		{
			musicChange = true;
			clipChanged = false;
			clipToChange = area;
			if (area == "darkwoods")
				volumeFade = 1.0f;
			else
				volumeFade = 0.6f;
		}
	}

	private void changeMusic2()
	{
		AudioClip clip = null;
		if (clipToChange == "boss")
			clip = boss;
		else if (clipToChange == "field")
			clip = field;
		else if (clipToChange == "woods")
			clip = woods;
		else if (clipToChange == "darkwoods")
			clip = darkwoods;
		else if (clipToChange == "cave")
			clip = cave;
		GameObject _boss = GameObject.FindGameObjectWithTag("boss1");
		if (_boss != null && _boss.GetComponent<cEnemyBoss1>().bossActive)
			clip = boss;

		if (clip == null)
			return;
		
		GetComponent<AudioSource>().volume += Time.deltaTime/volumeFade * volumeDirection;
		if (GetComponent<AudioSource>().volume >= 1.0f)
		{
			musicChange = false;
			volumeDirection = -1.0f;
			clipToChange = "";
			clipChanged = true;
			return;
		}
		if (GetComponent<AudioSource>().volume <= 0.0f)
		{
			GetComponent<AudioSource>().clip = clip;
			GetComponent<AudioSource>().Play();
			volumeDirection = 1.0f;
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

		moveClouds(p2);
		if (musicChange)
			changeMusic2 ();

		if (!soundInit)
		{
			AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
			if (audioSource)
			{
				if (audioSource.volume < 1.0f)
					audioSource.volume += Time.deltaTime/0.4f;

				if (audioSource.volume >= 1.0f)
					soundInit = true;
			}
		}

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

	private void moveClouds(Vector3 right_corner)
	{
		if (cloudSpawnTime <= cloudSpawnTimer)
		{
			for (int i=0; i< clouds.Length; i++)
			{
				GameObject go = GameObject.Instantiate(clouds[i]);
				Vector3 cloudPosition = clouds[i].transform.position;
				cloudPosition.x += first * 150.0f;
				go.transform.position = cloudPosition;

			}

			cloudSpawnTimer = 0.0f;
			first = 1.0f;
			cloudSpawnTime = Random.Range (60.0f,70.0f);
		}
		cloudSpawnTimer += Time.deltaTime;
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
