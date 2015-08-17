//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.18408
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
public class cUnit : MonoBehaviour
{
	public float startLife = 1.0f;
	public float currentLife = 1.0f;

	protected Vector2 targetSpeed = Vector2.zero;
	protected Vector2 currentSpeed = Vector2.zero;
	protected Vector2 acceleration = Vector2.zero;

	public cUnit ()
	{

	}

	public virtual void Start()
	{
		if (currentLife <= 0.0f)
			currentLife = startLife;
	}

	public bool isDead()
	{
		if (currentLife <= 0)
			return true;
		return false;
	}

	protected bool takeDmg(float dmg)
	{
		currentLife -= dmg;
		if (currentLife < 0.0f)
			currentLife = 0.0f;
		return isDead();
	}

	protected virtual void die()
	{
		gameObject.SetActive(false);
	}

	//todo: decrease method
	protected float IncrementTowards(float n, float target, float a)
	{
		if (n == target)
			return n;
		else
		{
			float dir = Mathf.Sign(target - n);
			if(transform.eulerAngles.y >= 180.0f)
				dir = -1;
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign (target-n))? n : target;
		}
	}
}


