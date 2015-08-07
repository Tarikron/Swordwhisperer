using UnityEngine;
using System.Collections;

public class cGrave : MonoBehaviour 
{

	private float fadeTime = 1.0f;
	private bool interact = false;
	private bool stopInteract = false;

	private void manageInteraction(Vector2 playerPos)
	{
		//todo
		float distance = Vector2.Distance(playerPos,transform.position);
		GameObject go = transform.FindChild("PressButton_go").gameObject;
		if (go)
		{
			Color c = go.GetComponent<SpriteRenderer>().color;
			
			if (distance <= 7.0f && !stopInteract)
			{
				c.a += Time.deltaTime/fadeTime;
				if (c.a > 1.0f)
				{
					c.a = 1.0f;
					interact = true;
				}
			}
			else
			{
				c.a -= Time.deltaTime/fadeTime;
				if (c.a < 0.0f)
				{
					c.a = 0.0f;
				}
			}
			go.GetComponent<SpriteRenderer>().color = c;
		}
		
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		GameObject player = GameObject.Find("Player");
		Vector2 playerPos = player.transform.position;	

		if (interact && !stopInteract && Input.GetButtonDown("CtrlBButton"))
		{
			stopInteract = true;
			player.GetComponent<cPlayer_c>().dialog.SendMessage("msg_eventTrigger","graveInteract",SendMessageOptions.RequireReceiver);
		}

		manageInteraction(playerPos);
	}
}

