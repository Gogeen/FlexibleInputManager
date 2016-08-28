using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputManagerHud : MonoBehaviour {
	
	[SerializeField] private VerticalLayoutGroup actionGrid; 	// reference to the layout group to show list of actions
	[SerializeField] private GameObject actionUIPrefab;			// action UI prefab 
	[SerializeField] private GameObject bindPanel;				// tooltip to show when key binding is active
	[SerializeField] private Slider sensitivitySlider;			// mouse sensitivity slider
	[SerializeField] private Text sensitivityValue;				// mouse sensitivity value label

	private void Awake()
	{
		// generate static action for handling key rebinding
		InputManager.CreateStaticAction("Escape", KeyCode.Escape);
	}

	private void OnEnable()
	{
		// get list of supported actions
		InputManager.Action[] actions = InputManager.GetActions();

		int siblingIndex = 0;
		foreach (InputManager.Action action in actions)
		{
			// for each action generate UI
			GameObject actionUIGO = Instantiate(actionUIPrefab) as GameObject;
			actionUIGO.transform.SetParent(actionGrid.transform, false);
			actionUIGO.transform.SetSiblingIndex(siblingIndex);
			siblingIndex++;

			// update UI info about the action this UI represents
			actionUIGO.GetComponent<ActionUI>().actionName = action.Name;
			actionUIGO.GetComponent<ActionUI> ().bindPanel = bindPanel;
		}
		sensitivitySlider.value = InputManager.MouseSensitivity;
	}

	private void OnDisable()
	{
		// destroy all actions UI 
		for(int childIndex = actionGrid.transform.childCount - 1; childIndex >= 0; childIndex--)
		{
			Destroy(actionGrid.transform.GetChild(childIndex).gameObject);
		}
	}

	public void ChangeSensitivity(Slider slider)
	{
		InputManager.MouseSensitivity = slider.value;
		sensitivityValue.text = ((int)(slider.value * 10)/10.0f).ToString();
	}
}