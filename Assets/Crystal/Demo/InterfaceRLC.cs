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
		CrystalMusicEventManager.Instance.Transition(null);
	}

	public void Play1()
	{
		CrystalMusicEventManager.Instance.Transition(song1);
	}

	public void Play2()
	{
		CrystalMusicEventManager.Instance.Transition(song2);
	}

	public void Play3()
	{
		CrystalMusicEventManager.Instance.Transition(song3);
	}

	public void Play4()
	{
		CrystalMusicEventManager.Instance.Transition(song4);
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
