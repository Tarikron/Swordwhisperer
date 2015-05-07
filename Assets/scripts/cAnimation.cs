using System;

public class cAnimation
{
	public string sAnimation;
	public bool bLoop;
	public float fDelay;
	public bool bEventAnimation;

	public cAnimation ()
	{
		sAnimation = "";
		bLoop = true;
		fDelay = 0.0f;
		bEventAnimation = false;
	}

	public cAnimation(bool loop, string anim, bool eventAnim = false)
	{
		sAnimation = anim;
		bLoop = loop;
		fDelay = 0.0f;
		bEventAnimation = eventAnim;
	}
	public cAnimation(bool loop, string anim, float delay, bool eventAnim = false)
	{
		sAnimation = anim;
		bLoop = loop;
		fDelay = delay;
		bEventAnimation = eventAnim;
	}

	public override bool Equals (object obj)
	{
		cAnimation other = obj as cAnimation;
		return (sAnimation == other.sAnimation && bLoop == other.bLoop);
	}
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public bool CompareTo(cAnimation other)
	{
		if (other == null)
			return false;

		return (sAnimation == other.sAnimation && bLoop == other.bLoop);
	} 
}


