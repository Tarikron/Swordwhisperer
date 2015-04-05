using UnityEngine;
using System.Collections;
using Spine;


public class cPlayer_c : MonoBehaviour {

	public SkeletonAnimation skeletonAnimation;
	private string currentAnim = "";
	public float jump_height = 3.0f;
	private Vector3 jump_target = Vector3.zero;
	private Vector3 origin_target = Vector3.zero;


	// Use this for initialization
	void Start () {
		skeletonAnimation.state.Start += this.startListener;
		skeletonAnimation.state.End += this.endListener;
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (jump_target != Vector3.zero) 
		{
			transform.position = Vector3.MoveTowards(transform.position,jump_target,5.0f * Time.deltaTime);
			if (jump_target == transform.position)
			{
				jump_target.y = origin_target.y;
				transform.position = Vector3.MoveTowards(transform.position,jump_target,5.0f * Time.deltaTime);
			}
		}

		if (Input.GetButtonDown("jump"))
		{
			origin_target = transform.position;
			jump_target = transform.position;
			jump_target.y += jump_height;
		}
		if (Input.GetButton ("Horizontal")) 
		{
			currentAnim = "walkcycle";
			skeletonAnimation.state.AddAnimation (0, "walkcycle", true, 0);

			Vector3 vPos = this.gameObject.transform.position;
			vPos.z += Input.GetAxis("Horizontal");
			this.gameObject.transform.LookAt(vPos);
			Debug.Log (vPos);

		}
		else if (currentAnim == "walkcycle")
		{
			skeletonAnimation.state.SetAnimation (0, "walkcycle", false);
			skeletonAnimation.state.AddAnimation (0, "Idle_NO_sword", true, 0);
			currentAnim = "Idle_NO_sword";
		}
	}

	public void startListener(Spine.AnimationState state, int trackIndex)
	{
		Debug.Log(trackIndex + " " + state.GetCurrent(trackIndex) + ": start ");
	}
	public void endListener(Spine.AnimationState state, int trackIndex)
	{
		Debug.Log(trackIndex + " " + state.GetCurrent(trackIndex) + ": end");
		string track = state.GetCurrent (trackIndex).ToString();
		if (track == "wakeup") 
		{
			skeletonAnimation.state.AddAnimation (0, "Idle_NO_sword", true, 0);
			currentAnim = "Idle_NO_sword";
		}
	}
}
