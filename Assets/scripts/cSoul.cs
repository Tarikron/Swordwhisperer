using UnityEngine;
using System.Collections;

public class cSoul : MonoBehaviour {

	public float angleSpeed = 10.0f;
	private float alpha = 0.0f;
	private float originY = 0.0f;
	// Use this for initialization
	void Start () 
	{
		originY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		alpha += angleSpeed * Time.deltaTime;
		if (alpha > 360.0f)
			alpha = alpha - 360.0f;

		Vector3 vPos = transform.position;
		vPos.y =  vPos.y + (Mathf.Sin (alpha * Mathf.PI/180)/80.0f );
		transform.position = vPos;
	}
}
