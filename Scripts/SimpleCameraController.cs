using UnityEngine;
using System.Collections;

public class SimpleCameraController : MonoBehaviour {

	[SerializeField] [Range(0, 90)] private float maxRotationX;		// camera max X rotation

	// returns correctly clamped value for camera rotation
	float GetClampedX(float x, float limit)
	{
		if (x > 180) { x -= 360; }
		return Mathf.Clamp (x, -limit, limit);
	}

	// rotates camera
	void RotateCamera(Vector2 delta)
	{
		transform.localEulerAngles = Vector3.right * GetClampedX (transform.localEulerAngles.x - delta.y * InputManager.MouseSensitivity, maxRotationX);
	}

	void OnEnable()
	{
		// subscribe to the mouse movement events to rotate camera
		InputManager.OnMouseDrag += RotateCamera;
	}

	void OnDisable()
	{
		// unsubscribe from the mouse movement events
		InputManager.OnMouseDrag -= RotateCamera;
	}
}
