using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.gameObject.tag == "sinkTrigger") {
			
			Debug.Log("Collision");
			sinkEffect.entertriggersink = true;
			
		}
		
	}
}
