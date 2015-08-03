using UnityEngine;
using System.Collections;

public class cameraCheck : MonoBehaviour 
{
	enum eCutsceneSteps {SCENE_NONE = 0 , SCENE_START = 1, SCENE_WHILE = 2, SCENE_END = 3, SCENE_DONE = 4};
	private eCutsceneSteps iState = eCutsceneSteps.SCENE_NONE;
	private GameObject player;
	
	public GameObject waypointStart;
	public GameObject waypointEnd;
	public float stayAtEndPoint = 2.0f;
	private float endTimer = 0.0f;
	public bool fadeInAtEnd = false;

	private Camera mainCam;
	private GameCamera gameCam;
	private bool camMovement;
	
	public Vector2 trackSpeed = Vector2.zero;
	private Vector2 originTrackSpeed = Vector2.zero;
	private Transform currentTarget;

	private bool IsCutsceneDone = false;
	private blackScreenHandler blackSceenHdl;

	private bool camAtGrave = false;

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("player");
		camMovement = false;

		mainCam = Camera.main;
		gameCam = mainCam.GetComponent<GameCamera>();

		GameObject blackScreen = GameObject.FindGameObjectWithTag("blackScreen");
		blackSceenHdl = blackScreen.GetComponent<blackScreenHandler>();

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (IsCutsceneDone)
			return;
		switch (iState)
		{
			case eCutsceneSteps.SCENE_NONE:
			{
				if (waypointStart.transform.position.x < player.transform.position.x)
				{
					//start camerea movement
					iState = eCutsceneSteps.SCENE_START;
					currentTarget = waypointEnd.transform;
					player.SendMessage("msg_externCutsceneStart",null,SendMessageOptions.RequireReceiver);
				}
				camAtGrave = false;
			}
			break;
			case eCutsceneSteps.SCENE_START:
			{
				gameCam.SetTarget(currentTarget);
				originTrackSpeed = gameCam.trackSpeed;
				gameCam.SetTrackSpeed(trackSpeed.x,trackSpeed.y);
				iState = eCutsceneSteps.SCENE_WHILE;
				camAtGrave = false;
			}
			break;
			case eCutsceneSteps.SCENE_WHILE:
			{
				if (mainCam.transform.position.x >= waypointEnd.transform.position.x)
				{
					currentTarget = player.transform;
					trackSpeed.x += 3;
					trackSpeed.y += 3;
					
					iState = eCutsceneSteps.SCENE_END;
				}				
			}
			break;
			case eCutsceneSteps.SCENE_END:
			{
				if (!camAtGrave)	
					player.GetComponent<cPlayer_c>().dialog.SendMessage("msg_eventTrigger","beforeCamDrive",SendMessageOptions.RequireReceiver);

				camAtGrave = true;
				if (endTimer >= stayAtEndPoint)
				{
					iState = eCutsceneSteps.SCENE_DONE;
					currentTarget = player.transform;
					endTimer = 0.0f;
					blackSceenHdl.init();
				}
				endTimer += Time.deltaTime;
			}
			break;
			case eCutsceneSteps.SCENE_DONE:
			{
				if (fadeInAtEnd)
				{
					blackSceenHdl.state = blackScreenHandler.eCutsceneSteps.FADE_IN;	
					
					if (blackSceenHdl.IsFadeDone())
					{
						gameCam.SetTrackSpeed(originTrackSpeed.x,originTrackSpeed.y);
						gameCam.SetTarget(player.transform);
						mainCam.transform.position = new Vector3(player.transform.position.x,mainCam.transform.position.y,mainCam.transform.position.z);
						
						blackSceenHdl.state = blackScreenHandler.eCutsceneSteps.FADE_OUT;
						blackSceenHdl.init();
						
						IsCutsceneDone = true;
						player.SendMessage("msg_externCutsceneEnd",null,SendMessageOptions.RequireReceiver);
					}
				}
				else
				{
					gameCam.SetTarget(currentTarget);
					gameCam.SetTrackSpeed(trackSpeed.x,trackSpeed.y);

					if (waypointEnd.transform.position.x >= mainCam.transform.position.x)
					{
						gameCam.SetTrackSpeed(originTrackSpeed.x,originTrackSpeed.y);
						IsCutsceneDone = true;
						player.SendMessage("msg_externCutsceneEnd",null,SendMessageOptions.RequireReceiver);
					}	
				}

			}
			break;
		}
		
	}
}
