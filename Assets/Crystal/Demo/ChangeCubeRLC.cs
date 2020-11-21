using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCubeRLC : MonoBehaviour {

	public CrystalMusicTrack track;

	public void OnMouseDown() {
		CrystalMusicEventManager.Instance.Transition(track);
	}

}
