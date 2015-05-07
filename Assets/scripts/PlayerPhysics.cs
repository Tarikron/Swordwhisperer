using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour 
{
	public LayerMask collisionMask;

	[HideInInspector]
	public bool grounded;
	[HideInInspector]
	public bool movementStopped;

	private BoxCollider2D _collider;
	private Vector3 size;
	private Vector3 centre;

	private float skin = .02f;

	private Ray ray;
	private RaycastHit2D hit;
	[HideInInspector]
	public Rigidbody2D rb2D;

	void Start()
	{
		_collider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		size = _collider.size;
		centre = _collider.offset;
	}

	public void Move(Vector3 moveAmount)
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
			Debug.DrawRay(ray.origin,ray.direction);
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

		movementStopped = false;
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
					deltaX = 0;

				movementStopped = true;
				break;
			}
		}

		if (!grounded && !movementStopped)
		{
			Vector3 playerDir = new Vector3 (deltaX, deltaY);
			Vector3 o = new Vector3(p.x + centre.x + size.x/2 * Mathf.Sign(deltaX),p.y + centre.y + size.y/2 * Mathf.Sign(deltaY));
			//Debug.DrawRay(o,playerDir.normalized);
			ray = new Ray(o, playerDir.normalized);
			if (Physics2D.Raycast(o, playerDir.normalized,Mathf.Sqrt(deltaX*deltaX + deltaY*deltaY),collisionMask))
			{
				grounded = true;
				deltaY = 0;
			}
		}

		Vector2 finalTransform = new Vector2(deltaX,deltaY);
		//transform.Translate(finalTransform);
		rb2D.MovePosition(rb2D.position + finalTransform);
	}
}
