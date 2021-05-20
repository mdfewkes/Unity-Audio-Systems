using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SFXUnpitched", menuName = "Audio/SFXUnpitched")]
public class SFXUnpitched : SFXBase {
	public float pitchWindow = 0.2f;
	public float volumeWindow = 0.3f;

	public override AudioSource Play(GameObject target) { return AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(), SeletVolume(), loop); }
	public override AudioSource Play(Vector3 target) { return AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(), SeletVolume(), loop); }
	public override AudioSource Play() { return AudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch(), SeletVolume(), loop); }

	protected float SelectPitch() {
		return 1f + Random.Range(-pitchWindow/2, pitchWindow/2);
	}

	protected float SeletVolume() {
		return 1f - Random.Range(0f, volumeWindow);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SFXUnpitched))]
[CanEditMultipleObjects]
public class SFXUnpitchedEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (!EditorApplication.isPlaying) return;

		if (GUILayout.Button("Play Sound")) {
			(target as SFXUnpitched).Play();
		}
	}
}
#endif