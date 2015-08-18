using UnityEngine;
using System.Collections;

public class endeScene : MonoBehaviour {

	public GameObject credits = null;
	private bool msgSend = false;

	private float fadeTime = 4.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (msgSend)
		{
			//start credits
			credits.GetComponent<CanvasGroup>().alpha += Time.deltaTime/fadeTime;
			if (credits.GetComponent<CanvasGroup>().alpha >= 1.0f)
				credits.GetComponent<CanvasGroup>().alpha = 1.0f;
			return;
		}

		Vector3 pos1 = Vector3.zero;
		Vector3 pos2 = Vector3.zero;

		GameObject player = GameObject.FindGameObjectWithTag("player");
		pos1.x = transform.position.x;
		pos2.x = player.transform.position.x;

		float distance = Vector3.Distance(pos1,pos2);

		if (distance <= 2.0f)
		{
			player.SendMessage("msg_looseLife",null,SendMessageOptions.RequireReceiver);
			msgSend = true;
		}
	}
}
