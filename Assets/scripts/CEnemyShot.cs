using UnityEngine;
using System.Collections;

public class CEnemyShot : MonoBehaviour {

	public float shotSpeed;
	private Vector3 headingTo = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.GetComponent<BoxCollider2D>().enabled == true && headingTo != Vector3.zero)
		{
			Vector3 vec3 = this.gameObject.transform.position;
			vec3 += Time.deltaTime * shotSpeed * headingTo;
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
				collision.gameObject.SendMessage("msg_hit",null,SendMessageOptions.RequireReceiver);
			}
			Destroy(this.gameObject);
		}
	}
}
