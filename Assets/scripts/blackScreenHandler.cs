using UnityEngine;
using System.Collections;


public class blackScreenHandler : MonoBehaviour {

	private bool fadeDone = false;

	CanvasGroup canvasGrp;

	public float fadeTime = 2.0f;
	public float timeDelay = 2.0f;
	private float timeCounter = 0.0f;
	private bool bFade = false;

	public enum eCutsceneSteps {FADE_OUT = 0 , FADE_IN = 1};
	public eCutsceneSteps state = eCutsceneSteps.FADE_OUT;

	[HideInInspector]
	public bool bInit = false;

	// Use this for initialization
	void Start () {

		canvasGrp = GetComponent<CanvasGroup>();
		canvasGrp.alpha = 1.0f;

		bInit = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!bInit)
			return;
		if (state == eCutsceneSteps.FADE_OUT)
			fadeOut();
		else if (state == eCutsceneSteps.FADE_IN)
			fadeIn();
	}

	public void init()
	{
		if (bInit)
			return;
		GameObject.FindGameObjectWithTag("player").SendMessage("msg_blackscreenArrive",null,SendMessageOptions.RequireReceiver);
		fadeDone = false;
		bFade = false;
		timeCounter = 0.0f;

		bInit  = true;
	}

	private void fadeIn()
	{
		if (fadeDone)
			return;
		if (timeCounter >= timeDelay)
			bFade = true;
		else
			timeCounter += Time.deltaTime;
		
		//100 = 2.0f
		//  x =  0.012421
		
		if (bFade)
		{
			float scale= Time.deltaTime/fadeTime;
			canvasGrp.alpha += scale/1.0f;
			
			if (canvasGrp.alpha >= 1.0f)
			{
				fadeDone = true;
				bInit = false;
				canvasGrp.alpha = 1.0f;
			}
		}
	}

	private void fadeOut()
	{
		if (fadeDone)
			return;
		if (timeCounter >= timeDelay)
			bFade = true;
		else
			timeCounter += Time.deltaTime;
		
		//100 = 2.0f
		//  x =  0.012421
		
		if (bFade)
		{
			float scale= Time.deltaTime/fadeTime;
			canvasGrp.alpha -= scale/1.0f;
			
			if (canvasGrp.alpha <= 0.01f)
			{
				fadeDone = true;
				bInit = false;
				GameObject.FindGameObjectWithTag("player").SendMessage("msg_blackscreenGone",null,SendMessageOptions.RequireReceiver);
			}
		}
	}

	public bool IsFadeDone()
	{
		return fadeDone;
	}

}
