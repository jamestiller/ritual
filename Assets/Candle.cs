using UnityEngine;
using System.Collections;

public class Candle : MonoBehaviour {

	public bool hasBeenTouched = false;

	public GameObject[] flames;
	private float flameTimer = 1000;
	public float lastTimeTriggered = 0;

	public Collider terrain;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < flames.Length; i++)
		{
			flames[i].SetActive(false);
		}

		RaycastHit hit;
		Ray ray = new Ray(transform.position, Vector3.down);
		terrain.Raycast(ray, out hit, 1000);//gameObject.transform.position, -transform.up, out hit);

		transform.position = hit.point;//- Vector3.up*.01;
	}
	
	// Update is called once per frame
	void Update () {
		if (flames.Length > 0
			&& flames[0].activeSelf == true
			&& Time.timeSinceLevelLoad - lastTimeTriggered > flameTimer)
		{
			for(int i = 0; i < flames.Length; i++)
			{
				ResetCandle(i);
			}
		}
	}

	public void ResetAllFlames()
	{
		for(int i = 0; i < flames.Length; i++)
		{
			ResetCandle(i);
		}
	}

	public void ResetCandle(int index)
	{
		flames[index].SetActive(false);
		hasBeenTouched = false;
	}

	void OnTriggerEnter()
	{
		hasBeenTouched = true;

		Debug.Log("enter");
		// activate the flames!
		for(int i = 0; i < flames.Length; i++)
		{
			flames[i].SetActive(true);
		}

		lastTimeTriggered = Time.timeSinceLevelLoad;
	}
}
