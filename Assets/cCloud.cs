using UnityEngine;
using System.Collections;

public class cCloud : MonoBehaviour {

	public float cloudSpeed = 5.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		Vector3 move = Vector3.zero;
		move.x = -cloudSpeed * Time.deltaTime;
		transform.Translate(move);

		if (transform.position.x < -50.0f)
		{
			Destroy(this.gameObject);
		}
	}
}
