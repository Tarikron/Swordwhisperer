using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIShadowFork : MonoBehaviour
{
	public Color color;
	private Color oldColor;
	public Shadow[] shadows;

	void Start()
	{
		SetColor();
	}

	void LateUpdate()
	{
		if(oldColor != color)
		{
			SetColor();
		}
	}

	public void SetColor()
	{
		foreach(Shadow shadow in shadows)
		{
			oldColor = color;
			shadow.effectColor = color;
		}
	}

	public void SetColor(Color effectColor)
	{
		foreach(Shadow shadow in shadows)
		{
			color = effectColor;
			oldColor = effectColor;
			shadow.effectColor = effectColor;
		}
	}
}
