using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeSpaceNP : MonoBehaviour
{
	public GameObject player;
	public GameObject target;

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == player)
		{
			player.transform.position = target.transform.position;
			PatchesMusicManager.Instance.TransitionMusicNow();
		}
	}


}
