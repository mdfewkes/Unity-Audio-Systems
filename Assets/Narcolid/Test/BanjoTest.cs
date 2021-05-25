using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanjoTest : MonoBehaviour {
    public SFXBase[] strings;
    public bool playing = false;
    public int index;

	void OnMouseDown() {
		playing = !playing;
		if (playing) MusicManager.Instance.OnHalfBeat += PlayStrings;
		else MusicManager.Instance.OnHalfBeat -= PlayStrings;
	}

	public void PlayStrings() {
		strings[index].Play();
		index = (index + 1) % strings.Length;
	}
}
