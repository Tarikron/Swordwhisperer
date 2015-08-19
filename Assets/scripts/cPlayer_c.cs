using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using XInputDotNetPure;


//conflict ??

[RequireComponent(typeof(SkeletonAnimation))]
[RequireComponent(typeof(PlayerPhysics))]
public class cPlayer_c : cUnit 
{
	public enum eSwordTakeAfter {NONE = 0, WHILE = 1, DONE = 2};
	private eSwordTakeAfter iSwordTakeAfter = eSwordTakeAfter.NONE;

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
	public Canvas helpMoveCanvas;

	private float helpMoveTimer = 0.0f;
	public float helpMoveTime = 5.0f;
	public float helpFadeTime = 1.0f;
	private bool wakeup = false;

	public float jumpHeight = 8.0f;
	public float jumpTime = 2.0f;
	public float accelerationY = 10.0f;
	
	public float accelerationX = 10.0f;
	public float hurtVelocity = 3.0f;
	public float walkVelocity = 3.0f;
	public float runVelocity = 6.0f;

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
	private int iJumpCounter = 0;
	private Vector2 movementDirection;
	private bool bSkipMovementForAnim = false;
	private bool bSkipAnimForAttack = false;

	private float attackDelayCounter = 0.0f;
	public float attackDelay = 0.1f;
	private bool attackNext = false;
	private eAttackType attackState = 0;
	private int attackCounter = 0;
	private float attackResetCurrent = 0.0f;
	public float attackResetTime = 0.2f;
	public float attackDmg = 1.0f;

	private bool bExternCutScene = false;
	private bool bLockScene = false;
	private struct stHitBoxes
	{
		public BoxCollider2D hitBox_attack_123;
		public BoxCollider2D hitBox_attack_3;
		public int maxDelay;
		public int current;
	}
	private stHitBoxes hitBoxes;

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

		[SpineAnimation]
		public string sleep;

	}
	public animPlayer animations;

	public List<AudioClip> audioClipsSteps;
	public List<AudioClip> audioClipsRun;
	public AudioSource audioSourceSteps;

	[System.Serializable]
	public struct audioSFX
	{
		public AudioClip attack1;
		public AudioClip attack2;
		public AudioClip attack3;

		public AudioClip attackVoice1;
		public AudioClip attackVoice2;
		public AudioClip attackVoice3;

		public AudioClip damageVoice1;
		public AudioClip damageVoice2;
		public AudioClip damageVoice3;
	}
	public audioSFX audioClipsSFX;
	public AudioSource audioSourceSFX;
	public AudioSource audioSourceSFX2;

	public int damageBlinkCount = 3;
	private int damageBlinkCounter = 0;
	private bool tookDamage = false;
	public float damageBlinkInterval = 0.2f;
	private float damageBlinkTimer = 0;
	private bool flyingBack = false;

	private Vector3 lastDirection = Vector3.zero;
	private float angleToFlyback = 0.0f;

	private bool bBlackScreenGone = false;

	private float hurtTimer = 0.0f;
	private float hurtTime = 2.0f;

	public float timeToGetOneLife = 2.0f;
	private float lifeTimer = 0.0f;
	public ParticleSystem PowerUp;
	private bool playback = true;

	private bool didDialog = false;

	public float OneLifePerTime = 3.0f;
	public float OneLifeTimer = 0.0f;

	private bool endScene = false;

	void playPowerUp(bool playbackspeed)
	{
		PowerUp.enableEmission = true;
		if (!PowerUp.isPlaying)
			PowerUp.Play ();
		if (!playbackspeed)
			PowerUp.Stop();
		ParticleSystem[] systems = PowerUp.gameObject.GetComponentsInChildren<ParticleSystem>();
		
		for (int i = 0; i < systems.Length; i++)
		{
			ParticleSystem ps = systems[i];
			if (ps)
			{
				ps.enableEmission = true;
				if (!PowerUp.isPlaying)
					ps.Play();
				if (!playbackspeed)
					ps.Stop();
			}
		}
	}

	void handleSwordAfterPickup()
	{
		if (iSwordTakeAfter == eSwordTakeAfter.WHILE)
		{
			bSkipMovementForAnim = true;
			bCutscene = true;
			if (timeToGetOneLife <= lifeTimer)
			{
				if (currentLife < startLife)
					currentLife++;

				lifeTimer = 0.0f;
			}
			lifeTimer += Time.deltaTime;

			playPowerUp(playback);

			ui.txtLife.text = currentLife+" Life";
			if (currentLife == startLife)
			{
				dialog.SendMessage("msg_eventTrigger","afterSwordTake",SendMessageOptions.RequireReceiver);

				playback = false;
				playPowerUp(playback);
				iSwordTakeAfter = eSwordTakeAfter.NONE;
				bSkipMovementForAnim = false;
				bCutscene = false;
			}
		}

	}

	void handleSwordPickup()
	{
		GameObject swTakePos = GameObject.Find("swordTakePosition");
		Vector3 enemyPos = swTakePos.transform.position;
		Vector3 playerPos = this.gameObject.transform.position;
		
		float distance = Vector2.Distance (playerPos,enemyPos);
		if (distance <= 4 && !sword.bCollectedSword && iAnimTakeSword == eAnimTakeSword.ANIM_NONE) 
		{
			dialog.SendMessage("msg_eventTrigger","sword_take",SendMessageOptions.RequireReceiver);

			if (Input.GetButtonDown("CtrlBButton"))
			{
				iAnimTakeSword = eAnimTakeSword.ANIM_WALK;
				dialog.SendMessage("msg_eventTriggerEnd","sword_take",SendMessageOptions.RequireReceiver);
			}
		}
		else if (sword.bCollectedSword && distance <= 8 && iAnimTakeSword != eAnimTakeSword.ANIM_NONE)
			dialog.SendMessage("msg_eventTriggerEnd","sword_take",SendMessageOptions.RequireReceiver);

		switch (iAnimTakeSword)
		{
			case eAnimTakeSword.ANIM_DONE:
			{
				
				//start swordAfterPickEvent
				iSwordTakeAfter = eSwordTakeAfter.WHILE;
				animHandler.addAnimation(animations.idle_sword,true);
				//Camera mainCam = Camera.main;
				//GameCamera gameCam = mainCam.GetComponent<GameCamera>();
				//gameCam.decreaseFactor = 0.5f;

				GameObject vineSoul = GameObject.FindGameObjectWithTag("vineSoul");
				GameObject playerSoul = GameObject.FindGameObjectWithTag("playerSoul");

				playerSoul.SendMessage("msg_vineSoul",vineSoul.transform.position,SendMessageOptions.RequireReceiver);
				vineSoul.SetActive(false);

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
				bSkipMovementForAnim = true;
				bCutscene = true;
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
				dialog.SendMessage("msg_eventTriggerEnd","sword_take",SendMessageOptions.RequireReceiver);

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
			Vector3 v1 = Vector3.zero;
			Vector3 v2 = Vector3.zero;

			v1.x = this.gameObject.transform.position.x;
			v2.x = go.transform.position.x - 2.0f;

			float distance = Vector2.Distance (v1,v2);

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
				{
					if (didDialog == false)
					{
						dialog.SendMessage("msg_eventTrigger","outOfCave",SendMessageOptions.RequireReceiver);
						didDialog = true;
					}
					bCanJump = true;

				}
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

		if  (absX > 0.7f && sword.bCollectedSword == true && !tookDamage)
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
		{
			if (!bFalling && anim.sAnimation == animations.jump_fall)
			{
				if (sword.bCollectedSword)
					animationToPlay = animations.idle_sword;
				else
					animationToPlay = animations.idle_nosword;
			}
			if (!audioSourceSteps.isPlaying)
			{
				if (animationToPlay == animations.run_sword)
				{
					int i = Random.Range(0,audioClipsRun.Count-1);
					audioSourceSteps.clip = audioClipsRun[i];
					audioSourceSteps.Play();
				}
				else if (animationToPlay == animations.walk_sword || animationToPlay == animations.walk_nosword)
				{
					int i = Random.Range(0,audioClipsSteps.Count-1);
					audioSourceSteps.clip = audioClipsSteps[i];
					audioSourceSteps.Play();
				}
			}
			if (!bSkipAnimForAttack && !bFalling)
				animHandler.addAnimation (animationToPlay, animLoop);
		}
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
		
		movementDirection.x = currentSpeed.x * Time.deltaTime;
		movementDirection.y = 0.0f;

		handleCaveExitFade(x);

	} //end - handleMovement

	void handleJump(float x)
	{
		if (Input.GetButtonDown ("jump"))
		{
			if (playerPhysics.grounded)
				iJumpCounter = 0;

			iJumpCounter++;
			if (iJumpCounter <= 2)
			{
				bJumping = true;
				if (!bSkipAnimForAttack)
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

	void handleBeamAttack(float x)
	{
		GameObject playerSoul = GameObject.FindGameObjectWithTag("playerSoul");

		if (playerSoul.GetComponent<cPlayerSoul>().beamAttack)
		{
			playerSoul.GetComponent<cPlayerSoul>().rotateKaMeHaMeHa(x);
		}
		else
		{
			if (Input.GetButtonDown ("beamAttack")) 
			{
				playerSoul.GetComponent<cPlayerSoul>().beamAttack = true;
				bSkipMovementForAnim = true;
			}
		}
	}

	void handleAttack()
	{
		//enable collider after delay
		if (hitBoxes.current >= hitBoxes.maxDelay)
		{
			hitBoxes.current = 0;
			hitBoxes.hitBox_attack_123.enabled = true;
			if (attackState == eAttackType.ATTACK_3)
				hitBoxes.hitBox_attack_3.enabled = true;

		}
		hitBoxes.current++;
		
		AudioClip attack = null;
		AudioClip attackVoice = null;

		/**/
		attackResetCurrent += Time.deltaTime;
		if (Input.GetButtonDown ("attack")) 
		{
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
				attack = audioClipsSFX.attack1;
				attackVoice = audioClipsSFX.attackVoice1;
				break;
			case eAttackType.ATTACK_2:
			{
				if (attackCounter <= 1)
				{
					attackAnim = "";
				}
				else	
				{
					attackAnim = animations.attack2;
					attack = audioClipsSFX.attack2;
					attackVoice = audioClipsSFX.attackVoice2;
				}
				break;
			}
			case eAttackType.ATTACK_3:
				if (attackCounter <= 2)
				{
					attackAnim = "";
				}
				else
				{
					attackAnim = animations.attack3;
					attack = audioClipsSFX.attack3;
					attackVoice = audioClipsSFX.attackVoice3;
				}
				break;
			default:
				attackAnim = "";
				hitBoxes.hitBox_attack_123.enabled = false;
				hitBoxes.hitBox_attack_3.enabled = false;
				break;
		}
		if (attackNext && attackAnim != "")
		{
			attackDelayCounter += Time.deltaTime;
			if (attackDelay <= attackDelayCounter)
			{
				bSkipAnimForAttack = true;
				attackDelayCounter = 0.0f;
				animHandler.timescale = 0.7f;
				if (!audioSourceSFX.isPlaying)
				{
					if (attack != null)
					{
						audioSourceSFX.clip = attack;
						audioSourceSFX.Play();
					}
					if (attackVoice != null)
					{
						audioSourceSFX2.clip = attackVoice;
						audioSourceSFX2.Play();
					}
				}
				GamePad.SetVibration(0,0.2f,0.2f);
				animHandler.addAnimation(attackAnim,false);
			}
		}
		else
			bSkipAnimForAttack = false;
		attackNext = false;
	}

	// Use this for initialization
	public override void Start () 
	{
		base.Start ();

		bCutscene = true;
		sleepAnim = true;

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		playerPhysics = GetComponent<PlayerPhysics>();

		sword.bCollectedSword = false;
		sword.sSword = "nosword";
		animHandler = new cAnimationHandler(skeletonAnimation);
		animHandler.delStart = startAnimListener;
		animHandler.delEnd = endAnimListener;

		ui.txtLife.text = currentLife+" Life";
		
		hitBoxes.hitBox_attack_123 = transform.FindChild("hitBox_attack_123").gameObject.GetComponent<BoxCollider2D>();
		hitBoxes.hitBox_attack_3 = transform.FindChild("hitBox_attack_3").gameObject.GetComponent<BoxCollider2D>();

		//sword.bCollectedSword = true;

		hitBoxes.current = 0;
		hitBoxes.maxDelay = 5;

		bBlackScreenGone = false;

		hurtTime = 0.0f;
		hurtTimer = 0.0f;
	
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

		if (endScene)
		{
			animHandler.addAnimation(animations.sleep,true);
			animHandler.playAnimation();
			return;
		}

		if (!bBlackScreenGone)
			return;
		if (bExternCutScene || bLockScene)
		{
			animHandler.addAnimation(animations.idle_sword,true);
			animHandler.playAnimation();

			return;
		}

		/*if (!sword.bCollectedSword)
		{
			if (hurtTime <= hurtTimer)
			{
				hurtTime = Random.Range (2.0f,4.0f);

				Camera mainCam = Camera.main;
				GameCamera gameCam = mainCam.GetComponent<GameCamera>();

				gameCam.startShake();
				hurtTimer = 0.0f;
			}
			hurtTimer += Time.deltaTime;
		}*/
		//tookDamage = false;
		if (tookDamage)
		{
			if (damageBlinkTimer <= 0.0f)
			{
				if (skeletonAnimation.skeleton.g == 0.0f)
				{
					makeNormal();
					if (damageBlinkCounter == damageBlinkCount)
					{
						tookDamage = false;
						damageBlinkCounter = 0;
					}
				}
				else
				{
					makeRed();
					damageBlinkCounter++;
				}
				damageBlinkTimer = damageBlinkInterval;
			}
			damageBlinkTimer-=Time.deltaTime;
		}

		if (sword.bCollectedSword && currentLife < startLife)
		{
			if (OneLifePerTime <= OneLifeTimer)
			{
				currentLife+=1.0f;
				OneLifeTimer = 0.0f;
			}
			OneLifeTimer += Time.deltaTime;
		}

		if (false)
		{
			movementDirection.x = 0.0f;
			movementDirection.y = 0.0f;
			//falling back 
			if (angleToFlyback > 0 && angleToFlyback < 90)
			{
				//initialize values

				if (currentSpeed.x == 0.0f)
					currentSpeed.x = -20.0f;
				else
				{
					currentSpeed.x *= -1.0f;
				}
			}

			currentSpeed.x = currentSpeed.x * Mathf.Cos(angleToFlyback * Mathf.PI/180);
			currentSpeed.y = -accelerationY * Time.deltaTime + currentSpeed.y * Mathf.Sin(angleToFlyback * Mathf.PI/180);

			movementDirection.x = currentSpeed.x * Time.deltaTime;
			float newSX = transform.position.x + movementDirection.x;
			movementDirection.y = (-accelerationY * newSX*newSX);
			movementDirection.y /= (2 * (currentSpeed.x * currentSpeed.x));
			movementDirection.y += Mathf.Tan (angleToFlyback * Mathf.PI/180)*newSX;

			lastDirection = playerPhysics.Move (movementDirection);

			if (playerPhysics.grounded)
				flyingBack = false;

			return;

		}

		float x = Input.GetAxis("Horizontal");
		float absX = Mathf.Abs(x);
		if (sleepAnim)
		{
			dialog.SendMessage("msg_eventTrigger","wakeUp",SendMessageOptions.RequireReceiver);

			if (helpMoveTimer >= helpMoveTime)
			{
				helpMoveCanvas.GetComponent<CanvasGroup>().alpha += Time.deltaTime/helpFadeTime;
				if (helpMoveCanvas.GetComponent<CanvasGroup>().alpha > 1.0f)
					helpMoveCanvas.GetComponent<CanvasGroup>().alpha = 1.0f;
			}
			else 
				helpMoveTimer += Time.deltaTime;
		}
		if (sleepAnim && absX>0)
		{
			animHandler.addAnimation(animations.wakeup,false);
			bCutscene = true;
			sleepAnim = false;
			wakeup = true;
		}
		if (wakeup)
		{
			helpMoveCanvas.GetComponent<CanvasGroup>().alpha -= Time.deltaTime/helpFadeTime;
			if (helpMoveCanvas.GetComponent<CanvasGroup>().alpha <= 0.0f)
			{
				helpMoveCanvas.GetComponent<CanvasGroup>().alpha = 0.0f;
				wakeup = false;
			}
		}
		
		handleSwordPickup();
		handleSwordAfterPickup();

		//skip all related input/movement/loop animation for cutscene
		if (bCutscene == false)
		{

			if ((x >= 0.0f && x <= 0.05f) || (x <= 0.0f && x >= -0.05f ))
				x = 0.0f;

			if (sword.bCollectedSword)
			{
				handleAttack ();
				handleBeamAttack(x);
			}
			if (!bSkipMovementForAnim)
			{
				//movement
				if (bCanJump && sword.bCollectedSword)
					handleJump (x);

				movementDirection.y = 0.0f;
				//jumps & gravitation
				//we start with jumpVelocity, now we decrease it
				if (currentSpeed.y > 0.0f)
					currentSpeed.y -= accelerationY * Time.deltaTime; 
				else
				{
					if (playerPhysics.grounded == false && playerPhysics.onSlope == false)
					{
						//we are falling if we have negative speedY and we are not grounded
						if (sword.bCollectedSword && bCanJump)
						{
							float distanceGround = playerPhysics.GetDistanceToGround();

							if  (distanceGround > 0.5f)
							{
								animHandler.addAnimation(animations.jump_fall,true);
								bFalling = true;
							}
						}
						//we want to gain speed if we are falling
						currentSpeed.y -= accelerationY * Time.deltaTime; 
					}
					else
					{
						//movement is handled earlier, if we jump we need to know if we go to idle
						bFalling = false;
						//reset to normal gravity 
						currentSpeed.y = -accelerationY * Time.deltaTime; 
					}
					bJumping = false;
				}
				handleMovement (x);

				//Debug.Log("jump speed: " + currentSpeedY);
				movementDirection.y = currentSpeed.y * Time.deltaTime;
				lastDirection = playerPhysics.Move (movementDirection);
				//rb2D.MovePosition( rb2D.position  + movementDirection);
			}
		}
		
		animHandler.playAnimation();

	}

	public void moveAgain()
	{
		bSkipMovementForAnim = false;
	}

	public bool isCutscene()
	{
		return bExternCutScene || bCutscene;
	}

	public eSwordTakeAfter GetSwordTakeState()
	{
		return iSwordTakeAfter;
	}
	public Vector2 GetCurrentSpeed()
	{
		return currentSpeed;
	}
	public bool GetBlackSreenState()
	{
		return bBlackScreenGone;
	}


	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.gameObject.tag == "enemyFlyingHurtBox")
		{
			collider.gameObject.SendMessage("msg_damage",attackDmg,SendMessageOptions.RequireReceiver);
			GamePad.SetVibration(0,1.0f,1.0f);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{

		if ((collision.gameObject.layer & LayerMask.NameToLayer("ground")) ==  LayerMask.NameToLayer("ground"))
		{
			iJumpCounter = 0;
		}

		if (collision.gameObject.tag == "soul")
		{
			GameObject.FindGameObjectWithTag("playerSoul").SendMessage("collectSoul",null,SendMessageOptions.DontRequireReceiver);
			collision.gameObject.SetActive(false);
		}
		else if (collision.gameObject.name == "groundGameEnd")
		{
			Application.LoadLevel("swordwhisperer");
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

		if (animName == animations.wakeup || animName == animations.walk_end_nosword || animName == animations.walk_end_sword ||
		    animName == animations.attack)
		{
			bSkipMovementForAnim = false;
			bCutscene = false; // for wakeup
			sleepAnim = false;
			iAnimWalk = eAnimWalk.ANIM_END;
			if (animName == animations.wakeup)
				currentLife = 6;

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
			GamePad.SetVibration(0,0.0f,0.0f);
			if (attackState != eAttackType.ATTACK_NONE)
			{
				animHandler.timescale = 1.0f;
				attackNext = true;
				attackState = eAttackType.ATTACK_2;;
			}


		}
		else if (animName == animations.attack2)
		{
			GamePad.SetVibration(0,0.0f,0.0f);
			if (attackState != eAttackType.ATTACK_NONE)
			{
				animHandler.timescale = 1.0f;
				attackNext = true;
				attackState = eAttackType.ATTACK_3;
			}

		}
		else if (animName == animations.attack3)
		{
			GamePad.SetVibration(0,0.0f,0.0f);
			animHandler.timescale = 1.0f;
			attackNext = false;
			attackState = eAttackType.ATTACK_NONE;
			attackCounter = 0;
			bSkipAnimForAttack = false;
		}
		//Debug.Log ("end:" + animName + "  " + attackCounter);

		
	}

	//########################################
	//################# Receiver/Messages ###########
	//########################################

	void msg_looseLife()
	{
		endScene = true;
	}

	void msg_looseStrength()
	{
		sword.bCollectedSword = false;
	}

	void msg_stopMovementStart()
	{
		bLockScene = true;
	}
	void msg_stopMovementEnd()
	{
		bLockScene = false;
	}

	void msg_externCutsceneStart()
	{
		bExternCutScene = true;
	}
	void msg_externCutsceneEnd()
	{

		bExternCutScene = false;

		//little bit dirty here... but we only have one event
		dialog.SendMessage("msg_eventTrigger","afterCamDrive",SendMessageOptions.RequireReceiver);

	}

	void msg_blackscreenArrive()
	{
		bBlackScreenGone = false;
	}
	void msg_blackscreenGone()
	{
		bBlackScreenGone = true;
	}

	void makeRed()
	{
		skeletonAnimation.skeleton.r = 1.0f;
		skeletonAnimation.skeleton.b = 0.3f;
		skeletonAnimation.skeleton.g = 0.0f;
		skeletonAnimation.skeleton.a = 1.0f;
	}
	void makeNormal()
	{
		skeletonAnimation.skeleton.r = 1.0f;
		skeletonAnimation.skeleton.b = 1.0f;
		skeletonAnimation.skeleton.g = 1.0f;
		skeletonAnimation.skeleton.a = 1.0f;
	}


	void msg_hit(float dmg)
	{
		if (!tookDamage)
		{
			takeDmg(dmg);

			makeRed ();

			Camera mainCam = Camera.main;
			GameCamera gameCam = mainCam.GetComponent<GameCamera>();
			gameCam.startShake();
			gameCam.decreaseFactor = 0.5f;


			ui.txtLife.text = currentLife+" Life";
		
			angleToFlyback = Vector3.Angle(lastDirection,new Vector3(1.0f,0.0f,0.0f) );
			tookDamage = true;
			flyingBack = true;

			playDamageVoiceClips();
		}
	}
	public Vector3 getMovement()
	{
		return lastDirection;
	}


	//########################################
	//################# Sounds ###########
	//########################################
	
	private void playDamageVoiceClips(){
		switch (UnityEngine.Random.Range(0,3)) {
			
		case 0:
			audioSourceSFX2.PlayOneShot(audioClipsSFX.damageVoice1);
			break;
			
		case 1:
			audioSourceSFX2.PlayOneShot(audioClipsSFX.damageVoice2);
			break;
			
		case 2:
			audioSourceSFX2.PlayOneShot(audioClipsSFX.damageVoice3);
			break;
			
		default:
			break;
		}
	}
		
}
