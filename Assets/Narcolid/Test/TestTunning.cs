using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTunning : MonoBehaviour
{
	public List<float> tunning;
	public float root;

	private void OnMouseDown()
	{
		NarcolidAudioManager.Instance.GenerateTuning(tunning, root);
	}
}
