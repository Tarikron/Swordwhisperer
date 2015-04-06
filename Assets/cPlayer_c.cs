using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SkeletonAnimation))]
public class cPlayer_c : MonoBehaviour 
{

	public SkeletonAnimation skeletonAnimation;
	private string currentAnimation = "";
	public float jump_height = 3.0f;
	
	private int iJumpCounter = 0;
	private float jumpForce = 300f;

	// Use this for initialization
	void Start () {
		//skeletonAnimation.state.Start += this.startListener;
		//skeletonAnimation.state.End += this.endListener;
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
				SetAnimation ("walkcycle", true);
			else
				SetAnimation ("Idle_NO_sword", true);

			if (Input.GetButtonDown ("jump"))
			{
				GetComponent<Rigidbody2D>().AddForce(new Vector2(0,jumpForce));
			}

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
