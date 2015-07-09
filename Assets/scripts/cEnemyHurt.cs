using UnityEngine;
using System.Collections;

public class cEnemyHurt : MonoBehaviour 
{
	void msg_damage(float dmg)
	{
		GameObject parent = transform.parent.gameObject;

		if (parent.CompareTag("enemyFlying"))
			parent.GetComponent<cFlyingEnemy>().SendMessage("msg_damage",dmg,SendMessageOptions.RequireReceiver);
		else if (parent.CompareTag("bossMinion"))
			parent.GetComponent<bossMinion>().SendMessage("msg_damage",dmg,SendMessageOptions.RequireReceiver);				
		else if (parent.CompareTag("heroTurtle"))
			parent.GetComponent<cTurtle>().SendMessage("msg_damage",dmg,SendMessageOptions.RequireReceiver);
	}
}
