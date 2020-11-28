using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MDFAudioManager : MonoBehaviour
{
	public static MDFAudioManager Instance;
	public GameObject audioSourcePrefab;

	void Awake() {
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void PlaySoundSimple(AudioClip clip, GameObject objectToPlayOn, float volume = 1f)
	{
		AudioSource.PlayClipAtPoint(clip, objectToPlayOn.transform.position, volume);
	}

	public void PlaySoundSimple(AudioClip clip, Vector3 positionToPlayAt, float volume = 1f)
	{
		AudioSource.PlayClipAtPoint(clip, positionToPlayAt, volume);
	}

	public AudioSource PlaySoundSFX(AudioClip clip, GameObject objectToPlayOn, float volume = 1f, float pitch = 1f, float blend = 1f, bool loop = false)
	{
		AudioSource freshAudioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
		freshAudioSource.gameObject.transform.position = objectToPlayOn.transform.position;
		freshAudioSource.gameObject.transform.parent = objectToPlayOn.gameObject.transform;
		freshAudioSource.pitch = pitch;
		freshAudioSource.volume = volume;
		freshAudioSource.spatialBlend = blend;
		freshAudioSource.clip = clip;
		freshAudioSource.loop = loop;
		freshAudioSource.Play();

		if (!loop)
		{
			Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length);
		}
		return freshAudioSource;
	}

	public AudioSource PlaySoundSFX(AudioClip clip, Vector3 positionToPlayAt, float volume = 1f, float pitch = 1f, float blend = 1f, bool loop = false)
	{
		AudioSource freshAudioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
		freshAudioSource.gameObject.transform.position = positionToPlayAt;
		freshAudioSource.pitch = pitch;
		freshAudioSource.volume = volume;
		freshAudioSource.spatialBlend = blend;
		freshAudioSource.clip = clip;
		freshAudioSource.loop = loop;
		freshAudioSource.Play();

		if (!loop)
		{
			Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length);
		}
		return freshAudioSource;
	}

	public AudioSource PlaySoundDialogue(AudioClip clip, GameObject objectToPlayOn, float volume = 1f)
	{
		AudioSource freshAudioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
		freshAudioSource.gameObject.transform.position = objectToPlayOn.transform.position;
		freshAudioSource.gameObject.transform.parent = objectToPlayOn.gameObject.transform;
		freshAudioSource.volume = volume;
		freshAudioSource.spatialBlend = 1f;
		freshAudioSource.clip = clip;
		freshAudioSource.Play();

		Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length);

		return freshAudioSource;
	}

	public AudioSource PlaySoundDialogue(AudioClip clip, Vector3 positionToPlayAt, float volume = 1f)
	{
		AudioSource freshAudioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
		freshAudioSource.gameObject.transform.position = positionToPlayAt;
		freshAudioSource.volume = volume;
		freshAudioSource.spatialBlend = 1f;
		freshAudioSource.clip = clip;
		freshAudioSource.Play();

		Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length);

		return freshAudioSource;
	}

	public AudioSource PlaySoundVO(AudioClip clip, float volume = 1f)
	{
		AudioSource freshAudioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
		freshAudioSource.gameObject.transform.position = gameObject.transform.position;
		freshAudioSource.gameObject.transform.parent = gameObject.transform;
		freshAudioSource.volume = volume;
		freshAudioSource.spatialBlend = 0f;
		freshAudioSource.clip = clip;
		freshAudioSource.Play();

		Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length);

		return freshAudioSource;
	}

	public void StopSound(AudioSource source)
	{
		source.Stop();
		Destroy(source.gameObject);
	}

	public void StopSound(AudioSource source, float fadeTime)
	{
		StartCoroutine(FadeOut(source, fadeTime));
	}

	IEnumerator FadeOut(AudioSource source, float fadeTime)
	{
		float startTime = Time.time;
		float currentTime = 0f;
		float startVolume = source.volume;

		while (startTime + fadeTime > Time.time)
		{
			currentTime = Time.time - startTime;

			source.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeTime);
			yield return null;
		}

		source.Stop();
		Destroy(source.gameObject);

	}
}