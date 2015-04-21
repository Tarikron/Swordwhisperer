using UnityEngine;
using System.Collections;
using UnityEngine.UI;


[RequireComponent(typeof(SkeletonAnimation))]
public class cPlayer_c : MonoBehaviour 
{

	enum eAnimTakeSword {ANIM_NONE = 0,ANIM_START,ANIM_WALK,ANIM_DONE};
	enum eAnimWalk{ANIM_NONE = 0,ANIM_WALK,ANIM_END};

	//settings via unity
	public SkeletonAnimation skeletonAnimation;

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

	//gameplay related
	// - movement and jump related
	private bool bGrounded = false;
	private int iGroundBridge = 0; //quick dirty solution
	private int iJumpCounter = 0;
	private float fTempTimeGravity = 0.0f;
	private Vector2 movementDirection;
	private Rigidbody2D rb2D;
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

	void handleSwordPickup()
	{
		if (Input.GetButtonDown("CtrlBButton"))
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
				GameObject vine = GameObject.Find("vine");
				vine.GetComponent<SkeletonAnimation>().state.SetAnimation(0,"Idle_nosword",true);
				bSkipMovementForAnim = true;
				bCutscene = false;
				iAnimTakeSword = eAnimTakeSword.ANIM_NONE;
				animHandler.addAnimation("walkcycle_end_sword",false);
				break;
			}
			case eAnimTakeSword.ANIM_START:
			{
				iAnimTakeSword = eAnimTakeSword.ANIM_START;
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
					animHandler.addAnimation("swordTake_QuicknDirty",false);
					bCutscene = true;
				}
				else
					animHandler.addAnimation("walkcycle_"+sword.sSword,true);
				break;
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
		
		if (x > 0)
			skeletonAnimation.skeleton.FlipX = false;
		else if (x < 0)
			skeletonAnimation.skeleton.FlipX = true;
		
		if  (absX > 0.7f && sword.bCollectedSword == true)
		{
			animHandler.addAnimation("runcycle_sword",true);
			velocity = 1.3f * runVelocity;
		}
		else if (absX > 0) 
		{
			if (iAnimWalk == eAnimWalk.ANIM_WALK)
			{
				velocity = walkVelocity;
				animationToPlay = "walkcycle_"+sword.sSword;
				animLoop = true;
			}
			else
			{
				iAnimWalk = eAnimWalk.ANIM_NONE;
				animationToPlay = "walkcycle_start_"+sword.sSword;
				animLoop = false;
				velocity = 0.3f*walkVelocity;
			}
		}
		else
		{
			//Debug.Log ("current: " + currentAnimation);
			if (anim.sAnimation == "walkcycle_"+sword.sSword || anim.sAnimation == "walkcycle_start_"+sword.sSword)
			{
				animLoop = false;
				animationToPlay = "walkcycle_end_"+sword.sSword;
				velocity = 0.3f * walkVelocity;
			}
			else if (anim.sAnimation == "")
			{
					animLoop = true;
					animationToPlay = "idle_"+sword.sSword;
			}
		}

		animHandler.addAnimation(animationToPlay,animLoop);
		movementDirection.x = velocity * Mathf.Sign(x) * Time.deltaTime;
		movementDirection.y = rb2D.velocity.y;
	} //end - handleMovement

	void handleJump(float x)
	{
		if (Input.GetButtonDown ("jump"))
		{
			float absX = Mathf.Abs(x);

			if (bGrounded)
				iJumpCounter = 0;

			iJumpCounter++;
			if (iJumpCounter <= 2)
			{
				jumpDestHeight = rb2D.position.y+jumpHeight;
				fTempTimeGravity = 0.0f;

				animHandler.addAnimation("jump_sword",false);

				cAnimation anim = animHandler.getCurrent();
				if (anim == null)
					animHandler.addToQueue("idle_"+sword.sSword,true);
				else
					animHandler.addToQueue(anim);
			
			}
		}
	}

	void handleAttack()
	{
		if (Input.GetButtonDown ("attack")) 
		{
			animHandler.addAnimation("attack_kick",false);
			bSkipMovementForAnim = true; //look after this one, just test
			cAnimation anim = animHandler.getCurrent();
			if (anim == null)
				animHandler.addToQueue("idle_"+sword.sSword,true);
			else
				animHandler.addToQueue(anim);

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

		rb2D = GetComponent<Rigidbody2D>();
		jumpDestHeight = -999.0f;
		sleepAnim = true;

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
			animHandler.addAnimation("wakeup_nosword",false);
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
				if (rb2D.position.y <= jumpDestHeight)
				{
					float  timePart = (jumpTime/1000.0f)/Time.deltaTime;
					float jumpChunk = jumpHeight/timePart;
					movementDirection.y += jumpChunk * Time.deltaTime;
				}
				else
				{
					jumpDestHeight = -999.0f;
					if (bGrounded == false)
					{
						fTempTimeGravity += gravity;
						movementDirection.y -= (9.80665f/2)*(fTempTimeGravity*fTempTimeGravity)  * Time.deltaTime;
					}
					else
						fTempTimeGravity = 0.0f;
				}

				rb2D.MovePosition( rb2D.position  + movementDirection);
			}
		}

		animHandler.playAnimation();

	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if ((collision.gameObject.layer & LayerMask.NameToLayer("ground")) ==  LayerMask.NameToLayer("ground"))
		{
			bGrounded = true;
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
				bGrounded = false;
		}
	}

	//########################################
	//################# Animation ################
	//########################################

	void startAnimListener()
	{
	}
	void endAnimListener (string animName)
	{

		if (animName == "wakeup_"+sword.sSword || 
		    animName == "walkcycle_end_"+sword.sSword ||
		    animName == "attack_kick")
		{
			bSkipMovementForAnim = false;
			bCutscene = false; // for wakeup
			iAnimWalk = eAnimWalk.ANIM_END;
		}
		else if (animName == "walkcycle_start_"+sword.sSword)
			iAnimWalk = eAnimWalk.ANIM_WALK;
		else if (animName == "swordTake_QuicknDirty")
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
