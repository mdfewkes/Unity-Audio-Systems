using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ReverbZone : MonoBehaviour
{
    public static List<ReverbZone> currentZones = new List<ReverbZone>();
	public AudioMixerGroup group;

	void OnEnable()
	{
		currentZones.Add(this);
	}

	void OnDisable()
	{
		currentZones.Remove(this);
	}

	public virtual bool IsOverlapping(GameObject target) { return false; }
}
