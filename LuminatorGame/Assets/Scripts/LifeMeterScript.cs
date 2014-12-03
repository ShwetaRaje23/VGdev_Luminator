using UnityEngine;
using System.Collections;

public class LifeMeterScript : MonoBehaviour {

	private static LifeMeterScript instance;
	public AudioClip gameoverclip;
	GUIStyle currentStyle = null;

	public int maxLife = 100;
	public float currLife = 100f;
	public static int numLives;
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
		CharacterMotor.SetMove (false);
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
		int numLives;

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
		
		 if ((int)currLife < 1) {
			numLives = otherScript.getNumLives();

			if(numLives == 0){
				//Stop Game !
				GUIManager.SetGameOver(LifeMeterScript.GetHealth());
				audio.PlayOneShot (gameoverclip);
				GameEventManager.TriggerGameOver();
			}
			else{
				otherScript.setNumLives(numLives - 1);
				currLife = 100;
			}
		}
	}

	void OnGUI()
	{
		//GUI.backgroundColor = Color.green;
		//GUI.color = Color.green;
		//GUISkin = currentStyle;
		InitStyles ();

		if (currLife < 10) {
			
			GUI.color = Color.red;

				}
		if(currLife >0)
			GUI.Box (new Rect (50, 50, (Screen.width / 2 / (maxLife/currLife)), 20) , "" + (int)currLife, currentStyle);

		if (currLife < 1) {

	//		gameText.text = "You are dead !! ";

			currLife = 0;

		}

		}

	public static int GetHealth()
	{
		return (int)instance.currLife;
	}

// new code for adding style to the Health bar


	
//	void OnGUI()
//	{  
//		InitStyles();
//		GUI.Box( new Rect( 0, 0, 100, 100 ), "Hello", currentStyle );
//	}
//	
	private void InitStyles()
	{
		if( currentStyle == null )
		{
			currentStyle = new GUIStyle( GUI.skin.box );
			currentStyle.normal.background = MakeTex( 2, 2, new Color( 1f, 1f, 1f, 0.3f ) );
		}
	}
	
	private Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}




}
