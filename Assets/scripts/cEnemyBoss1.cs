using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(cEnemyPhysic))]
public class cEnemyBoss1 : cEnemy
{
	public float accelerationX = 10.0f;
	public float hurtVelocity = 3.0f;
	public float flyVelocity = 6.0f;

	[System.Serializable]
	public struct stMinions
	{
		public int count;
		public float rotationSpeedAngle;
		public float acceleration;

		public float rotationDirection;
		public float radiusToBossCenter;
	};
	public stMinions minions;
	public GameObject minion;
	private List<GameObject> lMinions;
	private float currentAngleSpeed = 0.0f;      

	private Vector2 _centre;

	private float bossAlpha = 0.0f;
	private bossMinion bossMinion;

	private SkeletonAnimation skeletonAnimation;
	private bool tookDamge = false;
	private Color originColor;
	private float delayFrames=0.3f;
	private float frameCounter=0;

	[HideInInspector]
	public cEnemyPhysic enemyPhysics;

	private float deathAlpha = 0.0f;
	private float startSpeedGround = 2.0f;
	private float speedGround = 2.0f;
	
	private float startAlphaGround = 10.0f;
	private float speedAlphaGround = 10.0f;

	private float angleTemp = 0.0f;
	private float fadeOutTime = 0.5f;

	public override void Start()
	{
		base.Start();

		lMinions = new List<GameObject>();

		int minionsCount = minions.count;
		float alpha = 0.0f;
		float angleDistance = 360.0f/(float)minionsCount;
		
		_centre.x = transform.position.x;// + _colliderOffset.x + _colliderSize.x/2 + transform.lossyScale.x/2 + 0.5f;
		_centre.y = transform.position.y;// + _colliderOffset.y + _colliderSize.y/2 + transform.lossyScale.y/2 + 0.5f;

		float minionX = 0.0f;
		float minionY = 0.0f;

		for (int i = 0; i < minions.count; i++)
		{
			minionX = _centre.x + minions.radiusToBossCenter * Mathf.Cos (alpha * Mathf.PI/180);
			minionY = _centre.y + minions.radiusToBossCenter * Mathf.Sin (alpha * Mathf.PI/180);

			GameObject ga = GameObject.Instantiate(minion);		
			ga.GetComponent<bossMinion>().alpha = alpha;
			ga.transform.position = new Vector3(minionX,minionY,1.0f);

			lMinions.Add (ga);
			alpha += angleDistance;
		}

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		originColor.a = skeletonAnimation.skeleton.a;
		originColor.r = skeletonAnimation.skeleton.r;
		originColor.g = skeletonAnimation.skeleton.g;
		originColor.b = skeletonAnimation.skeleton.b;

		enemyPhysics = GetComponent<cEnemyPhysic>();
	}
	private bool death()
	{
		switch (iDieState)
		{	
		case eDieState.DIE_START:
		{
			bool playDeath = false;
			
			//if different namens, we have to play it, if null we can play anyway
			playDeath = cFunction.xor (audioSource.clip != null && audioSource.clip.name != deathClip.name, audioSource.clip == null);
			
			//last clip we give a shit and abusing .clip, since he will die..
			if (deathClip && playDeath)
			{
				audioSource.clip = deathClip;
				audioSource.PlayOneShot(deathClip);
			}
			
			//Vector3 scale = transform.localScale;
			Vector3 vec = new Vector3(0.8f,0.8f,0.8f) * Time.deltaTime;
			//scale -= vec;
			//if (scale.x < 0.0f)
			//scale.x = 0.0f;
			//if (scale.y < 0.0f)
			//	scale.y = 0.0f;
			//transform.localScale = scale;
			currentSpeed.y += -9.81f * Time.deltaTime;
			if (enemyPhysics.GetDistanceToGround() > 1.0f)
				transform.position += new Vector3(10.0f * Time.deltaTime,currentSpeed.y * Time.deltaTime,0.0f);
			else
			{
				transform.position += new Vector3(speedGround * Time.deltaTime,0.0f,0.0f);
				
				speedGround = -startSpeedGround * Time.deltaTime + speedGround;
				speedAlphaGround = -startAlphaGround * Time.deltaTime + speedAlphaGround;
			}
			transform.RotateAround(transform.position,Vector3.forward,speedAlphaGround * Time.deltaTime);
			
			if (speedAlphaGround >= 0.0f)
			{
				iDieState = eDieState.DIE_DONE;
			}
			return true;
			break;
		}
		case eDieState.DIE_DONE:
			die ();
			return true;
		}
		
		return false;
	}
	public void Update()
	{
		int minionCount = lMinions.Count;
		Vector3 movement = new Vector3(0.0f,0.0f,0.0f);
		
		if (tookDamge)
		{
			if (delayFrames < frameCounter)
			{
				skeletonAnimation.skeleton.r = 1.0f;
				skeletonAnimation.skeleton.b = 1.0f;
				skeletonAnimation.skeleton.g = 1.0f;
				skeletonAnimation.skeleton.a = 1.0f;
				
				tookDamge = false;
				frameCounter = 0.0f;
			}
			frameCounter+=Time.deltaTime;
		}
		if (iDieState == eDieState.DIE_NONE && isDead() && !tookDamge)
		{
			GetComponent<BoxCollider2D>().enabled = false;
			iDieState = eDieState.DIE_START;
		}
		
		if (death()) //if we are dead, no need for others
			return;

		transform.position = new Vector3(transform.position.x,transform.position.y + Time.deltaTime * Mathf.Sin(bossAlpha * Mathf.PI/180),transform.position.z);
		bossAlpha += 2.0f;
		if (bossAlpha > 360.0f)
			bossAlpha = 0.0f;

		_centre.x = transform.position.x;// + _colliderOffset.x + _colliderSize.x/2 + transform.lossyScale.x/2 + 0.5f;
		_centre.y = transform.position.y;// + _colliderOffset.y + _colliderSize.y/2 + transform.lossyScale.y/2 + 0.5f;


		defaultIdleMovement();

		int minionsC = 0;

		for (int i = 0; i<minionCount; i++)
		{
			GameObject _minion = lMinions[i];
			bossMinion = _minion.GetComponent<bossMinion>();
			if ( !_minion.activeSelf || bossMinion == null)
				continue;

			minionsC++;

			float alpha = bossMinion.alpha;


			//Time.deltaTime * Mathf.Sin(bossAlpha * Mathf.PI/180)

			currentAngleSpeed = IncrementTowards(currentAngleSpeed,minions.rotationSpeedAngle,minions.acceleration);

			alpha += currentAngleSpeed * Mathf.Sign (minions.rotationDirection) * Time.deltaTime;
			if (alpha > 360.0f)
				alpha = 0.0f;
			if (alpha < 0.0f)
				alpha = 360.0f;

			float radius = minions.radiusToBossCenter;
			if (radius > 0.0f)
				radius += bossMinion.radius;
			bossMinion.radius = -1.0f;
			movement.x = _centre.x + radius * Mathf.Cos (alpha * Mathf.PI/180);
			movement.y = _centre.y + radius * Mathf.Sin (alpha * Mathf.PI/180);
			movement.z = 1.0f;

			_minion.transform.position = movement;
			bossMinion.alpha = alpha;

			if (alpha < 180.0f && alpha > 90.0f)
			{
				//minions shot
				_minion.SendMessage("msg_shot",null,SendMessageOptions.RequireReceiver);
			}
		}

	}

	private void defaultIdleMovement()
	{
		GameObject player = GameObject.FindGameObjectWithTag("player");
		Vector3 pos1 = Vector3.zero;
		Vector3 pos2 = Vector3.zero;

		pos1.x = player.transform.position.x;
		pos2.x = transform.position.x;

		float distance = Vector3.Distance(pos1,pos2);
		
		if (distance <= triggerRange)
		{
			if (shots == 0)
			{
				shots = Random.Range (1,4);
				
				if (!firstProjectileShot && currentShots == 0)
					intervalTimer = 0.0f;
				else if (firstProjectileShot)
					intervalTimer = shotInterval;
			}
			
			if (shots > 0)
			{
				multibleProjectile = false;
				if (shots > 1)
					multibleProjectile = true;
				
				attackShot(player,player.transform.position);
				if (currentShots >= shots)
				{
					//we are done
					//time up ?
					currentShots = 0;
					shots = 0;
					//charge next ? or shot more ? or go back to origin?
				}
				
			}
		}
	}

	protected override void die()
	{
		
		skeletonAnimation.skeleton.a -= Time.deltaTime/fadeOutTime;
		if (skeletonAnimation.skeleton.a <= 0.0f)
		{
			skeletonAnimation.skeleton.a = 0.0f;
			base.die();
		}
	}

	void msg_damage(float dmg)
	{
		skeletonAnimation.skeleton.r = 1.0f;
		skeletonAnimation.skeleton.b = 0.3f;
		skeletonAnimation.skeleton.g = 0.0f;
		skeletonAnimation.skeleton.a = 1.0f;
		
		takeDmg(dmg);	
		
		tookDamge = true;
	}

}


