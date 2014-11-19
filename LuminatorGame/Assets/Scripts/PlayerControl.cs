using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	public AudioClip drowning;
	// Use this for initialization
	void Start () {
		GameEventManager.GameOver += GameOver;
<<<<<<< HEAD
	}

	private void GameOver()
	{
		//this.active = false;
		//GameObject.Find ("Player").transform.position
	}

=======
	}

	private void GameOver()
	{
		//this.active = false;
		//GameObject.Find ("Player").transform.position
	}

>>>>>>> origin/master
	
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


//		Debug.Log(" in trigger Collision");
//		transform.Translate(0,(float)(-0.5 * Time.deltaTime),0);
//		if (other.gameObject.tag == "PickUp") {
//			
//			Debug.Log("trigger Collision");
//
//		//	transform.Translate(0,(float)(-1* Time.deltaTime),0);



			// add sink motion 




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
