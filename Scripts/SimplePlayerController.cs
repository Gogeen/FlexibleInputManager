using UnityEngine;
using System.Collections;

public class SimplePlayerController : MonoBehaviour {

	Vector3 movementDirection = Vector3.zero;							// player movement direction vector
	[SerializeField] [Range(0,Mathf.Infinity)] float speed;				// player speed
	[SerializeField] [Range(0,Mathf.Infinity)] float runMultiplier;		// player speed multiplier while running
	private bool isRunning = false;										// is player running?

	CharacterController controller;										// reference to the character controller to move this object

	void Start () 
	{
		// get the controller reference
		controller = GetComponent<CharacterController> ();
	}

	void Update()
	{
		// move the player
		controller.SimpleMove (movementDirection.normalized * speed * (isRunning ? runMultiplier : 1));
		movementDirection = Vector3.zero;
	}

	// player action methods (moving, running, turning)
	void MoveLeft() { movementDirection -= transform.right; }
	void MoveRight() { movementDirection += transform.right; }
	void MoveForward() { movementDirection += transform.forward; }
	void MoveBack() { movementDirection -= transform.forward; }
	void Run() { isRunning = true; }
	void StopRun() { isRunning = false; }

	void RotatePlayer(Vector2 delta)
	{
		transform.Rotate (Vector3.up, delta.x * InputManager.MouseSensitivity);
	}

	void OnEnable()
	{
		// subscribe to the events
		InputManager.GetAction ("Move Left").OnHold += MoveLeft;
		InputManager.GetAction ("Move Right").OnHold += MoveRight;
		InputManager.GetAction ("Move Forward").OnHold += MoveForward;
		InputManager.GetAction ("Move Back").OnHold += MoveBack;

		InputManager.GetAction ("Run").OnPress += Run;
		InputManager.GetAction ("Run").OnRelease += StopRun;

		InputManager.OnMouseDrag += RotatePlayer;
	}

	void OnDisable()
	{
		// unsubscribe from the events
		InputManager.GetAction ("Move Left").OnHold -= MoveLeft;
		InputManager.GetAction ("Move Right").OnHold -= MoveRight;
		InputManager.GetAction ("Move Forward").OnHold -= MoveForward;
		InputManager.GetAction ("Move Back").OnHold -= MoveBack;

		InputManager.GetAction ("Run").OnPress -= Run;
		InputManager.GetAction ("Run").OnRelease -= StopRun;

		InputManager.OnMouseDrag -= RotatePlayer;
	}
}
