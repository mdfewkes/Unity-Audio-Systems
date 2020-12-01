using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(StepSequencer))]
public class StepSequencerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		StepSequencer sequencer = (StepSequencer)target;

		EditorGUI.BeginChangeCheck();

		DrawDefaultInspector();

		List<StepSequencer.Step> steps = sequencer.GetSteps();

		int numSteps = EditorGUILayout.IntSlider("# steps", steps.Count, 1, 32);

		while (numSteps > steps.Count)
		{
			steps.Add(new StepSequencer.Step());
		}
		while (numSteps < steps.Count)
		{
			steps.RemoveAt(steps.Count - 1);
		}

        for (int i = 0; i < steps.Count; ++i)
        {
            StepSequencer.Step step = steps[i];

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 40;
            EditorGUILayout.LabelField("Step " + (i + 1), GUILayout.Width(40));
            EditorGUIUtility.labelWidth = 40;
            step.Active = EditorGUILayout.Toggle("Active", step.Active, GUILayout.Width(60));
            EditorGUIUtility.labelWidth = 50;
            step.Volume = EditorGUILayout.FloatField("Volume", step.Volume);
            EditorGUIUtility.labelWidth = 30;
            step.MidiNoteNumber = EditorGUILayout.IntField("Note", step.MidiNoteNumber);
            EditorGUIUtility.labelWidth = 60;
            step.BaseNote = EditorGUILayout.IntField(" BaseNote", step.BaseNote);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 80;
            step.Clip = (AudioClip)EditorGUILayout.ObjectField(step.Clip, typeof(AudioClip), false);
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(" ");
            EditorGUILayout.EndHorizontal();
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }
    }
}
