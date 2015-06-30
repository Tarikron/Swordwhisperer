using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SplashScreenManager : MonoBehaviour 
{
	public enum eFadeState {FADE_IN = 0, FADE_DUR = 1, FADE_OUT = 2};
	public SplashScreen[] lScreens;

	private SplashScreen currentScreen;
	private int screenCounter = 0;
	private float fadeCounter = 0.0f;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (currentScreen == null && lScreens.Length > screenCounter)
			currentScreen = lScreens[screenCounter];

		if (currentScreen == null)
			return;

		Image img = currentScreen.GetComponent<Image>();

		if (img == null)
		{
			if (currentScreen.animIsDone)
			{
				fadeCounter += Time.deltaTime;
				if (fadeCounter >= currentScreen.duration)
				{
					Application.LoadLevel ("menu");
				}
			}
			else
			{
				fadeCounter = 0;
				currentScreen.Play();
			}
		}
		else
		{
			fadeCounter += Time.deltaTime;
			switch (currentScreen.fadeState)
			{
				case SplashScreen.eFadeState.FADE_IN:
					if (fadeCounter < currentScreen.fadeIn)
					{
						
						img.GetComponent<CanvasGroup>().alpha += Time.deltaTime * 1.0f/currentScreen.fadeIn;
					}
					else 
					{
						currentScreen.SetFadeStateDur();
						fadeCounter = 0;
					}
					break;
				case SplashScreen.eFadeState.FADE_DUR:
					if (fadeCounter >= currentScreen.duration)
					{
						currentScreen.SetFadeStateOut();
						fadeCounter = 0;
					}
					break;
				case SplashScreen.eFadeState.FADE_OUT:
					if (fadeCounter < currentScreen.fadeOut)
					{
						img.GetComponent<CanvasGroup>().alpha -= Time.deltaTime * 1.0f/currentScreen.fadeOut;
					}
					else 
					{
						img.GetComponent<CanvasGroup>().alpha = 0.0f;
						screenCounter++;
						currentScreen.SetFadeStateIn();
						fadeCounter = 0;
						currentScreen = null;
					}				
					break;
			}
		}
	}
}
