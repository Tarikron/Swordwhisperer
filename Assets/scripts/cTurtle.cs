using UnityEngine;
using System.Collections;
using Spine;

[RequireComponent(typeof(SkeletonAnimation))]
public class cTurtle : cEnemy {

	enum eTurtleMovement {ANIM_IDLE,ANIM_MOVE};

	public SkeletonAnimation skeletonAnimation;
	public float triggerDistance = 0.0f;
	public float triggerAttack = 0.0f;

	private string currentAnimation = "";
	private string animationToPlay = "";
	private bool animLoop = false;
	private float currentTimeScale = 1.0f;

	private bool tookDamge = false;
	private Color originColor;
	private int delayFrames=4;
	private int frameCounter=0;

	[System.Serializable]
	public struct animTurtle
	{
		[SpineAnimation]
		public string sleep;
		[SpineAnimation]
		public string wakeup;
		[SpineAnimation]
		public string idle;
		[SpineAnimation]
		public string death;
		[SpineAnimation]
		public string walk;
		[SpineAnimation]
		public string recieved_hit;
		[SpineAnimation]
		public string attack;
	}
	public animTurtle turtleAnimations;

	// Use this for initialization
	public override void Start () 
	{
		base.Start();

		skeletonAnimation.state.Start += startAnimListener;
		skeletonAnimation.state.End += endAnimListener;

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		originColor.a = skeletonAnimation.skeleton.a;
		originColor.r = skeletonAnimation.skeleton.r;
		originColor.g = skeletonAnimation.skeleton.g;
		originColor.b = skeletonAnimation.skeleton.b;
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameObject player = GameObject.Find("Player");
		Vector2 playerPos = player.transform.position;
		Vector2 enemyPos = this.gameObject.transform.position;

		if (tookDamge)
		{
			if (delayFrames < frameCounter)
			{
				skeletonAnimation.skeleton.r = 1.0f;
				skeletonAnimation.skeleton.b = 1.0f;
				skeletonAnimation.skeleton.g = 1.0f;
				skeletonAnimation.skeleton.a = 1.0f;
				
				tookDamge = false;
				frameCounter = 0;
			}
			frameCounter++;
		}
		if (iDieState == eDieState.DIE_NONE && isDead() && !tookDamge)
		{
			GetComponent<BoxCollider2D>().enabled = false;
			iDieState = eDieState.DIE_START;
		}
		
		if (defaultDeath()) //if we are dead, no need for others
			return;

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
		if (currentLife > 0)
		{
			currentTimeScale = 1.0f;
			animationToPlay = "recieved_hit";
			animLoop = false;
			currentLife--;
		}
		else if (currentLife <= 0)
		{
			currentTimeScale = 1.0f;
			animationToPlay = "death";
			animLoop = false;
		}
	}

	void msg_damage(float dmg)
	{
		skeletonAnimation.skeleton.r = 1.0f;
		skeletonAnimation.skeleton.b = 0.3f;
		skeletonAnimation.skeleton.g = 0.0f;
		skeletonAnimation.skeleton.a = 1.0f;
		
		takeDmg(dmg);	
		
		tookDamge = true;
	}

	void msg_die()
	{
		die();
	}
	protected override void die()
	{
		currentLife = 0;
		
		SetAnimation (turtleAnimations.death, false);
	}
}
