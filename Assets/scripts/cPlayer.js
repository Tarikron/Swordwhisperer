#pragma strict

public var fSpeed : float = 2.0f;
public var spine_player : GameObject = null;


function Start () {

}

function Update () {


	if (Input.GetButton("Horizontal"))
	{
 		//this.transform.position.x += fSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
 	}

}