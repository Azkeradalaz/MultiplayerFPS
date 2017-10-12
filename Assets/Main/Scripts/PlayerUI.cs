using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

	[SerializeField]
	private RectTransform thrusterFuelFill;

	[SerializeField]
	GameObject pauseMenu;

	private PlayerController controller;

	public void SetController(PlayerController _controller)
	{
		controller = _controller;

		
	}

	public void TogglePauseMenu()
	{
		pauseMenu.SetActive(!pauseMenu.activeSelf);
		PauseMenu.isOn = pauseMenu.activeSelf;
	}

	private void Start()
	{
		PauseMenu.isOn = false;
	}


	private void Update()
	{
		SetFuelAmout(controller.GetThrusterFuelAmount());
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePauseMenu();
		}
	}

	void SetFuelAmout(float _amount )
	{
		thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
	}
}
