using UnityEngine;
using System.Collections;

public class cVineSoul : cSoul
{

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		base.Update();


		if (alpha < -180.0f)
			GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = 29;
		if (alpha > -10.0f)
			GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = 27;
	}
}

