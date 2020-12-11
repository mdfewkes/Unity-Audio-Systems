using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
	public NarcolidSFXBase sound;

	private void OnMouseDown()
	{
		if (sound != null)
		{
			sound.Play(gameObject);
		}
	}
}
