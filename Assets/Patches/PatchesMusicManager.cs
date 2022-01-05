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
	
	private double nextEndTime = 0.0;
	private double nextOutTime = 0.0;
	private double nextBeat = 0.0;
	private double currentTime = 0.0;

	private double lookAhead = 0.25;

	[SerializeField]
	private PatchesMusicPool activePool;
	private PatchesMusicPool.Track activeTrack;
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
		currentTime = AudioSettings.dspTime;

		if (currentTime >= nextEndTime - lookAhead) {
			if (activeTrack.loopingAsset) {
				PatchesMusicPool.Track newTrack = ReturnNewTrack();
				if (newTrack == activeTrack) {
					SetNextEndTime();
				} else {
					FadeCurrentTrack(nextEndTime - currentTime);
					PlayNewTrack(newTrack, nextEndTime - currentTime);
					SetNextEndTime();
				}
			} else {
				float destroyTime = activeSource.clip.length - activeSource.time + Time.deltaTime;
				Destroy(activeSource.gameObject, destroyTime);
				PlayNewTrack(nextEndTime - currentTime);
				SetNextEndTime();
			}
		}
		if (currentTime >= nextOutTime - lookAhead) {
			if (cueNewPool) {
				FadeCurrentTrack(nextOutTime - currentTime);
				PlayNewTrack(nextOutTime - currentTime);
				SetNextOutTime();
			} else {
				SetNextOutTime();
			}
		}
		if (currentTime >= nextBeat - lookAhead) {
			if (hardOut) {
				FadeCurrentTrack(nextBeat - currentTime);
				PlayNewTrack(nextBeat - currentTime);
				hardOut = false;
				SetNextBeat();
			} else {
				SetNextBeat();
			}
		}

	}

	public void StartMusic() {
		if (!musicPlaying) {
			PlayNewTrack(0.0);
			musicPlaying = true;
		}
	}

	public void StopMusic() {
		if (musicPlaying) {
			FadeCurrentTrack(0.0);
			musicPlaying = false;
		}
	}

	public void FadeOutMusic(float fadeTime = 0.15f) {
		if (musicPlaying) {
			StartCoroutine(WaitAndFadeOutAndStop(activeSource, 0f, fadeTime));
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

	private void PlayNewTrack(PatchesMusicPool.Track newTrack, double waitTime) {
		activeTrack = newTrack;

		AudioSource freshMusicSource = Instantiate(musicAudioSourcePrefab).GetComponent<AudioSource>();
		freshMusicSource.gameObject.transform.parent = gameObject.transform;
		freshMusicSource.gameObject.name = activeTrack.musicStem.name;
		freshMusicSource.clip = activeTrack.musicStem;
		activeSource = freshMusicSource;
		if (activeTrack.loopingAsset) activeSource.loop = true;

		activeSource.PlayScheduled(AudioSettings.dspTime + waitTime);

		StartCoroutine(SetTimesWhenLoaded((float)waitTime));

		cueNewPool = false;
	}

	private void PlayNewTrack(double waitTime) {
		activeTrack = ReturnNewTrack();

		AudioSource freshMusicSource = Instantiate(musicAudioSourcePrefab).GetComponent<AudioSource>();
		freshMusicSource.gameObject.transform.parent = gameObject.transform;
		freshMusicSource.gameObject.name = activeTrack.musicStem.name;
		freshMusicSource.clip = activeTrack.musicStem;
		activeSource = freshMusicSource;
		if (activeTrack.loopingAsset) activeSource.loop = true;

		activeSource.PlayScheduled(AudioSettings.dspTime + waitTime);

		StartCoroutine(SetTimesWhenLoaded((float)waitTime));

		cueNewPool = false;
	}

	private void FadeCurrentTrack(double waitTime) {
		StartCoroutine(WaitAndFadeOutAndStop(activeSource, (float)waitTime, activeTrack.fadeTime));
	}

	private PatchesMusicPool.Track ReturnNewTrack() {
		return activePool.musicStems[Random.Range(0,activePool.musicStems.Length)];
	}

	private void SetNextEndTime() {
		if (activeTrack.loopingAsset)
			nextEndTime = ((double)(activeSource.clip.samples - activeSource.timeSamples) / (double)activeSource.clip.frequency) + AudioSettings.dspTime;
		else
			nextEndTime = (double)activeTrack.endTime - ((double)activeSource.timeSamples / (double)activeSource.clip.frequency) + AudioSettings.dspTime;
	}

	private void SetNextOutTime() {
		bool changed = false;
		for (int i = activeTrack.outTimes.Length - 1; i >= 0; i--) {
			if (activeTrack.outTimes[i] > activeSource.time + lookAhead) {
				nextOutTime = (double)activeTrack.outTimes[i] - ((double)activeSource.timeSamples / (double)activeSource.clip.frequency) + AudioSettings.dspTime;
				changed = true;
			}
		}
		if (!changed) {
			nextOutTime = ((double)activeSource.clip.samples / (double)activeSource.clip.frequency) + AudioSettings.dspTime;
		}
	}

	private void SetNextBeat() {
		if (60f / activeTrack.bpm <= lookAhead) activeTrack.bpm /= 2f;
		nextBeat = (activeSource.time + lookAhead) % (60f / activeTrack.bpm) + AudioSettings.dspTime + (60f / activeTrack.bpm);
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

	IEnumerator SetTimesWhenLoaded(float waitTime) {
		float startTime = Time.unscaledTime + waitTime;

		while (startTime > Time.unscaledTime) {
			yield return null;
		}

		while (activeSource.clip.loadState != AudioDataLoadState.Loaded) {
			yield return null;
		}
		
		SetNextEndTime();
		SetNextOutTime();
		SetNextBeat();
	}
}
