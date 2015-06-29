using UnityEngine;
using System.Collections;

public class cFlyingEnemy : cEnemy {

	private enum eDieState {DIE_NONE = 0, DIE_START = 1,DIE_DONE = 2};
	private eDieState iDieState = eDieState.DIE_NONE;

	public enum eFlyingType {SINUS_RANDOM_STRAIGHT=0,SINUS_LOOP=1,SINUS_CIRCLE=2};
	public eFlyingType flyingType = eFlyingType.SINUS_RANDOM_STRAIGHT;

	public enum eAttackType {ATTACK_CHARGE=0,ATTACK_SHOT=1};
	public eAttackType attackType = eAttackType.ATTACK_CHARGE;

	//for attack
	public float attackSpeed = 7.0f;
	public float flyingBackSpeed = 4.0f;
	private Vector3 originBeforeAttackPos = Vector3.zero;
	public enum eAttackState {ATTACK_NONE = 0,ATTACK_STAGE1=1,ATTACK_STAGE2=2};
	private eAttackState iAttackState;
	private bool isCharge = false;



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

	// Use this for initialization
	void Start () 
	{
		xDirection = 1;

		targetSpeed.x = speedX;
		acceleration.x = accelerationX;

		origin = transform.position;
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
		float y = transform.position.y;
		Vector3 movement = new Vector3(0.0f,0.0f,transform.position.z);

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
		movement.x += x;
		movement.y = yInc;
		transform.position = movement;
	}

	private void attackCharge(GameObject player, Vector3 target)
	{
		Vector3 enemyPos = transform.position;
		if ( iAttackState == eAttackState.ATTACK_NONE && lastPlayerPos == Vector3.zero)
		{
			//we are in attack range
			originBeforeAttackPos = transform.position;
			lastPlayerPos = target;
			lastPlayerPos.y += (player.GetComponent<BoxCollider2D>().size.y - player.GetComponent<BoxCollider2D>().size.y/3);
			iAttackState = eAttackState.ATTACK_STAGE1;
			isCharge = true;
		}
		else if (iAttackState == eAttackState.ATTACK_STAGE1) 
		{
			transform.position = Vector3.MoveTowards(transform.position,lastPlayerPos,attackSpeed*Time.deltaTime);

			//charged to latest player pos
			if (transform.position == lastPlayerPos)
			{
				lastPlayerPos = Vector3.zero;
				iAttackState = eAttackState.ATTACK_STAGE2;
			}
		}
		else if (iAttackState == eAttackState.ATTACK_STAGE2)
		{
			//flying back a bit for new charge
			transform.position = Vector3.MoveTowards(transform.position,originBeforeAttackPos,flyingBackSpeed*Time.deltaTime);
			if (transform.position == originBeforeAttackPos)
			{
				iAttackState = eAttackState.ATTACK_NONE;
				originBeforeAttackPos = Vector3.zero;
				isCharge = false;
			}
		}
	}

	private void manageAttack()
	{
		GameObject player = GameObject.Find("Player");
		Vector3 playerPos = player.gameObject.transform.position;
		Vector3 enemyPos = transform.position;
		
		float distance = Vector3.Distance(enemyPos,playerPos);
		
		//if get in range trigger attack
		if (distance <= attackDistance)
		{
			switch (attackType)
			{
				case eAttackType.ATTACK_CHARGE:
					attackCharge(player,playerPos);
					break;
				case eAttackType.ATTACK_SHOT:
					attackShot (player,playerPos);
					break;
			}
		}
		else
		{
			isCharge = false;
			iAttackState = eAttackState.ATTACK_NONE;
		}
	}

	// Update is called once per frame
	void Update () 
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
				transform.position += new Vector3(0.2f,currentSpeed.y * Time.deltaTime,0.0f);

				if (scale.x <= 0.0f)
					iDieState = eDieState.DIE_DONE;

				return;
				break;
			}
			case eDieState.DIE_DONE:
				die ();
				return;
				break;
		}
		manageAttack();

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

	void msg_die()
	{
		GetComponent<BoxCollider2D>().enabled = false;
		iDieState = eDieState.DIE_START;
		//die();
	}

	//collsions
	void OnCollisionEnter2D(Collision2D collision)
	{				
		if (collision.gameObject.tag == "player")
		{
			lastPlayerPos = Vector3.zero;
			iAttackState = eAttackState.ATTACK_STAGE2;
			collision.gameObject.SendMessage("msg_hit",null,SendMessageOptions.RequireReceiver);
		}
	}
}
