using UnityEngine;
using System.Collections;
using Spine;
public class cVine : MonoBehaviour {

	public SkeletonAnimation skeletonAnimation;

	// Use this for initialization
	void Start () {

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		skeletonAnimation.state.Start += startAnimListener;
		skeletonAnimation.state.End += endAnimListener;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//events from skeletonAnimation
	void startAnimListener(Spine.AnimationState state, int trackIndex)
	{
		Debug.Log("start");
	}
	void endAnimListener (Spine.AnimationState state, int trackIndex)
	{
		Debug.Log("end");
	}
}
