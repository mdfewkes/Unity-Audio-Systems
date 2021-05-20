﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	public static AudioManager Instance;
	public AudioSource audioSourcePrefabSFX;
	public AudioSource audioSourcePrefabUI;
	public AudioMixerGroup defaultSFXGroup;

	private List<AudioSource> loopingSources = new List<AudioSource>();

	void Awake() {
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	void Update() {
		foreach (AudioSource source in loopingSources) {
			//--//--Update source Listener
			VirtualAudioSource vas = source.GetComponent<VirtualAudioSource>();
			if (vas.listener != vas.GetClosestListener()) {
				Debug.Log("New Listener");
				VirtualAudioSource freshVAS = Instantiate(vas);
				freshVAS.transform.parent = transform;
				freshVAS.listener = vas.listener;
				Destroy(freshVAS.GetComponentInChildren<SFXSequenceComponent>());
				freshVAS.GetComponent<AudioSource>().time = source.time;
				StartCoroutine(FadeOutAndStop(freshVAS.GetComponent<AudioSource>(), 0.15f));

				vas.listener = vas.GetClosestListener();
				float targetVolume = source.volume;
				source.volume = 0f;
				StartCoroutine(FadeTo(source, targetVolume, 0.15f));
			}

			//--//--Update source ReverbZone
			ReverbZone.AssignOutputMixerGroupToAudioSource(source, vas.targetPosition);
		}
	}

	//--//-----SFX Functions

	//Play spatialized sounds
	public AudioSource PlaySoundSFX(Vector3 positionToPlayAt, AudioClip clip, float pitch = 1f, float volume = 1f, bool looping = false) {
		AudioSource freshAudioSource = Instantiate(audioSourcePrefabSFX);
		var vas = freshAudioSource.GetComponent<VirtualAudioSource>();
		vas.targetPosition = positionToPlayAt;
		
		freshAudioSource.gameObject.transform.parent = gameObject.transform;
		freshAudioSource.gameObject.name = clip.name;
		freshAudioSource.volume = volume;
		freshAudioSource.pitch = pitch;
		freshAudioSource.loop = looping;
		freshAudioSource.clip = clip;

		ReverbZone.AssignOutputMixerGroupToAudioSource(freshAudioSource, positionToPlayAt);

		freshAudioSource.Play();
		if (looping) loopingSources.Add(freshAudioSource);
		else Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length/pitch + 0.1f);

		return freshAudioSource;
	}

	public AudioSource PlaySoundSFX(GameObject objectToPlayOn, AudioClip clip, float pitch = 1f, float volume = 1f, bool looping = false) {
		AudioSource freshAudioSource = PlaySoundSFX(objectToPlayOn.transform.position, clip, pitch, volume, looping);
		freshAudioSource.gameObject.GetComponent<VirtualAudioSource>().target = objectToPlayOn;

		return freshAudioSource;
	}

	public AudioSource PlaySoundSFX(AudioClip clip, float pitch = 1f, float volume = 1f, bool looping = false) {
		return PlaySoundSFX(Camera.main.transform.position, clip, pitch, volume, looping);
	}

	public void StopSoundSFX(AudioSource source) {
		source.Stop();
		loopingSources.Remove(source);
		Destroy(source.gameObject);
	}

	//Plays an unspatialized sound
	public AudioSource PlaySoundUI(AudioClip clip, float pitch = 1f, float volume = 1f) {
		AudioSource freshAudioSource = Instantiate(audioSourcePrefabUI);
		freshAudioSource.gameObject.transform.parent = gameObject.transform;
		freshAudioSource.volume = volume;
		freshAudioSource.pitch = pitch;
		freshAudioSource.clip = clip;


		freshAudioSource.Play();
		Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length * pitch + 0.1f);

		return freshAudioSource;
	}

	//--//-----Volume automation
	public IEnumerator FadeIn(AudioSource source, float fadeTime) {
		float startTime = Time.unscaledTime;
		float currentTime = 0f;

		source.volume = 0f;

		while (startTime + fadeTime > Time.unscaledTime) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(0f, 1f, currentTime / fadeTime);
			yield return null;
		}

		source.volume = 1f;
	}

	public IEnumerator WaitAndFadeIn(AudioSource source, float waitTime, float fadeTime) {
		float startTime = Time.unscaledTime + waitTime;
		float currentTime = 0f;

		source.volume = 0f;

		while (startTime > Time.unscaledTime) {
			yield return null;
		}

		while (startTime + fadeTime > Time.unscaledTime) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(0f, 1f, currentTime / fadeTime);
			yield return null;
		}

		source.volume = 1f;
	}

	public IEnumerator FadeOut(AudioSource source, float fadeTime) {
		float startTime = Time.unscaledTime;
		float currentTime = 0f;

		source.volume = 1f;

		while (startTime + fadeTime > Time.unscaledTime) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(1f, 0f, currentTime / fadeTime);
			yield return null;
		}

		source.volume = 0f;
	}

	public IEnumerator WaitAndFadeOut(AudioSource source, float waitTime, float fadeTime) {
		float startTime = Time.unscaledTime + waitTime;
		float currentTime = 0f;

		source.volume = 1f;

		while (startTime > Time.unscaledTime) {
			yield return null;
		}

		while (startTime + fadeTime > Time.unscaledTime) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(1f, 0f, currentTime / fadeTime);
			yield return null;
		}

		source.volume = 0f;
	}

	public IEnumerator FadeOutAndStop(AudioSource source, float fadeTime) {
		float startTime = Time.unscaledTime;
		float currentTime = 0f;

		source.volume = 1f;

		while (startTime + fadeTime > Time.unscaledTime) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(1f, 0f, currentTime / fadeTime);
			yield return null;
		}

		source.Stop();
		Destroy(source.gameObject);
	}

	public IEnumerator WaitAndFadeOutAndStop(AudioSource source, float waitTime, float fadeTime) {
		float startTime = Time.unscaledTime + waitTime;
		float currentTime = 0f;

		while (startTime > Time.unscaledTime) {
			yield return null;
		}

		while (startTime + fadeTime > Time.unscaledTime) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(1f, 0f, currentTime / fadeTime);
			yield return null;
		}

		source.Stop();
		Destroy(source.gameObject);
	}

	public IEnumerator FadeTo(AudioSource source, float newVolume, float fadeTime) {
		float startTime = Time.unscaledTime;
		float currentTime = 0f;
		float startingVolume = source.volume;

		while (startTime + fadeTime > Time.unscaledTime) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(startingVolume, newVolume, currentTime / fadeTime);
			yield return null;
		}

		source.volume = newVolume;
	}

	public IEnumerator WaitAndFadeTo(AudioSource source, float waitTime, float newVolume, float fadeTime) {
		float startTime = Time.unscaledTime + waitTime;
		float currentTime = 0f;
		float startingVolume = source.volume;

		while (startTime > Time.unscaledTime) {
			yield return null;
		}

		while (startTime + fadeTime > Time.unscaledTime) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(startingVolume, newVolume, currentTime / fadeTime);
			yield return null;
		}

		source.volume = newVolume;
	}

}