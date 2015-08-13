using UnityEngine;
using System.Collections;
using Spine;

[RequireComponent(typeof(TurtlePhysics))]
[RequireComponent(typeof(SkeletonAnimation))]
public class cTurtle : cEnemy {

	enum eTurtleMovement {ANIM_IDLE,ANIM_MOVE};

	private TurtlePhysics physics;
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

	private float fadeTime = 1.0f;

	private bool interact = false;

	public bool sleeper = true;
	private bool sleep = true;
	private bool attackBite = false;

	private float sleepTime = 2.0f;
	private float sleepTimer = 0.0f;

	private int interacts = 0;

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

	private void manageMovement()
	{
		//todo
	}

	private void manageInteraction(Vector2 playerPos)
	{
		//todo
		float distance = Vector2.Distance(playerPos,transform.position);
		GameObject go = transform.FindChild("PressButton_go").gameObject;
		if (go)
		{
			Color c = go.GetComponent<SpriteRenderer>().color;

			if (distance <= 7.0f && sleep == false)
			{
				c.a += Time.deltaTime/fadeTime;
				if (c.a > 1.0f)
				{
					c.a = 1.0f;
					interact = true;
				}
			}
			else
			{
				c.a -= Time.deltaTime/fadeTime;
				if (c.a < 0.0f)
				{
					c.a = 0.0f;
					interact = false;
				}
			}
			go.GetComponent<SpriteRenderer>().color = c;
		}

	}

	private void manageBehavior(Vector2 playerPos)
	{
		manageMovement();

		manageInteraction(playerPos);
	}

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

			//animationToPlay = turtleAnimations.death;
			//animLoop = false;
		}
		
		if (death()) //if we are dead, no need for others
			return;
		if (iDieState == eDieState.DIE_NONE)
		{
			manageBehavior(playerPos);

			if (!attackBite && !sleep && interact && Input.GetButtonDown("CtrlBButton"))
			{
				animLoop = true;

				if (sleeper && interacts == 0)
				{
					currentTimeScale = 0.7f;
					animationToPlay = "sleep";
					sleep = true;
				}
				else
				{
					animationToPlay = "attack_Bite";
					transform.FindChild("biteBox").gameObject.GetComponent<BoxCollider2D>().enabled = true;
					attackBite = true;
					animLoop = false;
				}
				interacts = Random.Range (10,100);
				interacts = interacts % 2;
				GameObject go = transform.FindChild("PressButton_go").gameObject;
				if (go)
				{
					Color c = go.GetComponent<SpriteRenderer>().color;
					c.a = 0.0f;
					go.GetComponent<SpriteRenderer>().color = c;
				}
			}
			else
			{
				float distance = Vector2.Distance (enemyPos,playerPos);
				if (distance <= triggerDistance) 
				{
					if (animationToPlay == "sleep")
					{
						//wait random time
						if (sleepTime <= sleepTimer)
						{
							sleepTimer = 0.0f;
							sleepTime = Random.Range (2.0f,4.0f);
							animationToPlay = "";
						}
						sleepTimer += Time.deltaTime;
					}

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
			}
		}

		SetAnimation (animationToPlay, animLoop);
	}


	void OnTriggerEnter2D (Collider2D collider)
	{
		if (collider.gameObject.tag == "player")
		{
			transform.FindChild("biteBox").gameObject.GetComponent<BoxCollider2D>().enabled = false;
			collider.gameObject.SendMessage("msg_hit",1.0f,SendMessageOptions.RequireReceiver);
		}
	}
	void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log (collision.gameObject.tag);
		
		
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
		{
			animationToPlay = "idle";
			sleep = false;
		}
		else if (state.GetCurrent (trackIndex).Animation.Name == "attack_Bite")
		{
			attackBite = false;
			animationToPlay = "idle";
		}
		else if (state.GetCurrent (trackIndex).Animation.Name == "death")
			this.gameObject.SetActive(false);
	}
	
	void SetAnimation (string anim, bool loop) 
	{
		if (currentAnimation != anim) 
		{	
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

	private bool death()
	{
		if (currentLife <= 0.0f)
		{
			die ();
			return true;
		}
		return false;
	}

	void msg_die()
	{
		die();
	}
	protected override void die()
	{
		if (currentAnimation != turtleAnimations.death)
		{
			ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem ps in particleSystems)
			{
				ps.enableEmission = true;
			}
		}

		currentLife = 0;
		SetAnimation (turtleAnimations.death, false);
	}
}
