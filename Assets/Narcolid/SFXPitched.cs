using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SFXPitched", menuName = "Audio/SFXPitched")]
public class SFXPitched : SFXTuned {
	[System.Serializable]
	public class AudioListAndPitches {
		public List<AudioClip> clips;
		public float tuning;
	}

	public List<AudioListAndPitches> clipListAndTuning;
	public float relativeTuning;

	public override AudioSource Play(GameObject target, float delay = 0) { return base.Play(target, SelectBasePitch(), delay: delay); }
	public override AudioSource Play(Vector3 target, float delay = 0) { return base.Play(target, SelectBasePitch(), delay: delay); }
	public override AudioSource Play(float delay = 0) { return base.Play(SelectBasePitch(), delay: delay); }

	protected float SelectBasePitch() {
		tuning = Mathf.Infinity;

		float basePitch = MusicManager.Instance.root + relativeTuning;
		if (basePitch >= 12) basePitch -= 12f;
		foreach (AudioListAndPitches set in clipListAndTuning) {
			if (Mathf.Abs(tuning - basePitch) > Mathf.Abs(set.tuning - basePitch)) {
				clips = set.clips;
				tuning = set.tuning;
			}
		}

		return basePitch;
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SFXPitched))]
[CanEditMultipleObjects]
public class SFXPitchedEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (!EditorApplication.isPlaying) return;

		if (GUILayout.Button("Play Sound")) {
			(target as SFXPitched).Play();
		}
	}
}
#endif