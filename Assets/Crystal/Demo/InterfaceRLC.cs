using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceRLC : MonoBehaviour
{
	public CrystalMusicTrack song1;
	public CrystalMusicTrack song2;
	public CrystalMusicTrack song3;
	public CrystalMusicTrack song4;

	public int layers;

	public void PlayNone()
	{
		CrystalMusicEventManager.Instance.Transition(null, true);
	}

	public void Play1()
	{
		CrystalMusicEventManager.Instance.Transition(song1, true);
	}

	public void Play2()
	{
		CrystalMusicEventManager.Instance.Transition(song2, true);
	}

	public void Play3()
	{
		CrystalMusicEventManager.Instance.Transition(song3, true);
	}

	public void Play4()
	{
		CrystalMusicEventManager.Instance.Transition(song4, true);
	}

	public void Increase()
	{
		layers = CrystalMusicEventManager.Instance.IncreaseLayers();
		CrystalMusicEventManager.Instance.Variation();
	}

	public void Decrease()
	{
		layers = CrystalMusicEventManager.Instance.DecreaseLayers();
		CrystalMusicEventManager.Instance.Variation();
	}
}
