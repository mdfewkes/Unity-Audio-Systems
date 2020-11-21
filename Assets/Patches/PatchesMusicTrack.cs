using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatchesMusicTrack", menuName = "PatchesMusicTrack")]
public class PatchesMusicTrack : ScriptableObject {
	public AudioClip musicStem;

	public bool looping = false;

	public float bpm;
	public float endTime;
	public float[] outTimes;
	public float fadeTime = 0.25f;
}
