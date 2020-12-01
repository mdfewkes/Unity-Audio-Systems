/*
https://designingsound.org/2016/08/31/making-a-music-system-part-1/
https://designingsound.org/2016/09/07/making-a-music-system-part-2/
https://designingsound.org/2016/09/14/making-a-music-system-part-3/
https://designingsound.org/2016/09/21/making-a-music-system-part-4/
https://designingsound.org/2016/09/28/making-a-music-system-wrap-up/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MusicSequencer : MonoBehaviour
{
	public static MusicSequencer Instance;

	public bool isPlaying = false;

	[SerializeField] private double beatsPerMinute = 120.0;
	[SerializeField] private int ticksPerBeat = 4;
	private double tickLength;
	private double nextTickTime;
	private int ticksUntilNextBeat;
	private double startTime = 0;
	private double currentTime = 0;

	private List<SamplerVoice> voices;
	private int numberOfVoices = 16;
	[SerializeField]private List<Instrument> instruments = new List<Instrument>();
	private int numberOfChannels = 8;

	[SerializeField]private Sequence currentSequence;
	private Sequence nextSequence;
	private int currentEventIndex = 0;

	public event Action OnBeat;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}


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

		tickLength = 60.0 / beatsPerMinute / ticksPerBeat;
		nextTickTime = AudioSettings.dspTime + tickLength;
		ticksUntilNextBeat = ticksPerBeat;
		startTime = AudioSettings.dspTime;

		if (instruments.Count < numberOfChannels)
		{
			for (int i = instruments.Count; i < numberOfChannels; i++)
			{
				instruments.Add(ScriptableObject.CreateInstance<Instrument>());
			}
		}

		if (currentSequence != null)
		{
			StartSequenceFromStop();
		}
	}

	void Update()
	{
		currentTime = AudioSettings.dspTime + Time.deltaTime;

		if (currentTime >= nextTickTime)
		{
			nextTickTime += tickLength;
			ticksUntilNextBeat--;

			if (ticksUntilNextBeat <= 0)
			{
				//dude, the beat

				OnBeat?.Invoke();

				ticksUntilNextBeat = ticksPerBeat;
			}

			//dude, the sequence
			while (currentSequence.events[currentEventIndex].time <= nextTickTime - startTime && isPlaying)
			{
				SequenceEvent thisEvent = currentSequence.events[currentEventIndex];
				switch (thisEvent.type)
				{
					case SequenceEvent.EventType.Note:
						instruments[(int)thisEvent.parameter1].Play(thisEvent.time + AudioSettings.dspTime, thisEvent.parameter2, thisEvent.parameter3, thisEvent.parameter4);
						break;
					case SequenceEvent.EventType.LoopPoint:
						startTime = thisEvent.time + AudioSettings.dspTime;
						currentEventIndex = 0;
						break;
				}

				currentEventIndex++;
				if (currentEventIndex >= currentSequence.events.Count)
				{
					isPlaying = false;
					currentEventIndex = 0;
					break;
				}
			}

		}

	}

	public void Play(double startTime, AudioClip clip, float volume = 0.75f, float pitch = 1f, float duration = -1f)
	{
		FindOpenVoice().Play(startTime, clip, volume, pitch, duration);
	}

	public void SetBPM(float newTempo)
	{
		beatsPerMinute = newTempo;

		tickLength = 60.0 / beatsPerMinute / ticksPerBeat;
		nextTickTime = AudioSettings.dspTime + tickLength;
	}

	public void SetBPM(double newTempo, int newTicksPerBeat)
	{
		beatsPerMinute = newTempo;
		ticksPerBeat = newTicksPerBeat;

		tickLength = 60.0 / beatsPerMinute / ticksPerBeat;
		nextTickTime = AudioSettings.dspTime + tickLength;
	}

	public void StartNewSequence(Sequence newSequence)
	{
		nextSequence = newSequence;
	}

	private void StartSequenceFromStop()
	{
		if (currentSequence == null && nextSequence == null) return;
		if (nextSequence != null) currentSequence = nextSequence;

		startTime = AudioSettings.dspTime;
		beatsPerMinute = currentSequence.tempo;
		tickLength = 60.0 / beatsPerMinute / ticksPerBeat;
		nextTickTime = AudioSettings.dspTime + tickLength;
		ticksUntilNextBeat = ticksPerBeat;

		isPlaying = true;
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
	public float releaseTime = 0.15f;

	public void Play(double startTime, AudioClip clip, float volume = 0.75f, float pitch = 1f, float duration = -1, float release = 0.15f)
	{
		StopAllCoroutines();
		releaseTime = release;

		source.clip = clip;
		source.pitch = pitch;
		source.volume = volume;
		source.PlayScheduled(startTime);

		startTime = Time.time;
		if (duration > 0)
		{
			StartCoroutine(WaitForRelease(duration + ((float)startTime - Time.time)));
		}
	}

	public void Stop()
	{
		StopAllCoroutines();
		StartCoroutine(FadeOut(releaseTime));
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

[Serializable]
public class SequenceEvent
{
	public EventType type;
	public double time;
	public float parameter1 = 0f;
	public float parameter2 = 0f;
	public float parameter3 = 0f;
	public float parameter4 = 0f;

	public enum EventType
	{
		Note,
		LoopPoint
	}
}

[Serializable]
public class Sequence
{
	public double tempo;
	public List<SequenceEvent> events;
}

public static class MusicMathUtils
{
	public const int MidiNoteC4 = 60;

	public static float MidiNoteToPitch(float midiNote, float baseNote)
	{
		float semitoneOffset = midiNote - baseNote;
		return Mathf.Pow(2.0f, semitoneOffset / 12.0f);
	}
}