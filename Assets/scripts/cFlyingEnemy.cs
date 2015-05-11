using UnityEngine;
using System.Collections;

public class cFlyingEnemy : MonoBehaviour {

	public float speed = 2.0f;
	public float xLengthTurning = 20.0f;
	public enum eFlyingType {SINUS_RANDOM_STRAIGHT=0,SINUS_LOOP=1,SINUS_CIRCLE=2};
	public eFlyingType flyingType = eFlyingType.SINUS_RANDOM_STRAIGHT;

	public float circleSize = 2.0f;



	private int xDirection;

	//for random straight
	private float xTurningTemp;
	private float ampH;
	private float yCurrentDir;

	//for looping
	private float yOrigin;
	private int originCounter = 0;
	private float yMarkCheck = 0.0f;

	//for circle
	private float angleTemp = 0.0f;

	// Use this for initialization
	void Start () 
	{
		xTurningTemp = 0.0f;
		xDirection = 1;
		ampH = 2.0f;
		yCurrentDir = 1;

		yOrigin = transform.position.y;
	}
	
	void movementAirLoop()
	{
		float x = transform.position.x;
		float y = transform.position.y;
		Vector3 movement = new Vector3(0.0f,0.0f,0.0f);

		//checks if we got some distance between last check
		if (yMarkCheck != 0.0f && (  cFunction.xor( ((yMarkCheck + 0.5f) > y) , ((yMarkCheck - 0.5f) < y)  )  ))
		{
			yMarkCheck = 0.0f;
		}

		if (yMarkCheck == 0.0f && y <= yOrigin+0.15f && y >= yOrigin-0.15f)
		{
			//turning point
			yMarkCheck = y;
			originCounter++;
			if (originCounter >= 3)
			{
				originCounter = 0;
				xDirection *= -1;
			}
		}

		float yInc = Mathf.Sin (x)/0.2f * Time.deltaTime;
		y += yInc;
		x += (speed * xDirection) * Time.deltaTime;

		movement.x = x;
		movement.y = y;
		
		transform.position = movement;
	}
	void movementAirCircle()
	{
		float x = transform.position.x;
		float y = transform.position.y;
		Vector3 movement = new Vector3(0.0f,0.0f,0.0f);

		angleTemp += speed;
		if (angleTemp >= 360.0f )
			angleTemp = 0.0f;

		y += circleSize * Mathf.Sin (angleTemp) * Time.deltaTime;
		x += circleSize * Mathf.Cos (angleTemp) * Time.deltaTime;

		movement.x = x;
		movement.y = y;
		
		transform.position = movement;

	}
	void movementAirRandomStraight()
	{
		float x = transform.position.x;
		float y = transform.position.y;
		Vector3 movement = new Vector3(0.0f,0.0f,0.0f);
		
		xTurningTemp += speed * Time.deltaTime;
		if (xTurningTemp >= xLengthTurning)
		{
			//turning point
			xDirection *= -1;
			xTurningTemp = 0.0f;
		}
		float yInc = Mathf.Sin (2*x)/ampH * Time.deltaTime;
		float sign = Mathf.Sign (yInc);
		
		if (sign != yCurrentDir)
		{
			yCurrentDir = sign;
			ampH = Random.Range (0.1f,2.0f);
		}
		y += yInc;
		x += (speed * xDirection) * Time.deltaTime;
		
		movement.x = x;
		movement.y = y;
		
		transform.position = movement;
	}

	// Update is called once per frame
	void Update () 
	{
		switch (flyingType)
		{
			case eFlyingType.SINUS_RANDOM_STRAIGHT:
				movementAirRandomStraight();
				break;
			case eFlyingType.SINUS_LOOP:
				movementAirLoop();
				break;
			case eFlyingType.SINUS_CIRCLE:
				movementAirCircle();
				break;
		}
	}
}
