using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SFXTuned", menuName = "Audio/SFXTuned")]
public class SFXTuned : SFXBase {
	public float tuning;

	public override AudioSource Play(GameObject target, float delay = 0) {
		AudioSource freshAudioSource = AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(), looping: loop, delay: delay);
		if (loop) freshAudioSource.gameObject.AddComponent<SFXTunedComponent>().sound = this;
		return freshAudioSource;
	}
	public override AudioSource Play(Vector3 target, float delay = 0) {
		AudioSource freshAudioSource = AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(), looping: loop, delay: delay);
		if (loop) freshAudioSource.gameObject.AddComponent<SFXTunedComponent>().sound = this;
		return freshAudioSource;
	}
	public override AudioSource Play(float delay = 0) {
		AudioSource freshAudioSource = AudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch(), looping: loop, delay: delay);
		if (loop) freshAudioSource.gameObject.AddComponent<SFXTunedComponent>().sound = this;
		return freshAudioSource;
	}

	public AudioSource Play(GameObject target, float targetPitch, float delay = 0) {
		AudioSource freshAudioSource = AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(targetPitch), looping: loop, delay: delay);
		if (loop) freshAudioSource.gameObject.AddComponent<SFXTunedComponent>().sound = this;
		return freshAudioSource;
	}
	public AudioSource Play(Vector3 target, float targetPitch, float delay = 0) {
		AudioSource freshAudioSource = AudioManager.Instance.PlaySoundSFX(target, SelectClip(), SelectPitch(targetPitch), looping: loop, delay: delay);
		if (loop) freshAudioSource.gameObject.AddComponent<SFXTunedComponent>().sound = this;
		return freshAudioSource;
	}
	public AudioSource Play(float targetPitch, float delay = 0) {
		AudioSource freshAudioSource = AudioManager.Instance.PlaySoundSFX(SelectClip(), SelectPitch(targetPitch), looping: loop, delay: delay);
		if (loop) freshAudioSource.gameObject.AddComponent<SFXTunedComponent>().sound = this;
		return freshAudioSource;
	}

	public float SelectPitch() {
		float newPitchOffset = Mathf.Infinity;

		foreach (float interval in MusicManager.Instance.tuning) {
			if (Mathf.Abs(interval - tuning) < Mathf.Abs(newPitchOffset)) {
				newPitchOffset = interval - tuning;
			}
		}
		float newPitch = Mathf.Pow(2, (newPitchOffset / 12f));

		return newPitch;
	}

	public float SelectPitch(float targetPitch) {
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

public class SFXTunedComponent : MonoBehaviour {
	public SFXTuned sound;

	void Start() {
		MusicManager.Instance.OnChordChange += Retune;
	}

	void OnDestroy() {
		MusicManager.Instance.OnChordChange -= Retune;
	}

	private void Retune() {
		AudioSource source = GetComponent<AudioSource>();
		float newPitch = sound.SelectPitch();
		if (source.pitch == newPitch) return;

		AudioSource freshAudioSource = Instantiate(source);
		freshAudioSource.transform.parent = transform;
		freshAudioSource.time = source.time;
		AudioManager.Instance.StartCoroutine(AudioManager.Instance.FadeOutAndStop(freshAudioSource));

		AudioManager.Instance.StartCoroutine(AudioManager.Instance.FadeIn(source));
		source.pitch = newPitch;
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