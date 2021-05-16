using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Music Track", menuName = "Music Track")]
public class MusicTrack : ScriptableObject {
	public AudioClip baseTrack;
	public AudioClip lowHealth;
	public AudioClip highIntensity;

	public float bpm;
	public float startTime;
	public float endTime;
	public List<float> outTimes;

	public AudioClip GetClip(MusicManager.TrackState state) {
		switch(state) {
			case MusicManager.TrackState.HighIntensity:
				return highIntensity;
			case MusicManager.TrackState.LowHealth:
				return lowHealth;
			default:
				return baseTrack;
		}
	}
}
