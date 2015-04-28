using UnityEngine;
using System.Collections;
using Spine;
public class cVine : MonoBehaviour {

	private SkeletonAnimation skeletonAnimation;
	private cAnimationHandler animHandler;

	private bool bVineTookSword = false;
	private bool bVineLoops = false;

	// Use this for initialization
	void Start () {

		skeletonAnimation = GetComponent<SkeletonAnimation>();

		animHandler = new cAnimationHandler(skeletonAnimation);
		animHandler.delStart = startAnimListener;
		animHandler.delEnd = endAnimListener;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (bVineLoops == false)
		{
			if (bVineTookSword == true)
			{
				animHandler.addAnimation("idle_nosword",true);
				bVineLoops = true;
			}

			animHandler.playAnimation();
		}

	}

	void msg_startanim()
	{
		if (bVineTookSword == false)
			animHandler.addAnimation("idle_part2",false);
	}

	//events from skeletonAnimation
	void startAnimListener(string anim)
	{
		Debug.Log("start vine - frames:" + Time.frameCount + "   "  + anim);
	}
	void endAnimListener (string anim)
	{

		if (anim == "idle_part2")
			bVineTookSword = true;

		Debug.Log("end vine - frames:" + Time.frameCount + "   " + anim);
	}
}
