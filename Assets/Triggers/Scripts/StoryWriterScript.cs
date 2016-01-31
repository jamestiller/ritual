using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StoryWriterScript : MonoBehaviour
{
	[TextArea (3, 10)]
	public string Story;

	public float DisplayTime;

	private Text text;
	// Use this for initialization
	void Start ()
	{
		text = FindObjectOfType<Canvas> ().transform.FindChild ("Text").gameObject.GetComponent<Text> ();
	}

	public IEnumerator Write ()
	{
		Debug.Log ("Write!");
		text.text = Story;
		yield return new WaitForSeconds (DisplayTime);
		text.text = "";
	}
}
