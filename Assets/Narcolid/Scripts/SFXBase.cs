using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SFXBase", menuName = "SFXBase")]
[SerializeField]
public class SFXBase : ScriptableObject
{
	public List<AudioClip> clip;
	
	public virtual void Play() { AudioManager.Instance.PlaySoundSFX(SelectClip(), 1f); }

	protected  AudioClip SelectClip()
	{
		return clip[Random.Range(0, clip.Count)];
	}
}
