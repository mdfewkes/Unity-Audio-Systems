using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SIVirtualAudioListener : MonoBehaviour
{
	public static AudioListener masterListener;
	public static List<SIVirtualAudioListener> Listeners = new List<SIVirtualAudioListener>();

	void OnEnable()
	{
		if (masterListener == null)
		{
			masterListener = (AudioListener)FindObjectOfType(typeof(AudioListener));
		}

		Listeners.Add(this);
	}

	void OnDisable()
	{
		Listeners.Remove(this);
	}
}
