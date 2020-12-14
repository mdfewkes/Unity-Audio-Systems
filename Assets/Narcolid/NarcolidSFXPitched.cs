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

	public override AudioSource Play(GameObject target) { return base.Play(target, SelectBasePitch()); }
	public override AudioSource Play(Vector3 target) { return base.Play(target, SelectBasePitch()); }
	public override AudioSource Play() { return base.Play(SelectBasePitch()); }

	protected float SelectBasePitch()
	{
		tuning = 24f;

		float basePitch = NarcolidAudioManager.Instance.root + relativeTuning;
		if (basePitch > 12f) basePitch -= 12f;
		if (basePitch < 0f) basePitch += 12f;
		foreach (AudioListAndPitches set in clipListAndTuning)
		{
			if (Mathf.Abs(tuning - basePitch) > Mathf.Abs(set.tuning - basePitch))
			{
				clips = set.clips;
				tuning = set.tuning;
			}
		}

		return basePitch;
	}
}
