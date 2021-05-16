using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
	public static MusicManager Instance;

	[SerializeField]
	private AudioSource musicSourcePrefab;
	
	[SerializeField]
	private MusicTrack currentTrack;
	private MusicTrack scheduledTrack;
	private AudioSource currentSource;
	private bool playing = false;
	
	public float trackFadeTime = 0.15f;
	public float trackBufferTime = 0.25f;
	public bool playOnAwake = true;
	public TrackState state = TrackState.Base;

	private float currentTime = 0f;
	private float nextBeat = 0f;
	private float nextOutTime = 0f;
	private float endTime = 0f;

	//--//--Unity monobehavior calls 
	void Awake() {
		//Set as singleton
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
		DontDestroyOnLoad(gameObject);

		//Play on awake
		if (playOnAwake && currentTrack != null) PlayTrack(currentTrack);
	}

	void Update() {
		currentTime = currentSource.time;

		if (playing) {
			if (scheduledTrack != null //A track is scheduled
				&& currentTime >= nextOutTime - scheduledTrack.startTime - trackBufferTime //And we are past the bufferTime+PickupTime for the next outTime
				&& currentTime + scheduledTrack.startTime <= nextOutTime) { //And the pickupTime is less than the remaining transition time

				//Assigne the new track
				currentTrack = scheduledTrack;
				scheduledTrack = null;

				//Schedule the FadeOut of the old music (if not at the end of the track) and the start of the new music
				if (endTime != nextOutTime) StartCoroutine(WaitAndFadeOutAndStop(currentSource, nextOutTime - currentTime, trackFadeTime));
				StartClip(currentTrack.GetClip(state), nextOutTime - currentTrack.startTime - currentTime);

			} else if (scheduledTrack != null //A track is scheduled
				&& currentTime >= nextOutTime - scheduledTrack.startTime - trackBufferTime //And we are past the bufferTime+PickupTime for the next outTime
				&& currentTime + scheduledTrack.startTime > nextOutTime) { //And the pickup time is more than the remaining transition time

				//Assigne the new track
				currentTrack = scheduledTrack;
				scheduledTrack = null;

				//Schedule the FadeOut of the old music and the start of the new music
				float timeDifference = nextOutTime - currentTime;
				StartCoroutine(WaitAndFadeOutAndStop(currentSource, nextOutTime - currentTime, trackFadeTime));
				StartClip(currentTrack.GetClip(state), nextOutTime - currentTrack.startTime - currentTime);

				//Truncate the start of the new music to fit in our transition time
				StartCoroutine(FadeIn(currentSource, timeDifference));
				currentSource.time = currentTrack.startTime - timeDifference;

			} else if (scheduledTrack == null //No track is scheduled
				&& currentTime >= endTime - currentTrack.startTime - trackBufferTime) { //And we are past the bufferTime+PickupTime to start the next track
				//Schedule the next clip
				StartClip(currentTrack.GetClip(state), endTime - currentTrack.startTime - currentTime);
			}

			if (currentTime >= nextOutTime) { //We have passed an outTime
				//Set the next outTime
				CalculateNextOutTime();
			}

			if (currentTime >= nextBeat - trackFadeTime - trackBufferTime) { //We are past the next beat-fadeTime
				//If the current music clip doesn't match the current music state, swap the currrent music clip
				if (currentTrack.GetClip(state) != currentSource.clip) {
					SwapClip(currentTrack.GetClip(state), nextBeat - trackFadeTime - currentTime);
				}
				//Set the next beat
				CalculateNextBeat();
			}
		}
	}

	//--//--Interface for music control
	public void PlayTrack(MusicTrack newTrack) {

		if (playing && currentTrack != newTrack) { //Schedule the new track
			scheduledTrack = newTrack;

		} else if (playing && currentTrack == newTrack) { //Ensure no track is scheduled if already playing the new track
			scheduledTrack = null;

		} else { //Start the track if no music is playing
			currentTrack = newTrack;
			StartClip(currentTrack.GetClip(state));
			playing = true;
		}
	}

	public void Stop() { //Quick fade current music and reset the manager
		StartCoroutine(FadeOutAndStop(currentSource, trackFadeTime));

		currentTrack = null;
		scheduledTrack = null;
		playing = false;

		currentTime = 0f;
		nextBeat = 0f;
		nextOutTime = 0f;
		endTime = 0f;

		state = TrackState.Base;
	}

	public void Panic() { //Hard stop any attached AudioSources and reset the manager
		AudioSource[] allSources = GetComponentsInChildren<AudioSource>();

		foreach (AudioSource source in allSources) {
			source.Stop();
			Destroy(source.gameObject);
		}

		currentTrack = null;
		scheduledTrack = null;
		playing = false;

		currentTime = 0f;
		nextBeat = 0f;
		nextOutTime = 0f;
		endTime = 0f;

		state = TrackState.Base;
	}

	//--//--Interface to set the music state (multiple methods to do the same job)
	public void SetState(TrackState newState) {
		state = newState;
	}

	public void SetState(float newState) {
		state = (TrackState)newState;
	}

	public void SetStateToBaseTrack() {
		state = TrackState.Base;
	}
	public void SetStateToLowHealth() {
		state = TrackState.LowHealth;
	}
	public void SetStateToHighIntensity() {
		state = TrackState.HighIntensity;
	}

	//--//--Manages AudioSource and GameObject creation
	private void StartClip(AudioClip clip, float delay = 0f) {
		//Create new AudioSource and GameObject from the prefab
		currentSource = Instantiate(musicSourcePrefab);
		currentSource.gameObject.transform.parent = transform;
		currentSource.gameObject.name = clip.name;
		
		//Assign the clip
		currentSource.clip = clip;

		//Begin playback and schedule cleanup
		currentSource.PlayDelayed(delay);
		Destroy(currentSource.gameObject, currentSource.clip.length);

		//Calculate event times
		CalculateNextBeat();
		CalculateNextOutTime();
		CalculateNextEndTime();
	}

	private void SwapClip(AudioClip clip, float delay = 0f) {
		//Save the current playback position and schedule FadeOut for the current music
		float seekTo = currentSource.time + delay;
		StartCoroutine(WaitAndFadeOutAndStop(currentSource, trackFadeTime, trackFadeTime));

		//Create new AudioSource and GameObject from the prefab
		currentSource = Instantiate(musicSourcePrefab);
		currentSource.gameObject.transform.parent = transform;
		currentSource.gameObject.name = clip.name;

		//Assign the clip and set the playback position
		currentSource.clip = clip;
		currentSource.time = seekTo;
		currentSource.volume = 0f;

		//Pickup playback and schedule cleanup
		currentSource.PlayDelayed(delay);
		StartCoroutine(WaitAndFadeIn(currentSource, delay, trackFadeTime));
		Destroy(currentSource.gameObject, currentSource.clip.length);
	}

	//--//--Calculate event times
	private void CalculateNextBeat() {
		//current music playback position - (seconds until next beat) + (beat length)
		nextBeat = currentSource.time - (currentSource.time % (60 / currentTrack.bpm)) + (60 / currentTrack.bpm);
	}

	private void CalculateNextOutTime() {
		//If there are no outTimes, assign the endTime as the next outTime and return
		if (currentTrack.outTimes.Count == 0) {
			nextOutTime = currentTrack.endTime;
			return;
		}

		//Itterate backwards over the outTimes to find the one that comes after our current playback time
		bool changed = false;
		for (int i = currentTrack.outTimes.Count - 1; i >= 0; i--) {
			if (currentTrack.outTimes[i] > currentSource.time) {
				nextOutTime = currentTrack.outTimes[i];
				changed = true;
			}
		}
		//If there is no outTime between now and the endTime, assign the endTime as the next outTime
		if (!changed) {
			nextOutTime = currentTrack.endTime;
		}
	}

	private void CalculateNextEndTime() {
		//Set endTime
		endTime = currentTrack.endTime;
	}

	//--//--Volume automation
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

	IEnumerator WaitAndFadeTo(AudioSource source, float waitTime, float newVolume,float fadeTime) {
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

	public enum TrackState {
		Base,
		LowHealth,
		HighIntensity
	}
}
