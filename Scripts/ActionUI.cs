using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour {
	
	[SerializeField] private Text nameLabel;		// action name label
	[SerializeField] private Text keyLabel;			// action main key label
	[SerializeField] private Text altKeyLabel;		// action alt key  label
	[SerializeField] private Button keyButton;		// action key button
	[SerializeField] private Button altKeyButton;	// action alt key button

	public GameObject bindPanel;					// tooltip to show when key binding is active

	private bool isAlternative = false;				// is the alternative key changing?

	// property is using to instantly update UI
	public string actionName 
	{
		get { return nameLabel.text; }
		set 
		{
			if (InputManager.GetAction (nameLabel.text) != null) 
			{
				InputManager.GetAction (nameLabel.text).OnChange -= UpdateUI;
			}
			nameLabel.text = value; 
			UpdateUI (); 
			InputManager.GetAction (value).OnChange += UpdateUI;
		}
	}

	// updates UI
	private void UpdateUI()
	{
		InputManager.Action action = InputManager.GetAction(nameLabel.text);
		keyLabel.text = action.Key.ToString();
		altKeyLabel.text = action.AltKey.ToString();
	}

	// enables key binding for this action
	public void ChangeKey(bool isAlternative)
	{
		this.isAlternative = isAlternative;
		StartBinding ();
	}

	// bind pressed key to the action
	private void BindPressedKey(KeyCode key)
	{
		InputManager.SetActionKey(nameLabel.text, key, isAlternative);
		CancelBinding ();
	}

	// subscribe to the escape event to have an opportunity to cancel binding
	private void OnEnable()
	{
		InputManager.GetStaticAction ("Escape").OnPress += CancelBinding;
	}

	// unsubscribe from the escape event and the action key change event
	private void OnDisable()
	{
		InputManager.GetStaticAction ("Escape").OnPress -= CancelBinding;
		InputManager.GetAction (nameLabel.text).OnChange -= UpdateUI;
	}

	// enables tooltip and subscribes to the OnAnyKeyDown event to bind pressed key
	private void StartBinding(){
		InputManager.OnAnyKeyDown += BindPressedKey;
		bindPanel.SetActive (true);
	}

	// cancels binding
	private void CancelBinding(){
		InputManager.OnAnyKeyDown -= BindPressedKey;
		bindPanel.SetActive (false);
	}
}