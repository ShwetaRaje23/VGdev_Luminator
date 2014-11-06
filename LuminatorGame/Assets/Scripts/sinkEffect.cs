using UnityEngine;
using System.Collections;

public class sinkEffect : MonoBehaviour {

	// Use this for initialization
	public static bool entertriggersink = false;
	float height;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
			
		if (entertriggersink == true) {

			GameObject player = GameObject.Find("Player");

			Debug.Log(" Sinking !! ");

			while (height > 0f)
			{
				height -= player.transform.position.y * Time.deltaTime; 
				player.transform.position = new Vector3(transform.position.x, height, transform.position.z);
			}
		//		transform.position = Vector3.Lerp(transform.position, 	);
		}
	}


}
