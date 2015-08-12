using UnityEngine;
using System.Collections;

public class cPlayerSoul : cSoul
{

	// Use this for initialization
	public override void Start ()
	{
		base.Start();
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();

		ParticleSystem ps = GetComponent<ParticleSystem>();
		ps.enableEmission = true;
		ps.startSize = 1.0f;

	}
}

