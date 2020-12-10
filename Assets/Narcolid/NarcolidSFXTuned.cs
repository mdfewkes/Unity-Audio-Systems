using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXTuned", menuName = "SFXTuned")]
public class NarcolidSFXTuned : NarcolidSFXBase
{
	public float tuning;

	public override AudioSource Play()
	{
		return NarcolidAudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch());
	}

	public AudioSource Play(float targetPitch)
	{
		return NarcolidAudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch(targetPitch));
	}

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

		Debug.Log(newPitch);

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

		//Debug.Log(newPitchOffset + " " + newPitch);

		return newPitch;
	}

}
