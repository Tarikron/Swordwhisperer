using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SkeletonAnimation))]
public class cPlayer_c : MonoBehaviour 
{
	enum eAnimTakeSword {ANIM_NONE = 0,ANIM_START,ANIM_WALK,ANIM_DONE};

	//settings via unity
	public SkeletonAnimation skeletonAnimation;
	public float jumpHeight = 300f;
	public float jumpTime = 10.0f;
	public float walkVelocity = 3.0f;
	public float gravity = 1.0f;

	//animation stuff
	private bool animIsPlaying = false;
	private bool animAdd = false; //Test var
	private bool isAnimQueueDone = false;
	private bool animLoop = false;
	private string currentAnimation = "";
	private string animationToPlay = "idle_nosword";
	private string animationToPlayAfter = "idle_nosword";
	private bool animLoopAfter = false;

	private eAnimTakeSword iAnimTakeSword = 0;

	//gameplay related
	private bool bCollectedSword = false;
	private bool bGrounded = false;
	private int iJumpCounter = 0;
	private Vector2 movementDirection;
	private Rigidbody2D rb2D;
	private float jumpDestHeight = 0.0f;

	void collectSword()
	{

	}

	void handleMovement(float x)
	{
		float absX = Mathf.Abs(x);
		animAdd = false;
		float velocity = 0.0f;
		if (currentAnimation != "wakeup_nosword")
		{
			string sword = "nosword";
			if (bCollectedSword == true)
				sword = "sword";
			//Debug.Log("x: " + x + "  absX: " + absX + "  Mathf.Sign(x):" + Mathf.Sign(x));
			
			if (x > 0)
				skeletonAnimation.skeleton.FlipX = false;
			else if (x < 0)
				skeletonAnimation.skeleton.FlipX = true;
			
			if  (absX > 0.7f && bCollectedSword == true)
			{
				animLoop = true;
				walkVelocity = 6.0f;
				animationToPlay = "runcycle_sword";
				velocity = 1.3f;
			}
			else if (absX > 0) 
			{
				animLoop = true;
				if (animationToPlay != "walkcycle_"+sword)
				{
					animLoop = false;
					animationToPlay = "walkcycle_start_"+sword;
				}
				velocity = 1f;
			}
			else
			{
				//Debug.Log ("current: " + currentAnimation);
				if (currentAnimation == "walkcycle_"+sword || currentAnimation == "walkcycle_start_"+sword)
				{
					animLoop = false;
					animationToPlay = "walkcycle_end_"+sword;
					velocity = 0.3f;
				}
				else if (currentAnimation == "")
				{
					animLoop = true;
					animationToPlay = "idle_"+sword;
					velocity = 0f;
				}
			}
		}

		movementDirection.x = velocity * walkVelocity * Mathf.Sign(x) * Time.deltaTime;
		movementDirection.y = rb2D.velocity.y;
	} //end - handleMovement

	void handleJump(float x)
	{
		if (Input.GetButtonDown ("jump"))
		{
			float absX = Mathf.Abs(x);

			string sword = "nosword";
			if (bCollectedSword == true)
				sword = "sword";

			if (bGrounded)
				iJumpCounter = 0;

			iJumpCounter++;
			if (iJumpCounter <= 2)
				jumpDestHeight = rb2D.position.y+jumpHeight;
			animLoop = false;
			animationToPlay = "jump_sword";
			animAdd = true;
			if (currentAnimation != "")
				animationToPlayAfter = currentAnimation;
			else
				animationToPlayAfter = "idle_"+sword;

			animLoopAfter = true;

			if (animationToPlayAfter == "walkcycle_end_"+sword || animationToPlayAfter == "walkcycle_start_"+sword)
				animLoopAfter = false;

		}
	}

	void handleAttack()
	{
		if (Input.GetButtonDown ("attack")) 
		{
			string sword = "nosword";
			if (bCollectedSword == true)
				sword = "sword";

			animationToPlay = "attack_kick";
			if (currentAnimation != "")
				animationToPlayAfter = currentAnimation;
			else
				animationToPlayAfter = "idle_"+sword;

			animLoopAfter = true;

			Debug.Log (animationToPlay + " " + animationToPlayAfter);

			if (animationToPlayAfter == "walkcycle_end_"+sword || animationToPlayAfter == "walkcycle_start_"+sword)
				animLoopAfter = false;
			animLoop = false;
			animAdd = true;
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
		
		skeletonAnimation.state.Start += startAnimListener;
		skeletonAnimation.state.End += endAnimListener;

		rb2D = GetComponent<Rigidbody2D>();
		jumpDestHeight = -999.0f;
		isAnimQueueDone = true;
	}

	void FixedUpdate()
	{

	}

	// Update is called once per frame
	void Update () 
	{
		string sword = "nosword";
		if (bCollectedSword == true)
			sword = "sword";

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
				bCollectedSword = true;
				animLoop = false;
				GameObject vine = GameObject.Find("vine");
				vine.GetComponent<SkeletonAnimation>().state.SetAnimation(0,"Idle_nosword",true);

				animationToPlay = "walkcycle_end_"+sword;
				iAnimTakeSword = eAnimTakeSword.ANIM_NONE;
				break;
			}
			case eAnimTakeSword.ANIM_START:
			{
				iAnimTakeSword = eAnimTakeSword.ANIM_START;
				SetAnimation ("swordTake_QuicknDirty", false);
				return;
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
					SetAnimation ("swordTake_QuicknDirty", false);
					return;
				}
				else
					SetAnimation ("walkcycle_"+sword, true);	
				break;
			}
		}
		
		if (isAnimQueueDone == true) 
		{
			float x = Input.GetAxis("Horizontal");
			
			handleMovement (x);
			handleJump (x);

			//movement
			if (iJumpCounter >= 0)
			{
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
						movementDirection.y -= gravity * Time.deltaTime;
				}
			}
			//Debug.Log( rb2D.position + movementDirection);
			
			rb2D.MovePosition( rb2D.position  + movementDirection);
			handleAttack ();

			if (animAdd) 
			{
				isAnimQueueDone = false;
				if (animationToPlay != "")
					skeletonAnimation.state.SetAnimation (0, animationToPlay, animLoop);
				if (animationToPlayAfter != "")
					skeletonAnimation.state.AddAnimation (0, animationToPlayAfter, animLoopAfter, 0);
			} 
			else
				SetAnimation (animationToPlay, animLoop);
		}
		else
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
		
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		bGrounded = true;
		iJumpCounter = 0;
		jumpDestHeight = -999.0f;

		if (collision.gameObject.name == "groundGameEnd")
		{
			Application.LoadLevel("swordwhisperer");
		}
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		bGrounded = false;
		Debug.Log("exit");
	}

	//########################################
	//################# Animation ################
	//########################################

	void startAnimListener(Spine.AnimationState state, int trackIndex)
	{
		animIsPlaying = true;

		//Debug.Log("start anim: " + state.GetCurrent (trackIndex).Animation.Name);

	}
	void endAnimListener (Spine.AnimationState state, int trackIndex)
	{
		//Debug.Log("end anim: " + state.GetCurrent (trackIndex).Animation.Name);

		animIsPlaying = false;
		if (isAnimQueueDone == false) 
		{
			//Debug.Log (state.GetCurrent (trackIndex).Animation.Name + "==" + animationToPlayAfter);
			if (state.GetCurrent (trackIndex).Animation.Name == animationToPlayAfter)
			{
				string sword = "nosword";
				if (bCollectedSword == true)
					sword = "sword";

				isAnimQueueDone = true;
				animAdd = false;
				animationToPlay = "idle_"+sword;
				animationToPlayAfter = "";
			}
		} 
		else 
		{
			string sword = "nosword";
			if (bCollectedSword == true)
				sword = "sword";

			if (state.GetCurrent (trackIndex).Animation.Name == "wakeup_"+sword || 
			    state.GetCurrent (trackIndex).Animation.Name == "walkcycle_end_"+sword)
			{
				animationToPlay = "idle_"+sword;
			}
			else if (state.GetCurrent (trackIndex).Animation.Name == "walkcycle_start_"+sword)
			{
				animationToPlay ="walkcycle_"+sword;
			}
			else if (state.GetCurrent (trackIndex).Animation.Name == "swordTake_QuicknDirty")
				iAnimTakeSword = eAnimTakeSword.ANIM_DONE;
			currentAnimation ="";
		}
	}

	void SetAnimation (string anim, bool loop) 
	{

		string sword = "nosword";
		if (bCollectedSword == true)
			sword = "sword";

		if (anim == "")
			anim = "idle_"+sword;

		if (currentAnimation != anim) 
		{
			skeletonAnimation.state.SetAnimation(0, anim, loop);
			currentAnimation = anim;
		}
	}
}
