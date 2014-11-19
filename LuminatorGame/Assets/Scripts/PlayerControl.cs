using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	public AudioClip drowning;
	public GameObject foodPrefab1;
	public GameObject foodPrefab2;
	
	Dictionary<GameObject, bool>foodObjects;
	Dictionary<GameObject, int>treeFoodCountDone;
	GameObject headLight1, headLight2;
	bool hitTree;
	int blinkTime;
	bool dropFood;
	Vector3 treeHitPos;
	bool canEnter;
	GameObject hitTreeObject;


	// Use this for initialization
	void Start () {
		headLight1 = GameObject.Find("headlight1");
		headLight2 = GameObject.Find("headlight2");
		headLight1.light.color = new Color(1f, 0.96f, 0.88f, 1f);
		headLight2.light.color = new Color(1f, 0.96f, 0.88f, 1f);
		hitTree = false;
		hitTreeObject = null;
		blinkTime = 60;
		canEnter = true;
		dropFood = false;
		foodObjects = new Dictionary<GameObject, bool> ();
		GameObject[] trees = GameObject.FindGameObjectsWithTag ("Tree");
		treeFoodCountDone = new Dictionary<GameObject,int> ();
		foreach (var key in trees) {
			treeFoodCountDone [key] = 0;
		}

		GameEventManager.GameOver += GameOver;

	}

	private void GameOver()
	{
		//this.active = false;
		//GameObject.Find ("Player").transform.position
	}
	
	// Update is called once per frame
	void Update () {
		Color h1 = headLight1.light.color;
		Color h2 = headLight2.light.color;
		//print ("headlight1" + h1.r + "," + h1.g + "," + h1.b + "," + h1.a);
		//print ("headlight2" + h2.r + "," + h2.g + "," + h2.b + "," + h2.a);
		
		GameObject gg;
		//Vector3 v;
		if (dropFood && blinkTime == 60) {
			
			if(treeFoodCountDone[hitTreeObject] < 2){
				if(treeFoodCountDone[hitTreeObject] == 0){
					treeHitPos.x += 5;
					gg = Instantiate (foodPrefab1, treeHitPos,Quaternion.Euler (0,0,0)) as GameObject;
				}
				else{
					treeHitPos.x -= 5;
					gg = Instantiate (foodPrefab2, treeHitPos,Quaternion.Euler (0,0,0)) as GameObject;
				}
				
				gg.rigidbody.AddForce(new Vector3(5, -10, 5));
				gg.collider.enabled = false;
				foodObjects[gg] = false;
				treeFoodCountDone[hitTreeObject] = treeFoodCountDone[hitTreeObject] + 1;
			}
			dropFood = false;
		}
		if(hitTree){
			if(blinkTime > 0){
				var r = Random.Range(80, 100);
				var g = Random.Range(0, 30);
				var b = Random.Range(50, 80);
				headLight1.light.color = new Color(r, g, b, 255);
				headLight2.light.color = new Color(r, g, b, 255);
				blinkTime = blinkTime - 1;
			}
			else{
				hitTree = false;
				blinkTime = 60;
				hitTreeObject = null;
				headLight1.light.color = new Color(1f, 0.96f, 0.88f, 1f);
				headLight2.light.color = new Color(1f, 0.96f, 0.88f, 1f);
				canEnter = true;
			}
		}
		
		var buffer = new List<GameObject> (foodObjects.Keys);
		foreach(var key in buffer){
			if(key.transform.position.y < 39 && foodObjects[key] == false){
				foodObjects[key] = true;
				key.AddComponent("SphereCollider");
			}
		}
	}
	void OnControllerColliderHit(ControllerColliderHit hit){
		if(canEnter && hit.gameObject.tag == "Tree"){
			print ("Collided with " + hit.gameObject.tag);
			canEnter = false;
			hitTree = true;
			hitTreeObject = hit.gameObject;
			dropFood = true;
			treeHitPos =  hit.gameObject.transform.position;
			treeHitPos.y = 45;
		}
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
