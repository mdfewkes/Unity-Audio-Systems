using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Music Track", menuName = "Music Track")]
public class MeganMusicTrack : ScriptableObject {
	public AudioClip baseTrack;
	public AudioClip lowHealth;
	public AudioClip highIntensity;

	public float bpm;
	public float startTime;
	public float endTime;
	public List<float> outTimes;

	public AudioClip GetClip(MeganMusicManager.TrackState state) {
		switch(state) {
			case MeganMusicManager.TrackState.HighIntensity:
				return highIntensity;
			case MeganMusicManager.TrackState.LowHealth:
				return lowHealth;
			default:
				return baseTrack;
		}
	}
}
