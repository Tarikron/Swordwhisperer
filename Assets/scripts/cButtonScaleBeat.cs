using UnityEngine;
using System.Collections;

public class cButtonScaleBeat : MonoBehaviour {

	private float alpha = 0.0f;

	private Vector3 startScale = Vector3.zero;
	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () 
	{
		alpha += 120.0f * Time.deltaTime;

		Vector3 lossyScale = transform.localScale;

		lossyScale.x = startScale.x + Mathf.Sin (alpha * Mathf.PI/180)/10.0f;
		lossyScale.y = startScale.y + Mathf.Sin (alpha * Mathf.PI/180)/10.0f;

		transform.localScale = lossyScale;

		if (alpha > 360.0f)
			alpha = alpha - 360.0f;
	}
}
