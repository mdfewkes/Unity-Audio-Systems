using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SFXFretted", menuName = "Audio/SFXFretted")]
public class SFXFretted : SFXTuned {
	[System.Serializable]
	public class AudioListAndPitches {
		public List<AudioClip> clips;
		public float tuning;
	}

	public List<AudioListAndPitches> clipListAndTuning;
	public float openFretTuning;

	public override AudioSource Play(GameObject target, float delay = 0) { return Play(target, SelectBasePitch(), delay: delay); }
	public override AudioSource Play(Vector3 target, float delay = 0) { return Play(target, SelectBasePitch(), delay: delay); }
	public override AudioSource Play(float delay = 0) { return Play(SelectBasePitch(), delay: delay); }

	protected float SelectBasePitch() {
		float currentInterval = Mathf.Infinity;
		float currentFretTunning = Mathf.Infinity;

		foreach (float interval in MusicManager.Instance.tuning) {
			currentInterval = interval;
			while (currentInterval >= openFretTuning+12) currentInterval -= 12f;
			while (currentInterval < openFretTuning) currentInterval += 12f;

			if (currentInterval >= openFretTuning && currentInterval < currentFretTunning) {
				currentFretTunning = currentInterval;
			}
		}

		tuning = Mathf.Infinity;
		foreach (AudioListAndPitches set in clipListAndTuning) {
			if (Mathf.Abs(tuning - currentFretTunning) > Mathf.Abs(set.tuning - currentFretTunning)) {
				clips = set.clips;
				tuning = set.tuning;
			}
		}

		return currentFretTunning;
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