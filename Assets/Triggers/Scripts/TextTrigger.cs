using UnityEngine;
using System.Collections;

public class TextTrigger : MonoBehaviour
{

	public GameObject StoryWriter;

	private bool triggered;

	void Start ()
	{
		triggered = false;
	}

	public void OnTriggerEnter (Collider collider)
	{
		Debug.Log ("Trigger!");
		StoryWriterScript writer = (StoryWriterScript)StoryWriter.GetComponent<StoryWriterScript> ();
		Debug.Log (writer);
		if (!triggered) {
			StartCoroutine (writer.Write ());
		}
		triggered = true;
	}
}
