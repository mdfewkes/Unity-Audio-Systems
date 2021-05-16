using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchesMusicManager : MonoBehaviour {

	public static PatchesMusicManager Instance;

	[SerializeField]
	private GameObject musicAudioSourcePrefab;
	
	public bool playMusicOnStart = true;
	private bool musicPlaying = false;
	private bool cueNewPool = false;
	private bool hardOut = false;
	
	private float nextEndTime = 0f;
	private float nextOutTime = 0f;
	private float nextBeat = 0f;

	[SerializeField]
	private PatchesMusicPool activePool;
	private PatchesMusicTrack activeTrack;
	private AudioSource activeSource;

	private Stack<PatchesMusicPool> musicStack = new Stack<PatchesMusicPool>();

	void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	void Start () {
		if (musicAudioSourcePrefab == null)
		{
			musicAudioSourcePrefab = new GameObject();
			musicAudioSourcePrefab.AddComponent<AudioSource>();
			musicAudioSourcePrefab.GetComponent<AudioSource>().spatialBlend = 0;
			musicAudioSourcePrefab.GetComponent<AudioSource>().playOnAwake = false;
		}

		if (activePool != null) musicStack.Push(activePool);

		if (playMusicOnStart && activePool) {
			StartMusic();
		}
	}
	
	void Update () {
		if (!musicPlaying) return;

		if (Time.time >= nextEndTime) {
			if (activeTrack.looping) {
				PatchesMusicTrack newTrack = ReturnNewTrack();
				if (newTrack == activeTrack) {
					SetNextEndTime();
				} else {
					FadeCurrentTrack();
					PlayNewTrack(newTrack);
				}
			} else {
				float destroyTime = activeSource.clip.length - activeSource.time + Time.deltaTime;
				Destroy(activeSource.gameObject, destroyTime);
				PlayNewTrack();
			}
		}
		if (Time.time >= nextOutTime) {
			if (cueNewPool) {
				FadeCurrentTrack();
				PlayNewTrack();
			} else {
				SetNextOutTime();
			}
		}
		if (Time.time >= nextBeat) {
			if (hardOut) {
				FadeCurrentTrack();
				PlayNewTrack();
				hardOut = false;
			} else {
				SetNextBeat();
			}
		}
	}

	public void StartMusic() {
		if (!musicPlaying) {
			PlayNewTrack();
			musicPlaying = true;
		}
	}

	public void StopMusic() {
		if (musicPlaying) {
			FadeCurrentTrack();
			musicPlaying = false;
		}
	}

	public void FadeOutMusic(float fadeTime) {
		if (musicPlaying) {
			StartCoroutine(FadeOutAndStop(activeSource, fadeTime));
			musicPlaying = false;
		}
	}

	public void PushMusicPool(PatchesMusicPool newPool) {
		musicStack.Push(newPool);
		activePool = musicStack.Peek();
		cueNewPool = true;
	}

	public void PopMusicPool() {
		if (musicStack.Count > 1) {
			musicStack.Pop();
			activePool = musicStack.Peek();
			cueNewPool = true;
		}
	}

	public void TransitionMusicNow() {
		hardOut = true;
	}

	private void PlayNewTrack(PatchesMusicTrack newTrack) {
		activeTrack = newTrack;

		AudioSource freshMusicSource = Instantiate(musicAudioSourcePrefab).GetComponent<AudioSource>();
		freshMusicSource.gameObject.transform.parent = gameObject.transform;
		freshMusicSource.clip = activeTrack.musicStem;
		activeSource = freshMusicSource;
		if (activeTrack.looping) activeSource.loop = true;

		StartCoroutine(SetTimesWhenLoaded());

		activeSource.Play();

		cueNewPool = false;
	}

	private void PlayNewTrack() {
		activeTrack = ReturnNewTrack();

		AudioSource freshMusicSource = Instantiate(musicAudioSourcePrefab).GetComponent<AudioSource>();
		freshMusicSource.gameObject.transform.parent = gameObject.transform;
		freshMusicSource.clip = activeTrack.musicStem;
		activeSource = freshMusicSource;
		if (activeTrack.looping) activeSource.loop = true;
		
		StartCoroutine(SetTimesWhenLoaded());

		activeSource.Play();

		cueNewPool = false;
	}

	private void FadeCurrentTrack() {
		StartCoroutine(FadeOutAndStop(activeSource, activeTrack.fadeTime));
	}

	private PatchesMusicTrack ReturnNewTrack() {
		return activePool.musicStems[Random.Range(0,activePool.musicStems.Length)];
	}

	private void SetNextEndTime() {
		if (activeTrack.looping)
			nextEndTime = activeSource.clip.length - activeSource.time + Time.time - Time.deltaTime;
		else
			nextEndTime = activeTrack.endTime - activeSource.time + Time.time - Time.deltaTime;
	}

	private void SetNextOutTime() {
		bool changed = false;
		for (int i = activeTrack.outTimes.Length - 1; i >= 0; i--) {
			if (activeTrack.outTimes[i] > activeSource.time) {
				nextOutTime = activeTrack.outTimes[i] - activeSource.time + Time.time - Time.deltaTime;
				changed = true;
			}
		}
		if (!changed) {
			nextOutTime = activeSource.clip.length + Time.time;
		}
	}

	private void SetNextBeat() {
		nextBeat = activeSource.time % (60 / activeTrack.bpm) + Time.time + (60 / activeTrack.bpm) - Time.deltaTime;
	}

	IEnumerator FadeOutAndStop(AudioSource source, float fadeTime) {
		float startTime = Time.time;
		float currentTime = 0f;
		float startVolume = source.volume;

		while (startTime + fadeTime > Time.time) {
			currentTime = Time.time - startTime;

			source.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeTime);
			yield return null;
		}

		source.Stop();
		Destroy(source.gameObject);

	}

	IEnumerator FadeOut(AudioSource source, float fadeTime) {
		float startTime = Time.time;
		float currentTime = 0f;
		float startVolume = source.volume;

		while (startTime + fadeTime > Time.time) {
			currentTime = Time.time - startTime;

			source.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeTime);
			yield return null;
		}

		source.volume = 0f;

	}

	IEnumerator FadeIn(AudioSource source, float fadeTime) {
		float startTime = Time.time;
		float currentTime = 0f;
		float startVolume = source.volume;

		while (startTime + fadeTime > Time.time) {
			currentTime = Time.time - startTime;

			source.volume = Mathf.Lerp(startVolume, 1f, currentTime / fadeTime);
			yield return null;
		}

		source.volume = 1f;

	}

	IEnumerator FadeTo(AudioSource source, float newVolume, float fadeTime) {
		float startTime = Time.time;
		float currentTime = 0f;
		float startVolume = source.volume;

		while (startTime + fadeTime > Time.time) {
			currentTime = Time.time - startTime;

			source.volume = Mathf.Lerp(startVolume, newVolume, currentTime / fadeTime);
			yield return null;
		}
	}

	IEnumerator SetTimesWhenLoaded(){
		while (activeSource.clip.loadState != AudioDataLoadState.Loaded) {
			yield return null;
		}
		
		SetNextEndTime();
		SetNextOutTime();
		SetNextBeat();
	}
}
