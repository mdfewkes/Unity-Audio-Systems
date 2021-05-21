﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SFXTuned", menuName = "Audio/SFXTuned")]
public class SFXTuned : SFXBase {
	public float tuning;

	public override AudioSource Play(GameObject target, float delay = 0) { return AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(), looping: loop, delay: delay); }
	public override AudioSource Play(Vector3 target, float delay = 0) { return AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(), looping: loop, delay: delay); }
	public override AudioSource Play(float delay = 0) { return AudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch(), looping: loop, delay: delay); }

	public AudioSource Play(GameObject target, float basePitch, float delay = 0) { return AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(basePitch), looping: loop, delay: delay); }
	public AudioSource Play(Vector3 target, float basePitch, float delay = 0) { return AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(basePitch), looping: loop, delay: delay); }
	public AudioSource Play(float basePitch, float delay = 0) { return AudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch(basePitch), looping: loop, delay: delay); }

	protected float SelectPitch() {
		float newPitchOffset = Mathf.Infinity;

		foreach (float interval in MusicManager.Instance.tuning) {
			if (Mathf.Abs(interval - tuning) < Mathf.Abs(newPitchOffset)) {
				newPitchOffset = interval - tuning;
			}
		}
		float newPitch = Mathf.Pow(2, (newPitchOffset / 12f));

		return newPitch;
	}

	protected float SelectPitch(float targetPitch) {
		float newPitchOffset = Mathf.Infinity;
		float foundPitch = Mathf.Infinity;

		foreach (float interval in MusicManager.Instance.tuning) {
			if (Mathf.Abs(interval - targetPitch) < Mathf.Abs(foundPitch)) {
				newPitchOffset = interval - tuning;
				foundPitch = interval - targetPitch;
			}
		}
		float newPitch = Mathf.Pow(2, (newPitchOffset / 12f));

		return newPitch;
	}

}

#if UNITY_EDITOR
[CustomEditor(typeof(SFXTuned))]
[CanEditMultipleObjects]
public class SFXTunedEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (!EditorApplication.isPlaying) return;

		if (GUILayout.Button("Play Sound")) {
			(target as SFXTuned).Play();
		}
	}
}
#endif