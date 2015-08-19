using UnityEngine;
using System.Collections;

public class checkPlayerDistance : MonoBehaviour {

	public float triggerDistance = 2.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.FindGameObjectWithTag("player");

		float nonAbsDis = player.transform.position.x - transform.position.x;
		float distance = Mathf.Abs(nonAbsDis);
		if (distance <= triggerDistance)
		{
			Camera mainCam = Camera.main;
			mainCam.gameObject.GetComponent<GameManager>().changeMusic(this.gameObject.name);
		}
	}
}
