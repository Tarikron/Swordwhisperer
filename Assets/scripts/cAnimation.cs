using System;

public class cAnimation
{
	public string sAnimation;
	public bool bLoop;


	public cAnimation ()
	{
		sAnimation = "";
		bLoop = true;
	}

	public cAnimation(bool loop, string anim)
	{
		sAnimation = anim;
		bLoop = loop;
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


