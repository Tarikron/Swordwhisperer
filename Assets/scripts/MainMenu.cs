using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
	public GameObject optionPanel;
	public GameObject creditsPanel;
	public GameObject allPanel;
	public GameObject blackscreen;
	public GameObject loadingScreen;

	private CanvasGroup optionPanelGroup = null;
	private CanvasGroup creditsPanelGroup = null;
	private CanvasGroup allPanelGroup = null;
	private blackScreenHandler blackScreenGroup = null;

	public float fadeBlackScreenTime = 1.0f;
	public float delay = 3.0f;
	private float delayTimer = 0.0f;
	private float fadeTime = 1.0f;

	public string levelName = "swordwhisperer";
	AsyncOperation async;
	private bool fading = false;

	private bool test = false;

	void Start()
	{
		optionPanelGroup = optionPanel.GetComponent<CanvasGroup>();
		creditsPanelGroup = creditsPanel.GetComponent<CanvasGroup>();
		allPanelGroup = allPanel.GetComponent<CanvasGroup>();
		blackScreenGroup = blackscreen.GetComponent<blackScreenHandler>();
		
		blackScreenGroup.bInit = false;
		blackScreenGroup.init(0.0f);
	}

	void Update()
	{
		
		if (allPanelGroup.alpha >= 1.0f)
		{
			if (fading)
			{
				blackScreenGroup.timeDelay = 0.0f;
				blackScreenGroup.state = blackScreenHandler.eCutsceneSteps.FADE_IN;
				if (blackScreenGroup.IsFadeDone())
				{
					StartCoroutine(load());
				}
			}
			else
			{
				blackScreenGroup.timeCounter = 0.0f;
				if (Input.anyKeyDown)
					fading = true;
			}
		}
		else
		{
			blackScreenGroup.timeCounter = 0.0f;
			if (delayTimer >= delay)
			{
				allPanelGroup.alpha += Time.deltaTime/fadeTime;
				if (allPanelGroup.alpha >= 1.0f)
				{
					allPanelGroup.alpha = 1.0f;
					delayTimer = 0.0f;
					loadingScreen.GetComponent<MeshRenderer>().enabled = true;
					loadingScreen.GetComponent<SkeletonAnimation>().enabled = true;
				}
			}

			delayTimer += Time.deltaTime;
		}
	}

	IEnumerator load() {
		Debug.Log("ASYNC LOAD STARTED - " +
		                 "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH");
		async = Application.LoadLevelAsync(levelName);

		yield return async;

	}

	public void Play()
	{
		Application.LoadLevel("swordwhisperer");
	}

	public void Options()
	{
		optionPanelGroup.alpha = 1.0f;
	}

	public void Credits()
	{
		if(creditsPanelGroup.alpha == 1.0f){
			creditsPanelGroup.alpha = 0.0f;
		}else{
			creditsPanelGroup.alpha = 1.0f;
		}
	}

	public void Exit()
	{
		Application.Quit();
	}


	private void hideAll()
	{
		optionPanelGroup.alpha = 0.0f;
		creditsPanelGroup.alpha = 0.0f;
	}

}
