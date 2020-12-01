using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Instrument", menuName = "Instrument")]
public class Instrument : ScriptableObject
{
	[Serializable]
    public class Note
	{
		public float lowKey = 0f;
		public float highKey = 127f;
		public float rootKey = 60;
		public AudioClip clip;
	}

	[SerializeField]private List<Note> map = new List<Note>();

	public void Play(double startTime, float volume, float MidiNoteNumber, float duration)
	{
		if (map.Count == 0) return;

		int noteToMap = 0;

		for (int i = 0; i < map.Count; i++)
		{
			if (MidiNoteNumber < map[i].lowKey) continue;
			if (MidiNoteNumber > map[i].highKey) continue;
			if (map[i].clip == null) continue;

			noteToMap = i;

			break;
		}

		Note thisNote = map[noteToMap];

		MusicSequencer.Instance.Play(startTime, thisNote.clip, volume, MusicMathUtils.MidiNoteToPitch(MidiNoteNumber, thisNote.rootKey), duration);
	}
}
