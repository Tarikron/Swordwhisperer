using UnityEngine;
using System.Collections;

public class kamehameha : MonoBehaviour {

	public enum eKaMe {NONE = 0, SHOTON = 1, SHOTOFF = 2};
	public eKaMe state = eKaMe.NONE;

	private float origin_startSize = 0.0f;
	public bool animation = false;
	private float fadeOutTime = 0.2f;
	private ParticleSystem ps;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem>();
		origin_startSize = ps.startSize;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (state == eKaMe.NONE)
			return;

		if (state == eKaMe.SHOTON)
		{
			if (!animation)
			{
				ps.enableEmission = true;
				ps.startSize = origin_startSize;
				return;
			}
			ps.enableEmission = true;
			ps.startSize += Time.deltaTime/fadeOutTime;
			if (ps.startSize >= origin_startSize)
				ps.startSize = origin_startSize;
		}
		else if (state == eKaMe.SHOTOFF)
		{
			if (!animation)
			{
				ps.enableEmission = false;
				ps.startSize = 0.0f;
				state = eKaMe.NONE;
				return;
			}
			ps.startSize -= Time.deltaTime/fadeOutTime;
			if (ps.startSize <= 0.0f)
			{
				ps.startSize = 0.0f;
				ps.enableEmission = false;
				state = eKaMe.NONE;
			}
		}
	}

	public float getStartSize()
	{
		return origin_startSize;
	}

	void OnParticleCollision (GameObject gameObject)
	{
		Debug.Log ("beam collide1");
		cEnemyBoss1 ceb = gameObject.GetComponentInParent<cEnemyBoss1>();
		if (ceb == null)
		{
			cFlyingEnemy cfe = gameObject.GetComponentInParent<cFlyingEnemy>();
			if (cfe != null)
				cfe.gameObject.SendMessage("msg_damage",1.0f,SendMessageOptions.DontRequireReceiver);
		}
		else
			ceb.gameObject.SendMessage("msg_damage",1.0f,SendMessageOptions.DontRequireReceiver);
	}
}
