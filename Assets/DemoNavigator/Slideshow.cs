using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slideshow : MonoBehaviour {
	public Sprite[] pictures;
	public float holdTime = 4f;
	public Image imageComponent;

	private float timer = 0;
	private int index = 0;


	void Start() {
		if (imageComponent == null) imageComponent = GetComponentInChildren<Image>();
		timer = holdTime;
		imageComponent.sprite = pictures[0];
	}

	void Update() {
		timer -= Time.deltaTime;
		if (timer <= 0) {
			index++;
			if (index >= pictures.Length) index = 0;
			imageComponent.sprite = pictures[index];
			timer = holdTime;
		}
	}

	public void NextSlide() {
		index++;
		if (index >= pictures.Length) index = 0;
		imageComponent.sprite = pictures[index];
		timer = holdTime;
	}

	public void PreviouseSlide() {
		index--;
		if (index < 0) index = pictures.Length - 1;
		imageComponent.sprite = pictures[index];
		timer = holdTime;
	}
}
