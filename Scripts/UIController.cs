using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

	[SerializeField] private GameObject InputPanel;						// UI panel to open on pressing escape button
	[SerializeField] private SimplePlayerController playerController;	// reference to the player controller script to disable it while setting key bindings
	[SerializeField] private SimpleCameraController cameraController;	// reference to the player camera script to disable it while setting key bindings


	private void Awake()
	{
		// generate static action for handling escape event
		InputManager.CreateStaticAction("Escape", KeyCode.Escape);
	}

	void Start () 
	{
		// subscribe to the escape event 
		InputManager.GetStaticAction ("Escape").OnPress += ToggleGameState;
	}

	// toggle input panel window
	void ToggleGameState()
	{
		// if opening input panel window then disable player scripts, show mouse and open UI window, otherwise do opposite action
		bool isPause = !InputPanel.activeSelf;
		playerController.enabled = !isPause;
		cameraController.enabled = !isPause;
		Cursor.lockState = isPause ? CursorLockMode.None : CursorLockMode.Locked;
		InputPanel.SetActive (isPause);
	}
}
