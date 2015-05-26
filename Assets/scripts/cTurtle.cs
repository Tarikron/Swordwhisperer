using UnityEngine;
using System.Collections;
using Spine;

[RequireComponent(typeof(SkeletonAnimation))]
public class cTurtle : cEnemy {

	enum eTurtleMovement {ANIM_IDLE,ANIM_MOVE};

	public SkeletonAnimation skeletonAnimation;
	public float triggerDistance = 0.0f;
	public float triggerAttack = 0.0f;

	public int life = 3;

	private string currentAnimation = "";
	private string animationToPlay = "";
	private bool animLoop = false;
	private float currentTimeScale = 1.0f;

	// Use this for initialization
	void Start () 
	{
		skeletonAnimation.state.Start += startAnimListener;
		skeletonAnimation.state.End += endAnimListener;
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameObject player = GameObject.Find("Player");
		Vector2 playerPos = player.transform.position;
		Vector2 enemyPos = this.gameObject.transform.position;

		float distance = Vector2.Distance (enemyPos,playerPos);
		if (distance <= triggerDistance) 
		{
			if (animationToPlay == "")
			{
				currentTimeScale = 0.7f;
				animationToPlay = "wakeUP";
				animLoop = false;
			}
			else if (animationToPlay == "idle")
			{
				currentTimeScale = 1.0f;
				animLoop = true;
			}
		}

		SetAnimation (animationToPlay, animLoop);
	}

	//########################################
	//################# Animation ############
	//########################################
	
	void startAnimListener(Spine.AnimationState state, int trackIndex)
	{

	}
	void endAnimListener (Spine.AnimationState state, int trackIndex)
	{
		currentAnimation ="";
		//Debug.Log ("end - " + state.GetCurrent (trackIndex).Animation.Name);
		if (state.GetCurrent (trackIndex).Animation.Name == "wakeUP" ||
		    state.GetCurrent (trackIndex).Animation.Name == "recieved_hit")
			animationToPlay = "idle";
		else if (state.GetCurrent (trackIndex).Animation.Name == "death")
			this.gameObject.SetActive(false);
	}
	
	void SetAnimation (string anim, bool loop) 
	{
		if (currentAnimation != anim) 
		{	
			Debug.Log ("  " + anim + "   " + loop);
			skeletonAnimation.state.SetAnimation(0,anim,loop);
			skeletonAnimation.timeScale = currentTimeScale;
			currentAnimation = anim;
		}
	}

	void receive_hit()
	{
		if (life > 0)
		{
			currentTimeScale = 1.0f;
			animationToPlay = "recieved_hit";
			animLoop = false;
			life--;
		}
		else if (life <= 0)
		{
			currentTimeScale = 1.0f;
			animationToPlay = "death";
			animLoop = false;
		}
	}
}
