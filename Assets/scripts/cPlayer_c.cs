using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


//conflict ??

[RequireComponent(typeof(SkeletonAnimation))]
[RequireComponent(typeof(PlayerPhysics))]
public class cPlayer_c : cUnit 
{
	enum eAttackType {ATTACK_NONE = 0 , ATTACK_1 = 1, ATTACK_2 = 2, ATTACK_3 = 3};
	enum eAnimTakeSword {ANIM_NONE = 0,ANIM_START,ANIM_WALK,ANIM_TAKESTART,ANIM_TAKEIDLE,ANIM_DONE};
	enum eAnimWalk{ANIM_NONE = 0,ANIM_WALK,ANIM_END};
	public enum eAnimAxis {pX=1,nX=-1,pY=1,nY=-1};

	[System.Serializable]
	public struct fadeCaveExit
	{
		public eAnimAxis fadeInAxis;
		public eAnimAxis fadeOutAxis;
		public float fadeDistance;
	}
	public fadeCaveExit _fadeCaveExit;


	//settings via unity
	public Canvas dialog;

	public float jumpHeight = 8.0f;
	public float jumpTime = 2.0f;
	public float accelerationY = 10.0f;
	
	public float accelerationX = 10.0f;
	public float hurtVelocity = 3.0f;
	public float walkVelocity = 3.0f;
	public float runVelocity = 6.0f;
	public int life = 20;

	//animation stuff
	cAnimationHandler animHandler;
	private bool bCutscene = false;
	private bool sleepAnim = true;
	private eAnimTakeSword iAnimTakeSword = 0;
	private eAnimWalk iAnimWalk = 0;
	private SkeletonAnimation skeletonAnimation;

	//gameplay related
	// - movement and jump related
	[HideInInspector]
	public PlayerPhysics playerPhysics;
	private bool bCanJump = false;
	private bool bJumping = false;
	private bool bFalling = false;
	private int iGroundBridge = 0; //quick dirty solution
	private int iJumpCounter = 0;
	private float fTempTimeGravity = 0.0f;
	private Vector2 movementDirection;
	private float jumpDestHeight = 0.0f;
	private bool bSkipMovementForAnim = false;

	private float attackDelayCounter = 0.0f;
	public float attackDelay = 0.1f;
	private bool attackNext = false;
	private bool attackCombo = false;
	private eAttackType attackState = 0;
	private int attackCounter = 0;
	private float attackResetCurrent = 0.0f;
	public float attackResetTime = 0.2f;

	//sword
	private struct stSword
	{
		public bool bCollectedSword;
		public string sSword;
	}
	private stSword sword;


	//interactible UI Stuff
	[System.Serializable]
	public struct uiIngame
	{
		public Text txtLife;
	}
	public uiIngame ui;

	[System.Serializable]
	public struct animPlayer
	{
		[SpineAnimation]
		public string wakeup;

		//walk
		[SpineAnimation]
		public string walk_start_nosword;
		[SpineAnimation]
		public string walk_nosword;
		[SpineAnimation]
		public string walk_end_nosword;
		[SpineAnimation]
		public string walk_start_sword;
		[SpineAnimation]
		public string walk_sword;
		[SpineAnimation]
		public string walk_end_sword;

		[SpineAnimation]
		public string idle_sword;
		[SpineAnimation]
		public string idle_nosword;

		[SpineAnimation]
		public string run_sword;
		[SpineAnimation]
		public string jump_sword;
		[SpineAnimation]
		public string attack;
		[SpineAnimation]
		public string attack1;
		[SpineAnimation]
		public string attack2;
		[SpineAnimation]
		public string attack3;

		[SpineAnimation]
		public string swordtake_start;
		[SpineAnimation]
		public string swordtake_idle;
		[SpineAnimation]
		public string swordtake_end;

		[SpineAnimation]
		public string jump_fall;

	}
	public animPlayer animations;

	void handleSwordPickup()
	{
		GameObject swTakePos = GameObject.Find("swordTakePosition");
		Vector3 enemyPos = swTakePos.transform.position;
		Vector3 playerPos = this.gameObject.transform.position;
		
		float distance = Vector2.Distance (playerPos,enemyPos);
		if (distance <= 4) 
		{
			dialog.SendMessage("msg_eventTrigger","sword_take",SendMessageOptions.RequireReceiver);

			if (Input.GetButtonDown("CtrlBButton") && iAnimTakeSword == eAnimTakeSword.ANIM_NONE)
				iAnimTakeSword = eAnimTakeSword.ANIM_WALK;
		}
		else
			dialog.SendMessage("msg_eventTriggerEnd",null,SendMessageOptions.RequireReceiver);

		switch (iAnimTakeSword)
		{
			case eAnimTakeSword.ANIM_DONE:
			{
				sword.bCollectedSword = true;
				sword.sSword = "sword";
				bSkipMovementForAnim = false;
				bCutscene = false;
				iAnimTakeSword = eAnimTakeSword.ANIM_NONE;
				//animHandler.addAnimation(animations.walk_end_sword,false);
				break;
			}
			case eAnimTakeSword.ANIM_START:
			{
				iAnimTakeSword = eAnimTakeSword.ANIM_START;
				break;
			}
			case eAnimTakeSword.ANIM_TAKESTART:
			{
				animHandler.addAnimation(animations.swordtake_idle,false,0.0f,true);
				break;
			}
			case eAnimTakeSword.ANIM_TAKEIDLE:
			{
				GameObject vine = GameObject.Find("vine");
				vine.SendMessage("msg_startanim",null,SendMessageOptions.RequireReceiver);
				animHandler.addAnimation(animations.swordtake_end,false,0.0f,true);
				break;
			}
			case eAnimTakeSword.ANIM_WALK:
			{
				dialog.SendMessage("msg_eventTriggerEnd",null,SendMessageOptions.RequireReceiver);

				transform.position = Vector3.MoveTowards(playerPos,enemyPos,Time.deltaTime * walkVelocity);
				
				if (transform.position == enemyPos)
				{
					iAnimTakeSword = eAnimTakeSword.ANIM_START;
					animHandler.addAnimation(animations.swordtake_start,false,0.0f,true);
					bCutscene = true;
				}
				else
				{
					if (sword.bCollectedSword)
						animHandler.addAnimation(animations.walk_sword,true,0.0f,true);
					else
					{
						animHandler.addAnimation(animations.walk_nosword,true,0.0f,true);
					}
				}
				break;
			}
		}
	}

	//x - movement direction
	void handleCaveExitFade(float x)
	{
		GameObject[] go_s = GameObject.FindGameObjectsWithTag("fadeCaveExit");

		for (int i=0; i< go_s.Length; i++)
		{
			GameObject go = go_s[i];
			float distance = Vector2.Distance (this.gameObject.transform.position,go.transform.position);
			if (distance <= _fadeCaveExit.fadeDistance)
			{
				Color c = go.GetComponent<SpriteRenderer>().color;

				if (x < 0)
					c.a -= 0.05f;
				else if (x > 0)
					c.a += 0.05f;

				if (c.a < 0.0f)
					c.a = 0.0f;
				if (c.a > 1.0f)
					c.a = 1.0f;


				if (c.a  >= 1.0f)
					bCanJump = true;
				else
					bCanJump = false;

				go.GetComponent<SpriteRenderer>().color = c;
					
			}
		}
	}

	void handleMovement(float x)
	{
		float absX = Mathf.Abs(x);
		float velocity = 0.0f;

		cAnimation anim = animHandler.getCurrent();
		bool animLoop = false;
		string animationToPlay = "";

		if  (absX > 0.7f && sword.bCollectedSword == true)
		{
			animationToPlay = animations.run_sword;
			animLoop = true;
			velocity = runVelocity;
		}
		else if (absX > 0) 
		{
			if (iAnimWalk == eAnimWalk.ANIM_WALK)
			{
				if (sword.bCollectedSword)
				{
					animationToPlay = animations.walk_sword;
					velocity = walkVelocity;
				}
				else
				{
					velocity = hurtVelocity;
					animationToPlay = animations.walk_nosword;
				}
				animLoop = true;
			}
			else
			{
				iAnimWalk = eAnimWalk.ANIM_NONE;
				if (sword.bCollectedSword)
				{
					animationToPlay = animations.walk_start_sword;
					velocity = walkVelocity;
				}
				else
				{
					velocity = hurtVelocity;
					animationToPlay = animations.walk_start_nosword;
				}
				animLoop = false;
			}
		}
		else
		{

			//Debug.Log ("current: " + currentAnimation);
			if (anim.sAnimation == animations.walk_sword || anim.sAnimation == animations.walk_start_sword ||
			    anim.sAnimation == animations.walk_nosword || anim.sAnimation == animations.walk_start_nosword)
			{
				animLoop = false;
				if (sword.bCollectedSword)
				{
					animationToPlay = animations.walk_end_sword;
					velocity = walkVelocity;
				}
				else
				{
					animationToPlay = animations.walk_end_nosword;
					velocity = hurtVelocity;
				}

			}
			else if (anim.sAnimation == "")
			{
				animLoop = true;
				if (sword.bCollectedSword)
					animationToPlay = animations.idle_sword;
				else
					animationToPlay = animations.idle_nosword;
			}
		}
		if (!bFalling && !bJumping)
			animHandler.addAnimation(animationToPlay,animLoop);

		float xAxis = Input.GetAxisRaw("Horizontal");
		if (xAxis < 0 && transform.eulerAngles.y < 0.1f)
			transform.RotateAround(transform.position,Vector3.up,180);
		else if (xAxis > 0 && transform.eulerAngles.y >= 180.0f)
		{
			transform.RotateAround(transform.position,Vector3.up,-180);
		}

		if (transform.eulerAngles.y >= 180.0f)
			xAxis *= -1;

			
		targetSpeed.x = velocity;
		currentSpeed.x = IncrementTowards(currentSpeed.x, targetSpeed.x, accelerationX);

		//Debug.Log (currentSpeed + "    " + targetSpeed);

		movementDirection.x = currentSpeed.x * Time.deltaTime;
		movementDirection.y = 0.0f;

		handleCaveExitFade(x);

	} //end - handleMovement

	void handleJump(float x)
	{
		if (Input.GetButtonDown ("jump"))
		{
			float absX = Mathf.Abs(x);
			
			if (playerPhysics.grounded)
				iJumpCounter = 0;

			iJumpCounter++;
			if (iJumpCounter <= 2)
			{
				bJumping = true;
				animHandler.addAnimation(animations.jump_sword,true);
				currentSpeed.y = jumpHeight/jumpTime;

				/*
				cAnimation anim = animHandler.getCurrent();
				if (anim == null)
				{
					if (sword.bCollectedSword)
						animHandler.addToQueue(animations.idle_sword,true,0.1f);
					else
						animHandler.addToQueue(animations.idle_nosword,true,0.1f);
				}
				else
					animHandler.addToQueue(anim.sAnimation,anim.bLoop,0.1f);*/
			}
		}
	}

	void handleAttack()
	{

		/**/

		attackResetCurrent += Time.deltaTime;
		if (Input.GetButtonDown ("attack")) 
		{
			Debug.Log ("attack: " + attackResetCurrent);

			if (attackCounter < 3)
				attackCounter++;
			if (attackCounter == 1)
			{
				attackState = eAttackType.ATTACK_1;
				attackNext = true;
			}
			attackResetCurrent = 0.0f;
			attackDelayCounter = 0.0f;
		}

		if (attackResetCurrent >= attackResetTime)
		{
			attackCounter = 0;
			attackState = eAttackType.ATTACK_NONE;
			attackResetCurrent = 0.0f;
			attackDelayCounter = 0.0f;
			attackNext = false;
		}
		else if (attackCounter > 0)
			attackNext = true;


		string attackAnim = "";
		switch (attackState)
		{
			case eAttackType.ATTACK_1:
				attackAnim = animations.attack1;
				break;
			case eAttackType.ATTACK_2:
			{
				if (attackCounter <= 1)
				{
					attackAnim = "";
				}
				else	
					attackAnim = animations.attack2;
				break;
			}
			case eAttackType.ATTACK_3:
				if (attackCounter <= 2)
				{
					attackAnim = "";
				}
				else
					attackAnim = animations.attack3;
				break;
			default:
				attackAnim = "";
				break;
		}
		if (attackNext && attackAnim != "")
		{
			attackDelayCounter += Time.deltaTime;
			if (attackDelay <= attackDelayCounter)
			{
				//if enemy near by
				GameObject[] flyingEnemys = GameObject.FindGameObjectsWithTag("enemyFlying");
				foreach (GameObject flyingEnemy in flyingEnemys)
				{
					if (flyingEnemy == null)
						continue;
					Vector3 enemyPos = flyingEnemy.transform.position;
					Vector3 playerPos = this.gameObject.transform.position;
					
					float distance = Vector2.Distance (playerPos,enemyPos);
					if (distance <= 4) 
					{
						flyingEnemy.SendMessage("msg_die",null,SendMessageOptions.RequireReceiver);
					}
				}
				bSkipMovementForAnim = true;
				attackDelayCounter = 0.0f;
				animHandler.timescale = 0.7f;
				animHandler.addAnimation(attackAnim,false);
			}
		}
		attackNext = false;
	}

	// Use this for initialization
	void Start () {

		jumpDestHeight = -999.0f;
		sleepAnim = true;

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		playerPhysics = GetComponent<PlayerPhysics>();

		sword.bCollectedSword = false;
		sword.sSword = "nosword";
		animHandler = new cAnimationHandler(skeletonAnimation);
		animHandler.delStart = startAnimListener;
		animHandler.delEnd = endAnimListener;

		ui.txtLife.text = life+" Life";

		//sword.bCollectedSword = true;

	}

	void FixedUpdate()
	{

	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.F2))
			Application.LoadLevel ("swordwhisperer");
		else if (Input.GetKeyDown (KeyCode.Escape))
			Application.LoadLevel("menu");

		if (sleepAnim)
		{
			animHandler.addAnimation(animations.wakeup,false);
			bCutscene = true;
			sleepAnim = false;
		}
		handleSwordPickup();
		
		//skip all related input/movement/loop animation for cutscene
		if (bCutscene == false)
		{
			float x = Input.GetAxis("Horizontal");
			
			if ((x >= 0.0f && x <= 0.05f) || (x <= 0.0f && x >= -0.05f ))
				x = 0.0f;

			if (sword.bCollectedSword)
				handleAttack ();

			if (!bSkipMovementForAnim)
			{
				//movement
				if (bCanJump && sword.bCollectedSword)
					handleJump (x);
				handleMovement (x);

				//jumps & gravitation
				//we start with jumpVelocity, now we decrease it
				if (currentSpeed.y > 0.0f)
					currentSpeed.y -= accelerationY * Time.deltaTime; 
				else
				{
					if (playerPhysics.grounded == false && playerPhysics.onSlope == false)
					{
						//we are falling if we have negative speedY and we are not grounded
						if (sword.bCollectedSword)
							animHandler.addAnimation(animations.jump_fall,true);
						bFalling = true;
						//we want to gain speed if we are falling
						currentSpeed.y -= accelerationY * Time.deltaTime; 
					}
					else
					{
						if (bFalling && movementDirection.x == 0.0f)
						{
							if (sword.bCollectedSword)
								animHandler.addAnimation(animations.idle_sword,true);
							else
								animHandler.addAnimation(animations.idle_nosword,true);
						}
						bFalling = false;
						movementDirection.y = 0.0f;
						//reset to normal gravity 
						currentSpeed.y = -accelerationY * Time.deltaTime; 
					}
					bJumping = false;
				}
				//Debug.Log("jump speed: " + currentSpeedY);
				movementDirection.y = currentSpeed.y * Time.deltaTime;
				playerPhysics.Move (movementDirection);
				//rb2D.MovePosition( rb2D.position  + movementDirection);
			}
		}

		animHandler.playAnimation();

	}
	public Vector2 GetCurrentSpeed()
	{
		return currentSpeed;
	}
	void OnCollisionEnter2D(Collision2D collision)
	{
		if ((collision.gameObject.layer & LayerMask.NameToLayer("ground")) ==  LayerMask.NameToLayer("ground"))
		{
			//bGrounded = true;
			iJumpCounter = 0;
			jumpDestHeight = -999.0f;

			iGroundBridge++;
		}

		if (collision.gameObject.name == "groundGameEnd")
		{
			Application.LoadLevel("swordwhisperer");
		}
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		if ((collision.gameObject.layer & LayerMask.NameToLayer("ground")) ==  LayerMask.NameToLayer("ground"))
		{
			iGroundBridge--;
			if (iGroundBridge <= 0)
				;//bGrounded = false;
		}
	}

	//########################################
	//################# Animation ################
	//########################################

	void startAnimListener(string animName)
	{
		//Debug.Log("start player - frames: " + Time.frameCount + "   " + animName );
	}
	void endAnimListener (string animName)
	{
		//Debug.Log("end player - " + Time.frameCount + "   " + animName);

		if (animName == animations.wakeup || 
		    animName == animations.walk_end_nosword || animName == animations.walk_end_sword ||
		    animName == animations.attack)
		{
			bSkipMovementForAnim = false;
			bCutscene = false; // for wakeup
			iAnimWalk = eAnimWalk.ANIM_END;
		}
		else if (animName == animations.walk_start_sword || animName == animations.walk_start_nosword)
			iAnimWalk = eAnimWalk.ANIM_WALK;
		else if (animName == animations.swordtake_start)
			iAnimTakeSword = eAnimTakeSword.ANIM_TAKESTART;
		else if (animName == animations.swordtake_idle)
			iAnimTakeSword = eAnimTakeSword.ANIM_TAKEIDLE;
		else if (animName == animations.swordtake_end)
			iAnimTakeSword = eAnimTakeSword.ANIM_DONE;
		else if (animName == animations.attack1)
		{
			if (attackState != eAttackType.ATTACK_NONE)
			{
				animHandler.timescale = 1.0f;
				attackNext = true;
				attackState = eAttackType.ATTACK_2;
				bSkipMovementForAnim = false;
			}

		}
		else if (animName == animations.attack2)
		{
			if (attackState != eAttackType.ATTACK_NONE)
			{
				animHandler.timescale = 1.0f;
				attackNext = true;
				attackState = eAttackType.ATTACK_3;
				bSkipMovementForAnim = false;
			}

		}
		else if (animName == animations.attack3)
		{
			animHandler.timescale = 1.0f;
			attackNext = false;
			attackState = eAttackType.ATTACK_NONE;
			attackCounter = 0;
			bSkipMovementForAnim = false;
		}
		Debug.Log ("end:" + animName + "  " + attackCounter);
		
	}

	//########################################
	//################# Receiver/Messages ###########
	//########################################

	void msg_hit()
	{
		if (life > 0)
			life--;

		ui.txtLife.text = life+" Life";
	}
		
}
