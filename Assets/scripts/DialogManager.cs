using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


[RequireComponent (typeof(DialogLoadXml))]
public class DialogManager : MonoBehaviour {

	string currentXml;
	string currentEvent;

	Dictionary<string,Dialog> dialogs;
	CanvasGroup dlgCanvas;
	public Text dlgMessage;
	private int currentIndex = 0;
	public float timerNextMessage = 2.0f;
	private float timerCurrentMessage = 0.0f;

	private bool nextMessage = false;
	private bool stopEvent = false;

	// Use this for initialization
	void Start () {
		dlgCanvas = GetComponent<CanvasGroup>();
		currentXml = "dialog_system.xml";
		currentEvent = "";

		DialogLoadXml loadXml = new DialogLoadXml(currentXml);
		dialogs = loadXml.parseXml();

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (dialogs != null && dialogs.ContainsKey(currentEvent))
		{
			Dialog dlg = dialogs[currentEvent];

			if (currentIndex+1 > dlg.persons.Count)
			{
				stopEvent = false;
				nextMessage = false;
				return;
			}
			//we have event in dialogs
			if (!nextMessage && !stopEvent && dlgCanvas.alpha < 1.0f)
			{
				float fade_in = dlg.persons[currentIndex].fade_in;
				dlgCanvas.alpha += (1.0f * Time.deltaTime)/fade_in;
				if (dlgCanvas.alpha > 1.0f)
					dlgCanvas.alpha = 1.0f;
				string text = dlg.persons[currentIndex].text;
				dlgMessage.text = text;
			}
			else if (stopEvent || nextMessage)
			{
				float fade_out = dlg.persons[currentIndex].fade_out;
				dlgCanvas.alpha -= (1.0f * Time.deltaTime)/fade_out;
				if (dlgCanvas.alpha < 0.0f)
					dlgCanvas.alpha = 0.0f;

				if (dlgCanvas.alpha <= 0.01f)
				{
					if (nextMessage)
						currentIndex++;
					else
						currentEvent = "";
					nextMessage = false;
					if (stopEvent)
						currentIndex = dlg.persons.Count+1;
					stopEvent = false;
				}
				
			}
			else if (dlgCanvas.alpha >= 1.0f)
			{
				timerCurrentMessage += Time.deltaTime;
				if (timerNextMessage <= timerCurrentMessage)
				{
					nextMessage = true;
					timerCurrentMessage = 0.0f;
				}
			}

		}
	}

	void msg_eventTrigger(string nextEvent)
	{
		currentEvent = nextEvent;
	}
	void msg_eventTriggerEnd()
	{
		if (currentEvent != "")
			stopEvent = true;
	}
}
