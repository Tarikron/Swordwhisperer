using UnityEngine;
using System.Collections;

public class cEnemyHurt : MonoBehaviour 
{

	void msg_die()
	{
		this.GetComponentInParent<cFlyingEnemy>().SendMessage("msg_die",null,SendMessageOptions.RequireReceiver);
	}
}
