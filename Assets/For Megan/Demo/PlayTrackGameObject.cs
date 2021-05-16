using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTrackGameObject : MonoBehaviour {
	public MusicTrack track;
	public bool playOnAwake = false;

	void Start() {
		if (playOnAwake) MusicManager.Instance.PlayTrack(track);
	}

	void OnMouseDown() {
		MusicManager.Instance.PlayTrack(track);
	}
}
