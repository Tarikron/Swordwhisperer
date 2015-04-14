using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SkeletonAnimation))]
public class cPlayer_c : MonoBehaviour 
{

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
			//Debug.Log("x: " + x + "  absX: " + absX + "  Mathf.Sign(x):" + Mathf.Sign(x));
			
			if (x > 0)
				skeletonAnimation.skeleton.FlipX = false;
			else if (x < 0)
				skeletonAnimation.skeleton.FlipX = true;
			
			if  (absX > 0.7f && bCollectedSword == true)
			{
				animLoop = true;
				animationToPlay = "Runcycle_sword";
				velocity = 1.3f;
			}
			else if (absX > 0) 
			{
				animLoop = true;
				if (animationToPlay != "walkcycle_nosword")
				{
					animLoop = false;
					animationToPlay = "walkcycle_start_nosword";
				}
				velocity = 1f;
			}
			else
			{
				if (currentAnimation == "walkcycle_nosword")
				{
					animLoop = false;
					animationToPlay = "walkcycle_end_nosword";
					velocity = 0.3f;
				}
				else if (currentAnimation == "")
				{
					animLoop = true;
					animationToPlay = "idle_nosword";
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

			if (bGrounded)
				iJumpCounter = 0;

			iJumpCounter++;
			if (iJumpCounter <= 2)
				jumpDestHeight = rb2D.position.y+jumpHeight;
			//animLoop = false;
			//animationToPlay = "jump_sword";
		}
	}

	void handleAttack()
	{
		if (Input.GetButtonDown ("attack")) 
		{
			animationToPlay = "attack_kick";
			if (currentAnimation != "")
				animationToPlayAfter = currentAnimation;
			else
				animationToPlayAfter = "idle_nosword";
			animLoop = false;
			animAdd = true;
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
			
			GameObject turtle = GameObject.Find("turtle");
			if (turtle == null)
				return;
			Vector2 enemyPos = turtle.transform.position;
			Vector2 playerPos = this.gameObject.transform.position;
			
			float distance = Vector2.Distance (playerPos,enemyPos);
			if (distance <= 3) 
			{
				turtle.SetActive(false);
			}
			
		}
	}

	// Use this for initialization
	void Start () {
		SetAnimation ("wakeup_nosword", false);

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
			Debug.Log( rb2D.position + movementDirection);
			
			rb2D.MovePosition( rb2D.position  + movementDirection);

			handleAttack ();

			if (animAdd) 
			{
				isAnimQueueDone = false;
				if (animationToPlay != "")
					skeletonAnimation.state.SetAnimation (0, animationToPlay, animLoop);
				if (animationToPlayAfter != "")
					skeletonAnimation.state.AddAnimation (0, animationToPlayAfter, true, 0);
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
	}
	void endAnimListener (Spine.AnimationState state, int trackIndex)
	{
		animIsPlaying = false;
		if (isAnimQueueDone == false) 
		{
			if (state.GetCurrent (trackIndex).Animation.Name == animationToPlayAfter)
			{
				isAnimQueueDone = true;
				animAdd = false;
				animationToPlay = "idle_nosword";
				animationToPlayAfter = "";
			}
			
		} 
		else 
		{
			if (state.GetCurrent (trackIndex).Animation.Name == "wakeup_nosword" || 
			    state.GetCurrent (trackIndex).Animation.Name == "walkcycle_end_nosword")
			{
				animationToPlay = "idle_nosword";
			}
			else if (state.GetCurrent (trackIndex).Animation.Name == "walkcycle_start_nosword")
			{
				animationToPlay ="walkcycle_nosword";
			}
			currentAnimation ="";
		}
	}

	void SetAnimation (string anim, bool loop) 
	{
		if (anim == "")
			anim = "idle_nosword";

		if (currentAnimation != anim) 
		{
			skeletonAnimation.state.SetAnimation(0, anim, loop);
			currentAnimation = anim;
		}
	}
}
