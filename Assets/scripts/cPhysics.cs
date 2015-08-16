using UnityEngine;
using System.Collections;

public class cPhysics : MonoBehaviour 
{
	public LayerMask collisionMask;
	
	[HideInInspector]
	public bool grounded;
	[HideInInspector]
	public bool onSlope;
	[HideInInspector]
	public bool movementStopped;
	[HideInInspector]
	public float distanceToGround;
	
	private BoxCollider2D _collider;
	private Vector3 size;
	private Vector3 centre;
	
	private float skin = .02f;
	
	private Ray ray;
	private RaycastHit2D hit;
	
	
	
	public virtual void Start()
	{
		_collider = GetComponent<BoxCollider2D>();
		size = _collider.size;
		
		Vector3 vec3 = _collider.offset;
		centre = vec3;
	}
	
	public float GetDistanceToGround()
	{
		Vector2 p = transform.position;
		
		float dir = -1.0f;
		float x = (p.x + centre.x - size.x/2) + size.x/2;
		float y = p.y + centre.y + size.y/2 * dir;
		
		ray = new Ray(new Vector2(x,y), new Vector2 (0, dir));
		//Debug.DrawRay(ray.origin,ray.direction);
		if ((hit = Physics2D.Raycast (new Vector2(x,y), new Vector2 (0, dir),float.PositiveInfinity,collisionMask)))
			return Vector3.Distance (ray.origin,hit.point);
		
		return float.PositiveInfinity;
	}
	
	public Vector3 Move(Vector3 moveAmount)
	{
		float deltaY = moveAmount.y;
		float deltaX = moveAmount.x;
		Vector2 p = transform.position;
		
		grounded = false;
		for (int i = 0; i <3; i++)
		{
			float dir = Mathf.Sign(deltaY);
			float x = (p.x + centre.x - size.x/2) + size.x/2 * i;
			float y = p.y + centre.y + size.y/2 * dir;
			
			ray = new Ray(new Vector2(x,y), new Vector2 (0, dir));
			//Debug.DrawRay(ray.origin,ray.direction);
			if ((hit = Physics2D.Raycast (new Vector2(x,y), new Vector2 (0, dir),Mathf.Abs (deltaY) + skin,collisionMask)))
			{
				float dst = Vector3.Distance (ray.origin,hit.point);
				
				if (dst > skin)
					deltaY = dst * dir - skin * dir;
				else
					deltaY = 0;
				grounded = true;
				break;
			}
		}
		
		/*
		movementStopped = false;
		if (deltaX == 0.0f)
		{
			for (int i = 0; i <3; i++)
			{
				float dir = Mathf.Sign(deltaX);
				float x = p.x + centre.x + size.x/2 * dir;
				float y = p.y + centre.y - size.y/2 + size.y/2 * i;
				
				ray = new Ray(new Vector2(x,y), new Vector2 (dir, 0));
				//Debug.DrawRay(ray.origin,ray.direction);
				if ((hit = Physics2D.Raycast (new Vector2(x,y), new Vector2 (dir, 0),Mathf.Abs (deltaX) + skin,collisionMask)))
				{
					float dst = Vector3.Distance (ray.origin,hit.point);
					if (dst > skin)
						deltaX = dst * dir - skin * dir;
					else
						deltaX = 0; // if we have large slope cancel x

					movementStopped = true;
					break;
				}
			}
		}*/
		//todo right detector
		onSlope = false;
		if (grounded)
		{
			float yDir = -1;
			float xDir = Mathf.Sign(deltaX);
			if (transform.eulerAngles.y >= 180.0f)
				xDir = -1.0f;
			float x1 = p.x + centre.x - size.x/2 + size.x;
			float x2 = p.x + centre.x - size.x/2;
			float y = p.y + centre.y + size.y/2 * yDir;
			
			
			Ray ray_left= new Ray(new Vector2(x2,y), new Vector2 (0, yDir));
			Ray ray_right = new Ray(new Vector2(x1,y), new Vector2 (0, yDir));
			
			RaycastHit2D hit_left = Physics2D.Raycast (new Vector2(x2,y), new Vector2 (0, yDir),float.PositiveInfinity,collisionMask);
			RaycastHit2D hit_right = Physics2D.Raycast (new Vector2(x1,y), new Vector2 (0, yDir),float.PositiveInfinity,collisionMask);
			
			float slopeL = Vector3.Angle(transform.up,hit_left.normal);
			float slopeR = Vector3.Angle(transform.up,hit_right.normal);
			
			Debug.DrawRay(ray_left.origin,ray_left.direction);
			Debug.DrawRay(ray_right.origin,ray_right.direction);
			
			float dst_left = Vector3.Distance (ray_left.origin,hit_left.point);
			float dst_right = Vector3.Distance (ray_right.origin,hit_right.point);
			//Debug.Log("left: " + slopeL + " right: " + slopeR);
			
			if (slopeL > 1.0f && slopeL < 90.0f || slopeR > 1.0f && slopeR < 90.0f )
			{
				//Debug.Log ("1 - " + deltaY);

				if ( slopeL < 90.0f)
				{
					deltaY += Mathf.Tan ((slopeL * Mathf.PI)/180.0f) * deltaX * xDir;
					onSlope = true;
				}
				else if (slopeR < 90.0f)
				{
					deltaY += Mathf.Tan ((slopeR * Mathf.PI)/180.0f) * deltaX * xDir;
					onSlope = true;
				}
				
				if (cFunction.xor(dst_left < dst_right && xDir > 0.0f,dst_left < dst_right && xDir < 0.0f)) //moving down to left or up to right
					deltaY *= -1.0f;

				//Debug.Log ("2 - " + deltaY);
			}
			
			ray = new Ray(new Vector2(transform.position.x,transform.position.y), (new Vector2 (deltaX*xDir, deltaY)));
			Debug.DrawRay(ray.origin,ray.direction*10.0f);
			
		}
		
		Vector3 playerDir = new Vector3 (deltaX, deltaY);
		if (!grounded && !movementStopped)
		{
			Vector3 o = new Vector3(p.x + centre.x + size.x/2 * Mathf.Sign(deltaX),p.y + centre.y + size.y/2 * Mathf.Sign(deltaY));
			//Debug.DrawRay(o,playerDir.normalized);
			ray = new Ray(o, playerDir.normalized);
			if (Physics2D.Raycast(o, playerDir.normalized,Mathf.Sqrt(deltaX*deltaX + deltaY*deltaY),collisionMask))
			{
				grounded = true;
				deltaY = 0;
			}
		}
		
		Vector3 finalTransform = new Vector3(deltaX,deltaY,0.0f);
		transform.Translate (finalTransform);
		
		return playerDir.normalized;
	}
}
