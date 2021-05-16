using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanciateTest : MonoBehaviour {
	public AudioSource toInstanciate;

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			AudioSource newSource = Instantiate(toInstanciate);
			newSource.time = toInstanciate.time;
			newSource.panStereo = 1f;
		}
	}
}
