using UnityEngine;
using System.Collections;


public class blackScreenHandler : MonoBehaviour {

	private bool fadeOutDone = false;

	CanvasGroup canvasGrp;

	public float timeDelay = 2.0f;
	private float timeCounter = 0.0f;
	private bool bFadeOut = false;


	public float fadeOutTime = 2.0f;
	private float fadeOutCounter = 0.0f;

	// Use this for initialization
	void Start () {

		canvasGrp = GetComponent<CanvasGroup>();
		canvasGrp.alpha = 1.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (fadeOutDone)
			return;
		if (timeCounter >= timeDelay)
			bFadeOut = true;
		else
			timeCounter += Time.deltaTime;

		//100 = 2.0f
		//  x =  0.012421

		if (bFadeOut)
		{
			float scale= Time.deltaTime/2.0f;
			canvasGrp.alpha -= scale/1.0f;

			if (canvasGrp.alpha <= 0.1f)
			{
				fadeOutDone = true;
				GameObject.FindGameObjectWithTag("player").SendMessage("msg_blackscreen",null,SendMessageOptions.RequireReceiver);
			}
		}


	}
}
