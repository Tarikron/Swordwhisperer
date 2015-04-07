using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SkeletonAnimation))]
public class cPlayer_c : MonoBehaviour 
{

	public SkeletonAnimation skeletonAnimation;
	private string currentAnimation = "";
		
	private int iJumpCounter = 0;
	public float jumpForce = 300f;
	public float walkVelocity = 3.0f;

	// Use this for initialization
	void Start () {
		SetAnimation ("wakeup", false);

		skeletonAnimation.state.End += endAnimListener;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (currentAnimation != "wakeup")
		{
			float x = Input.GetAxis("Horizontal");
			float absX = Mathf.Abs(x);

			if (x > 0)
				skeletonAnimation.skeleton.FlipX = false;
			else if (x < 0)
				skeletonAnimation.skeleton.FlipX = true;

			if (absX > 0) 
			{
				SetAnimation ("walkcycle", true);
				GetComponent<Rigidbody2D>().velocity = new Vector2(walkVelocity * Mathf.Sign(x), GetComponent<Rigidbody2D>().velocity.y);
			}
			else
			{
				SetAnimation ("Idle_NO_sword", true);
				GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
			}
			if (Input.GetButtonDown ("jump"))
			{
				GetComponent<Rigidbody2D>().AddForce(new Vector2(0,jumpForce));
			}

		}
	}
	void endAnimListener (Spine.AnimationState state, int trackIndex)
	{
		if (state.GetCurrent (trackIndex).Animation.Name == "wakeup")
			currentAnimation = "";
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
