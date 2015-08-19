using UnityEngine;
using System.Collections;

public class spikeAttack : MonoBehaviour {

	public GameObject spikeSprite = null;
	public GameObject spikeParticle = null;

	private float stayingTime = 0.2f;
	private float stayingTimer = 0.0f;


	// Use this for initialization
	void Start () 
	{
		spikeParticle.GetComponent<ParticleSystem>().playbackSpeed = 4.6f;
		ParticleSystem[] systems = spikeParticle.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem ps in systems)
		{
			ps.playbackSpeed = 4.6f;
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		float time = spikeParticle.GetComponent<ParticleSystem>().time;
		Color col = spikeSprite.GetComponent<SpriteRenderer>().color;

		if (time >= 3.5f && col.a <= 0.0f)
		{
			col.a = 1.0f;
			spikeSprite.GetComponent<SpriteRenderer>().color = col;
		}
		if (time >= 5.0f)
		{
			spikeParticle.GetComponent<ParticleSystem>().loop = false;
			spikeParticle.GetComponent<ParticleSystem>().Clear();
			spikeParticle.GetComponent<ParticleSystem>().Stop ();
			spikeParticle.SetActive(false);
			ParticleSystem[] systems = spikeParticle.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem ps in systems)
			{
				ps.loop = false;
				ps.Clear();
				ps.Stop ();
				ps.gameObject.SetActive(false);
			}


			if (stayingTime <= stayingTimer)
			{
				
				col.a -= Time.deltaTime/0.2f;
				spikeSprite.GetComponent<SpriteRenderer>().color = col;
				
				if (col.a <= 0.0f)
				{
					col.a = 0.0f;
					Destroy (this.gameObject);
				}
			}
			else
				stayingTimer += Time.deltaTime;
		}

	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "player")
		{
			collision.gameObject.SendMessage("msg_hit",1.0f,SendMessageOptions.RequireReceiver);	
		}
	}
}
