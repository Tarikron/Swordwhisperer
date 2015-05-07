using UnityEngine;
using System.Collections;
using UnityEngine.UI;


//conflict ??

[RequireComponent(typeof(SkeletonAnimation))]
[RequireComponent(typeof(PlayerPhysics))]
public class cPlayer_c : MonoBehaviour 
{

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
	public float jumpHeight = 300f;
	public float jumpTime = 10.0f;
	public float hurtVelocity = 3.0f;
	public float walkVelocity = 3.0f;
	public float runVelocity = 6.0f;
	public float gravity = 1.0f;
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
	private PlayerPhysics playerPhysics;

	private bool bJumping = false;
	private bool bFalling = false;
	private int iGroundBridge = 0; //quick dirty solution
	private int iJumpCounter = 0;
	private float fTempTimeGravity = 0.0f;
	private Vector2 movementDirection;
	private float jumpDestHeight = 0.0f;
	private bool bSkipMovementForAnim = false;


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
		if (Input.GetButtonDown("CtrlBButton") && iAnimTakeSword == eAnimTakeSword.ANIM_NONE)
		{
			GameObject swTakePos = GameObject.Find("swordTakePosition");
			Vector3 enemyPos = swTakePos.transform.position;
			Vector3 playerPos = this.gameObject.transform.position;
			
			float distance = Vector2.Distance (playerPos,enemyPos);
			if (distance <= 3) 
			{
				iAnimTakeSword = eAnimTakeSword.ANIM_WALK;
			}
		}
		switch (iAnimTakeSword)
		{
			case eAnimTakeSword.ANIM_DONE:
			{
				sword.bCollectedSword = true;
				sword.sSword = "sword";
				bSkipMovementForAnim = true;
				bCutscene = false;
				iAnimTakeSword = eAnimTakeSword.ANIM_NONE;
				animHandler.addAnimation(animations.walk_end_sword,false);
				break;
			}
			case eAnimTakeSword.ANIM_START:
			{
				iAnimTakeSword = eAnimTakeSword.ANIM_START;
				break;
			}
			case eAnimTakeSword.ANIM_TAKESTART:
			{
				animHandler.addAnimation(animations.swordtake_idle,false,true);
				break;
			}
			case eAnimTakeSword.ANIM_TAKEIDLE:
			{
				GameObject vine = GameObject.Find("vine");
				vine.SendMessage("msg_startanim",null,SendMessageOptions.RequireReceiver);
				animHandler.addAnimation(animations.swordtake_end,false,true);
				break;
			}
			case eAnimTakeSword.ANIM_WALK:
			{
				GameObject swTakePos = GameObject.Find("swordTakePosition");
				Vector3 enemyPos = swTakePos.transform.position;
				Vector3 playerPos = this.gameObject.transform.position;
				transform.position = Vector3.MoveTowards(playerPos,enemyPos,Time.deltaTime * walkVelocity);
				
				if (transform.position == enemyPos)
				{
					iAnimTakeSword = eAnimTakeSword.ANIM_START;
					animHandler.addAnimation(animations.swordtake_start,false,true);
					bCutscene = true;
				}
				else
				{
					if (sword.bCollectedSword)
						animHandler.addAnimation(animations.walk_sword,true,true);
					else
						animHandler.addAnimation(animations.walk_nosword,true,true);
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
			velocity = 1.3f * runVelocity;
		}
		else if (absX > 0) 
		{
			if (iAnimWalk == eAnimWalk.ANIM_WALK)
			{
				velocity = walkVelocity;
				if (sword.bCollectedSword)
					animationToPlay = animations.walk_sword;
				else
					animationToPlay = animations.walk_nosword;
				animLoop = true;
			}
			else
			{
				iAnimWalk = eAnimWalk.ANIM_NONE;
				if (sword.bCollectedSword)
					animationToPlay = animations.walk_start_sword;
				else
					animationToPlay = animations.walk_start_nosword;
				animLoop = false;
				velocity = 0.3f*walkVelocity;
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
					animationToPlay = animations.walk_end_sword;
				else
					animationToPlay = animations.walk_end_nosword;

				velocity = 0.3f * walkVelocity;
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
		
		movementDirection.x = velocity * Mathf.Sign(x) * Time.deltaTime;
		movementDirection.y = playerPhysics.rb2D.velocity.y;

		float xTemp = Input.GetAxisRaw("Horizontal");
		if (xTemp < 0 && transform.eulerAngles.y < 0.1f)
			transform.RotateAround(transform.position,Vector3.up,180);
		else if (xTemp > 0 && transform.eulerAngles.y >= 180.0f)
			transform.RotateAround(transform.position,Vector3.up,-180);
		
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
				jumpDestHeight = playerPhysics.rb2D.position.y+jumpHeight;
				fTempTimeGravity = 0.0f;

				bJumping = true;
				animHandler.addAnimation(animations.jump_sword,true);
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
		if (Input.GetButtonDown ("attack")) 
		{

			animHandler.addAnimation(animations.attack,false);
			bSkipMovementForAnim = true; //look after this one, just test
			cAnimation anim = animHandler.getCurrent();
			if (anim == null)
			{
				if (sword.bCollectedSword)
					animHandler.addToQueue(animations.idle_sword,true,0.1f);
				else
					animHandler.addToQueue(animations.idle_nosword,true,0.1f);
			}
			else
			{
				animHandler.addToQueue(anim.sAnimation,anim.bLoop,0.1f);
			}
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
			
			GameObject turtle = GameObject.Find("turtle");
			if (turtle == null)
				return;
			Vector2 enemyPos = turtle.transform.position;
			Vector2 playerPos = this.gameObject.transform.position;
			
			float distance = Vector2.Distance (playerPos,enemyPos);
			if (distance <= 5) 
			{
				turtle.SendMessage("receive_hit",SendMessageOptions.RequireReceiver);
			}
		}
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
			Application.Quit();

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
				if (sword.bCollectedSword)
					handleJump (x);
				handleMovement (x);

				//jumps & gravitation
				//jump chunk
				if (playerPhysics.rb2D.position.y <= jumpDestHeight)
				{
					float  timePart = (jumpTime/1000.0f)/Time.deltaTime;
					float jumpChunk = jumpHeight/timePart;
					movementDirection.y += jumpChunk * Time.deltaTime;
				}
				else
				{

					jumpDestHeight = -999.0f;
					if (playerPhysics.grounded == false)
					{
						fTempTimeGravity += gravity;
						if (sword.bCollectedSword)
							animHandler.addAnimation(animations.jump_fall,true);
						bFalling = true;
					}
					else
					{
						if (bFalling && movementDirection.x == 0.0f)
						{
							if (sword.bCollectedSword)
								animHandler.addAnimation(animations.idle_sword,false);
							else
								animHandler.addAnimation(animations.idle_nosword,false);
						}
						bFalling = false;
						fTempTimeGravity = gravity;
						movementDirection.y = 0.0f;

					}
					bJumping = false;
				}

				movementDirection.y -= (9.80665f/2)*(fTempTimeGravity*fTempTimeGravity) * Time.deltaTime;
				playerPhysics.Move (movementDirection);
				//rb2D.MovePosition( rb2D.position  + movementDirection);
			}
		}

		animHandler.playAnimation();

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
		Debug.Log("start player - frames: " + Time.frameCount + "   " + animName );
	}
	void endAnimListener (string animName)
	{
		Debug.Log("end player - " + Time.frameCount + "   " + animName);

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
