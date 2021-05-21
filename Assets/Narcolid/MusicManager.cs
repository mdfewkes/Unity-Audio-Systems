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

	public AudioSource currentSource;
	public MuxBase currentTrack;
	public MuxBase scheduledTrack;
	
	public AudioClip clip;
	public float bpm = 120f;
	public float trackStartTime = 0f;
	public float trackEndTime = Mathf.Infinity;
	public bool loop = false;
	public List<float> outTimes = new List<float>();
	public List<ChordChange> changes = new List<ChordChange>();
	
	public float trackPickupTime = 0f;
	public int currentChordIndex = 0;

	public float startTime = 0f;
	public float currentTime = 0f;
	public float nextQuarterBeat = 0f;
	public float nextHalfBeat = 0f;
	public float nextBeat = 0f;
	public float nextChordChange = Mathf.Infinity;
	public float nextOutTime = Mathf.Infinity;
	public float endTime = Mathf.Infinity;

	public Action<float> CueBeat = (float delay) => { };
	public Action<float> CueHalfBeat = (float delay) => { };
	public Action<float> CueQuarterBeat = (float delay) => { };
	public Action<float> CueChordChange = (float delay) => { };
	public Action<float> CueOutTime = (float delay) => { };
	public Action<float> CueEndTime = (float delay) => { };

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

	void Awake() {
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	void Start() {
		CueChordChange += HandleChordChange;
		CueOutTime += HandleOutTime;
		CueEndTime += HandleEndTime;
	}

	void OnDestroy() {
		CueChordChange -= HandleChordChange;
		CueOutTime -= HandleOutTime;
		CueEndTime -= HandleEndTime;
	}

	void Update() {
		if (currentSource) currentTime = currentSource.time;
		else currentTime = Time.unscaledTime - startTime;

		//Quarter beat
		if (currentTime >= nextQuarterBeat - BufferTime/4 && !cuedQuarterBeat) {
			cuedQuarterBeat = true;
			CueQuarterBeat(nextQuarterBeat - currentTime);

		} else if (currentTime >= nextQuarterBeat) {
			OnQuarterBeat();
			CalculateNextQuarterBeat();
		}

		//Half beat
		if (currentTime >= nextHalfBeat - BufferTime/2 && !cuedHalfBeat) {
			cuedHalfBeat = true;
			CueHalfBeat(nextHalfBeat - currentTime);

		} else if (currentTime >= nextHalfBeat) {
			OnHalfBeat();
			CalculateNextHalfBeat();
		}

		//Beat
		if (currentTime >= nextBeat - BufferTime && !cuedBeat) {
			cuedBeat = true;
			CueBeat(nextBeat - currentTime);

		} else if (currentTime >= nextBeat) {
			OnBeat();
			CalculateNextBeat();
		}

		//Chord change
		if (currentTime >= nextChordChange - BufferTime && !cuedChordChange) {
			cuedChordChange = true;
			CueChordChange(nextChordChange - currentTime);

		} else if (currentTime >= nextChordChange) {
			OnChordChange();
			CalculateNextChordChange();
		}

		//Out time
		if (currentTime >= nextOutTime - trackPickupTime - BufferTime && !cuedOutTime) {
			cuedOutTime = true;
			CueOutTime(nextQuarterBeat - currentTime);

		} else if (currentTime >= nextOutTime) {
			OnOutTime();
			CalculateNextOutTime();
		}

		//End time
		if (currentTime >= endTime - trackPickupTime - BufferTime && !cuedEndTime) {
			cuedEndTime = true;
			CueEndTime(endTime - currentTime);

		} else if (currentTime >= endTime) {
			OnEndTime();
			CalculateEndTime();
		}

	}
	
	//MusicManager Interface
	public void StartMux(MuxBase newTrack, float delay = 0) {
		newTrack.PrepTrack();

		clip = newTrack.clip;
		bpm = newTrack.bpm;
		trackStartTime = newTrack.trackStartTime;
		trackEndTime = newTrack.trackEndTime;
		loop = newTrack.loop;
		outTimes = newTrack.outTimes;
		changes = newTrack.changes;

		currentSource = StartClip(delay);
		currentTrack = newTrack;
		scheduledTrack = null;
		trackPickupTime = trackStartTime;

		CalculateNextBeat();
		CalculateNextHalfBeat();
		CalculateNextQuarterBeat();
		CalculateNextChordChange();
		CalculateNextOutTime();
		CalculateEndTime();

		if (currentChordIndex >= 0 && changes.Count > 0) GenerateTuning(changes[0].tuning, changes[0].root);
	}

	public void ScheduleMux(MuxBase newTrack) {
		if (currentTrack) {
			scheduledTrack = newTrack;
			trackPickupTime = newTrack.trackStartTime;
		} else {
			StartMux(newTrack);
		}
	}

	private void CueMux(MuxBase newTrack, float delay = 0) {
		if (delay >= BufferTime) {
			StartMux(newTrack, delay - BufferTime);
		} else {
			StartMux(newTrack);
			StartCoroutine(FadeIn(currentSource, trackStartTime + delay));
			currentSource.time = trackStartTime + delay;
			startTime = Time.unscaledTime + delay + BufferTime;
		}
	}

	//--//-----Music functions
	public void GenerateTuning(List<float> newTuning, float newRoot) {
		if (newTuning.Count <= 0) return;

		root = newRoot;

		tuning = new List<float>();
		for (int i = 0; i < newTuning.Count; i++) {
			while (newTuning[i] >= 12f) newTuning[i] -= 12f;
			while (newTuning[i] < 0f) newTuning[i] += 12f;
			tuning.Add(newTuning[i]);
		}

		tuning.Sort();
		tuning.Insert(0, tuning[tuning.Count - 1] - 12f);
		tuning.Add(tuning[1] + 12f);
	}

	private AudioSource StartClip(float delay = 0) {
		if (currentSource && endTime != nextOutTime && currentTime < endTime - trackPickupTime - BufferTime)
			StartCoroutine(WaitAndFadeOutAndStop(currentSource, delay + trackPickupTime, FadeTime));

		startTime = Time.unscaledTime + delay + BufferTime;
		currentTime = 0f;

		if (clip) {
			AudioSource freshAudioSource = Instantiate(audioSourcePrefabMusic);
			freshAudioSource.gameObject.transform.parent = gameObject.transform;
			freshAudioSource.gameObject.name = clip.name;
			freshAudioSource.clip = clip;

			freshAudioSource.PlayDelayed(delay + BufferTime);
			Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length);

			return freshAudioSource;
		} else return null;
	}

	private void HandleChordChange(float cueTime) {
		if (currentChordIndex >= 0 && changes.Count > 0)
			StartCoroutine(WaitAndGenerateTuning(changes[currentChordIndex].tuning, changes[currentChordIndex].root, cueTime));
	}

	private void HandleOutTime(float cueTime) {
		if (scheduledTrack) {
			CueMux(scheduledTrack, cueTime - trackPickupTime);
			scheduledTrack = null;
		}
	}

	private void HandleEndTime(float cueTime) {
		StartMux(currentTrack);
		if (!loop) currentSource.volume = 0f;
	}

	//--//-----Calculate event times

	private void CalculateNextQuarterBeat() {
		nextQuarterBeat = currentTime - (currentTime % (15f / bpm)) + (15f / bpm);
		cuedQuarterBeat = false;
	}

	private void CalculateNextHalfBeat() {
		nextHalfBeat = currentTime - (currentTime % (30f / bpm)) + (30f / bpm);
		cuedHalfBeat = false;
	}

	private void CalculateNextBeat() {
		nextBeat = currentTime  - (currentTime % (60f / bpm)) + (60f / bpm);
		cuedBeat = false;
	}

	private void CalculateNextChordChange() {
		if (changes.Count == 0) {
			nextChordChange = Mathf.Infinity;
			currentChordIndex = -1;
			return;
		}

		bool changed = false;
		for (int i = changes.Count - 1; i >= 0; i--) {
			if (changes[i].time > currentTime) {
				nextChordChange = changes[i].time;
				currentChordIndex = i;
				changed = true;
			}
		}
		
		if (!changed) {
			nextChordChange = Mathf.Infinity;

			if (currentTime < endTime) currentChordIndex = changes.Count - 1;
			else currentChordIndex = -1;

		}

		cuedChordChange = false;
	}

	private void CalculateNextOutTime() {
		if (outTimes.Count == 0) {
			nextOutTime = trackEndTime;
			return;
		}
		
		bool changed = false;
		for (int i = outTimes.Count - 1; i >= 0; i--) {
			if (outTimes[i] > currentTime) {
				nextOutTime = outTimes[i];
				changed = true;
			}
		}
		if (!changed) {
			if (currentTime < trackEndTime) nextOutTime = trackEndTime;
			else nextOutTime = Mathf.Infinity;
		}

		cuedOutTime = false;
	}

	private void CalculateEndTime() {
		if (currentTime < trackEndTime) endTime = trackEndTime;
		else endTime = Mathf.Infinity;

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

		yield return new WaitForSecondsRealtime(waitTime);

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

		yield return new WaitForSecondsRealtime(waitTime);

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

		yield return new WaitForSecondsRealtime(waitTime);

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

		yield return new WaitForSecondsRealtime(waitTime);

		while (startTime + fadeTime > Time.unscaledTime) {
			currentTime = Time.unscaledTime - startTime;

			source.volume = Mathf.Lerp(startingVolume, newVolume, currentTime / fadeTime);
			yield return null;
		}

		source.volume = newVolume;
	}

	IEnumerator WaitAndGenerateTuning(List<float> newTuning, float newRoot, float waitTime) {
		yield return new WaitForSecondsRealtime(waitTime);

		GenerateTuning(newTuning, newRoot);
	}

}