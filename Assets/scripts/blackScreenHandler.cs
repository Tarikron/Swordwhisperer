using UnityEngine;
using System.Collections;


public class blackScreenHandler : MonoBehaviour {

	public GameObject sendMsgTo = null;
	private bool fadeDone = false;

	CanvasGroup canvasGrp;

	public float fadeTime = 2.0f;
	public float timeDelay = 2.0f;
	[HideInInspector]
	public float timeCounter = 0.0f;
	private bool bFade = false;

	public enum eCutsceneSteps {FADE_OUT = 0 , FADE_IN = 1};
	public eCutsceneSteps state = eCutsceneSteps.FADE_OUT;

	[HideInInspector]
	public bool bInit = false;

	// Use this for initialization
	void Start () {

		canvasGrp = GetComponent<CanvasGroup>();

		if (state == eCutsceneSteps.FADE_OUT)
			canvasGrp.alpha = 1.0f;
		else if (state == eCutsceneSteps.FADE_IN)
			canvasGrp.alpha = 0.0f;

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

	public void init(float defaultAlpha = 1.0f)
	{
		if (bInit)
			return;

		if (canvasGrp == null) //strange unity behavior, called init will be executed after start, why???
			canvasGrp = GetComponent<CanvasGroup>();
		sendMsgTo.SendMessage("msg_blackscreenArrive",null,SendMessageOptions.DontRequireReceiver);
		fadeDone = false;
		bFade = false;
		timeCounter = 0.0f;

		bInit  = true;

		canvasGrp.alpha = defaultAlpha;
	}

	private void fadeIn()
	{
		if (fadeDone)
			return;
		if (timeCounter >= timeDelay)
			bFade = true;
		else
			timeCounter += Time.deltaTime;

		if (bFade)
		{
			float scale= Time.deltaTime/fadeTime;
			canvasGrp.alpha += scale/1.0f;
			
			if (canvasGrp.alpha >= 1.0f)
			{
				fadeDone = true;
				bInit = false;
				canvasGrp.alpha = 1.0f;
				sendMsgTo.SendMessage("msg_blackscreenFull",null,SendMessageOptions.DontRequireReceiver);
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
				sendMsgTo.SendMessage("msg_blackscreenGone",null,SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public bool IsFadeDone()
	{
		return fadeDone;
	}

}
