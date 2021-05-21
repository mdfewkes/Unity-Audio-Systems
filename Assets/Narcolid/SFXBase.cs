using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SFXBase", menuName = "Audio/SFXBase")]
[SerializeField]
public class SFXBase : ScriptableObject {
	public List<AudioClip> clips;
	public bool loop;

	public virtual AudioSource Play(GameObject target, float delay = 0) { return AudioManager.Instance.PlaySoundSFX(target, clip: SelectClip(), looping: loop, delay:delay); }
	public virtual AudioSource Play(Vector3 target, float delay = 0) { return AudioManager.Instance.PlaySoundSFX(target, clip: SelectClip(), looping: loop, delay: delay); }
	public virtual AudioSource Play(float delay = 0) { return AudioManager.Instance.PlaySoundSFX(clip: SelectClip(), looping: loop, delay: delay); }

	protected AudioClip SelectClip() {
		return clips[Random.Range(0, clips.Count)];
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SFXBase))]
[CanEditMultipleObjects]
public class SFXBaseEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (!EditorApplication.isPlaying) return;

		if (GUILayout.Button("Play Sound")) {
			(target as SFXBase).Play();
		}
	}
}
#endif