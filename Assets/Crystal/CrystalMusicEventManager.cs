using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalMusicEventManager : MonoBehaviour
{



	public static CrystalMusicEventManager Instance;
	[SerializeField]
	public GameObject audioSourcePrefab;
	
	private int minLayers = 2;
	private int maxLayers = 4;
	public int runningLayers = 2;


	[SerializeField]
	private float variationFrequencyInSeconds = 20f;
	private float nextVariationTime = 0f;
	[SerializeField]
	private float variationFadeTimeInSeconds = 0.25f;

	[SerializeField]
	private float transitionFadeTimeInSeconds = 0.5f;

	[SerializeField]
	private CrystalMusicTrack currentTrack;
	private List<AudioSource> currentSources = new List<AudioSource>();
	private int currentListLenght = 0;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		if (currentTrack != null)
		{
			StartTrack();
		}

	}

	void Update()
	{
		if (Time.time > nextVariationTime && currentTrack != null)
		{
			Variation();
		}
	}

	public void Transition(CrystalMusicTrack newTrack)
	{
		if (currentTrack != null && newTrack != null)
		{
			minLayers = newTrack.minTracks;
			maxLayers = newTrack.maxTracks;

			StartCoroutine(TransitionTimer(newTrack));
		}
		else if (currentTrack == null && newTrack != null)
		{
			currentTrack = newTrack;

			minLayers = currentTrack.minTracks;
			maxLayers = currentTrack.maxTracks;

			StartTrack();
		}
		else if (currentTrack != null && newTrack == null)
		{
			for (int i = 0; i < currentListLenght; i++)
			{
				StartCoroutine(FadeOutAndStop(currentSources[i], transitionFadeTimeInSeconds));
			}

			currentTrack = null;
		}
	}

	public void Variation()
	{
		nextVariationTime = Time.time + variationFrequencyInSeconds + SecondsToNextBeat() - variationFadeTimeInSeconds;
		int bringIn = FindUnusedNumber();
		int bringOut = FindUsedNumber();
		int running = TracksRunning();

		SetLayers(runningLayers);

		if (running < runningLayers)
		{
			StartCoroutine(FadeIn(currentSources[bringIn], variationFadeTimeInSeconds));
			currentSources[bringIn].gameObject.name = "(On***) " + currentSources[bringIn].clip.name;
		}
		else if (running > runningLayers)
		{
			StartCoroutine(FadeOut(currentSources[bringOut], variationFadeTimeInSeconds));
			currentSources[bringOut].gameObject.name = "(Off) " + currentSources[bringOut].clip.name;
		}
		else
		{
			StartCoroutine(FadeIn(currentSources[bringIn], variationFadeTimeInSeconds));
			currentSources[bringIn].gameObject.name = "(On**) " + currentSources[bringIn].clip.name;
			StartCoroutine(FadeOut(currentSources[bringOut], variationFadeTimeInSeconds));
			currentSources[bringOut].gameObject.name = "(Off) " + currentSources[bringOut].clip.name;
		}

	}

	private void StartTrack()
	{
		currentSources.Clear();
		currentListLenght = currentTrack.musicStems.Length;
		for (int i = 0; i < currentListLenght; i++)
		{
			AudioSource freshMusicSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
			freshMusicSource.gameObject.transform.parent = gameObject.transform;
			freshMusicSource.clip = currentTrack.musicStems[i];
			freshMusicSource.volume = 0f;
			freshMusicSource.Play();

			currentSources.Add(freshMusicSource);
			currentSources[i].gameObject.name = "(Off) " + currentSources[i].clip.name;
		}

		SetLayers(runningLayers);

		for (int i = 0; i < runningLayers; i++)
		{
			int bringIn = FindUnusedNumber();
			currentSources[bringIn].volume = 1f;
			currentSources[bringIn].gameObject.name = "(On*) " + currentSources[i].clip.name;
		}

		nextVariationTime = Time.time + variationFrequencyInSeconds + SecondsToNextBeat() - variationFadeTimeInSeconds;

	}

	private int FindUnusedNumber()
	{
		int unusedNumber = Random.Range(0, currentListLenght);
		int exitCondition = currentListLenght;
		
		while (currentSources[unusedNumber].volume > 0.9f && exitCondition > 0)
		{
			unusedNumber++;
			exitCondition--;
			if (unusedNumber >= currentListLenght) unusedNumber = 0;
		}
		
		return unusedNumber;
	}

	private int FindUsedNumber()
	{
		int usedNumber = Random.Range(0, currentListLenght);
		int exitCondition = currentListLenght;

		while (currentSources[usedNumber].volume < 0.1f && exitCondition > 0)
		{
			usedNumber++;
			exitCondition--;
			if (usedNumber >= currentListLenght) usedNumber = 0;
		}

		return usedNumber;
	}

	private float SecondsToNextBeat()
	{
		if (currentTrack == null) return 0;
		return currentSources[0].time % (60 / currentTrack.BPM);
	}

	private int TracksRunning()
	{
		int running = 0;
		for (int i = 0; i < currentListLenght; i++)
		{
			if (currentSources[i].volume == 1f) running++;
		}

		return running;
	}

	public int IncreaseLayers()
	{
		runningLayers++;
		if (runningLayers > maxLayers) runningLayers = maxLayers;

		return runningLayers;
	}

	public int DecreaseLayers()
	{
		runningLayers--;
		if (runningLayers < minLayers) runningLayers = minLayers;

		return runningLayers;
	}

	public int SetLayers(int numberOfLayers)
	{
		runningLayers = numberOfLayers;
		if (runningLayers > maxLayers)
		{
			runningLayers = maxLayers;
		}
		if (runningLayers < minLayers)
		{
			runningLayers = minLayers;
		}

		return runningLayers;
	}

	public void SetSecondsToTransition(float seconds)
	{
		variationFrequencyInSeconds = seconds;
	}

	IEnumerator FadeOutAndStop(AudioSource source, float fadeTime)
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

		source.volume = 0f;

	}

	IEnumerator FadeIn(AudioSource source, float fadeTime)
	{
		float startTime = Time.time;
		float currentTime = 0f;
		float startVolume = source.volume;

		while (startTime + fadeTime > Time.time)
		{
			currentTime = Time.time - startTime;

			source.volume = Mathf.Lerp(startVolume, 1f, currentTime / fadeTime);
			yield return null;
		}

		source.volume = 1f;

	}

	IEnumerator TransitionTimer(CrystalMusicTrack newTrack)
	{
		float goTime = Time.time + SecondsToNextBeat();

		while (Time.time < goTime)
		{
			yield return null;
		}

		for (int i = 0; i < currentListLenght; i++)
		{
			StartCoroutine(FadeOutAndStop(currentSources[i], transitionFadeTimeInSeconds));
			currentSources[i].gameObject.name = "(Old) " + currentSources[i].clip.name;
		}

		currentTrack = newTrack;
		StartTrack();

	}
}