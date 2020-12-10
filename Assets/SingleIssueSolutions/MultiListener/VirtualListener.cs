using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualListener : MonoBehaviour
{
	public static AudioListener masterListener;
	public static List<VirtualListener> Listeners = new List<VirtualListener>();

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
