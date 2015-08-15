using UnityEngine;
using System.Collections;

public class cPlayerSoul : cSoul
{
	Vector3 playerDirection = Vector3.zero;
	cPlayer_c player = null;

	private ParticleSystem ps = null;
	private Vector3 vinePosition = Vector3.zero;


	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		player = GetComponentInParent<cPlayer_c>();
		ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
		
		if (ps && vinePosition != Vector3.zero)
		{
			transform.position = Vector3.MoveTowards(transform.position,origin,10.0f * Time.deltaTime);
			if (transform.position == origin)
				vinePosition = Vector3.zero;
		}
	}

	void msg_vineSoul(Vector3 vineOrigin)
	{
		origin = transform.position;
		vinePosition = vineOrigin;
		transform.position = vinePosition;
		ps.enableEmission = true;
		ps.startSize = 1.0f;
	}
}

