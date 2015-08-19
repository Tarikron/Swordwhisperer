using UnityEngine;
using System.Collections;

public class cPlayerSoul : cSoul
{
	Vector3 playerDirection = Vector3.zero;
	cPlayer_c player = null;
	GameObject chainAttack = null;

	private Vector3 vinePosition = Vector3.zero;

	public bool beamAttack = false;
	private bool beamStarted = false;

	public float uptimeforbeam = 7.0f;
	private float beamTimer = 0.0f;

	private bool pulsiere = false;
	private float origin_startSize = 0.0f;
	private float pulsarDirection = 1.0f;
	public int pulseCount = 0;

	public int requiredSouls = 3;
	private int collectedSouls = 0;

	// Use this for initialization
	public override void Start ()
	{
		base.Start();

		player = GetComponentInParent<cPlayer_c>();

		chainAttack = transform.FindChild("ChainAttack").gameObject;

		endKaMeHaMeHa(false);
	}

	void collectSoul()
	{
		pulsiere = true;
		collectedSouls++;

		if (collectedSouls == requiredSouls)
		{
			//ready to fire
			pulsiere = true;
			pulsarTime = 0.2f;
		}
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

		if (!beamStarted && beamAttack && ps.enableEmission == true)
			startKaMeHaMeHa(true);

		if (beamStarted)
		{
			if (beamTimer >= uptimeforbeam)
			{
				endKaMeHaMeHa(true);
				beamStarted = false;
				GetComponentInParent<cPlayer_c>().moveAgain();
			}

			beamTimer += Time.deltaTime;
		}
		else
		{

			if (pulsiere)
			{
				if (pulseCount >= 2)
				{
					pulsarDirection = -1.0f;
					ps.startSize += pulsarDirection * Time.deltaTime/pulsarTime;
					if (ps.startSize <= origin_startSize)
					{
						ps.startSize = origin_startSize;
						pulsiere = false;
						pulsarDirection = 1.0f;
						pulseCount = 0;
					}
				}
				else
				{
					ps.startSize += pulsarDirection * Time.deltaTime/pulsarTime;
					if (ps.startSize >= 1.5f)
					{
						if (collectedSouls < requiredSouls)
							pulseCount++;

						ps.startSize = 1.5f;
						pulsarDirection = -1.0f;
					}
					else if (ps.startSize <= 0.5f)
					{
						ps.startSize = 0.5f;
						pulsarDirection = 1.0f;
					}
				}


			}
		}

	}

	public void rotateKaMeHaMeHa(float x)
	{
		Vector3 rotAround = Vector3.zero;
		rotAround.z = 1.0f;
		transform.RotateAround(transform.position,rotAround,x);
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
		beamStarted = true;
		stopMovement = true;
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
		beamStarted = false;
		stopMovement = false;
		beamAttack = false;
	}

	void msg_vineSoul(Vector3 vineOrigin)
	{
		origin = transform.position;
		origin.y = 0.0f;
		vinePosition = vineOrigin;
		transform.position = vinePosition;
		ps.enableEmission = true;
		ps.startSize = 1.0f;
		origin_startSize = ps.startSize;
	}
}

