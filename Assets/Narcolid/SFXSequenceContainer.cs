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

	public override AudioSource Play(GameObject target, float delay = 0) {
		GameObject freshGameObject = new GameObject();
		AudioSource freshAudioSource = freshGameObject.AddComponent<SFXSequenceComponent>().Play(target, steps, loop, delay:delay);
		return freshAudioSource;
	}
	public override AudioSource Play(Vector3 target, float delay = 0) {
		GameObject freshGameObject = new GameObject();
		freshGameObject.transform.position = target;
		return Play(freshGameObject, delay: delay);
	}
	public override AudioSource Play(float delay = 0) {
		return Play(Camera.main.transform.position, delay:delay);
	}
}

public class SFXSequenceComponent : MonoBehaviour {
	public AudioSource source;
	public GameObject target;
	public List<SFXSequenceContainer.StepList> steps;
	public int index;
	public bool loop;

	void Update() {
		if (source.time > steps[index].stepTime - Time.deltaTime) {
			index++;
			if (index < steps.Count) {
				source = steps[index].sfx.Play(target);
				transform.parent = source.transform;
			} else if (loop) {
				Play(target, steps, loop);
			} else {
				Destroy(gameObject);
			}
		}
	}

	public AudioSource Play(GameObject targetGameObject, List<SFXSequenceContainer.StepList> stepList, bool looping, float delay = 0) {
		steps = stepList;
		target = targetGameObject;
		index = 0;
		loop = looping;
		source = steps[index].sfx.Play(target, delay: delay);
		transform.parent = source.transform;
		return source;
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