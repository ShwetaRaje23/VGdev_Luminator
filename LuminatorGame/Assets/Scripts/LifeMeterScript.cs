using UnityEngine;
using System.Collections;

public class LifeMeterScript : MonoBehaviour {

	private static LifeMeterScript instance;

	public int maxLife = 100;
	public float currLife = 100f;

	private int numFoodItemsPicked;
	private bool gameOver = false;
	//public AudioClip timer;
	//public GUIText gameText;
//	GUIStyle style = new GUIStyle();
//	Texture2D texture = new Texture2D(128,128);
	// Use this for initialization
	void Start () {
		instance = this;
		GameEventManager.GameOver += GameOver;
		numFoodItemsPicked = 0;
	}

	void GameOver()
	{
		gameOver = true;
	}

	
	// Update is called once per frame
	void Update () {
	if (!gameOver)
						UpdateHealth ();

	}

	void UpdateHealth()
	{
		GameObject g = GameObject.Find ("Player");
		PlayerControl otherScript = g.GetComponent<PlayerControl>();
		int currentNumFoodItemsPicked;

		currentNumFoodItemsPicked = otherScript.getNumFoodItemsPicked();

		if (currentNumFoodItemsPicked > numFoodItemsPicked) {
			currLife = Mathf.Min (currLife + (currentNumFoodItemsPicked - numFoodItemsPicked) * 5, maxLife);
			numFoodItemsPicked = currentNumFoodItemsPicked;
		}

		if (ToggleLight.lightOn == true) {
			currLife -= Time.deltaTime;
		}
		
		else if (ToggleLight.lightOn == false)
			currLife = currLife - (int)Time.deltaTime;
		//Debug.Log ("Life " + currLife);
		
		if ((int)currLife < 10) {
			//audio.PlayOneShot(timer);
			
		} else if ((int)currLife < 1) {
			
			//Stop Game !
			//ToggleLight.light.enabled = false;
			
			//gameOverText.text = "You are dead !!"; 
			
			//			gameText.text = "You are dead !! ";
			//			StartCoroutine(GUIManager.FadeInstructions(gameText));
			//			currLife = 0;
			//			GameEventManager.TriggerGameOver();	
			// Kill Charcter	
			
		}
		}

	void OnGUI()
	{
		GUI.backgroundColor = Color.green;
		GUI.color = Color.green;

		if (currLife < 10) {
			
						GUI.color = Color.red;

				}
		if(currLife >0)
		GUI.Box (new Rect (150, 150, (Screen.width / 2 / (maxLife/currLife)), 20) , " " + (int)currLife);

		if (currLife < 1) {

	//		gameText.text = "You are dead !! ";

			currLife = 0;

		}

		}

	public static int GetHealth()
	{
		return (int)instance.currLife;
	}
}
