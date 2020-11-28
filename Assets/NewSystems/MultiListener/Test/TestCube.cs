using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestCube : MonoBehaviour
{
	public UnityEvent click;

	void OnMouseDown()
	{
		click?.Invoke();
	}
}
