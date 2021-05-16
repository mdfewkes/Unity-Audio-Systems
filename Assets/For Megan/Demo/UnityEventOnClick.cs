using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventOnClick : MonoBehaviour {
	public UnityEvent uEvent;

	void OnMouseDown() {
		uEvent?.Invoke();
	}
}
