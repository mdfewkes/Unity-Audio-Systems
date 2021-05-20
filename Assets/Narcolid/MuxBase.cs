using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "MuxBase", menuName = "Audio/MuxBase")]
[SerializeField]
public class MuxBase : ScriptableObject {
	public AudioClip clip;
	public float bpm = 120f;
	public float startTime;
	public float endTime;
	public bool loop;
	public List<float> outTimes;
	public List<ChordChange> changes;

	public virtual AudioSource PlayTrack() {
		return MusicManager.Instance.StartMux(clip, this);
	}

}

[Serializable]
public class ChordChange {
	public float time;
	public List<float> tuning;
	public float root;
}

#if UNITY_EDITOR
[CustomEditor(typeof(MuxBase))]
public class MuxBaseEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (!EditorApplication.isPlaying) return;

		if (GUILayout.Button("Play Music")) {
			(target as MuxBase).PlayTrack();
		}
	}
}
#endif