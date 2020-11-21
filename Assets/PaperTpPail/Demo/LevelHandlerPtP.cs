using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandlerPtP : MonoBehaviour
{
	public GameObject titleScreen;
	public GameObject findThePailScreen;
	public GameObject dogFightScreen;

	public AudioClip[] gameplayMusic = new AudioClip[7];

	private void Start()
	{
		MusicManagerPtP.Instance.PlayTrack(gameplayMusic[6], 20f);
		MusicManagerPtP.Instance.ScheduleTrack(gameplayMusic[5]);

		titleScreen.SetActive(true);
		findThePailScreen.SetActive(false);
		dogFightScreen.SetActive(false);
	}

	public void TitleScreen()
	{
		MusicManagerPtP.Instance.PlayTrack(gameplayMusic[5]);

		titleScreen.SetActive(true);
		findThePailScreen.SetActive(false);
		dogFightScreen.SetActive(false);
	}

	public void DogFight()
	{
		MusicManagerPtP.Instance.PlayTrack(gameplayMusic[2]);
		MusicManagerPtP.Instance.ScheduleTrack(gameplayMusic[3]);

		titleScreen.SetActive(false);
		findThePailScreen.SetActive(false);
		dogFightScreen.SetActive(true);
	}

	public int currentIndex = 1;
	public void FindThePail()
	{
		NextTrack();

		titleScreen.SetActive(false);
		findThePailScreen.SetActive(true);
		dogFightScreen.SetActive(false);
	}

	public void NextTrack()
	{

		int newIndex = Random.Range(0, 2);
		while (newIndex == currentIndex)
		{
			newIndex = Random.Range(0, 2);
		}
		currentIndex = newIndex;

		switch (currentIndex)
		{
			case 0:
				MusicManagerPtP.Instance.PlayTrack(gameplayMusic[0]);
				MusicManagerPtP.Instance.ScheduleTrack(gameplayMusic[1]);
				break;
			case 1:
				MusicManagerPtP.Instance.PlayTrack(gameplayMusic[4], 112f);
				break;
		}
	}
}
