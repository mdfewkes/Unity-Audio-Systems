using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXTone", menuName = "SFXTone")]
public class SFXTone : SFXBase
{
	public int tuning;

	public override void Play()
	{

		AudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch());
	}

	protected float SelectPitch()
	{
		int newPitchOffset = 1200;

		foreach (int interval in AudioManager.Instance.tuning)
		{
			if (Mathf.Abs(interval - tuning) < Mathf.Abs(newPitchOffset))
			{
				newPitchOffset = interval - tuning;
			}
		}

		float newPitch = Mathf.Pow(2, ((float)newPitchOffset / 1200));

		Debug.Log(newPitch);

		return newPitch;
	}

}
