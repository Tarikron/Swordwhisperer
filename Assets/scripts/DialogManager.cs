using UnityEngine;
using System.Collections;

[RequireComponent (typeof(DialogLoadXml))]
public class DialogManager : MonoBehaviour {

	string currentXml;
	string currentEvent;



	CanvasGroup dlgCanvas;

	// Use this for initialization
	void Start () {
		dlgCanvas = GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void loadXML()
	{

	}


}
