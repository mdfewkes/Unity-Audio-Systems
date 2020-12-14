using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SFXBase", menuName = "SFXBase")]
[SerializeField]
public class NarcolidSFXBase : ScriptableObject
{
	public List<AudioClip> clips;
	public bool loop = false;

	public virtual AudioSource Play(GameObject target) { return NarcolidAudioManager.Instance.PlaySoundSFX(target, SelectClip(), looping: loop); }
	public virtual AudioSource Play(Vector3 target) { return NarcolidAudioManager.Instance.PlaySoundSFX(target, SelectClip(), looping: loop); }
	public virtual AudioSource Play() { return NarcolidAudioManager.Instance.PlaySoundSFX(SelectClip(), looping: loop); }

	protected  AudioClip SelectClip()
	{
		return clips[Random.Range(0, clips.Count)];
	}
}
