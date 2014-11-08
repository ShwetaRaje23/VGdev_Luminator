using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	private Color[] colors = {Color.red,Color.blue,Color.gray,Color.green,Color.magenta,Color.black,Color.yellow,Color.cyan,Color.white};

	void Start()
	{
				transform.renderer.material.color = colors [Random.Range (0, 8)];
		}
	// Update is called once per frame
	void Update () {
	
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
		int randomNbr = Random.Range (0, 100);
		if(randomNbr > 70 && randomNbr < 90) //included this logic to slightly slower the speed with which colors change
			transform.renderer.material.color = colors [Random.Range (0, 8)];		
	}
}
