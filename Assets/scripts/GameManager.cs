using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject player;
	private GameCamera cam;

	void Start () 
	{
		cam = GetComponent<GameCamera>();

		cam.SetTarget(player.transform);
		cam.SetOffset(new Vector3(0.0f,player.transform.position.y*-1.0f ,0.0f));
	}

	private void SpawnPlayer()
	{
		cam.SetTarget((Instantiate(player, Vector3.zero,Quaternion.identity) as GameObject).transform);
	}
}
