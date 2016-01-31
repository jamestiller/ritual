using UnityEngine;
using System.Collections;

public class CandleManager : MonoBehaviour {

	Candle[] candles;
	public GameObject master;
	public GameObject particles;
	public VRMover character;

	int characterLevel = 0;

	void RefreshLevel()
	{

	}

	// Use this for initialization
	void Start () {
		candles = Resources.FindObjectsOfTypeAll<Candle>();
		master.gameObject.SetActive(false);
	}

	void WinGame()
	{
		Debug.Log("WIN GAME! Figure out something cool to do.");
		characterLevel = 0;
		KillTheCandles();

		LightMaster();
		// FIGURE OUT WHAT TO DO!
		Invoke("KillMaster", 600);
		Invoke("RestoreSight", 600);
	}

	void RestoreSight()
	{
		character.SetSuddenBlindness(0);
	}

	// Update is called once per frame
	void Update () {
		
		bool litAll = true;
		// Win condition is are all flames lit.
		for(int i = 0; i < candles.Length; i++)
		{
			if (candles[i].hasBeenTouched == false)
			{
				litAll = false;
				break;
			}
		}

		// master flame.
		if (litAll)
		{
			characterLevel++;

			// THIS IS THE END OF THE GAME!!!!!
			if (characterLevel == 10)
			{
				Invoke("WinGame", 10);
				return;
			}
			
			LevelRefresh();
		}
	}

	void LevelRefresh()
	{
		// You can do whatever you want when all candles are lit.
		particles.SetActive(false);

		character.SetSuddenBlindness(characterLevel);

		KillTheCandles();
		LightMaster();
		Invoke("KillMaster", 60);
	}

	void KillTheCandles()
	{
		for(int i = 0; i < candles.Length; i++)
		{
			candles[i].ResetAllFlames();
		}
	}

	void KillMaster()
	{
		master.gameObject.SetActive(false);
	}

	void LightMaster()
	{
		master.gameObject.SetActive(true);
	}


}
