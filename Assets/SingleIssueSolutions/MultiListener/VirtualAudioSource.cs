using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualAudioSource : MonoBehaviour
{
	public GameObject target;
	public VirtualListener listener;

	void LateUpdate()
	{
		if (target == null) gameObject.SetActive(false);
		if (listener == null) listener = GetClosestListener();

			transform.position = Quaternion.Inverse(listener.transform.rotation) * (target.transform.position - listener.transform.position) + VirtualListener.masterListener.transform.position;
	}

	public VirtualListener GetClosestListener()
	{
		int indexToReturn = 0;
		float shortestDistance = Mathf.Infinity;
		for (int i = 0; i < VirtualListener.Listeners.Count; i++)
		{
			if ((target.transform.position - VirtualListener.Listeners[i].transform.position).sqrMagnitude < shortestDistance)
			{
				indexToReturn = i;
				shortestDistance = (target.transform.position - VirtualListener.Listeners[i].transform.position).sqrMagnitude;
			}
		}

		return VirtualListener.Listeners[indexToReturn];
	}

	public void CalculateClosestListener()
	{
		GetClosestListener();
	}
}
