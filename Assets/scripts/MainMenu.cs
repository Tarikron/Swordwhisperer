using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
	public GameObject optionPanel;
	public GameObject creditsPanel;

	private CanvasGroup optionPanelGroup = null;
	private CanvasGroup creditsPanelGroup = null;

	void Start()
	{
		optionPanelGroup = optionPanel.GetComponent<CanvasGroup>();
		creditsPanelGroup = creditsPanel.GetComponent<CanvasGroup>();
	}

	void Update()
	{
		if (Input.anyKeyDown)
		{
			Application.LoadLevel("swordwhisperer");
		}
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
