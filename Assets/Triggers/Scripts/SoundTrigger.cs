using UnityEngine;
using System.Collections;

[RequireComponent (typeof(BoxCollider))]
public class SoundTrigger : MonoBehaviour
{

	public GameObject[] SoundSources;

	// Use this for initialization
	void Start ()
	{
	
	}

	void OnTriggerEnter (Collider collision)
	{
		foreach (GameObject source in SoundSources) {
			AudioSource audio = source.GetComponent<AudioSource> ();
			audio.Play ();
		}
	}
}
