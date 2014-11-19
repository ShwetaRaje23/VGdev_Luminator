using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	public GUIText gameOverText;
	
	// this seems pretty dodgy, although I guess if you know there is just one ... ugh
	private static GUIManager instance;

	void Start () {
		// perhaps should check here to make sure only one?
		instance = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		gameOverText.enabled = false;
//		instructionsText.fontStyle = FontStyle.Bold;
//		instructionsText.fontSize = 32;
//		instructionsText.color = Color.white;
//		instructionsText.text = "Welcome to Treasure Hunt!!\n Press 'space' to begin the game\n Press 'i' to view controls and game utilities ";
//		instructionsText.enabled = false;
//		jumpsText.enabled = false;
//		scoreText.enabled = false;
//		timerText.enabled = false;
	}

	void Update () {
//		if(Input.GetButtonDown("Jump")){
//			GameEventManager.TriggerGameStart();
//		}

	}
	
	//IEnumerator FadeInstructions() {
//		for (float f = 5f; f >= 0; f -= 0.05f) {
//			Color c = instructionsText.color;
//			c.a = f/5f;
//			instructionsText.color = c;
//			yield return new WaitForSeconds(.01f);
//		}
//		instructionsText.enabled = false;
	//}
	
	private void GameStart () {
		gameOverText.enabled = false;
		//instructionsText.enabled = false;
//		StartCoroutine(FadeInstructions());
//		//jumpsText.enabled = true;
//		scoreText.enabled = true;
//		timerText.enabled = true;
		enabled = false;
	}

	private void GameOver () {
		gameOverText.enabled = true;
		//instructionsText.enabled = true;
//		jumpsText.enabled = false;
//		scoreText.enabled = false;
//		timerText.enabled = false;
		enabled = true;
	}
	
	public static void SetJumps(int jumps){
		//instance.jumpsText.text = "";
	}

	public static void SetScore(int score){
		//instance.scoreText.text = "\nHealth: "+score;
	}

	public static void SetGameOver(int health){
		//Debug.Log ("Its called");
		instance.gameOverText.text = "You have won with a health of "+health;
	}

}