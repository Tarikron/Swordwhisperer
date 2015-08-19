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



	public void SetCameraZ(float z)
	{
		Vector3 v = Vector3.zero;
		v.z = z;
		transform.Translate(v);
	}

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
			if (vShake.y > 0.9f)
				vShake.y = 0.9f;
			else if (vShake.y < -0.9f)
				vShake.y = -0.9f;

			vShake.z = 0.0f;

			float y = transform.position.y;

			float x = IncrementTowards(transform.position.x,target.position.x+offset.x,trackSpeed.x);
			if (stopCam == false)
				y = IncrementTowards(y,target.position.y+offset.y,trackSpeed.y);


			if (x >= -6.94)
				transform.position = new Vector3(x,y,transform.position.z);

			Vector3 zoom = transform.position;
			GameObject zoom1 = GameObject.FindGameObjectWithTag("camZoomStart");
			GameObject zoom2 = GameObject.FindGameObjectWithTag("camZoomEnd");

			Vector3 zoom1Pos = zoom1.transform.position;
			Vector3 zoom2Pos = zoom2.transform.position;
			if (zoom1Pos.x <= zoom.x && zoom2Pos.x >= zoom.x)
			{
				float endZ = -14.0f;
				float startZ = -9.3f+0.7f;

				//zoom.z + -9.4f;
				float distanceX_ALL = zoom2Pos.x - zoom1Pos.x;
				float distanceX_SC = zoom.x - zoom1Pos.x;

				float percentage = distanceX_SC/distanceX_ALL;

				float distanceZ_ALL = endZ - startZ;
				float currentZoom = startZ + distanceZ_ALL * percentage;

				zoom.z = currentZoom;

				transform.position = zoom;
				if (zoom.x <= 31.0f)
				{
					zoom.z = -9.3f;
					transform.position = zoom;
				}
			}

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
