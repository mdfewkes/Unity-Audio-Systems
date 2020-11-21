using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSequencer : MonoBehaviour
{
	[Header("Debug")]
	public bool testPlay = false;
	public AudioClip testSample;

	private List<SamplerVoice> voices;
	private int numberOfVoices = 16;
	//private List<Instruments> instruments;
	//private int numberOfChannels = 8;


	void Start()
	{
		voices = new List<SamplerVoice>();

		for (int i = 0; i < numberOfVoices; i++)
		{
			GameObject newVoiceObject = new GameObject();
			newVoiceObject.AddComponent<SamplerVoice>();
			newVoiceObject.transform.parent = gameObject.transform;
			newVoiceObject.name = "Sampler Voice";
			voices.Add(newVoiceObject.GetComponent<SamplerVoice>());
			voices[i].source = newVoiceObject.AddComponent<AudioSource>();
		}
	}

	void Update()
	{
		if (testPlay)
		{
			Play(testSample);
			testPlay = false;
		}
	}

	public void Play(AudioClip clip)
	{
		FindOpenVoice().Play(clip);
	}

	private SamplerVoice FindOpenVoice()
	{
		float oldestTime = Time.time;
		int oldestIndex = 0;

		foreach (SamplerVoice voice in voices)
		{
			if (!voice.source.isPlaying)
			{
				return voice;
			}

			if (voice.startTime < oldestTime)
			{
				oldestTime = voice.startTime;
				oldestIndex = voices.IndexOf(voice);
			}
		}

		return voices[oldestIndex];
	}
}

public class SamplerVoice : MonoBehaviour
{
	public AudioSource source;
	public float startTime;
	public float releaseTime;

	public void Play(AudioClip clip, float volume = 0.75f, float pitch = 1f, float duration = -1, float release = 0.15f)
	{
		source.clip = clip;
		source.pitch = pitch;
		source.volume = volume;
		source.Play();

		startTime = Time.time;
		if (duration > 0)
		{
			StartCoroutine(WaitForRelease(duration));
		}
	}

	IEnumerator WaitForRelease(float duration)
	{
		yield return new WaitForSeconds(duration);
		StartCoroutine(FadeOut(releaseTime));
	}

	IEnumerator FadeOut(float fadeTime)
	{
		float startVolume = source.volume;
		float startTime = Time.time;

		while (source.volume > 0)
		{
			source.volume = Mathf.Lerp(startVolume, 0f, (Time.time - startTime) / fadeTime);

			yield return null;
		}

		source.Stop();
	}
}