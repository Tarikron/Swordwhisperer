using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class cHurtScreenMgr : MonoBehaviour {

	public float hurtpercentage = 25.0f;
	private cPlayer_c player = null;
	private int currentLife = 0;

	public int alphaFadeInFrames = 5;
	public List<Image> hurtScreens;
	private int currentHurtScreen = 0;
	private int oldHurtScreen = -1;
	private int direction = 1;

	public int skipFrame = 0;
	private int skip = 0;

	// Use this for initialization
	void Start () {
		player = GetComponent<cPlayer_c>();
	}
	
	// Update is called once per frame
	void Update () {
		currentLife = player.currentLife;
		int startLife = player.startLife;
		Color col;

		skip++;

		if (skipFrame == skip)
		{

		
			if (currentLife < startLife * (hurtpercentage/100.0f))
			{
				//show hurtscreen
				Image currentImg = hurtScreens[currentHurtScreen];
				col = currentImg.color;
				col.a += 1.0f/(float)alphaFadeInFrames;
				currentImg.color = col;

				if (oldHurtScreen >= 0)
				{
					Image oldImg = hurtScreens[oldHurtScreen];
					col = oldImg.color;
					col.a -= 1.0f/(float)alphaFadeInFrames;
					oldImg.color = col;
				}

				if (currentImg.color.a >= 1.0f)
				{
					oldHurtScreen = currentHurtScreen;
					if (currentHurtScreen+direction >= hurtScreens.Count)
						direction = -1;
					else if (currentHurtScreen+direction < 0 )
						direction = 1;
					currentHurtScreen = currentHurtScreen + direction;

				}
			}
			skip = 0;
		}
	}
}
