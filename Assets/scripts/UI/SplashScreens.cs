using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SplashScreens : MonoBehaviour
{
	[System.Serializable]
	public class SplashScreenObject
	{
		public GameObject gameObjectRef;
		public float duration;
		public bool fadeIn = true;
		public bool fadeOut = true;
		public float fadeInDuration;
		public float fadeOutDuration;

		public bool fadeImages = true;
		public bool fadeTexts = true;
		public bool fadeOutlines = true;
		public bool fadeShadows = true;
		public bool useShadowFork = true;
	}

	public List<SplashScreenObject> splashScreens = new List<SplashScreenObject>();

	public bool allowSkipping = true;
	public float skipFadeOutFactor = 2.5f;

	public bool loadScene = true;
	public string sceneName = "Menu";

	private float currentDuration;
	private int currentSplashScreenIndex = 0;
	private float currentFadeDuration = 0;

	private Image[] currentImages = new Image[0];
	private Text[] currentTexts = new Text[0];
	private Outline[] currentOutlines = new Outline[0];
	private Shadow[] currentShadows = new Shadow[0];
	private UIShadowFork[] currentShadowForks = new UIShadowFork[0];

	void Start()
	{
		if(splashScreens.Count > 0 && splashScreens[0] != null)
			StartSplashScreen(0);
	}

	IEnumerator SplashScreenDuration(int i)
	{
		if(splashScreens[i].gameObjectRef.activeSelf == false)
		{
			splashScreens[i].gameObjectRef.SetActive(true);
		}

		currentDuration = splashScreens[i].duration;
		while(currentDuration > 0)
		{
			currentDuration -= Time.deltaTime * Time.timeScale;
			if(Input.anyKey == true && allowSkipping == true)
				currentDuration = 0;

			yield return new WaitForEndOfFrame();
		}

		if(splashScreens[i].fadeOut == false)
		{
			splashScreens[i].gameObjectRef.SetActive(false);
			NextSplashScreen();
		}
		else
		{
			StartCoroutine("FadeOut", i);
		}
	}

	IEnumerator FadeIn(int i)
	{

		if(splashScreens[i].gameObjectRef.activeSelf == false)
		{
			splashScreens[i].gameObjectRef.SetActive(true);
		}

		if(splashScreens[i].fadeImages)
			currentImages = splashScreens[i].gameObjectRef.GetComponentsInChildren<Image>(true);

		if(splashScreens[i].fadeTexts)
			currentTexts = splashScreens[i].gameObjectRef.GetComponentsInChildren<Text>(true);

		if(splashScreens[i].fadeOutlines)
			currentOutlines = splashScreens[i].gameObjectRef.GetComponentsInChildren<Outline>(true);

		if(splashScreens[i].fadeShadows)
		{
			if(splashScreens[i].useShadowFork == true)
			{
				currentShadowForks = splashScreens[i].gameObjectRef.GetComponentsInChildren<UIShadowFork>(true);
			}
			else
			{
				currentShadows = splashScreens[i].gameObjectRef.GetComponentsInChildren<Shadow>(true);
			}
		}

		SetTransparency(0f);

		currentFadeDuration = splashScreens[i].fadeInDuration;
		while(currentFadeDuration > 0)
		{
			currentFadeDuration -= Time.deltaTime * Time.timeScale;

			SetTransparency(SmoothedLerp(1f, 0f, currentFadeDuration / splashScreens[i].fadeInDuration));
			if(Input.anyKey == true && allowSkipping == true)
				currentFadeDuration = 0;

			yield return new WaitForEndOfFrame();
		}

		StartCoroutine("SplashScreenDuration", i);
		yield break;
	}

	IEnumerator FadeOut(int i)
	{		
		SetTransparency(1f);

		currentFadeDuration = splashScreens[i].fadeOutDuration;
		while(currentFadeDuration > 0)
		{
			float factor = 1f;

			if(Input.anyKey == true && allowSkipping == true)
				factor = skipFadeOutFactor;

			currentFadeDuration -= factor * Time.deltaTime * Time.timeScale;
			
			SetTransparency(SmoothedLerp(0f, 1f, currentFadeDuration / splashScreens[i].fadeOutDuration));
			
			yield return new WaitForEndOfFrame();
		}
		
		if(splashScreens[i].gameObjectRef.activeSelf == true)
		{
			splashScreens[i].gameObjectRef.SetActive(false);
		}

		NextSplashScreen();
	}

	void SetTransparency(float transparency)
	{
		foreach(Image image in currentImages)
		{
			Color tempColor = image.color;
			tempColor.a = transparency;
			image.color = tempColor;
		}
		
		foreach(Text text in currentTexts)
		{
			Color tempColor = text.color;
			tempColor.a = transparency;
			text.color = tempColor;
		}
		
		foreach(Outline outline in currentOutlines)
		{
			Color tempColor = outline.effectColor;
			tempColor.a = transparency;
			outline.effectColor = tempColor;
		}
		
		foreach(Shadow shadow in currentShadows)
		{
			Color tempColor = shadow.effectColor;
			tempColor.a = transparency;
			shadow.effectColor = tempColor;
		}

		foreach(UIShadowFork shadowFork in currentShadowForks)
		{
			Color tempColor = shadowFork.color;
			tempColor.a = transparency;
			shadowFork.SetColor(tempColor);
		}
	}

	public float SmoothedLerp(float from, float to, float lerpInput)
	{
		lerpInput = Mathf.Clamp01(lerpInput);
		return Mathf.Lerp(from, to, Mathf.SmoothStep(0, 1f, lerpInput));
	}

	void StartSplashScreen(int i)
	{
		currentSplashScreenIndex = i;
		if(splashScreens[i].fadeIn == true)
		{
			StartCoroutine("FadeIn", i);
		}
		else
		{
			StartCoroutine("SplashScreenDuration", i);
		}
	}

	bool NextSplashScreen()
	{
		if(currentSplashScreenIndex < splashScreens.Count - 1 && splashScreens[currentSplashScreenIndex + 1] != null)
		{
			currentSplashScreenIndex++;
			StartSplashScreen(currentSplashScreenIndex);
			return true;
		}
		else if(currentSplashScreenIndex >= splashScreens.Count - 1)
		{
			if(loadScene == true)
				LoadScene();
		}
		return false;
	}

	public void LoadScene()
	{
		Application.LoadLevel(1);
	}
}
