using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanjoTest : MonoBehaviour {
    public SFXBase[] strings;
    public bool playing = false;
    public int index;

	void OnMouseDown() {
		playing = !playing;
		if (playing) MusicManager.Instance.CueHalfBeat += PlayStrings;
		else MusicManager.Instance.CueHalfBeat -= PlayStrings;
	}

	public void PlayStrings(float delay) {
		strings[index].Play(delay);
		index = (index + 1) % strings.Length;
	}
}
