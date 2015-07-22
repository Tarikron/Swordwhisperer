
using UnityEngine;
using System.Collections;

public class cEnemy : cUnit
{
	protected enum eDieState {DIE_NONE = 0, DIE_START = 1,DIE_DONE = 2};
	protected eDieState iDieState = eDieState.DIE_NONE;

	public float aggroRange = 2.0f;
	public float triggerRange = 1.0f;

	protected bool firstProjectileDelay = false;
	protected bool firstProjectileShot = false;

	public bool multibleProjectile = false;
	public float fastShotInterval = 0.2f;
	public float shotInterval = 2.0f;
	protected float intervalTimer = 0.0f;
	protected Vector3 lastPlayerPos = Vector3.zero;
	public float attackCollideDmg = 1.0f;

	protected int shots = 0;
	protected int currentShots = 0;

	public AudioSource audioSource;

	[SerializeField]
	protected  AudioClip damagedClip;

	[SerializeField]
	protected AudioClip deathClip;

	[SerializeField]
	protected AudioClip movementClip;

	protected bool defaultDeath()
	{
		switch (iDieState)
		{	
		case eDieState.DIE_START:
		{
			Vector3 scale = transform.localScale;
			Vector3 vec = new Vector3(0.8f,0.8f,0.8f) * Time.deltaTime;
			scale -= vec;
			if (scale.x < 0.0f)
				scale.x = 0.0f;
			if (scale.y < 0.0f)
				scale.y = 0.0f;
			transform.localScale = scale;
			currentSpeed.y += -9.81f * Time.deltaTime;
			transform.position += new Vector3(10.0f * Time.deltaTime,currentSpeed.y * Time.deltaTime,0.0f);

			if (scale.x <= 0.0f)
				iDieState = eDieState.DIE_DONE;
			
			return true;
		}
		case eDieState.DIE_DONE:
			if (deathClip)
			{
				audioSource.clip = deathClip;
				if(!audioSource.isPlaying){
					audioSource.Play();
				}
				die ();
			}
			return true;
		}

		return false;
	}



	protected void attackShot(GameObject player, Vector3 target)
	{
		Vector3 enemyPos = transform.position;
		Vector3 targetH = target;
		targetH.y += 1.5f + (float)(Random.Range(0,100) - 50)/30;
		Vector3 heading = targetH - enemyPos;

		Transform t = transform.GetChild(0);
		GameObject shot = t.gameObject;

		intervalTimer -= Time.deltaTime;
		
		if (intervalTimer <= 0.0f)
		{
			if (multibleProjectile)
				intervalTimer = fastShotInterval;
			else
				intervalTimer = shotInterval;
			//we are in attack range
			lastPlayerPos = target;
			lastPlayerPos.y += 0.5f;
			shot.transform.position = transform.position + (heading.normalized * 2.5f);
			
			GameObject shotClone = GameObject.Instantiate(shot);
			shotClone.SendMessage("msg_shotfired",heading.normalized,SendMessageOptions.RequireReceiver);
			shotClone.transform.localScale = shot.transform.lossyScale;
			shotClone.transform.position = shot.transform.position;
			//shotClone.transform.lossyScale = shot.transform.localScale;
			//shotClone.GetComponent<SpriteRenderer>().enabled = true;
			shotClone.GetComponent<BoxCollider2D>().enabled = true;
		
			ParticleSystem ps = shotClone.GetComponent<ParticleSystem>();
			if (ps)
			{
				ps.enableEmission = true;
			}

			currentShots++;
			if (currentShots > 0)
				firstProjectileShot = true;
		}
	}


}


