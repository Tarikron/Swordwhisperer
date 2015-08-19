using UnityEngine;
using System.Collections;

public class endeScene : MonoBehaviour {

	public GameObject credits = null;
	public GameObject creditsMoveUp = null;

	private bool msgSend = false;

	private float fadeTime = 2.0f;

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
			{
				Vector3 pos = creditsMoveUp.transform.position;
				pos.y += Time.deltaTime * 6.0f;
				if (pos.y < 8.5f)
					creditsMoveUp.transform.position = pos; 
				credits.GetComponent<CanvasGroup>().alpha = 1.0f;
			}
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

			Camera mainCam = Camera.main;
			GameCamera gameCam = mainCam.GetComponent<GameCamera>();
			Vector3 pos = Vector3.zero;
			pos.x = 10.0f;
			pos.y = 5.0f;
			gameCam.SetOffset(pos);
			gameCam.SetTrackSpeed(8.0f,8.0f);
		}
	}
}
