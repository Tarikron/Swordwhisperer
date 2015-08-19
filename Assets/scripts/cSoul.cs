using UnityEngine;
using System.Collections;

public class cSoul : MonoBehaviour {

	public float angleSpeed = 10.0f;
	public float amplitude = 80.0f;
	protected float alpha = 0.0f;
	protected Vector3 origin = Vector3.zero;

	protected bool stopMovement = false;


	// Use this for initialization
	public virtual void Start () 
	{
		origin = transform.position;
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
		if (!stopMovement)
		{
			alpha += angleSpeed * Time.deltaTime;
			if (alpha > 360.0f)
				alpha = alpha - 360.0f;
			if (alpha < -360.0f)
				alpha = alpha + 360.0f;

			Vector3 vPos = transform.position;
			vPos.y =  vPos.y + (Mathf.Sin (alpha * Mathf.PI/180)/amplitude );
			transform.position = vPos;
		}

	}
}
