using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnTheBeat : MonoBehaviour {
	public SFXBase sound;

	void Start() {
		MusicManager.Instance.OnBeat += PlaySound;
	}

	void OnDistroy() {
		MusicManager.Instance.OnBeat -= PlaySound;
	}

	public void PlaySound() {
		sound.Play();
	}
}
