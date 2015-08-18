using UnityEngine;
using System.Collections;

public class cPlayerSoul : cSoul
{
	Vector3 playerDirection = Vector3.zero;
	cPlayer_c player = null;
	GameObject chainAttack = null;

	private ParticleSystem ps = null;
	private Vector3 vinePosition = Vector3.zero;

	public bool beamAttack = false;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		player = GetComponentInParent<cPlayer_c>();
		ps = GetComponent<ParticleSystem>();

		chainAttack = transform.FindChild("ChainAttack").gameObject;

		endKaMeHaMeHa(false);
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();
		
		if (ps && vinePosition != Vector3.zero)
		{
			transform.position = Vector3.MoveTowards(transform.position,origin,4.0f * Time.deltaTime);

			if (transform.position == origin)
				vinePosition = Vector3.zero;
		}

		if (beamAttack && ps.enableEmission == true)
			startKaMeHaMeHa(true);
	}



	private void startKaMeHaMeHa(bool animation = true)
	{
		ParticleSystem[] systems = chainAttack.GetComponentsInChildren<ParticleSystem>();

		for (int i = 0; i < systems.Length; i++)
		{
			ParticleSystem ps = systems[i];
			if (ps)
			{
				kamehameha ka = ps.gameObject.GetComponent<kamehameha>();
				ka.animation = true;
				ka.state = kamehameha.eKaMe.SHOTON;
			}
		}
	}
	private void endKaMeHaMeHa(bool animation = true)
	{
		ParticleSystem[] systems = chainAttack.GetComponentsInChildren<ParticleSystem>();
		
		for (int i = 0; i < systems.Length; i++)
		{
			ParticleSystem ps = systems[i];
			if (ps)
			{
				kamehameha ka = ps.gameObject.GetComponent<kamehameha>();
				ka.animation = true;
				ka.state = kamehameha.eKaMe.SHOTOFF;
			}
		}
	}

	void msg_vineSoul(Vector3 vineOrigin)
	{
		origin = transform.position;
		origin.y = 0.0f;
		vinePosition = vineOrigin;
		transform.position = vinePosition;
		ps.enableEmission = true;
		ps.startSize = 1.0f;
	}
}

