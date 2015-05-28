using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{

	public void Play()
	{
		Application.LoadLevel("swordwhisperer");
	}

	public void Options()
	{
	}

	public void Credits()
	{
	}

	public void Exit()
	{
		Application.Quit();
	}


}
