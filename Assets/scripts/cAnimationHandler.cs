
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cAnimationHandler
{
	private Queue<cAnimation> animations;
	public SkeletonAnimation skeletonAnimation;

	private bool bIsPlaying = false;
	private cAnimation currentAnimation = null;
	private cAnimation lastInList  = null;

	public delegate void startAnim(string animName);
	public delegate void endAnim(string animName);

	public startAnim delStart;
	public endAnim delEnd;

	public cAnimationHandler (SkeletonAnimation skeletonAnim)
	{
		animations = new Queue<cAnimation>();

		currentAnimation = new cAnimation();
		this.skeletonAnimation = skeletonAnim;
		if (this.skeletonAnimation != null)
		{
			skeletonAnimation.state.Start += startAnimListener;
			skeletonAnimation.state.End += endAnimListener;
		}

	}
	public bool contains (string anim, bool loop, bool eventAnim = false)
	{
		cAnimation cAnim = new cAnimation (loop,anim,eventAnim);
		return animations.Contains(cAnim);
	}
	public cAnimation getCurrent()
	{
		return currentAnimation;
	}

	public void addAnimation(string anim, bool loop,bool eventAnim = false)
	{
		if (anim == "" || currentAnimation.sAnimation == anim || currentAnimation.bEventAnimation == true)
			return;

		cAnimation cAnim = new cAnimation (loop,anim, eventAnim);
		if (animations.Contains(cAnim) == false)
			animations.Enqueue(cAnim);
	}
	public void addAnimation (cAnimation anim)
	{
		if (anim.sAnimation == "" || currentAnimation.sAnimation == anim.sAnimation || currentAnimation.bEventAnimation == true)
			return;
		if (animations.Contains(anim) == false)
			animations.Enqueue(anim);
	}

	public void addToQueue(string anim, bool loop, float delay, bool eventAnim = false)
	{
		if (anim == ""  || currentAnimation.bEventAnimation == true)
			return;
		
		cAnimation cAnim = new cAnimation (loop,anim,delay, eventAnim);
		if (animations.Contains(cAnim) == false)
			animations.Enqueue(cAnim);
	}
	public void addToQueue (cAnimation anim)
	{
		if (anim.sAnimation == ""  || currentAnimation.bEventAnimation == true)
			return;

		cAnimation cAnim = new cAnimation (anim.bLoop,anim.sAnimation);

		if (animations.Contains(cAnim) == false)
			animations.Enqueue(cAnim);
	}

	public cAnimation removeAnimation()
	{
		return animations.Dequeue();
	}

	public void clearAnimations()
	{
		animations.Clear();
	}

	public void playAnimation()
	{
		if (skeletonAnimation == null || lastInList != null)
			return;
		if (animations.Count <= 0)
			return;
		
		bool bAdd = false;
		if (animations.Count > 1)
			bAdd = true;

		//Works like fifo, first in, first out
		cAnimation anim = this.removeAnimation();

		if (anim.sAnimation != "")
			skeletonAnimation.state.SetAnimation(0,anim.sAnimation,anim.bLoop);
		if (bAdd == true)
		{
			int count = animations.Count;
			cAnimation[] ary = animations.ToArray();
			lastInList = ary[ary.Length-1];
		
			for (int i = 0; i<count;i++)
			{
				cAnimation cAnim = this.removeAnimation();
				this.skeletonAnimation.state.AddAnimation(0,cAnim.sAnimation,cAnim.bLoop,cAnim.fDelay);
			}
		}

	}


	//events from skeletonAnimation
	void startAnimListener(Spine.AnimationState state, int trackIndex)
	{
		bIsPlaying = true;
		delStart(state.GetCurrent (trackIndex).Animation.Name);

		currentAnimation.sAnimation = state.GetCurrent (trackIndex).Animation.Name;
		currentAnimation.bLoop = state.GetCurrent (trackIndex).loop;

		if (currentAnimation.CompareTo(lastInList) == true)
			lastInList = null;
	}
	void endAnimListener (Spine.AnimationState state, int trackIndex)
	{
		bIsPlaying = false;
		delEnd(state.GetCurrent (trackIndex).Animation.Name);
		currentAnimation.sAnimation = "";
	}


}


