using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {
	public AudioClip drowning;
	public GameObject foodPrefab1;
	public GameObject foodPrefab2;
	public GameObject foodPrefab3;
	public GameObject foodPrefab4;
	public GameObject foodPrefab5;
	
	Dictionary<GameObject, bool>foodObjects;
	Dictionary<GameObject, int>treeFoodItemsDone;
	GameObject headLight1, headLight2;
	bool hitTree;
	int blinkTime;
	bool dropFood;
	Vector3 foodFallStartPos;
	bool canEnter;
	GameObject hitTreeObject;
	int foodObjectHitCount;
	private Animator anim;
	private int s_count;

	// Use this for initialization
	void Start () {
		headLight1 = GameObject.Find("headlight1");
		headLight2 = GameObject.Find("headlight2");
		headLight1.light.color = new Color(1f, 0.96f, 0.88f, 1f);
		headLight2.light.color = new Color(1f, 0.96f, 0.88f, 1f);
		hitTree = false;
		hitTreeObject = null;
		blinkTime = 20;
		canEnter = true;
		dropFood = false;
		foodObjects = new Dictionary<GameObject, bool> ();
		GameObject[] trees = GameObject.FindGameObjectsWithTag ("Tree");
		treeFoodItemsDone = new Dictionary<GameObject,int> ();
		foreach (var key in trees) {
			treeFoodItemsDone [key] = 0;
		}
		foodObjectHitCount = 0;
		GameEventManager.GameOver += GameOver;
		anim = GameObject.Find ("ForBumpyanimation").GetComponent<Animator> ();
		s_count = 0;
	}

	private void GameOver()
	{
		//this.active = false;
		//GameObject.Find ("Player").transform.position
	}
	

	public int getNumFoodItemsPicked(){
		return foodObjectHitCount;
	}

	// Update is called once per frame
	void FixedUpdate () {

		GameObject gg;
		if (dropFood && blinkTime == 20) {
			
			if(treeFoodItemsDone[hitTreeObject] < 5){
				foodFallStartPos = gameObject.transform.position;
				foodFallStartPos.y = 45;
				if(treeFoodItemsDone[hitTreeObject] == 0){
					foodFallStartPos.x += 5;
					gg = Instantiate (foodPrefab1, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
				}
				else if(treeFoodItemsDone[hitTreeObject] == 1){
					foodFallStartPos.x -= 5;
					gg = Instantiate (foodPrefab2, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
				}
				else if(treeFoodItemsDone[hitTreeObject] == 2){
					foodFallStartPos.x += 4;
					gg = Instantiate (foodPrefab3, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
				}
				else if(treeFoodItemsDone[hitTreeObject] == 3){
					foodFallStartPos.x -= 4;
					gg = Instantiate (foodPrefab4, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
				}
				else{
					foodFallStartPos.x += 3;
					gg = Instantiate (foodPrefab5, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
				}

				gg.gameObject.tag = "foodObject";
				gg.rigidbody.AddForce(new Vector3(5, -10, 5));
				gg.collider.enabled = false;
				foodObjects[gg] = false;
				treeFoodItemsDone[hitTreeObject] = treeFoodItemsDone[hitTreeObject] + 1;
			}
			dropFood = false;
		}
		if(hitTree){
			if(blinkTime > 0){
				var r = Random.Range(0.0F, 1.0F);
				var g = Random.Range(0.0F, 0.3F);
				var b = Random.Range(0.5F, 0.8F);
				headLight1.light.color = new Color(r, g, b, 1F);
				headLight2.light.color = new Color(r, g, b, 1F);
				blinkTime = blinkTime - 1;
			}
			else{
				hitTree = false;
				blinkTime = 20;
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
		print ("Food Object hit count " + foodObjectHitCount);

		if (Input.GetKey (KeyCode.Space))
						s_count++;
		if (s_count > 10)
						anim.SetBool ("sink", false);

	}
	void OnControllerColliderHit(ControllerColliderHit hit){
		if(canEnter && hit.gameObject.tag == "Tree"){
			print ("Collided with " + hit.gameObject.tag);
			canEnter = false;
			hitTree = true;
			hitTreeObject = hit.gameObject;
			dropFood = true;
		}

		if (hit.gameObject.tag == "foodObject") {
			foodObjectHitCount++;
			if(foodObjects.ContainsKey(hit.gameObject)){
				foodObjects.Remove(hit.gameObject);
			}
			Destroy (hit.gameObject);
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


			//trying out PlayerPrefs restriction
			Vector3 pos = transform.position;
			pos.z = -15;
			pos.x =39;
			transform.position = new Vector3(pos.x, transform.position.y, pos.z );
			//transform.position.z = pos.z;


			transform.position = pos;
			//anim.SetBool("sink",true);
			//audio.PlayOneShot(drowning);
			s_count = 0;
//			GameObject playermotion = GameObject.Find("ForBumpyanimation");
//
//
//			
//			for(int loopVariable = 0 ; loopVariable < 3; loopVariable++)
//			{
//				playermotion.animation.Play("SinkMotion");
//				
//				audio.PlayOneShot(drowning);
//				
//				if (Input.GetKeyDown("s"))
//				{
//					playermotion.animation.Stop("SinkMotion");
//					break;
//				}
//			}
			
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
