using UnityEngine;
using System.Collections;

public class cHovering : MonoBehaviour 
{

	public float angleSpeed = 10.0f;
	private float alpha = 0.0f;
	private float originY = 0.0f;

	public float direction = 1.0f;


	// Use this for initialization
	void Start () {
		originY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		alpha += angleSpeed * Time.deltaTime;
		if (alpha > 360.0f)
			alpha = alpha - 360.0f;
		
		Vector3 vPos = transform.position;
		vPos.y =  originY + (direction * Mathf.Sin (alpha * Mathf.PI/180));
		transform.position = vPos;
	}
}
