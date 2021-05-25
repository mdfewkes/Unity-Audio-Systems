using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SFXMonophonicContainer", menuName = "Audio/SFXMonophonicContainer")]
public class SFXMonophonicContainer : SFXBase {
	public SFXBase sfx;
	public AudioSource lastVoice;
	
	public override AudioSource Play(GameObject target, float delay = 0) {
		if (lastVoice) AudioManager.Instance.StopSoundSFX(lastVoice);
		lastVoice = sfx.Play(target, delay);
		return lastVoice;
	}
	public override AudioSource Play(Vector3 target, float delay = 0) {
		if (lastVoice) AudioManager.Instance.StopSoundSFX(lastVoice);
		lastVoice = sfx.Play(target, delay:delay);
		return lastVoice;
	}
	public override AudioSource Play(float delay = 0) {
		if (lastVoice) AudioManager.Instance.StopSoundSFX(lastVoice);
		lastVoice = sfx.Play(delay);
		return lastVoice;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SFXMonophonicContainer))]
[CanEditMultipleObjects]
public class SFXMonophonicContainerEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (!EditorApplication.isPlaying) return;

		if (GUILayout.Button("Play Sound")) {
			(target as SFXMonophonicContainer).Play();
		}
	}
}
#endif