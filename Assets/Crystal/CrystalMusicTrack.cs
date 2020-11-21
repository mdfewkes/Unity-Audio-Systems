using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrystalMusicTrack", menuName = "CrystalMusicTrack")]
public class CrystalMusicTrack : ScriptableObject
{
	public float BPM;
	public int minTracks;
	public int maxTracks;
	public AudioClip[] musicStems;
}