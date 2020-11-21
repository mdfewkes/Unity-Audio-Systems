using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTunning : MonoBehaviour
{
	public List<int> tunning;
	public int root;

	private void OnMouseDown()
	{
		AudioManager.Instance.GenerateTuning(tunning, root);
	}
}
