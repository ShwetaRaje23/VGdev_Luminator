using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	public AudioClip drowning;
	bool sinkCollided = false;
	// Use this for initialization
	void Start () {
		GameEventManager.GameOver += GameOver;

	}

	private void GameOver()
	{
		//this.active = false;
		//GameObject.Find ("Player").transform.position
	}

		// Update is called once per frame
	void Update () {
		
	}

void OnTriggerEnter(Collider other)
	{
		Debug.Log ("Collision with " + other.name);
		if (other.name == "ExitZone") {
			GUIManager.SetGameOver(LifeMeterScript.GetHealth());
			GameEventManager.TriggerGameOver();
		}
		
		if (other.name == "sinkTrigger" && sinkCollided == false) {
			GameObject playermotion = GameObject.Find("ForBumpyanimation");


		// sink animation - letting it be here	
		//	for(int loopVariable = 0 ; loopVariable < 3; loopVariable++)
		//	{
			playermotion.animation.Play("testsinkCurve");
				audio.PlayOneShot(drowning);
				sinkCollided = true; // it would not go into any other sink collisions - change later
			playermotion.animation.CrossFade("Bumpy", 1F); 

			if (Input.GetKeyDown("v"))
				{
				playermotion.animation.Stop("testsinkCurve");

					//break;
				}
		//	}

			//playermotion.transform 

		} // end of detection of collision with sink


}



//		Debug.Log(" in trigger Collision");
//		transform.Translate(0,(float)(-0.5 * Time.deltaTime),0);
//		if (other.gameObject.tag == "PickUp") {
//			
//			Debug.Log("trigger Collision");
//
//		//	transform.Translate(0,(float)(-1* Time.deltaTime),0);



			// add sink motion 




		
	
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