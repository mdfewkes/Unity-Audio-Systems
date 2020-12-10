﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SFXBase", menuName = "SFXBase")]
[SerializeField]
public class NarcolidSFXBase : ScriptableObject
{
	public List<AudioClip> clips;
	
	public virtual AudioSource Play() { return NarcolidAudioManager.Instance.PlaySoundSFX(SelectClip(), 1f); }

	protected  AudioClip SelectClip()
	{
		return clips[Random.Range(0, clips.Count)];
	}
}