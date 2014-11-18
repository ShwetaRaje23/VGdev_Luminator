using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider other)
		{
		Debug.Log(" in trigger Collision");
		transform.Translate(0,(float)(-0.5 * Time.deltaTime),0);
		if (other.gameObject.tag == "PickUp") {
			
			Debug.Log("trigger Collision");

			transform.Translate(0,(float)(-1* Time.deltaTime),0);



			// add sink motion 



		}
		}
	
//	void OnControllerColliderHit(ControllerColliderHit hit) {
//		Debug.Log(" in Collision");
//
//		if (hit.gameObject.tag == "PickUp") {
//			
//			Debug.Log("Collision");
//			//sinkEffect.entertriggersink = true;
//			
//		}
//		
//	}
}
