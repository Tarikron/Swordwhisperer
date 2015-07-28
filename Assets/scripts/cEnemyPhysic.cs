using UnityEngine;
using System.Collections;

public class cEnemyPhysic : MonoBehaviour 
{

	public LayerMask collisionMask;

	private BoxCollider2D _collider;
	private Vector3 size;
	private Vector3 centre;
	
	private float skin = .02f;
	
	private Ray ray;
	private RaycastHit2D hit;

	void Start()
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
}
