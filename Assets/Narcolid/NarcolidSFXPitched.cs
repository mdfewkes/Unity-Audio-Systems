using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXPitched", menuName = "SFXPitched")]
public class NarcolidSFXPitched : NarcolidSFXTuned
{
	[System.Serializable]
	public class AudioListAndPitches
	{
		public List<AudioClip> clips;
		public float tuning;
	}

	public List<AudioListAndPitches> clipListAndTuning;

	public float relativeTuning;

	public override AudioSource Play()
	{
		tuning = 24f;

		float basePitch = NarcolidAudioManager.Instance.root + relativeTuning;
		if (basePitch > 12) basePitch -= 12f;
		foreach (AudioListAndPitches set in clipListAndTuning)
		{
			if (Mathf.Abs(tuning - basePitch) > Mathf.Abs(set.tuning - basePitch))
			{
				clips = set.clips;
				tuning = set.tuning;
			}
		}

		Debug.Log(basePitch);

		return base.Play(basePitch);
	}
}
