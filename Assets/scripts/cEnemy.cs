using System;
using UnityEngine;

public class cEnemy : cUnit
{
	public float shotInterval = 2.0f;
	protected float intervalTimer = 0.0f;
	protected Vector3 lastPlayerPos = Vector3.zero;
	public float attackDistance = 2.0f;


	protected void attackShot(GameObject player, Vector3 target)
	{
		Vector3 enemyPos = transform.position;
		Vector3 heading = target - enemyPos;

		Transform t = transform.GetChild(0);
		GameObject shot = t.gameObject;
		
		intervalTimer += Time.deltaTime;
		
		if (intervalTimer >= shotInterval)
		{
			intervalTimer = 0.0f;
			//we are in attack range
			lastPlayerPos = target;
			lastPlayerPos.y += 0.5f;
			shot.transform.position = transform.position + (heading.normalized * 2);
			
			GameObject shotClone = GameObject.Instantiate(shot);
			shotClone.SendMessage("msg_shotfired",heading.normalized,SendMessageOptions.RequireReceiver);
			shotClone.transform.localScale = shot.transform.lossyScale;
			shotClone.transform.position = shot.transform.position;
			//shotClone.transform.lossyScale = shot.transform.localScale;
			shotClone.GetComponent<SpriteRenderer>().enabled = true;
			shotClone.GetComponent<BoxCollider2D>().enabled = true;
			
			
		}
	}
}


