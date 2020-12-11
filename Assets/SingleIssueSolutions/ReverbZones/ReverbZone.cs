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

	public virtual bool IsOverlapping(Vector3 target) { return true; }
	public virtual bool IsOverlapping(GameObject target) { return true; }

	public static void AssignOutputMixerGroupToAudioSource(AudioSource source)
	{
		foreach (ReverbZone zone in ReverbZone.currentZones)
		{
			if (zone.IsOverlapping(source.gameObject.transform.position))
			{
				source.outputAudioMixerGroup = zone.group;
			}
		}
	}

	public static void AssignOutputMixerGroupToAudioSource(AudioSource source, GameObject target)
	{
		foreach (ReverbZone zone in ReverbZone.currentZones)
		{
			if (zone.IsOverlapping(target.transform.position))
			{
				source.outputAudioMixerGroup = zone.group;
			}
		}
	}

	public static void AssignOutputMixerGroupToAudioSource(AudioSource source, Vector3 targetPosition)
	{
		foreach (ReverbZone zone in ReverbZone.currentZones)
		{
			if (zone.IsOverlapping(targetPosition))
			{
				source.outputAudioMixerGroup = zone.group;
			}
		}
	}
}
