using UnityEngine;
using System.Collections;

public class CollisionCheck : MonoBehaviour {

	// Use this for initialization

	public AudioClip drowning;
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject);
		Debug.Log ("Collision with " + other.name);
		if (other.name == "ExitZone") {
			GUIManager.SetGameOver(LifeMeterScript.GetHealth());
			GameEventManager.TriggerGameOver();
		}

		if (other.name == "sinkTrigger") {
			GameObject playermotion = GameObject.Find("ForBumpyanimation");
		
			for(int loopVariable = 0 ; loopVariable < 3; loopVariable++)
			{
				playermotion.animation.Play("SinkMotion");

				audio.PlayOneShot(drowning);

				if (Input.GetKeyDown("s"))
				{
					playermotion.animation.Stop("SinkMotion");
					break;
				}
			}
			
			}

	}
}
