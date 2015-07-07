using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	private Transform target;
	private Vector3 offset;
	public bool stopCam = false;
	public Vector2 trackSpeed = Vector2.zero;

	private float shake = 0.0f;
	public float shakeAmount = 4.0f;
	public float decreaseFactor = 0.5f;

	private float timer = 0.0f;



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

	public void startShake()
	{
		if (shake <= 0.0f)
		{
			shake = shakeAmount;
			timer = 0.0f;
		}
	}

	void LateUpdate()
	{
		if (target)
		{
			Vector3 vShake = Vector3.zero;
			if (shake > 0.0f) {
				vShake += Random.insideUnitSphere * shake;
				shake -= (timer*timer) * decreaseFactor - timer * decreaseFactor;
				timer += Time.deltaTime;

			} else {
				shake = 0.0f;
			}
			if (vShake.x < -3.0f)
				vShake.x = -3.0f;
			else if (vShake.x > 3.0f)
				vShake.x = 3.0f;
			if (vShake.y > 0.2f)
				vShake.y = 0.2f;
			else if (vShake.y < -0.2f)
				vShake.y = -0.2f;

			vShake.z = 0.0f;

			float y = transform.position.y;

			float x = IncrementTowards(transform.position.x,target.position.x+offset.x,trackSpeed.x);
			if (stopCam == false)
				y = IncrementTowards(y,target.position.y+offset.y,trackSpeed.y);


			if (x >= -6.94)
				transform.position = new Vector3(x,y,transform.position.z);
				
			transform.position += vShake * Time.deltaTime;

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
