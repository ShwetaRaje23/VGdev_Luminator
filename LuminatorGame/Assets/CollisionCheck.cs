using UnityEngine;
using System.Collections;

public class CollisionCheck : MonoBehaviour {

	// Use this for initialization


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
	

	}
}
