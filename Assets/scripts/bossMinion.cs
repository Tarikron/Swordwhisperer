using UnityEngine;
using System.Collections;

public class bossMinion : cEnemy {

	[HideInInspector]
	public float alpha = 0.0f;
	[HideInInspector]
	public float radius = -1.0f;

	public float angleSpeed = 20.0f;
	public float amplitude = 2.0f;
	private float minionAlpha = 0.0f;

	private Color originColor;
	private SkeletonAnimation skeletonAnimation;

	private bool tookDamge = false;
	private int delayFrames=4;
	private int frameCounter=0;

	// Use this for initialization
	public override void Start () 
	{
		base.Start();

		radius = 0.0f;
		minionAlpha = Random.Range (0.0f,360.0f);

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		originColor.a = skeletonAnimation.skeleton.a;
		originColor.r = skeletonAnimation.skeleton.r;
		originColor.g = skeletonAnimation.skeleton.g;
		originColor.b = skeletonAnimation.skeleton.b;
	}
	
	// Update is called once per frame
	void Update () 
	{

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

		if (isDead() && !tookDamge)
		{
			GetComponent<BoxCollider2D>().enabled = false;
			iDieState = eDieState.DIE_START;
		}
		if (defaultDeath()) //if we are dead, no need for others
			return;

		minionAlpha += angleSpeed * Time.deltaTime;
		
		if (minionAlpha > 360.0f)
			minionAlpha = 0.0f;
		if (minionAlpha < 0.0f)
			minionAlpha = 360.0f;

		radius = Mathf.Sin( minionAlpha * Mathf.PI/180)/amplitude;

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

	void msg_shot()
	{
		GameObject player = GameObject.Find("Player");
		Vector3 playerPos = player.gameObject.transform.position;
		Vector3 enemyPos = transform.position;
		float distance = Vector3.Distance(enemyPos,playerPos);
		if (distance <= triggerRange)
			attackShot(player,playerPos);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{				
		if (collision.gameObject.tag == "player")
		{
			collision.gameObject.SendMessage("msg_hit",attackCollideDmg,SendMessageOptions.RequireReceiver);
		}
	}
}
