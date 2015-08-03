﻿using UnityEngine;
using System.Collections;


[RequireComponent(typeof(cEnemyPhysic))]
public class cFlyingEnemy : cEnemy {

	private enum eDefaultCollideType {COLLIDE_NONE = 0, COLLIDE_MOVE=1, COLLIDE_STOP=2, COLLIDE_DONE=3};
	private eDefaultCollideType iDefaultCollide = eDefaultCollideType.COLLIDE_NONE;

	public enum eFlyingType {SINUS_RANDOM_STRAIGHT=0,SINUS_LOOP=1,SINUS_CIRCLE=2};
	public eFlyingType flyingType = eFlyingType.SINUS_RANDOM_STRAIGHT;

	public enum eAttackType {ATTACK_CHARGE=0,ATTACK_SHOT=1};
	public eAttackType attackType = eAttackType.ATTACK_CHARGE;

	//for attack
	public float chargeSpeed = 7.0f;
	private float currentChargeSpeed = 0.0f;
	public float chargeAcceleration = 0.5f;
	private bool IsCharging = false;

	public float flyingBackSpeed = 4.0f;
	private Vector3 originBeforeAttackPos = Vector3.zero;
	public enum eAttackState {ATTACK_NONE = 0,ATTACK_CHARGE=1,ATTACK_SHOT=2, ATTACK_TO_ORIGIN=3};
	private eAttackState iAttackState;
	public float attackChargeDmg = 1.0f;

	public float circleSize = 2.0f;
	public float xLengthTurning = 20.0f;
	public float speedX = 0.0f;
	public float accelerationX = 0.0f;
	public float angleSpeed = 20.0f;
	private float currentAngleSpeed = 0.0f;
	public float angleAcceleration = 5.0f;

	private float xTurningTemp = 0.0f;
	private float angleTemp = 0.0f;
	private float xDirection = 1.0f;

	private Vector3 origin = Vector3.zero;

	private bool bDecreasingSpeed = false;
	private Vector3 last_direction = Vector3.zero;
	private int pauseMovement = 30;
	private int pauseCounter = 0;

	private int backMovement = 20;
	private int backCounter = 0;

	private float delayFrames=0.3f;
	private float frameCounter=0.0f;

	private Color originColor;
	private SkeletonAnimation skeletonAnimation;

	private bool tookDamge = false;

	private GameObject player;
	private Vector3 playerDirection = Vector3.zero;

	public LayerMask collisionMask;

	private bool waitForAttack = false;
	private bool waitForSec = false;

	private float deathAlpha = 0.0f;
	private float startSpeedGround = 2.0f;
	private float speedGround = 2.0f;

	private float startAlphaGround = 10.0f;
	private float speedAlphaGround = 10.0f;

	private float fadeOutTime = 0.5f;

	[SerializeField]
	private AudioClip attackClip1;
	
	[SerializeField]
	private AudioClip attackClip2;
	
	[SerializeField]
	private AudioClip attackClip3;

	[HideInInspector]
	public cEnemyPhysic enemyPhysics;

	// Use this for initialization
	public override void Start () 
	{
		base.Start();

		xDirection = 1;
			
		angleSpeed += Random.Range(-1.0f,1.0f); 
		if (angleSpeed == 0.0f)
			angleSpeed = 1.0f;
		xDirection = Random.Range (-1,1);
		if (xDirection == 0)
			xDirection = 1;

		speedX += Random.Range (0.1f,0.6f);

		targetSpeed.x = speedX;
		acceleration.x = accelerationX;

		origin = transform.position;

		enemyPhysics = GetComponent<cEnemyPhysic>();
		skeletonAnimation = GetComponent<SkeletonAnimation>();
		originColor.a = skeletonAnimation.skeleton.a;
		originColor.r = skeletonAnimation.skeleton.r;
		originColor.g = skeletonAnimation.skeleton.g;
		originColor.b = skeletonAnimation.skeleton.b;

		player = GameObject.Find("Player");

		iAttackState = eAttackState.ATTACK_NONE;

		Debug.Log ("startSpeed: " + speedX);
		deathAlpha = -50.0f;

		startAlphaGround = deathAlpha;
		speedAlphaGround = deathAlpha;
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

	private void movementAirLoop()
	{

	}
	private void movementAirCircle()
	{
		float x = transform.position.x;
		float y = transform.position.y;
		Vector3 movement = new Vector3(0.0f,0.0f,transform.position.z);

		currentAngleSpeed += IncrementTowards(currentAngleSpeed,angleSpeed,angleAcceleration);
		angleTemp += currentAngleSpeed;
		if (angleTemp >= 360.0f )
			angleTemp = 0.0f;

		y += circleSize * Mathf.Sin (angleTemp * Mathf.PI/180) * Time.deltaTime;
		x += circleSize * Mathf.Cos (angleTemp * Mathf.PI/180) * Time.deltaTime;

		movement.x = x;
		movement.y = y;
		
		transform.position = movement;

	}
	private void movementAirRandomStraight()
	{
		float x = transform.position.x;
		Vector3 movement = new Vector3(0.0f,0.0f,0.0f);

		if (bDecreasingSpeed)
		{
			currentSpeed.x = IncrementTowards(currentSpeed.x, 0, acceleration.x);
			if (currentSpeed.x <= 0.1f)
			{
				bDecreasingSpeed = false;
				xDirection *= -1;
				xTurningTemp = 0.0f;
			}
		}
		else
			currentSpeed.x = IncrementTowards(currentSpeed.x, targetSpeed.x, acceleration.x);
		movement.x = (currentSpeed.x  * xDirection) * Time.deltaTime;

		xTurningTemp += Mathf.Abs (movement.x);
		if (bDecreasingSpeed == false && xTurningTemp >= xLengthTurning)
		{
			//turning point
			bDecreasingSpeed = true;
		}

		currentAngleSpeed = IncrementTowards(currentAngleSpeed,angleSpeed,angleAcceleration);
		angleTemp += currentAngleSpeed;
		if (angleTemp > 360.0f )
			angleTemp = angleTemp - 360.0f;
		else if (angleTemp < 0.0f)
			angleTemp = 360.0f-angleTemp;

		
		float yInc = Time.deltaTime * Mathf.Sin(angleTemp * Mathf.PI/180);

		movement.y = yInc;

		last_direction.x = movement.x;
		last_direction.y = yInc;

		RaycastHit2D hit;
		Vector2 pos = Vector2.zero;

		//can we move in that direction?
		float collideX = 0.0f;
		float collideY = 0.0f;
		if (Mathf.Abs(last_direction.normalized.x) > 0.5f)
			collideX = GetComponent<BoxCollider2D>().size.x * Mathf.Sign(last_direction.x) * 1.2f;
		if (Mathf.Abs(last_direction.normalized.y) > 0.5f)
			collideY = GetComponent<BoxCollider2D>().size.y * Mathf.Sign(last_direction.y) * 1.2f;
		
		pos.x = collideX + transform.position.x;
		pos.y = collideY + transform.position.y;
		Debug.DrawRay(pos,last_direction.normalized * 3.0f);
		if (hit = Physics2D.Raycast (pos, last_direction.normalized ,3.0f,Physics.AllLayers))
		{
			//if we hit something, went opposite direction
			bDecreasingSpeed = false;
			xDirection *= -1;
			xTurningTemp = 0.0f;

			last_direction *= xDirection;
		}

		transform.Translate(last_direction);

	}
	private void manageMovementAfterCollide()
	{
		switch (iDefaultCollide)
		{
			case eDefaultCollideType.COLLIDE_MOVE: //go back in oppsite direction
				backCounter++;
				if (backMovement >= backCounter)
					transform.Translate(-last_direction.normalized * Time.deltaTime);
				else
				{
					currentAngleSpeed = 0.0f;
					iDefaultCollide = eDefaultCollideType.COLLIDE_STOP;
					backCounter = 0;
					origin = transform.position;
					angleTemp = 0.0f;
				}
				break;
			case eDefaultCollideType.COLLIDE_STOP: //then stop for a while
				pauseCounter++;

				currentAngleSpeed = IncrementTowards(currentAngleSpeed,20.0f,5.0f);
				angleTemp += currentAngleSpeed;
				if (angleTemp > 360.0f )
					angleTemp = angleTemp - 360.0f;
				else if (angleTemp < 0.0f)
					angleTemp = 360.0f-angleTemp;

				Vector3 movement = Vector3.zero;
				float y = transform.position.y;

				y = origin.y + circleSize * Mathf.Sin (angleTemp * Mathf.PI/180) * Time.deltaTime;
				movement.x = transform.position.x;
				movement.y = y;
				movement.z = transform.position.z;

				transform.position = movement;

				if (pauseCounter >= pauseMovement)
				{
					currentAngleSpeed = 0;
					angleTemp = 0;
					iDefaultCollide = eDefaultCollideType.COLLIDE_DONE;
					pauseCounter = 0;
				}
				break;
			case eDefaultCollideType.COLLIDE_DONE:
				origin = transform.position;
				iAttackState = eAttackState.ATTACK_NONE;
				iDefaultCollide = eDefaultCollideType.COLLIDE_NONE;
				if (last_direction.x > 0.0f)
					xDirection = Mathf.Sign(last_direction.x)*-1.0f;
				break;
		}
		
	}

	private  IEnumerator WaitForSec(float sec)
	{
		yield return new WaitForSeconds(sec);
		iAttackState = eAttackState.ATTACK_TO_ORIGIN;

		currentChargeSpeed = 0.0f;

		yield break;
	}


	private  IEnumerator WaitForNextAttack(float sec)
	{
		yield return new WaitForSeconds(sec);
		waitForAttack = false;

		int x = Random.Range (0,100);
		if  (x < 70)
		{
			iAttackState = eAttackState.ATTACK_CHARGE;
			firstProjectileShot = false;
			playAttackClips();
		}
		else
			iAttackState = eAttackState.ATTACK_SHOT;
				
		yield break;
	}

	private void startIdleAttack(bool shot = false)
	{
		//wait 1 sec
		defaultIdleMovement(shot);
		if (!waitForAttack)
		{
			StartCoroutine("WaitForNextAttack",5.5f); 
			waitForAttack = true;
		}
	}
	private void startIdle(bool shot = false)
	{
		currentChargeSpeed = 0.0f;
		defaultIdleMovement(shot);
		if (!waitForSec)
		{
			//wait 3sec before going back to origin
			StartCoroutine("WaitForSec",1000.0f); 
			waitForSec = true;
		}
	}
	private void moveToDirection(Vector3 direction)
	{
		Vector3 movement = Vector3.zero;
		
		//nothing between me and target
		currentChargeSpeed = IncrementTowards(currentChargeSpeed,chargeSpeed,chargeAcceleration);
		
		movement.x = currentChargeSpeed * direction.normalized.x * Time.deltaTime;
		movement.y = currentChargeSpeed * direction.normalized.y * Time.deltaTime;
		
		transform.Translate(movement);
	}

	private void attackCharge(GameObject player)
	{
		RaycastHit2D hit;
		IsCharging = true;
		Vector3 playerPos = player.transform.position;
		playerPos.y += player.GetComponent<BoxCollider2D>().size.y- (player.GetComponent<BoxCollider2D>().size.y*0.2f);
		Vector3 direction = Vector3.zero;
		direction = playerPos-transform.position;

		Vector2 pos = Vector2.zero;

		if (iAttackState == eAttackState.ATTACK_TO_ORIGIN)
		{

			waitForSec = false;
			Vector3 origin_direction = Vector3.zero;
			origin_direction = originBeforeAttackPos-transform.position;
			float dist = Vector3.Distance(transform.position,origin_direction);

			if (origin_direction.sqrMagnitude < 1.0f)
			{
				iAttackState = eAttackState.ATTACK_NONE;
				//we are done
				waitForSec = false;
				waitForAttack = false;
				StopCoroutine("WaitForNextAttack"); //if we can reach our goal again just stop coroutine
				StopCoroutine("WaitForSec"); //if we can reach our goal again just stop coroutine
				return;
			}

			float collideX = 0.0f;
			float collideY = 0.0f;
			if (Mathf.Abs(origin_direction.normalized.x) > 0.5f)
				collideX = GetComponent<BoxCollider2D>().size.x * Mathf.Sign(origin_direction.x) * 1.2f;
			if (Mathf.Abs(origin_direction.normalized.y) > 0.5f)
				collideY = GetComponent<BoxCollider2D>().size.y * Mathf.Sign(origin_direction.y) * 1.2f;

			pos.x = collideX + transform.position.x;
			pos.y = collideY + transform.position.y;
			Debug.DrawRay(pos,origin_direction.normalized * dist);
			if (hit = Physics2D.Raycast (pos, origin_direction.normalized ,5.0f,Physics.AllLayers))
			{
				//something at our place :/ live here for now
				iAttackState = eAttackState.ATTACK_NONE;
				waitForSec = false;
				waitForAttack = false;
				StopCoroutine("WaitForNextAttack"); //if we can reach our goal again just stop coroutine
				StopCoroutine("WaitForSec"); //if we can reach our goal again just stop coroutine
			}
			else
				moveToDirection(origin_direction);

		}
		else if (iAttackState == eAttackState.ATTACK_SHOT)
		{
			moveBackABit(direction,playerPos);

		}
		else if (iAttackState == eAttackState.ATTACK_CHARGE)
		{
			waitForAttack = false;
			float collideX = 0.0f;
			float collideY = 0.0f;
			if (Mathf.Abs(direction.normalized.x) > 0.5f)
				collideX = GetComponent<BoxCollider2D>().size.x * Mathf.Sign(direction.x) * 1.2f;
			if (Mathf.Abs(direction.normalized.y) > 0.5f)
				collideY = GetComponent<BoxCollider2D>().size.y * Mathf.Sign(direction.y) * 1.2f;
			pos.x = collideX + transform.position.x;
			pos.y = collideY + transform.position.y;

			//can we reach player ?
			float dist = Vector3.Distance(transform.position,playerPos) + 1.0f;
			Debug.DrawRay(pos,direction.normalized * dist);
			if (hit = Physics2D.Raycast (pos, direction.normalized ,dist,Physics.AllLayers))
			{
				//nothing between me and target
				if (hit.collider.tag == "player")
				{
					waitForSec = false;
					waitForAttack = false;
					StopCoroutine("WaitForNextAttack"); //if we can reach our goal again just stop coroutine
					StopCoroutine("WaitForSec"); //if we can reach our goal again just stop coroutine
					moveToDirection(direction);
				}
				else
					startIdleAttack(true);
			}
			else
			{
				//something else ahead, wait
				startIdleAttack(true);
			}
		}
	}

	private void moveBackABit(Vector3 currentDirection,Vector3 playerPos)
	{
		RaycastHit2D hit;
		Vector2 pos = Vector2.zero;
		//go back a bit and wait 1sec for next attack 
		float goBackDistance = Vector3.Distance (lastPlayerPos,transform.position);
		if (goBackDistance < 12.0f)
		{
			currentDirection *=  -1.0f;
			currentDirection.y *= 0.05f;

			float dist = Vector3.Distance(transform.position,currentDirection);
			LayerMask newMask= ~collisionMask;
			float collideX = 0.0f;
			float collideY = 0.0f;
			if (Mathf.Abs(currentDirection.normalized.x) > 0.5f)
				collideX = GetComponent<BoxCollider2D>().size.x * Mathf.Sign(currentDirection.x) * 1.2f;
			if (Mathf.Abs(currentDirection.normalized.y) > 0.5f)
				collideY = GetComponent<BoxCollider2D>().size.y * Mathf.Sign(currentDirection.y) * 1.2f;
			pos.x = collideX + transform.position.x;
			pos.y = collideY + transform.position.y;
			Debug.DrawRay(pos,currentDirection.normalized * 4.0f);

			if (hit = Physics2D.Raycast (pos, currentDirection.normalized ,2.0f,newMask))
				startIdleAttack(true);
			else
				moveToDirection(currentDirection);
		}
		else
			startIdleAttack(true);
	}

	private void defaultIdleMovement(bool shot)
	{
		Vector3 movement = Vector3.zero;
		movement.y = Time.deltaTime * Mathf.Sin(angleTemp * Mathf.PI/180);

		transform.Translate(movement);

		angleTemp += 2.0f;
		if (angleTemp > 360.0f)
			angleTemp = 0.0f;
		if (shot)
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

	private void manageAttack(float playerDistance)
	{
		if (player.GetComponent<cPlayer_c>().isCutscene())
			return;

		//if get in range trigger attack
		if (iAttackState != eAttackState.ATTACK_NONE || playerDistance <= triggerRange)
		{
			switch (attackType)
			{
			case eAttackType.ATTACK_CHARGE:
				attackCharge(player);
				break;
			case eAttackType.ATTACK_SHOT:
				attackShot (player,player.transform.position);
				break;
			}
		}
		else
		{
			iAttackState = eAttackState.ATTACK_NONE;
		}
	}

	void manageMovement(float playerDistance)
	{
		IsCharging = false;
		if (aggroRange <= playerDistance) //we are out of range
		{
			if (iAttackState != eAttackState.ATTACK_NONE)
				iAttackState = eAttackState.ATTACK_TO_ORIGIN;
		}
		else
		{
			//we are in range
			if (iAttackState == eAttackState.ATTACK_NONE && triggerRange >= playerDistance)
			{
				iAttackState = eAttackState.ATTACK_CHARGE;
				originBeforeAttackPos = transform.position;
			}
		}
		if (iAttackState != eAttackState.ATTACK_NONE)
			manageAttack(playerDistance);
		
	}

	// Update is called once per frame
	void Update () 
	{

		if (!isActiveAndEnabled)
			return;


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
				
		Vector3 playerPos = player.gameObject.transform.position;
		Vector3 enemyPos = transform.position;
		float distance = Vector3.Distance(enemyPos,playerPos);

		manageMovement(distance);

		//manageAttack(distance);

		//we are charging to player or flying back.. so no need for movement calculation
		if (!IsCharging)
		{
			targetSpeed.x = speedX;
			acceleration.x = accelerationX;
			switch (flyingType)
			{
				case eFlyingType.SINUS_RANDOM_STRAIGHT:
					movementAirRandomStraight();
					break;
				case eFlyingType.SINUS_LOOP:
					movementAirLoop();
					break;
				case eFlyingType.SINUS_CIRCLE:
					movementAirCircle();
					break;
			}
		}

	}

	void msg_damage(float dmg)
	{
		skeletonAnimation.skeleton.r = 1.0f;
		skeletonAnimation.skeleton.b = 0.3f;
		skeletonAnimation.skeleton.g = 0.0f;
		skeletonAnimation.skeleton.a = 1.0f;

		if (damagedClip)
		{
			audioSource.clip = damagedClip;
			if(!audioSource.isPlaying){
				audioSource.Play();
			}
		}

		takeDmg(dmg);	

		tookDamge = true;
		lastPlayerPos = player.transform.position;
		iAttackState = eAttackState.ATTACK_SHOT;
	}

	//collsions
	void OnCollisionEnter2D(Collision2D collision)
	{				
		if (collision.gameObject.tag == "player")
		{
			if (eAttackType.ATTACK_CHARGE == attackType)
			{
				Debug.Log ("collide");
				lastPlayerPos = collision.gameObject.transform.position;
				iAttackState = eAttackState.ATTACK_SHOT;
				currentChargeSpeed = 5.0f;
//				playAttackClips();
				collision.gameObject.SendMessage("msg_hit",attackChargeDmg,SendMessageOptions.RequireReceiver);
			}
			else
			{
				collision.gameObject.SendMessage("msg_hit",attackCollideDmg,SendMessageOptions.RequireReceiver);
				iDefaultCollide = eDefaultCollideType.COLLIDE_MOVE;
			}
		}
		else
			iDefaultCollide = eDefaultCollideType.COLLIDE_MOVE;
	}

	private void playAttackClips(){
		switch (Random.Range(0,3)) {
			
		case 0:
			audioSource.PlayOneShot(attackClip1);
			break;
			
		case 1:
			audioSource.PlayOneShot(attackClip2);
			break;
			
		case 2:
			audioSource.PlayOneShot(attackClip3);
			break;
			
		default:
			break;
		}
	}
}
