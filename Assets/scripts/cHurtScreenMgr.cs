using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class cHurtScreenMgr : MonoBehaviour {

	public float hurtpercentage = 25.0f;
	private cPlayer_c player = null;
	private float currentLife = 0;

	public List<Image> hurtScreens;
	private int currentHurtScreen = 0;
	private int oldHurtScreen = -1;
	private int direction = 1;

	private float frameTimer = 0;

	public AudioSource audioSrcHearth;
	public List<AudioClip> audioHearthBeats;

	// Use this for initialization
	void Start () {
		player = GetComponent<cPlayer_c>();
	}
	
	// Update is called once per frame
	void Update () {
		currentLife = player.currentLife;
		float startLife = player.startLife;
		Color col;

		if (!player.GetBlackSreenState())
			return;

		bool stop = false;
		float fadeInTimePerFrame = 1.0f/(float)hurtScreens.Count;
		float scale = Time.deltaTime/fadeInTimePerFrame;

		if (player.GetSwordTakeState() == cPlayer_c.eSwordTakeAfter.WHILE)
		{
			direction = -1;
			stop = true;

			oldHurtScreen = currentHurtScreen;
			currentHurtScreen = currentHurtScreen + direction;
		}
		//if (waitTime <= frameTimer)
		{
			if (cFunction.xor( (currentLife < startLife * (hurtpercentage/100.0f)),stop) )
			{
				//1.0f = alphaFadeInTime
				//x = deltaTime

				//3 frames und alpha channel
				// calculate current framerate

				//Time.deltaTime  = 1 frame
				//1sec = x frames

				//-> fade in process of alpha needs 1/3 sec of 1 frametime
				if (currentHurtScreen < 0)
				{
					//dirty solution, todo do it better
					Image img1 = hurtScreens[0];
					col = img1.color;
					col.a -= scale;
					img1.color = col;

					Image img2 = hurtScreens[1];
					col = img2.color;
					col.a -= scale;
					img2.color = col;

					Image img3 = hurtScreens[2];
					col = img3.color;
					col.a -= scale;
					img3.color = col;

					if (img1.color.a <= 0.0f && img2.color.a <= 0.0f && img3.color.a <= 0.0f)
					{
						col = img1.color;
						col.a = 0.0f;
						img1.color = col;
						col = img2.color;
						col.a = 0.0f;
						img2.color = col;
						col = img3.color;
						col.a = 0.0f;
						img3.color = col;

						currentHurtScreen = 0;
						stop = false;
					}
					currentHurtScreen = -1;
					return;
				}
				//show hurtscreen
				Image currentImg = hurtScreens[currentHurtScreen];
				col = currentImg.color;
				col.a += scale;
				if (col.a > 1.0f)
					col.a = 1.0f;
				currentImg.color = col;

				if (currentHurtScreen == 2) //we are at peak play hearthbeat
				{
					if (!audioSrcHearth.isPlaying)
					{
						int audioIndex = Random.Range (0,audioHearthBeats.Count-1);

						audioSrcHearth.clip = audioHearthBeats[audioIndex];
						audioSrcHearth.Play();
					}
				}

				if (oldHurtScreen >= 0)
				{
					Image oldImg = hurtScreens[oldHurtScreen];
					col = oldImg.color;
					col.a -= scale;
					if (col.a < 0.0f)
						col.a = 0.0f;
					oldImg.color = col;
				}

				if (currentImg.color.a >= 1.0f)
				{
					oldHurtScreen = currentHurtScreen;
					if (!stop)
					{
						if (currentHurtScreen+direction >= hurtScreens.Count)
							direction = -1;
						else if (currentHurtScreen+direction < 0 )
							direction = 1;
					}
					currentHurtScreen = currentHurtScreen + direction;

				}
			}
			else
			{
				//if some screens alive

				//dirty solution, todo do it better
				Image img1 = hurtScreens[0];
				col = img1.color;
				col.a -= scale;
				img1.color = col;
				
				Image img2 = hurtScreens[1];
				col = img2.color;
				col.a -= scale;
				img2.color = col;
				
				Image img3 = hurtScreens[2];
				col = img3.color;
				col.a -= scale;
				img3.color = col;
				
				if (img1.color.a <= 0.0f && img2.color.a <= 0.0f && img3.color.a <= 0.0f)
				{
					col = img1.color;
					col.a = 0.0f;
					img1.color = col;
					col = img2.color;
					col.a = 0.0f;
					img2.color = col;
					col = img3.color;
					col.a = 0.0f;
					img3.color = col;
					
					currentHurtScreen = 0;
					stop = false;
				}
				return;

			}
		}
	}
}
