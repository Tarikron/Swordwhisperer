﻿using UnityEngine;
using System.Collections;

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
	public enum eAttackState {ATTACK_NONE = 0,ATTACK_STAGE1=1,ATTACK_STAGE2=2};
	private eAttackState iAttackState;
	private bool isCharge = false;
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

	private int delayFrames=4;
	private int frameCounter=0;

	private Color originColor;
	private SkeletonAnimation skeletonAnimation;

	private bool bAttack = false;
	private bool tookDamge = false;

	private GameObject player;
	private Vector3 playerDirection = Vector3.zero;

	public LayerMask collisionMask;

	// Use this for initialization
	public override void Start () 
	{
		base.Start();

		xDirection = 1;

		targetSpeed.x = speedX;
		acceleration.x = accelerationX;

		origin = transform.position;

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		originColor.a = skeletonAnimation.skeleton.a;
		originColor.r = skeletonAnimation.skeleton.r;
		originColor.g = skeletonAnimation.skeleton.g;
		originColor.b = skeletonAnimation.skeleton.b;

		player = GameObject.Find("Player");
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
		
		float yInc = origin.y + (circleSize) * Mathf.Sin (angleTemp * Mathf.PI/180) * Time.deltaTime;

		movement.y = yInc;

		last_direction.x = movement.x;
		last_direction.y = movement.y - origin.y;

		movement.x += x;
		movement.z = transform.position.z;
		transform.position = movement;
		
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

	private void attackCharge(GameObject player)
	{
		Vector3 playerPos = player.transform.position;
		playerPos.y += player.GetComponent<BoxCollider2D>().size.y- (player.GetComponent<BoxCollider2D>().size.y*0.2f);

		Vector3 direction = Vector3.zero;
		direction = playerPos-transform.position;
		RaycastHit2D hit;
		//can we reach player ?
		IsCharging = false;
		if (hit = Physics2D.Raycast (new Vector2(transform.position.x,transform.position.y), direction.normalized ,triggerRange,collisionMask))
		{
			Vector3 movement = Vector3.zero;
			//nothing between me and target
			currentChargeSpeed = IncrementTowards(currentChargeSpeed,chargeSpeed,chargeAcceleration);

			movement.x = currentChargeSpeed * direction.normalized.x * Time.deltaTime;
			movement.y = currentChargeSpeed * direction.normalized.y * Time.deltaTime;

			transform.Translate(movement);

			IsCharging = true;
		}
		else
		{
			//something else ahead, wait

			currentChargeSpeed = 0.0f;
			defaultIdleMovement();

			//wait 3sec before going back to origin

		}
	}

	private void defaultIdleMovement()
	{
		Vector3 movement = Vector3.zero;
		movement.y = Time.deltaTime * Mathf.Sin(angleTemp * Mathf.PI/180);

		transform.Translate(movement);

		angleTemp += 2.0f;
		if (angleTemp > 360.0f)
			angleTemp = 0.0f;
	}

	private void manageAttack(float playerDistance)
	{
		//if get in range trigger attack
		if (iAttackState != eAttackState.ATTACK_NONE || playerDistance <= attackDistance)
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
			isCharge = false;
			iAttackState = eAttackState.ATTACK_NONE;
		}
	}

	void manageMovement(float playerDistance)
	{
		if (aggroRange <= playerDistance) //we are out of range
		{
			bAttack = false;
		}
		else
		{
			//we are in range
			if (!bAttack && triggerRange >= playerDistance)
			{
				bAttack = true;
				originBeforeAttackPos = transform.position;
			}

			if (bAttack)
				manageAttack(playerDistance);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (tookDamge)
		{
			if (delayFrames < frameCounter)
			{
				skeletonAnimation.skeleton.r = 1.0f;
				skeletonAnimation.skeleton.b = 1.0f;
				skeletonAnimation.skeleton.g = 1.0f;
				skeletonAnimation.skeleton.a = 1.0f;

				tookDamge = false;
				frameCounter = 0;
			}
			frameCounter++;
		}
		if (isDead() && !tookDamge)
		{
			GetComponent<BoxCollider2D>().enabled = false;
			iDieState = eDieState.DIE_START;
		}

		if (defaultDeath()) //if we are dead, no need for others
			return;
		if (cFunction.xor(tookDamge, iDefaultCollide != eDefaultCollideType.COLLIDE_NONE))
		{
			manageMovementAfterCollide();
			return; 
		}
		
		Vector3 playerPos = player.gameObject.transform.position;
		Vector3 enemyPos = transform.position;
		float distance = Vector3.Distance(enemyPos,playerPos);

		//manageMovement(distance);

		manageAttack(distance);

		//we are charging to player or flying back.. so no need for movement calculation
		if (!isCharge)
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

		takeDmg(dmg);	

		tookDamge = true;
	}

	//collsions
	void OnCollisionEnter2D(Collision2D collision)
	{				
		if (collision.gameObject.tag == "player")
		{
			if (eAttackType.ATTACK_CHARGE == attackType)
			{
				lastPlayerPos = Vector3.zero;
				iAttackState = eAttackState.ATTACK_STAGE2;
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
}
