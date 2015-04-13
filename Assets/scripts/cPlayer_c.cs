using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SkeletonAnimation))]
public class cPlayer_c : MonoBehaviour 
{

	public SkeletonAnimation skeletonAnimation;
	public float jumpForce = 300f;
	public float walkVelocity = 3.0f;

	private bool animIsPlaying = false;
	private bool animAdd = false; //Test var
	private bool animLoop = false;
	private string currentAnimation = "";
	private string animationToPlay = "idle_nosword";
	private string animationToPlayAfter = "idle_nosword";
	private bool bCollectedSword = false;


	void collectSword()
	{

	}

	void handleMovement()
	{
		animAdd = false;
		if (currentAnimation != "wakeup_nosword")
		{
			float x = Input.GetAxis("Horizontal");
			float absX = Mathf.Abs(x);
			
			//Debug.Log("x: " + x + "  absX: " + absX + "  Mathf.Sign(x):" + Mathf.Sign(x));
			
			if (x > 0)
				skeletonAnimation.skeleton.FlipX = false;
			else if (x < 0)
				skeletonAnimation.skeleton.FlipX = true;
			
			if  (absX > 0.7f && bCollectedSword == true)
			{
				animLoop = true;
				animationToPlay = "Runcycle_sword";
				GetComponent<Rigidbody2D>().velocity = new Vector2(walkVelocity * Mathf.Sign(x), GetComponent<Rigidbody2D>().velocity.y);
			}
			else if (absX > 0) 
			{
				animLoop = true;
				if (animationToPlay != "walkcycle_nosword")
				{
					animLoop = false;
					animationToPlay = "walkcycle_start_nosword";
				}
				
				GetComponent<Rigidbody2D>().velocity = new Vector2(walkVelocity * Mathf.Sign(x), GetComponent<Rigidbody2D>().velocity.y);
			}
			else
			{
				if (currentAnimation == "walkcycle_nosword")
				{
					animLoop = false;
					animationToPlay = "walkcycle_end_nosword";
				}
				else if (currentAnimation == "")
				{
					animLoop = true;
					animationToPlay = "idle_nosword";
					GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
				}
			}
		}
	} //end - handleMovement

	void handleJump()
	{
		if (Input.GetButtonDown ("jump"))
		{
			//animLoop = false;
			//animationToPlay = "jump_sword";
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0,jumpForce));
		}
	}

	// Use this for initialization
	void Start () {
		SetAnimation ("wakeup_nosword", false);

		skeletonAnimation.state.Start += startAnimListener;
		skeletonAnimation.state.End += endAnimListener;
	}
	
	// Update is called once per frame
	void Update () 
	{

		handleMovement ();
		handleJump ();

		Debug.Log(animationToPlay + "   " + animLoop);
		SetAnimation (animationToPlay, animLoop);
		
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log("collision start");
	}

	void OnCollisionExit2D(Collision2D collision)
	{
		Debug.Log("collision exit");
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
		currentAnimation ="";

		if (state.GetCurrent (trackIndex).Animation.Name == "wakeup_nosword" || 
		    state.GetCurrent (trackIndex).Animation.Name == "walkcycle_end_nosword")
		{
			animationToPlay = "idle_nosword";
		}
		else if (state.GetCurrent (trackIndex).Animation.Name == "walkcycle_start_nosword")
		{
			animationToPlay ="walkcycle_nosword";
		}
	}

	void SetAnimation (string anim, bool loop) 
	{
		if (currentAnimation != anim) 
		{
			skeletonAnimation.state.SetAnimation(0, anim, loop);
			currentAnimation = anim;
		}
	}
}
