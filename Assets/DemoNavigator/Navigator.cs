using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour
{
	public GameObject exitButton;
	public GameObject menuButtons;

	private void Start()
	{
		DontDestroyOnLoad(this);
	}

	public void BackToNav()
	{
		SceneManager.LoadScene(0);
		exitButton.SetActive(false);
		menuButtons.SetActive(true);
		Destroy(gameObject);
	}

	public void ToScene(int scene)
	{
		SceneManager.LoadScene(scene);
		exitButton.SetActive(true);
		menuButtons.SetActive(false);
	}
}
