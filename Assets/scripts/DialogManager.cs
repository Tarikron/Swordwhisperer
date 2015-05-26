using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(DialogLoadXml))]
public class DialogManager : MonoBehaviour {

	string currentXml;
	string currentEvent;

	Dictionary<string,Dialog> dialogs;
	CanvasGroup dlgCanvas;

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
			//we have event in dialogs
			Dialog dlg = dialogs[currentEvent];
			string text = dlg.persons[0].text;
		}
	}


}
