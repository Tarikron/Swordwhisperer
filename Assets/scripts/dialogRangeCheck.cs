using UnityEngine;
using System.Collections;

public class dialogRangeCheck : MonoBehaviour 
{

	public string eventTrigger = "";
	public float triggerDistance = 2.0f;

	private cPlayer_c player;

	private bool messageShown = false;

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("player").GetComponent<cPlayer_c>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (messageShown)
			return;

		Vector3 pos1 = Vector3.zero;
		Vector3 pos2 = Vector3.zero;
		pos1.x = transform.position.x;
		pos2.x = player.gameObject.transform.position.x;
		float distance = Vector3.Distance (pos1,pos2);
		if (distance <= triggerDistance)
		{
			player.dialog.SendMessage("msg_eventTrigger",eventTrigger,SendMessageOptions.RequireReceiver);
			messageShown = true;
		}
	}
}
