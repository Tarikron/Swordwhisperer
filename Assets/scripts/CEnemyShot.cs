﻿using UnityEngine;
using System.Collections;

public class CEnemyShot : MonoBehaviour {

	public float shotSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.GetComponent<BoxCollider2D>().enabled == true)
		{
			Vector3 vec3 = this.gameObject.transform.localPosition;
			vec3 = Time.deltaTime * shotSpeed * this.transform.up;
			this.gameObject.transform.localPosition -= vec3;
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag != "enemyShot" && collision.gameObject.tag != "enemyFlying" )
		{
			if (collision.gameObject.tag == "player")
			{
				collision.gameObject.SendMessage("msg_hit",null,SendMessageOptions.RequireReceiver);
			}
			//Destroy(this.gameObject);
		}
	}
}
