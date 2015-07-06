using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	private Transform target;
	private Vector3 offset;
	public bool stopCam = false;
	private Vector2 trackSpeed = Vector2.zero;

	public void SetTrackSpeed(float accX, float accY)
	{
		trackSpeed.x = accX;
		trackSpeed.y = accY;
	}
	public void SetOffset(Vector3 o)
	{
		offset = o;
	}
	public void SetTarget(Transform t)
	{
		target = t;
	}
	
	void LateUpdate()
	{
		if (target)
		{
			float y = transform.position.y;

			float x = IncrementTowards(transform.position.x,target.position.x+offset.x,trackSpeed.x);
			if (stopCam == false)
				y = IncrementTowards(y,target.position.y+offset.y,trackSpeed.y);
			if (x >= -6.94)
				transform.position = new Vector3(x,y,transform.position.z);
		}
	}

	
	private float IncrementTowards(float n, float target, float a)
	{
		if (n == target)
			return n;
		else
		{
			float dir = Mathf.Sign(target - n);
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign (target-n))? n : target;
		}
	}
}
