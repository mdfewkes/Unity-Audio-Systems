using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "MusicSequence", menuName = "Audio/MusicSequence")]
public class MusicSequence : ScriptableObject {
	public List<MusicSequenceEvent> sequence = new List<MusicSequenceEvent>();

}

[System.Serializable]
public class MusicSequenceEvent {
	public MusicEventType type;
	public float time;
	public AudioClip track;
	public List<float> tuning;
	public float root;

	public void TriggerEvent() {
		switch (type) {
			case MusicEventType.PlayTrack:
				//MusicManager.Instance.PlaySoundMusic(track);
				break;
			case MusicEventType.ChangeChord:
				//MusicManager.Instance.GenerateTuning(tuning, root);
				break;
			case MusicEventType.Repeat:
				//MusicManager.Instance.RepeatMusicSequence();
				break;
		}
	}
	
	public enum MusicEventType {
		PlayTrack,
		ChangeChord,
		Repeat
	}
}

[CustomPropertyDrawer(typeof(MusicSequenceEvent))]
public class MusicSequenceEventDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		//base.OnGUI(position, property, label);
		EditorGUI.BeginProperty(position, label, property);
		
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		
		var typeRect = new Rect(position.x, position.y, position.width, 20);
		var timeRect = new Rect(position.x, position.y + 20, position.width, 20);
		var trackRect = new Rect(position.x, position.y + 40, position.width, 20);
		var tuningRect = new Rect(position.x, position.y + 40, position.width, 20);
		int tuningMod = property.FindPropertyRelative("tuning").arraySize * 20;
		var rootRect = new Rect(position.x, position.y + 80 + tuningMod, position.width, 20);
		
		
		EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);
		EditorGUI.PropertyField(timeRect, property.FindPropertyRelative("time"), new GUIContent("Time"));
		switch (property.FindPropertyRelative("type").enumValueIndex) {
			case 0:
				EditorGUI.PropertyField(trackRect, property.FindPropertyRelative("track"), GUIContent.none);
				break;
			case 1:
				EditorGUI.PropertyField(tuningRect, property.FindPropertyRelative("tuning"), new GUIContent("Tunning"), true);
				EditorGUI.PropertyField(rootRect, property.FindPropertyRelative("root"), new GUIContent("Root"));
				break;
		}
		

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		float height = 40f;
		switch (property.FindPropertyRelative("type").enumValueIndex) {
			case 0:
				height = 60f;
				break;
			case 1:
				int tuningMod = property.FindPropertyRelative("tuning").arraySize * 20;
				height = 100f + tuningMod;
				break;
		}

		return height;
	}
}