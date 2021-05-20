using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTrackGameObject : MonoBehaviour {
	public MeganMusicTrack track;
	public bool playOnAwake = false;

	void Start() {
		if (playOnAwake) MeganMusicManager.Instance.PlayTrack(track);
	}

	void OnMouseDown() {
		MeganMusicManager.Instance.PlayTrack(track);
	}
}
