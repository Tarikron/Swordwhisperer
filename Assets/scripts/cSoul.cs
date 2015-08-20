﻿using UnityEngine;
using System.Collections;

public class cSoul : MonoBehaviour {

	public float angleSpeed = 10.0f;
	public float amplitude = 80.0f;
	protected float alpha = 0.0f;
	protected Vector3 origin = Vector3.zero;

	public bool stopMovement = false;
	protected bool pulseDie = false;
	protected ParticleSystem ps = null;
	public float pulsarTime = 0.4f;

	public float min = 0.0f;
	public float max = 0.0f;

	void msg_pulseDie()
	{
		pulseDie = true;
		GetComponent<BoxCollider2D>().enabled = false;
	}

	// Use this for initialization
	public virtual void Start () 
	{
		origin = transform.position;
		ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
		if (!stopMovement)
		{
			alpha += angleSpeed * Time.deltaTime;
			if (alpha > 360.0f)
				alpha = alpha - 360.0f;
			if (alpha < -360.0f)
				alpha = alpha + 360.0f;

			Vector3 vPos = transform.localPosition;
			vPos.y =  vPos.y + (Mathf.Sin (alpha * Mathf.PI/180)/amplitude ) * (Time.deltaTime * 100.0f);
			if (min != 0.0f && max != 0.0f)
			{
				if (vPos.y > max)
				{
					vPos.y = max;
					alpha *= -1.0f;
				}
				if (vPos.y < min)
				{
					vPos.y = min;
					alpha *= -1.0f;
				}
			}
			transform.localPosition = vPos;
		}
		if (pulseDie)
		{
			ps.startSize += -1.0f * Time.deltaTime/pulsarTime;
			
			if (ps.startSize <= 0.0f)
			{
				ps.startSize = 0.0f;
				Destroy(this.gameObject);
				return;
			}
		}

	}
}
