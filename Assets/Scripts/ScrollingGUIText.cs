
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollingGUIText : MonoBehaviour 
{
	public TextMesh scrollingText;
	public int speed = 1;
	public float scrollTime = 30.0f;
	public float timeToPause = 8.0f;

	void Update()
	{
		// Pause game to wait for warning then start credit scroll
		timeToPause -= Time.deltaTime * 1.0f;

		if (timeToPause < 0) 
		{	
			scrollingText.gameObject.SetActive(true);
			scrollingText.transform.Translate (Vector3.up * Time.deltaTime * speed);	
		}
	}
}