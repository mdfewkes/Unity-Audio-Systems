using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXTuned", menuName = "SFXTuned")]
public class NarcolidSFXTuned : NarcolidSFXBase
{
	public float tuning;

	public override AudioSource Play(GameObject target) { return NarcolidAudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch()); }
	public override AudioSource Play(Vector3 target) { return NarcolidAudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch()); }
	public override AudioSource Play() 	{ return NarcolidAudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch()); }

	public AudioSource Play(GameObject target, float basePitch) { return NarcolidAudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(basePitch)); }
	public AudioSource Play(Vector3 target, float basePitch) { return NarcolidAudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(basePitch)); }
	public AudioSource Play(float basePitch) { return NarcolidAudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch(basePitch)); }

	protected float SelectPitch()
	{
		float newPitchOffset = Mathf.Infinity;

		foreach (float interval in NarcolidAudioManager.Instance.tuning)
		{
			if (Mathf.Abs(interval - tuning) < Mathf.Abs(newPitchOffset))
			{
				newPitchOffset = interval - tuning;
			}
		}
		float newPitch = Mathf.Pow(2, (newPitchOffset / 12f));

		return newPitch;
	}

	protected float SelectPitch(float targetPitch)
	{
		float newPitchOffset = Mathf.Infinity;
		float foundPitch = Mathf.Infinity;

		foreach (float interval in NarcolidAudioManager.Instance.tuning)
		{
			if (Mathf.Abs(interval - targetPitch) < Mathf.Abs(foundPitch))
			{
				newPitchOffset = interval - tuning;
				foundPitch = interval - targetPitch;
			}
		}
		float newPitch = Mathf.Pow(2, (newPitchOffset / 12f));

		return newPitch;
	}

}
