using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	public static AudioManager Instance;
	const float FadeTime = 0.15f;
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
				VirtualAudioSource freshVAS = Instantiate(vas);
				freshVAS.transform.parent = vas.transform;
				freshVAS.listener = vas.listener;
				Destroy(freshVAS.GetComponentInChildren<SFXSequenceComponent>());
				freshVAS.GetComponent<AudioSource>().time = source.time;
				StartCoroutine(FadeOutAndStop(freshVAS.GetComponent<AudioSource>()));

				vas.listener = vas.GetClosestListener();
				float targetVolume = source.volume;
				source.volume = 0f;
				StartCoroutine(FadeTo(source, targetVolume));
			}

			//--//--Update source ReverbZone
			ReverbZone.AssignOutputMixerGroupToAudioSource(source, vas.targetPosition);
		}
	}

	//--//-----SFX Functions
	public AudioSource PlaySoundSFX(Vector3 positionToPlayAt, AudioClip clip, float pitch = 1f, float volume = 1f, bool looping = false, float delay = 0f) {
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

		freshAudioSource.PlayDelayed(delay);
		if (looping) loopingSources.Add(freshAudioSource);
		else Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length/pitch + delay);

		return freshAudioSource;
	}

	public AudioSource PlaySoundSFX(GameObject objectToPlayOn, AudioClip clip, float pitch = 1f, float volume = 1f, bool looping = false, float delay = 0f) {
		AudioSource freshAudioSource = PlaySoundSFX(objectToPlayOn.transform.position, clip, pitch, volume, looping, delay);
		freshAudioSource.gameObject.GetComponent<VirtualAudioSource>().target = objectToPlayOn;

		return freshAudioSource;
	}

	public AudioSource PlaySoundSFX(AudioClip clip, float pitch = 1f, float volume = 1f, bool looping = false, float delay = 0f) {
		return PlaySoundSFX(Camera.main.transform.position, clip, pitch, volume, looping, delay);
	}

	public void StopSoundSFX(AudioSource source, float fadeTime = FadeTime) {
		StartCoroutine(FadeOutAndStop(source, fadeTime));
		loopingSources.Remove(source);
	}
	
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
	public IEnumerator FadeIn(AudioSource source, float fadeTime = FadeTime) {
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

	public IEnumerator WaitAndFadeIn(AudioSource source, float waitTime, float fadeTime = FadeTime) {
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

	public IEnumerator FadeOut(AudioSource source, float fadeTime = FadeTime) {
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

	public IEnumerator WaitAndFadeOut(AudioSource source, float waitTime, float fadeTime = FadeTime) {
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

	public IEnumerator FadeOutAndStop(AudioSource source, float fadeTime = FadeTime) {
		float startTime = Time.unscaledTime;
		float currentTime = 0f;

		source.volume = 1f;

		while (startTime + fadeTime > Time.unscaledTime && source) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(1f, 0f, currentTime / fadeTime);
			yield return null;
		}

		source?.Stop();
		Destroy(source.gameObject);
	}

	public IEnumerator WaitAndFadeOutAndStop(AudioSource source, float waitTime, float fadeTime = FadeTime) {
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

	public IEnumerator FadeTo(AudioSource source, float newVolume, float fadeTime = FadeTime) {
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

	public IEnumerator WaitAndFadeTo(AudioSource source, float waitTime, float newVolume, float fadeTime = FadeTime) {
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