using UnityEngine;
using System.Collections;

public class cFlyingEnemy : cEnemy {

	public float speed = 2.0f;
	private float speedY = 0.0f;
	public float xLengthTurning = 20.0f;

	private enum eDieState {DIE_NONE = 0, DIE_START = 1,DIE_DONE = 2};
	private eDieState iDieState = eDieState.DIE_NONE;

	public enum eFlyingType {SINUS_RANDOM_STRAIGHT=0,SINUS_LOOP=1,SINUS_CIRCLE=2};
	public eFlyingType flyingType = eFlyingType.SINUS_RANDOM_STRAIGHT;

	public enum eAttackType {ATTACK_CHARGE=0,ATTACK_SHOT=1};
	public eAttackType attackType = eAttackType.ATTACK_CHARGE;
	public float circleSize = 2.0f;

	private int xDirection;

	//for random straight
	private float xTurningTemp;
	private float ampH;
	private float yCurrentDir;

	//for looping
	private float yOrigin;
	private int originCounter = 0;
	private float yMarkCheck = 0.0f;

	//for circle
	private float angleTemp = 0.0f;


	//for attack
	public float attackSpeed = 7.0f;
	public float flyingBackSpeed = 4.0f;
	private Vector3 originBeforeAttackPos = Vector3.zero;
	public enum eAttackState {ATTACK_NONE = 0,ATTACK_STAGE1=1,ATTACK_STAGE2=2};
	private eAttackState iAttackState;
	private bool isCharge = false;



	// Use this for initialization
	void Start () 
	{
		xTurningTemp = 0.0f;
		xDirection = 1;
		ampH = 1.0f;
		yCurrentDir = 1;

		yOrigin = transform.position.y;

		speedY = speed;
		
	}
	
	private void movementAirLoop()
	{
		float x = transform.position.x;
		float y = transform.position.y;
		Vector3 movement = new Vector3(0.0f,0.0f,transform.position.z);

		//checks if we got some distance between last check
		if (yMarkCheck != 0.0f && (  cFunction.xor( ((yMarkCheck + 0.5f) > y) , ((yMarkCheck - 0.5f) < y)  )  ))
		{
			yMarkCheck = 0.0f;
		}

		if (yMarkCheck == 0.0f && y <= yOrigin+0.15f && y >= yOrigin-0.15f)
		{
			//turning point
			yMarkCheck = y;
			originCounter++;
			if (originCounter >= 3)
			{
				originCounter = 0;
				xDirection *= -1;
			}
		}

		float yInc = Mathf.Sin (x)/0.2f * Time.deltaTime;
		y += yInc;
		x += (speed * xDirection) * Time.deltaTime;

		movement.x = x;
		movement.y = y;
		
		transform.position = movement;
	}
	private void movementAirCircle()
	{
		float x = transform.position.x;
		float y = transform.position.y;
		Vector3 movement = new Vector3(0.0f,0.0f,transform.position.z);

		angleTemp += speed;
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
		
		xTurningTemp += speed * Time.deltaTime;
		if (xTurningTemp >= xLengthTurning)
		{
			//turning point
			xDirection *= -1;
			xTurningTemp = 0.0f;
		}
		float yInc = Mathf.Sin (2*x)/ampH * Time.deltaTime;
		float sign = Mathf.Sign (yInc);
		
		if (sign != yCurrentDir)
		{
			yCurrentDir = sign;
			ampH = Random.Range (0.2f,1.0f);
		}
		y += yInc;
		x += (speed * xDirection) * Time.deltaTime;
		
		movement.x = x;
		movement.y = y;
		
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
				speedY += -9.81f * Time.deltaTime;
				transform.position += new Vector3(0.2f,speedY * Time.deltaTime,0.0f);

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
