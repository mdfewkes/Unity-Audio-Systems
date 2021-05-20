using System.Collections.Generic;
using UnityEngine;

public class VirtualAudioSource : MonoBehaviour {
	public GameObject target;
	public Vector3 targetPosition;

	/// <summary>
	/// Current listener for the sound.
	/// </summary>
	public VirtualAudioListener listener;

	private AudioSource source;

	public void Awake() {
		source = GetComponent<AudioSource>();
		if (!listener) listener = GetClosestListener();
	}

	void LateUpdate() {
		if (target) targetPosition = target.transform.position;

		if (!listener) {
			source.mute = true;
			return;
		} else {
			source.mute = false;
			transform.position =
				Quaternion.Inverse(listener.transform.rotation) * (targetPosition - listener.transform.position) +
				VirtualAudioListener.masterListener.transform.position;
		}
	}

	public VirtualAudioListener GetClosestListener() {
		int nearestIndex = -1;
		float shortestDistance = Mathf.Infinity;
		for (int i = 0; i < VirtualAudioListener.Listeners.Count; i++) {
			var dist = (targetPosition - VirtualAudioListener.Listeners[i].transform.position).sqrMagnitude;
			if (dist < shortestDistance) {
				nearestIndex = i;
				shortestDistance = dist;
			}
		}

		if (nearestIndex < 0) return null;
		else return VirtualAudioListener.Listeners[nearestIndex];
	}
}