using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class NarcolidAudioManager : MonoBehaviour
{

	public static NarcolidAudioManager Instance;
	public GameObject audioSourcePrefabSFX;
	public GameObject audioSourcePrefabUI;
	public GameObject audioSourcePrefabVO;

	public List<float> tuning;
	public float root;

	void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	//--//-----SFX Functions
	//Plays spatialized, in world sounds
	public AudioSource PlaySoundSFX(Vector3 positionToPlayAt, AudioClip clip, float pitch = 1f, float volume = 1f)
	{
		GameObject targetGameObject = Instantiate(audioSourcePrefabSFX);
		targetGameObject.GetComponent<VirtualAudioSource>().CalculateClosestListener(positionToPlayAt);
		AudioSource freshAudioSource = targetGameObject.GetComponent<AudioSource>();
		freshAudioSource.gameObject.transform.parent = gameObject.transform;
		freshAudioSource.volume = volume;
		freshAudioSource.pitch = pitch;
		freshAudioSource.clip = clip;

		ReverbZone.AssignOutputMixerGroupToAudioSource(freshAudioSource, positionToPlayAt);

		freshAudioSource.Play();
		Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length * pitch + 0.1f);

		return freshAudioSource;
	}

	public AudioSource PlaySoundSFX(GameObject objectToPlayOn, AudioClip clip, float pitch = 1f, float volume = 1f)
	{
		AudioSource freshAudioSource = PlaySoundSFX(objectToPlayOn.transform.position, clip, pitch, volume);
		freshAudioSource.gameObject.GetComponent<VirtualAudioSource>().target = objectToPlayOn;

		return freshAudioSource;
	}

	public AudioSource PlaySoundSFX(AudioClip clip, float pitch = 1f, float volume = 1f)
	{
		return PlaySoundSFX(gameObject.transform.position, clip, pitch, volume);
	}


	public AudioSource PlaySoundUI(AudioClip clip, float pitch = 1f, float volume = 1f)
	{
		GameObject targetGameObject = Instantiate(audioSourcePrefabUI);
		AudioSource freshAudioSource = targetGameObject.GetComponent<AudioSource>();
		freshAudioSource.gameObject.transform.parent = gameObject.transform;
		freshAudioSource.volume = volume;
		freshAudioSource.pitch = pitch;
		freshAudioSource.clip = clip;


		freshAudioSource.Play();
		Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length * pitch + 0.1f);

		return freshAudioSource;
	}

	public AudioSource PlaySoundVO(AudioClip clip)
	{
		GameObject targetGameObject = Instantiate(audioSourcePrefabVO);
		AudioSource freshAudioSource = targetGameObject.GetComponent<AudioSource>();
		freshAudioSource.gameObject.transform.parent = gameObject.transform;
		freshAudioSource.clip = clip;

		freshAudioSource.Play();
		Destroy(freshAudioSource.gameObject, freshAudioSource.clip.length + 0.1f);

		return freshAudioSource;
	}

	//--//-----Music Functions
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
