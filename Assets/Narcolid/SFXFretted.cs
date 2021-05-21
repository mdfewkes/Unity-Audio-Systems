using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SFXFretted", menuName = "Audio/SFXFretted")]
public class SFXFretted : SFXBase {
	public float tuning;
	public float openFretTuning;

	public override AudioSource Play(GameObject target, float delay = 0) { return AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(), looping: loop, delay: delay); }
	public override AudioSource Play(Vector3 target, float delay = 0) { return AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(), looping: loop, delay: delay); }
	public override AudioSource Play(float delay = 0) { return AudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch(), looping: loop, delay: delay); }

	protected float SelectPitch() {
		float newPitchOffset = Mathf.Infinity;
		float currentInterval = Mathf.Infinity;
		float currentFretTunning = Mathf.Infinity;

		foreach (float interval in MusicManager.Instance.tuning) {
			currentInterval = interval;
			while (currentInterval >= openFretTuning+12) currentInterval -= 12f;
			while (currentInterval < openFretTuning) currentInterval += 12f;

			if (currentInterval >= openFretTuning && currentInterval < currentFretTunning) {
				currentFretTunning = currentInterval;
				newPitchOffset = currentInterval - tuning;
			}
		}
		float newPitch = Mathf.Pow(2, (newPitchOffset / 12f));

		return newPitch;
	}
}


#if UNITY_EDITOR
[CustomEditor(typeof(SFXFretted))]
[CanEditMultipleObjects]
public class SFXFrettedEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (!EditorApplication.isPlaying) return;

		if (GUILayout.Button("Play Sound")) {
			(target as SFXFretted).Play();
		}
	}
}
#endif