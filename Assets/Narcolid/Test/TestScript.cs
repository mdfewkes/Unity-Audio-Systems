using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
	public SFXBase sound;

	private void OnMouseDown()
	{
		if (sound != null)
		{
			sound.Play();
		}
	}
}
