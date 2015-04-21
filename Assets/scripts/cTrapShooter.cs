using UnityEngine;
using System.Collections;

public class cTrapShooter : MonoBehaviour {

	public GameObject shot = null;
	public float interval = 1.0f;

	private float intervalTimer = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		intervalTimer += Time.deltaTime;

		if (intervalTimer >= interval)
		{
			intervalTimer = 0.0f;

			//clone shot
			GameObject clonedShot = GameObject.Instantiate(shot);
			clonedShot.transform.position = shot.transform.position;
			clonedShot.transform.rotation = this.transform.rotation;
			clonedShot.transform.localScale = shot.transform.lossyScale;
			//clonedShot.transform.lossyScale = shot.transform.lossyScale;
			clonedShot.GetComponent<SpriteRenderer>().enabled = true;
			clonedShot.GetComponent<BoxCollider2D>().enabled = true;


		}
	}
}
