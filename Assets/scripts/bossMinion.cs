using UnityEngine;
using System.Collections;

public class bossMinion : cEnemy {

	[HideInInspector]
	public float alpha = 0.0f;
	[HideInInspector]
	public float radius = -1.0f;

	private float minionAlpha = 0.0f;

	// Use this for initialization
	void Start () 
	{
		radius = 0.0f;
		minionAlpha = Random.Range (0.0f,360.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		minionAlpha += 10.0f;
		
		if (minionAlpha > 360.0f)
			minionAlpha = 0.0f;
		if (minionAlpha < 0.0f)
			minionAlpha = 360.0f;

		radius = Mathf.Sin( minionAlpha * Mathf.PI/180)/0.5f;

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
