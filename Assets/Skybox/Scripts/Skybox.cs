using UnityEngine;
using System.Collections;

public class Skybox : MonoBehaviour
{

	private Transform playerTransform;
	public Material[] Skyboxes;
	public float maxHeight;

	// Use this for initialization
	void Start ()
	{
		var player = GameObject.FindGameObjectWithTag ("Player");
		playerTransform = player.GetComponent<Transform> ();
		RenderSettings.skybox = Skyboxes [0];
	}
	
	// Update is called once per frame
	void Update ()
	{
		RenderSettings.skybox = Skyboxes [Mathf.RoundToInt (playerTransform.position.y / (maxHeight / 15))];
	}
}
