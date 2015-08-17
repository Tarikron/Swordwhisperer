using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bossWayPoint : MonoBehaviour 
{

	public List<GameObject> unityShitWorkaround = null;

	// Use this for initialization
	void Start () 
	{
		if (unityShitWorkaround == null)
			unityShitWorkaround = new List<GameObject>();

		Transform[] t = gameObject.GetComponentsInChildren<Transform>();
		for (int i=0; i < t.Length; i++)
		{
			Transform child = t[i];
			if (child.CompareTag("waypoint"))
			{
				unityShitWorkaround.Add (child.gameObject);
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
