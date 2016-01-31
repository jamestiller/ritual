using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
	public GameObject destination;

	// Use this for initialization
	void Start () {
	}
	
public void OnTriggerEnter (Collider other){
		if (other.transform.CompareTag ("Player"))
						other.transform.position = destination.transform.position;
		}
}
