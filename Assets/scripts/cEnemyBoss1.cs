using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cEnemyBoss1 : cEnemy
{
	public float accelerationX = 10.0f;
	public float hurtVelocity = 3.0f;
	public float flyVelocity = 6.0f;

	[System.Serializable]
	public struct stMinions
	{
		public int count;
		public float rotationSpeedAngle;
		public float acceleration;

		public float rotationDirection;
		public float radiusToBossCenter;
	};
	public stMinions minions;
	public GameObject minion;
	private List<GameObject> lMinions;
	private float currentAngleSpeed = 0.0f;      

	private Vector2 _colliderSize;
	private Vector2 _colliderOffset;
	private Vector2 _centre;

	private float bossAlpha = 0.0f;

	public void Start()
	{
		lMinions = new List<GameObject>();

		int minionsCount = minions.count;
		float radius = minions.radiusToBossCenter;
		float alpha = 0.0f;
		float angleDistance = 360.0f/(float)minionsCount;

		BoxCollider2D collider2d = GetComponent<BoxCollider2D>();
		_colliderSize = collider2d.size;
		_colliderOffset = collider2d.offset;

		_centre.x = transform.position.x + _colliderOffset.x + _colliderSize.x/2 + transform.lossyScale.x/2 + 0.5f;
		_centre.y = transform.position.y + _colliderOffset.y + _colliderSize.y/2 + transform.lossyScale.y/2 + 0.5f;

		float minionX = 0.0f;
		float minionY = 0.0f;

		for (int i = 0; i < minions.count; i++)
		{
			minionX = _centre.x + minions.radiusToBossCenter * Mathf.Cos (alpha * Mathf.PI/180);
			minionY = _centre.y + minions.radiusToBossCenter * Mathf.Sin (alpha * Mathf.PI/180);

			GameObject ga = GameObject.Instantiate(minion);		
			ga.GetComponent<bossMinion>().alpha = alpha;
			ga.transform.position = new Vector3(minionX,minionY,1.0f);

			lMinions.Add (ga);
			alpha += angleDistance;
		}
	}

	public void Update()
	{
		int minionCount = lMinions.Count;
		Vector3 movement = new Vector3(0.0f,0.0f,0.0f);

		transform.position = new Vector3(transform.position.x,transform.position.y + Time.deltaTime * Mathf.Sin(bossAlpha * Mathf.PI/180),transform.position.z);
		bossAlpha += 2.0f;
		if (bossAlpha > 360.0f)
			bossAlpha = 0.0f;

		_centre.x = transform.position.x + _colliderOffset.x + _colliderSize.x/2 + transform.lossyScale.x/2 + 0.5f;
		_centre.y = transform.position.y + _colliderOffset.y + _colliderSize.y/2 + transform.lossyScale.y/2 + 0.5f;

		for (int i = 0; i<minionCount; i++)
		{
			GameObject _minion = lMinions[i];
			float alpha = _minion.GetComponent<bossMinion>().alpha;

			currentAngleSpeed = IncrementTowards(currentAngleSpeed,minions.rotationSpeedAngle,minions.acceleration);
			alpha += currentAngleSpeed * Mathf.Sign (minions.rotationDirection);
			if (alpha > 360.0f)
				alpha = 0.0f;
			if (alpha < 0.0f)
				alpha = 360.0f;

			float radius = minions.radiusToBossCenter;

			movement.x = _centre.x + radius * Mathf.Cos (alpha * Mathf.PI/180);
			movement.y = _centre.y + radius * Mathf.Sin (alpha * Mathf.PI/180);
			movement.z = 1.0f;

			_minion.transform.position = movement;
			_minion.GetComponent<bossMinion>().alpha = alpha;

			if (alpha < 270.0f && alpha > 90.0f)
			{
				//minions shot
				_minion.SendMessage("msg_shot",null,SendMessageOptions.RequireReceiver);
			}
		}

	}

}


