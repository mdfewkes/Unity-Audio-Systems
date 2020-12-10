using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

	public static AudioManager Instance;
	public GameObject audioSourcePrefab;

	public List<int> tuning;
	public int root;

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

	//--// SFX Functions


	public AudioSource PlaySoundSFX(AudioClip clip, float pitch = 1f)
	{
		AudioSource freshAudioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
		freshAudioSource.gameObject.transform.parent = gameObject.transform;
		freshAudioSource.pitch = pitch;
		freshAudioSource.clip = clip;
		freshAudioSource.Play();
		
		Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length);

		return freshAudioSource;
	}

	//--// Music Functions

	public void GenerateTuning(List<int> newTuning, int newRoot)
	{
		if (newTuning.Count <= 0) return;

		root = newRoot;

		tuning = new List<int>();
		foreach (int value in newTuning)
		{
			tuning.Add(value);
		}

		tuning.Sort();
		tuning.Insert(0, tuning[tuning.Count - 1] - 1200);
		tuning.Add(tuning[1] + 1200);

	}

}
