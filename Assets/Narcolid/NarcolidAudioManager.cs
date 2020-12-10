using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarcolidAudioManager : MonoBehaviour
{

	public static NarcolidAudioManager Instance;
	public GameObject audioSourcePrefab;

	public List<float> tuning;
	public float root;

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

	public void GenerateTuning(List<float> newTuning, float newRoot)
	{
		if (newTuning.Count <= 0) return;

		root = newRoot;

		tuning = new List<float>();
		for (int i = 0; i < newTuning.Count; i++)
		{
			if (newTuning[i] > 12f) newTuning[i] -= 12f;
			if (newTuning[i] < 0f) newTuning[i] += 12f;
			tuning.Add(newTuning[i]);
		}

		tuning.Sort();
		tuning.Insert(0, tuning[tuning.Count - 1] - 12);
		tuning.Add(tuning[1] + 12);

	}

}
