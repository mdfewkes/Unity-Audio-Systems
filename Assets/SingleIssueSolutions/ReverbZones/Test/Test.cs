using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Test : MonoBehaviour
{
    //public bool isin = false;
    public AudioMixerGroup defaultGroup;

    void Update()
    {
        //isin = false;
        GetComponent<AudioSource>().outputAudioMixerGroup = defaultGroup;
        foreach (SIReverbZone zone in SIReverbZone.currentZones)
		{
            if (zone.IsOverlapping(gameObject))
			{
                //isin = true;
                GetComponent<AudioSource>().outputAudioMixerGroup = zone.group;
			}
        }
    }

	void OnMouseDown()
	{
        GetComponent<AudioSource>().Play();
	}
}
