using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour {
	public static MusicManager Instance;
	public float FadeTime = 0.15f;
	public float BufferTime = 0.25f;

	public AudioSource audioSourcePrefabMusic;
	public List<float> tuning;
	public float root;

	public static AudioSource currentSource;
	public static MuxBase currentTrack;
	public bool playing = false;
	public float trackPickupTime = 0f;
	public int currentChordIndex = 0;

	public float startTime = 0f;
	public float currentTime = 0f;
	public float nextQuarterBeat = 0f;
	public float nextHalfBeat = 0f;
	public float nextBeat = 0f;
	public float nextChordChange = 0f;
	public float nextOutTime = 0f;
	public float endTime = 0f;

	public bool cuedQuarterBeat = false;
	public bool cuedHalfBeat = false;
	public bool cuedBeat = false;
	public bool cuedChordChange = false;
	public bool cuedOutTime = false;
	public bool cuedEndTime = false;

	public Action OnBeat = () => { };
	public Action OnHalfBeat = () => { };
	public Action OnQuarterBeat = () => { };
	public Action OnChordChange = () => { };
	public Action OnOutTime = () => { };
	public Action OnEndTime = () => { };

	public Action<float> QueBeat = (float delay) => { };
	public Action<float> QueHalfBeat = (float delay) => { };
	public Action<float> QueQuarterBeat = (float delay) => { };
	public Action<float> QueChordChange = (float delay) => { };
	public Action<float> QueOutTime = (float delay) => { };
	public Action<float> QueEndTime = (float delay) => { };

	void Start() {
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	void Update() {
		if (playing) {
			if (currentSource) currentTime = currentSource.time;
			else currentTime = Time.time - startTime;

			//Quarter beat
			if (currentTime >= nextQuarterBeat - BufferTime/4 && !cuedQuarterBeat) {
				cuedQuarterBeat = true;
				QueQuarterBeat(nextQuarterBeat - currentTime- BufferTime);

			} else if (currentTime >= nextQuarterBeat) {
				OnQuarterBeat();
				CalculateNextQuarterBeat();
			}

			//Half beat
			if (currentTime >= nextHalfBeat - BufferTime/2 && !cuedHalfBeat) {
				cuedHalfBeat = true;
				QueHalfBeat(nextHalfBeat - currentTime- BufferTime);

			} else if (currentTime >= nextHalfBeat) {
				OnHalfBeat();
				CalculateNextHalfBeat();
			}

			//Beat
			if (currentTime >= nextBeat - BufferTime && !cuedBeat) {
				cuedBeat = true;
				QueBeat(nextBeat - currentTime- BufferTime);

			} else if (currentTime >= nextBeat) {
				OnBeat();
				CalculateNextBeat();
			}

			//Chord change
			if (currentTime >= nextChordChange - BufferTime && !cuedChordChange) {
				cuedChordChange = true;
				QueChordChange(nextChordChange - currentTime- BufferTime);

				GenerateTuning(currentTrack.changes[currentChordIndex].tuning, currentTrack.changes[currentChordIndex].root);

			} else if (currentTime >= nextChordChange) {
				OnChordChange();
				CalculateNextChordChange();
			}

			//Out time
			if (currentTime >= nextOutTime - trackPickupTime - BufferTime && !cuedOutTime) {
				cuedOutTime = true;
				QueOutTime(nextQuarterBeat - currentTime- BufferTime);

			} else if (currentTime >= nextOutTime) {
				OnOutTime();
				CalculateNextOutTime();
			}

			//End time
			if (currentTime >= endTime - trackPickupTime - BufferTime && !cuedEndTime) {
				cuedEndTime = true;
				QueEndTime(endTime - currentTime- BufferTime);

			} else if (currentTime >= endTime) {
				OnEndTime();
				CalculateEndTime();
			}

		}

	}
	
	//Play a music track
	public AudioSource StartMux(AudioClip clip, MuxBase newTrack, float delay = 0) {
		currentTrack = newTrack;
		currentSource = Instance.StartClip(clip, delay + BufferTime);

		startTime = Time.time + BufferTime;
		trackPickupTime = currentTrack.startTime;
		currentTime = 0f;
		playing = true;

		CalculateNextBeat();
		CalculateNextHalfBeat();
		CalculateNextQuarterBeat();
		CalculateNextChordChange();
		CalculateNextOutTime();
		CalculateEndTime();

		if (currentChordIndex >= 0) GenerateTuning(currentTrack.changes[0].tuning, currentTrack.changes[0].root);

		return currentSource;
	}

	private AudioSource StartClip(AudioClip clip, float delay = 0) {
		if (currentSource && endTime != nextOutTime && currentTime < endTime - trackPickupTime - BufferTime)
			StartCoroutine(WaitAndFadeOutAndStop(currentSource, delay + trackPickupTime, FadeTime));

		if (clip) {
			AudioSource freshAudioSource = Instantiate(audioSourcePrefabMusic);
			freshAudioSource.gameObject.transform.parent = gameObject.transform;
			freshAudioSource.gameObject.name = clip.name;
			freshAudioSource.clip = clip;

			freshAudioSource.PlayDelayed(delay);
			Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length);

			return freshAudioSource;
		} else return null;
	}

	//--//-----Music functions
	public void GenerateTuning(List<float> newTuning, float newRoot) {
		if (newTuning.Count <= 0) return;

		root = newRoot;

		tuning = new List<float>();
		for (int i = 0; i < newTuning.Count; i++) {
			if (newTuning[i] >= 12f) newTuning[i] -= 12f;
			if (newTuning[i] < 0f) newTuning[i] += 12f;
			tuning.Add(newTuning[i]);
		}

		tuning.Sort();
		tuning.Insert(0, tuning[tuning.Count - 1] - 12);
		tuning.Add(tuning[1] + 12);
	}

	//--//-----Calculate event times

	private void CalculateNextQuarterBeat() {
		if (!currentTrack || !playing) {
			currentTrack = null;
			playing = false;
			nextQuarterBeat = Mathf.Infinity;
			cuedQuarterBeat = false;
			return;
		}

		nextQuarterBeat = currentTime - (currentTime % (15f / currentTrack.bpm)) + (15f / currentTrack.bpm);
		cuedQuarterBeat = false;
	}

	private void CalculateNextHalfBeat() {
		if (!currentTrack || !playing) {
			currentTrack = null;
			playing = false;
			nextHalfBeat = Mathf.Infinity;
			cuedHalfBeat = false;
			return;
		}

		nextHalfBeat = currentTime - (currentTime % (30f / currentTrack.bpm)) + (30f / currentTrack.bpm);
		cuedHalfBeat = false;
	}

	private void CalculateNextBeat() {
		if (!currentTrack || !playing) {
			currentTrack = null;
			playing = false;
			nextBeat = Mathf.Infinity;
			cuedBeat = false;
			return;
		}

		nextBeat = currentTime  - (currentTime % (60f / currentTrack.bpm)) + (60f / currentTrack.bpm);
		cuedBeat = false;
	}

	private void CalculateNextChordChange() {
		if (!currentTrack || !playing) {
			currentTrack = null;
			playing = false;
			nextChordChange = Mathf.Infinity;
			cuedChordChange = false;
			return;
		}

		if (currentTrack.changes.Count == 0) {
			nextChordChange = Mathf.Infinity;
			currentChordIndex = -1;
			return;
		}

		bool changed = false;
		for (int i = currentTrack.changes.Count - 1; i >= 0; i--) {
			if (currentTrack.changes[i].time > currentTime) {
				nextChordChange = currentTrack.changes[i].time;
				currentChordIndex = i;
				changed = true;
			}
		}
		
		if (!changed) {
			nextChordChange = Mathf.Infinity;

			if (currentTime < endTime) currentChordIndex = currentTrack.changes.Count - 1;
			else currentChordIndex = -1;

		}

		cuedChordChange = false;
	}

	private void CalculateNextOutTime() {
		if (!currentTrack || !playing) {
			currentTrack = null;
			playing = false;
			nextOutTime = Mathf.Infinity;
			cuedOutTime = false;
			return;
		}
		
		if (currentTrack.outTimes.Count == 0) {
			nextOutTime = currentTrack.endTime;
			return;
		}
		
		bool changed = false;
		for (int i = currentTrack.outTimes.Count - 1; i >= 0; i--) {
			if (currentTrack.outTimes[i] > currentTime) {
				nextOutTime = currentTrack.outTimes[i];
				changed = true;
			}
		}
		if (!changed) {
			if (currentTime < currentTrack.endTime) nextOutTime = currentTrack.endTime;
			else nextOutTime = Mathf.Infinity;
		}

		cuedOutTime = false;
	}

	private void CalculateEndTime() {
		if (!currentTrack || !playing) {
			currentTrack = null;
			playing = false;
			endTime = Mathf.Infinity;
			cuedEndTime = false;
			return;
		}

		if (currentTime < currentTrack.endTime) {
			endTime = currentTrack.endTime;
		} else {
			playing = false;
			endTime = Mathf.Infinity;
			Debug.Log(currentTime);
		}
		cuedEndTime = false;
	}

	//--//-----Automation
	IEnumerator FadeIn(AudioSource source, float fadeTime) {
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

	IEnumerator WaitAndFadeIn(AudioSource source, float waitTime, float fadeTime) {
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

	IEnumerator FadeOut(AudioSource source, float fadeTime) {
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

	IEnumerator WaitAndFadeOut(AudioSource source, float waitTime, float fadeTime) {
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

	IEnumerator FadeOutAndStop(AudioSource source, float fadeTime) {
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

	IEnumerator WaitAndFadeOutAndStop(AudioSource source, float waitTime, float fadeTime) {
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

	IEnumerator FadeTo(AudioSource source, float newVolume, float fadeTime) {
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

	IEnumerator WaitAndFadeTo(AudioSource source, float waitTime, float newVolume, float fadeTime) {
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