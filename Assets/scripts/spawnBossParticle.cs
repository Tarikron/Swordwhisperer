using UnityEngine;
using System.Collections;

public class spawnBossParticle : MonoBehaviour 
{

	public int spawnMinions = 8;
	public float spawnInterval = 0.3f;
	public float bigInterval = 0.0f;
	private float bigTimer = 0.0f;

	public GameObject minionToSpawn = null;
	private float spawnTimer = 0.3f;
	private int currentSpawn = 0;

	private float startSize = 2.0f;
	private float startSizeTime = 3.0f;
	private int startSizeCounter = 0;

	enum eSpawnSteps {SPAWN_NONE = 0 , SPAWN_START = 1, SPAWN_MINIONS = 2, SPAWN_END = 3, SPAWN_DONE = 4};
	private eSpawnSteps iState = eSpawnSteps.SPAWN_NONE;

	private int randomRoute = 1;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{

		switch (iState)
		{
			case eSpawnSteps.SPAWN_DONE:
			{
				if (bigInterval <= bigTimer)
				{
					bigTimer = 0.0f;
					GameObject go = GameObject.FindGameObjectWithTag("boss1");

					if (go)
						iState = eSpawnSteps.SPAWN_START;
				}
				bigTimer += Time.deltaTime;
				break;
			}
			case eSpawnSteps.SPAWN_START:
			{
				ParticleSystem[] systems = gameObject.GetComponentsInChildren<ParticleSystem>();
				do
				{
					for (int i = 0; i < systems.Length; i++)
					{
						ParticleSystem ps = systems[i];
						if (ps)
						{
							if (!ps.enableEmission)
							{
								ps.enableEmission = true;
								ps.Play();
								randomRoute = Random.Range(1,3);
							}
						//startSizeTime = startSize
						//Time.deltaTime = x
							if (ps.startSize <= 2.0f)
								ps.startSize += (startSize * Time.deltaTime)/startSizeTime;

							if (ps.startSize > 2.0f)
								startSizeCounter++;
							
						}
					}
					if (systems.Length-1 <= startSizeCounter)
					{
						iState = eSpawnSteps.SPAWN_MINIONS;		
						startSizeCounter=0;
					}
				} while (iState == eSpawnSteps.SPAWN_START);
				break;
			}
			case eSpawnSteps.SPAWN_MINIONS:
			{
				if (spawnTimer >= spawnInterval)
				{
					if (currentSpawn < spawnMinions)
					{
						currentSpawn++;
						//spawn minions here
						GameObject go = GameObject.Instantiate(minionToSpawn);
						go.transform.position = transform.position;
						SplineController spline = go.GetComponent<SplineController>();

						spline.SplineRoot = GameObject.Find ("splineBossWaypoints"+randomRoute);
						spline.Init();
					}
					spawnTimer = 0.0f;
				}
				if (currentSpawn >= spawnMinions)
				{
					iState = eSpawnSteps.SPAWN_END;
					currentSpawn = 0;
				}
				spawnTimer += Time.deltaTime;
				break;
			}
			case eSpawnSteps.SPAWN_END:
			{
				ParticleSystem[] systems = gameObject.GetComponentsInChildren<ParticleSystem>();
				do
				{
					for (int i = 0; i < systems.Length; i++)
					{
						ParticleSystem ps = systems[i];
						if (ps)
						{
							if (!ps.enableEmission)
							{
								ps.enableEmission = true;
								ps.Play();
							}
							//startSizeTime = startSize
							//Time.deltaTime = x
							if (ps.startSize >= 0.0f)
								ps.startSize -= (startSize * Time.deltaTime)/startSizeTime;
							
							if (ps.startSize < 0.0f)
							{
								startSizeCounter++;
								ps.startSize = 0.0f;
							}
						}
					}
					if (systems.Length-1 <= startSizeCounter)
					{
						iState = eSpawnSteps.SPAWN_DONE;		
						startSizeCounter=0;
					}
				} while (iState == eSpawnSteps.SPAWN_END);
				break;
			}
		}

	}

	void msg_spawnNow()
	{
		if (iState == eSpawnSteps.SPAWN_NONE)
			iState = eSpawnSteps.SPAWN_START;
	}

}
