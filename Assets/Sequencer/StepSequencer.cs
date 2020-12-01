using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StepSequencer : MonoBehaviour
{
	[Serializable]
	public class Step
	{
		public bool Active = false;
		public float Volume = 0.75f;
		public int MidiNoteNumber = 60;
		public int BaseNote = 60;
		public AudioClip Clip;
	}

	private MusicSequencer metronome;
	[SerializeField, HideInInspector] private List<Step> steps = new List<Step>();

#if UNITY_EDITOR
	public List<Step> GetSteps()
	{
		return steps;
	}
#endif

	private int currentStep = 0;

	void OnEnable()
	{
		metronome = FindObjectOfType<MusicSequencer>();
		if (metronome != null)
		{
			metronome.OnBeat += HandleBeat;
		}
	}

	void OnDisable()
	{
		if (metronome != null)
		{
			metronome.OnBeat -= HandleBeat;
		}
	}

	public void HandleBeat()
	{
		int numSteps = steps.Count;
		if (numSteps == 0) return;
		if (currentStep >= numSteps) currentStep = 0;

		Step stepNow = steps[currentStep];

		if (stepNow.Active && stepNow.Clip != null)
		{
			metronome.Play(0f, stepNow.Clip, stepNow.Volume, MusicMathUtils.MidiNoteToPitch(stepNow.MidiNoteNumber, stepNow.BaseNote));
		}

		currentStep++;
	}
}