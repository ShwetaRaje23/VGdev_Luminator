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
	public Camera cam1, cam2;
	Vector3 camRotation;
	public AudioClip gameoverclip;
	
	Dictionary<GameObject, bool>foodObjects;
	Dictionary<GameObject, int>treeFoodItemsDone;
	Dictionary<GameObject, string>foodNames;
	GameObject headLight1, headLight2;
	GameObject displayText;
	GameObject scoreText;
	GameObject livesText;
	bool hitTree;
	int blinkTime;
	bool dropFood;
	Vector3 foodFallStartPos;
	bool canEnter;
	GameObject hitTreeObject;
	int foodObjectHitCount;
	private Animator anim;
	private int s_count;
	AudioSource[] audioSource;
	public static int score;
	int lives;
	int highScoreFactor;



	// Use this for initialization
	void Start () {
		headLight1 = GameObject.Find("headlight1");
		headLight2 = GameObject.Find("headlight2");
		displayText = GameObject.Find ("DisplayText");
		headLight1.light.color = new Color(1f, 0.96f, 0.88f, 1f);
		headLight2.light.color = new Color(1f, 0.96f, 0.88f, 1f);
		hitTree = false;
		hitTreeObject = null;
		blinkTime = 30;
		canEnter = true;
		dropFood = false;
		foodObjects = new Dictionary<GameObject, bool> ();
		foodNames = new Dictionary<GameObject, string > ();
		GameObject[] trees = GameObject.FindGameObjectsWithTag ("Tree");
		GameObject[] alreadyExistingFoodObjects = GameObject.FindGameObjectsWithTag ("foodObject");

		scoreText = GameObject.Find ("ScoreText2");
		livesText = GameObject.Find ("LivesText2");
		highScoreFactor = 1;

		score = 0; // start with 0 points
		lives = 0; // start with 0 lives
		scoreText.guiText.text = score.ToString(); 
		livesText.guiText.text = lives.ToString();

		foreach (var i in alreadyExistingFoodObjects) {
			if (i.name == "Food1" || i.name == "Food2" || i.name == "Food13" || i.name == "Food14"
			    || i.name == "Food15" || i.name == "Food16")
				foodNames [i] = "Carrot";
			else if (i.name == "Food3" || i.name == "Food4" || i.name == "Food17" || i.name == "Food18"
			         || i.name == "Food19" || i.name == "Food20")
				foodNames [i] = "Corn";
			else if (i.name == "Food5" || i.name == "Food6" || i.name == "Food12" || i.name == "Food21"
			         || i.name == "Food22" || i.name == "Food23" || i.name == "Food24")
				foodNames [i] = "Pizza";
			else if (i.name == "Food7" || i.name == "Food8" || i.name == "Food25" || i.name == "Food26"
			         || i.name == "Food27" || i.name == "Food28")
				foodNames [i] = "Sushi";
			else
				foodNames [i] = "WaterMelon";
		}
		//alreadyExistingFoodObjects [0].name
		treeFoodItemsDone = new Dictionary<GameObject,int> ();
		foreach (var key in trees) {
			treeFoodItemsDone [key] = 0;
		}
		foodObjectHitCount = 0;

		audioSource = gameObject.GetComponents<AudioSource> ();
		print ("length of audioSource is " + audioSource.Length);

		GameEventManager.GameOver += GameOver;
		anim = GameObject.Find ("ForBumpyanimation").GetComponent<Animator> ();
		s_count = 0;

		cam1.enabled = true;
		cam2.enabled = false;
	}

	private void GameOver()
	{
		//this.active = false;
		//GameObject.Find ("Player").transform.position
	}
	

	public int getNumFoodItemsPicked(){
		return foodObjectHitCount;
	}

	public int getNumLives(){
		return lives;
	}
	public void setNumLives(int numLives){
		lives = numLives;
		livesText.guiText.text = lives.ToString ();
		displayText.guiText.enabled = true;
		displayText.guiText.text = "Hurry, you have " + lives.ToString() + " lives remaining after this!";
		StartCoroutine (displayNotificationFade());
		
	}

	// Update is called once per frame
	void FixedUpdate () {

		GameObject gg;
		if (dropFood && blinkTime == 30) {
			
			if(treeFoodItemsDone[hitTreeObject] < 5){
				foodFallStartPos = gameObject.transform.position;
				foodFallStartPos.y = 45;
				if(treeFoodItemsDone[hitTreeObject] == 0){
					foodFallStartPos.x += 5;
					gg = Instantiate (foodPrefab1, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
					foodNames[gg] = "Corn";
				}
				else if(treeFoodItemsDone[hitTreeObject] == 1){
					foodFallStartPos.x -= 5;
					gg = Instantiate (foodPrefab2, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
					foodNames[gg] = "Carrot";
				}
				else if(treeFoodItemsDone[hitTreeObject] == 2){
					foodFallStartPos.x += 4;
					gg = Instantiate (foodPrefab3, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
					foodNames[gg] = "Pizza";
				}
				else if(treeFoodItemsDone[hitTreeObject] == 3){
					foodFallStartPos.x -= 4;
					gg = Instantiate (foodPrefab4, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
					foodNames[gg] = "WaterMelon";
				}
				else{
					foodFallStartPos.x += 3;
					gg = Instantiate (foodPrefab5, foodFallStartPos,Quaternion.Euler (0,0,0)) as GameObject;
					foodNames[gg] = "Sushi";
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
			if(!audioSource[1].isPlaying){
				audioSource[1].Play();
			}
			if(blinkTime > 0){
				var r = Random.Range(0.3F, 0.4F);
				var g = Random.Range(0.0F, 0.2F);
				var b = Random.Range(0.9F, 1F);
				headLight1.light.color = new Color(r, g, b, 1F);
				headLight2.light.color = new Color(r, g, b, 1F);
				blinkTime = blinkTime - 1;
			}
			else{
				hitTree = false;
				blinkTime = 30;
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


		//script to toggle camera
		if (Input.GetKeyDown(KeyCode.R)) {
			cam1.enabled = !cam1.enabled;
			cam2.enabled = !cam2.enabled;


// trying to add rear view mirror code// trying to add rear view mirror code
//			if(cam2.enabled == true){
//				if (Input.GetKeyDown(KeyCode.R) && camRotation.y <=180)
//					
//					//cam2.transform.rotation = Quaternion.Euler(0,180,0);
//				
//			camRotation += new Vector3(0f,15f,0f);	
//			cam2.transform.rotation = Quaternion.Euler(0f,camRotation.y,0f);
//		}

		}
		
	}

	IEnumerator displayNotificationFade(){
		for (float f = 5f; f >= 0; f -= 0.1f) {
			Color c = displayText.guiText.color;
			c.a = f/5f;
			displayText.guiText.color = c;
			yield return new WaitForSeconds(.01f);
		}
		displayText.guiText.enabled = false;
	}
	void OnControllerColliderHit(ControllerColliderHit hit){
		string s;
		if(canEnter && hit.gameObject.tag == "Tree"){
			print ("Collided with " + hit.gameObject.tag);
			canEnter = false;
			hitTree = true;
			hitTreeObject = hit.gameObject;
			dropFood = true;
		}

		if (hit.gameObject.tag == "foodObject") {
			score += 5;
			if(score > 30 * highScoreFactor){
				lives++;
				livesText.guiText.text = lives.ToString ();
				highScoreFactor++;
				s = "You gained an Extra Life!";
				audioSource[3].Play ();
			}
			else{
				s = "+5 points,\n+5 Health,\nfor grabbing " + foodNames[hit.gameObject]+"!";
				audioSource[2].Play ();
			}
			scoreText.guiText.text = score.ToString();
			displayText.guiText.enabled = true;
			displayText.guiText.text = s;
			StartCoroutine (displayNotificationFade());
			foodObjectHitCount++;

			if(foodObjects.ContainsKey(hit.gameObject)){
				foodObjects.Remove(hit.gameObject);
				foodNames.Remove (hit.gameObject);
			}
			Destroy (hit.gameObject);
		}
	}
	void OnTriggerEnter(Collider other)
		{


		Debug.Log ("Collision with " + other.name);
		if (other.name == "ExitZone") {
			GUIManager.SetGameOver(LifeMeterScript.GetHealth());

			audio.PlayOneShot (gameoverclip);
			GameEventManager.TriggerGameOver();
		}
		
		if (other.name == "sinkTrigger") {

			/*
			//trying out PlayerPrefs restriction
			Vector3 pos = transform.position;
			pos.z = -15;
			pos.x =39;
			transform.position = new Vector3(pos.x, transform.position.y, pos.z );
			//transform.position.z = pos.z;


			transform.position = pos;
			//anim.SetBool("sink",true);
			//audio.PlayOneShot(drowning);
			s_count = 0;*/
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
