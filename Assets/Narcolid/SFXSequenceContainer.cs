using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "SFXSequenceContainer", menuName = "Audio/SFXSequenceContainer")]
public class SFXSequenceContainer : SFXBase {
	[System.Serializable]
	public class StepList {
		public SFXBase sfx;
		public float stepTime;
	}
	public List<StepList> steps;

	public override AudioSource Play(GameObject target) {
		GameObject freshGameObject = new GameObject();
		AudioSource freshAudioSource = freshGameObject.AddComponent<SFXSequenceComponent>().Play(target, steps);
		return freshAudioSource;
	}
	public override AudioSource Play(Vector3 target) {
		GameObject freshGameObject = new GameObject();
		freshGameObject.transform.position = target;
		return Play(freshGameObject);
	}
	public override AudioSource Play() {
		return Play(Camera.main.transform.position);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SFXSequenceContainer))]
[CanEditMultipleObjects]
public class SFXSequenceContainerEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (!EditorApplication.isPlaying) return;

		if (GUILayout.Button("Play Sound")) {
			(target as SFXSequenceContainer).Play();
		}
	}
}
#endif

public class SFXSequenceComponent : MonoBehaviour {
	public List<SFXSequenceContainer.StepList> steps;
	public GameObject target;
	public AudioSource source;
	public int index;

	void Update() {
		if (source.time > steps[index].stepTime - Time.deltaTime) {
			index++;
			if (index < steps.Count) {
				source = steps[index].sfx.Play(target);
				transform.parent = source.transform;
			} else {
				Destroy(gameObject);
			}
		}
	}

	public AudioSource Play(GameObject targetGameObject, List<SFXSequenceContainer.StepList> stepList) {
		steps = stepList;
		target = targetGameObject;
		index = 0;
		source = steps[index].sfx.Play(target);
		transform.parent = source.transform;
		return source;
	}
}