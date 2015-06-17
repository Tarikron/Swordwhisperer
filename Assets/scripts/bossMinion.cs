using UnityEngine;
using System.Collections;

public class bossMinion : cEnemy {

	[HideInInspector]
	public float alpha = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void msg_shot()
	{
		GameObject player = GameObject.Find("Player");
		Vector3 playerPos = player.gameObject.transform.position;
		Vector3 enemyPos = transform.position;
		float distance = Vector3.Distance(enemyPos,playerPos);
		if (distance <= attackDistance)
			attackShot(player,playerPos);
	}
}
