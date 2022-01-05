using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchesMusicRegion : MonoBehaviour {
	
	public bool startMusicOnTriggerEnter = false;
	public bool stopMusicOnTriggerExit = false;
	public float fadeTime = 2f;

	public bool hardOutOnTriggerEnter = false;
	public bool hardOutOnTriggerExit = false;

	public PatchesMusicPool pool;
	public PatchesMusicPool.Track[] tracks = new PatchesMusicPool.Track[0];

	public LayerMask layers;

	private void Start()
	{
		if (pool == null && tracks.Length == 0)
		{
			gameObject.SetActive(false);
		}
		else if (pool == null)
		{
			pool = new PatchesMusicPool();
			pool.musicStems = new PatchesMusicPool.Track[tracks.Length];
			pool.musicStems = tracks;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (0 != (layers.value & 1 << other.gameObject.layer)) {
			PatchesMusicManager.Instance.PushMusicPool(pool);
			if (startMusicOnTriggerEnter) PatchesMusicManager.Instance.StartMusic();
			if (hardOutOnTriggerEnter) PatchesMusicManager.Instance.TransitionMusicNow();
		}
	}

	void OnTriggerExit(Collider other) {
		if (0 != (layers.value & 1 << other.gameObject.layer))
			PatchesMusicManager.Instance.PopMusicPool();
		if (stopMusicOnTriggerExit) PatchesMusicManager.Instance.FadeOutMusic(fadeTime);
		if (hardOutOnTriggerExit) PatchesMusicManager.Instance.TransitionMusicNow();
	}
}
