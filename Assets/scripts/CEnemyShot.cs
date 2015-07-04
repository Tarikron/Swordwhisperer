using UnityEngine;
using System.Collections;

public class CEnemyShot : MonoBehaviour {

	public float shotSpeed;
	private float shotSpeedY = 0.0f;
	public float gravity = 1.0f;
	private Vector3 headingTo = Vector3.zero;
	public float damage = 1.0f;

	// Use this for initialization
	void Start () 
	{
		shotSpeedY = shotSpeed;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.GetComponent<BoxCollider2D>().enabled == true && headingTo != Vector3.zero)
		{
			//calculate to better values
			Vector3 vec3 = this.gameObject.transform.position;
			vec3.x += Time.deltaTime * shotSpeed * Mathf.Sign (headingTo.x);
			shotSpeedY += Time.deltaTime * gravity;
			vec3.y +=  shotSpeedY * Time.deltaTime;
			this.gameObject.transform.position = vec3;
		}
	}


	void msg_shotfired(Vector3 heading)
	{
		this.headingTo = heading;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag != "enemyShot" && collision.gameObject.tag != "enemyFlying"  && collision.gameObject.tag != "boss1"   && collision.gameObject.tag != "bossMinion" )
		{
			if (collision.gameObject.tag == "player")
			{
				collision.gameObject.SendMessage("msg_hit",damage,SendMessageOptions.RequireReceiver);
			}
			Destroy(this.gameObject);
		}
	}
}
