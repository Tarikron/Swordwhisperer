using UnityEngine;
using System.Collections;

public class cCloudMenuMgr : MonoBehaviour {
	
	public GameObject[] clouds;

	private float cloudTimer = 0.0f;
	private float cloudTime = 12.0f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (cloudTime <= cloudTimer)
		{
			for (int i=0; i < clouds.Length; i++)
			{
				GameObject go = GameObject.Instantiate(clouds[i]);
				Vector3 v = go.transform.position;
				v.x += 150.0f;
				v.y += Random.Range (-3.0f,3.0f);
				go.transform.position = v;
			}
			cloudTimer = 0.0f;
			cloudTime = 64.0f;
		}

		cloudTimer += Time.deltaTime;
	}
}
