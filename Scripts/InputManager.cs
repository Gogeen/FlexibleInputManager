using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {
	
	private static InputManager singleton;										// singleton is using to write some static methods with instance data
	[SerializeField] private bool dontDestroyOnLoad;							// should object be destroyed on scene loading?
	[Tooltip("Should the settings be saved in PlayerPrefs?")]
	[SerializeField] private bool saveChanges;									// should changed key bindings be saved in PlayerPrefs?
	[SerializeField] private List<Action> actions = new List<Action>();			// actions that can be changed
	[SerializeField] private List<Action> staticActions = new List<Action>();	// actions that cannot be changed
	public static float MouseSensitivity = 1;									// mouse sensitivity

	public delegate void PressedKeyDelegate(KeyCode key);
	public static PressedKeyDelegate OnAnyKeyDown;								// called on any key down, with pressed key code

	public delegate void MouseDelegate(Vector2 delta);
	public static MouseDelegate OnMouseDrag;									// called on mouse move, with mouse delta position

	[System.Serializable]
	public class Action
	{
		[SerializeField] private string name;		// action unique name
		[SerializeField] private KeyCode key;		// action main key
		[SerializeField] private KeyCode altKey;	// action alternative key
		private bool pressed = false;				// is key or alt key pressed?

		public string Name { get { return name; } }					// action name getter
		public KeyCode Key 											// key property is using to catch key changes to call OnChange event
		{ 
			get { return key; } 
			set 
			{ 
				bool change = key != value;
				key = value; 
				if (change && OnChange != null) { OnChange (); } 
			} 
		}
		public KeyCode AltKey 										// alt key property is using to catch key changes to call OnChange event
		{ 
			get { return altKey; } 
			set 
			{ 
				bool change = altKey != value;
				altKey = value; 
				if (change && OnChange != null) { OnChange (); } 
			} 
		}
		public bool Pressed { get { return pressed; } }				// action pressed state getter

		public delegate void VoidDelegate();
		public VoidDelegate OnPress;		// called once when key or alt key pressed
		public VoidDelegate OnHold;			// called every frame while key or alt key held
		public VoidDelegate OnRelease;		// called once when key or alt key released
		public VoidDelegate OnChange;		// called once for a key when key bindings changed

		// proccesses user input and calls required events
		public void Proccess()
		{
			// if any action key pressed for the first time, call OnPress
			// pressing another action key while one was pressed will have no effect
			// if any action key released for the first time, call OnRelease
			// releasing another action key while one was released will have no effect
			// otherwise, if pressed state is true, call OnHold

			if (!pressed)
			{
				if (Input.GetKeyDown (key) || Input.GetKeyDown (altKey)) 
				{
					SimulateKeyPress ();
				}
			}
			else 
			{
				if (Input.GetKeyUp (key) || Input.GetKeyUp (altKey)) 
				{
					SimulateKeyRelease ();
				} 
				else 
				{
					if (OnHold != null) { OnHold(); }
				}
			}
		}

		// simulates key press
		public void SimulateKeyPress()
		{
			pressed = true;
			if (OnPress != null) { OnPress (); }
		}

		// simulates key release
		public void SimulateKeyRelease()
		{
			pressed = false;
			if (OnRelease != null) { OnRelease (); }
		}

		public Action(string name, KeyCode key)
		{
			this.name = name;
			this.key = key;
		}
	}
	
	private void Awake()
	{
		if (singleton != null) 
		{
			Destroy(gameObject);
			return;
		}
		singleton = this;

		if (dontDestroyOnLoad) 
		{
			DontDestroyOnLoad (gameObject);
		}
	}

	/// <summary>
	/// Returns array of supported non-static actions.
	/// </summary>
	public static Action[] GetActions()
	{
		return singleton.actions.ToArray();
	}

	/// <summary>
	/// Returns non-static action by name.
	/// </summary>
	/// <param name="name">Action name.</param>
	public static Action GetAction(string name)
	{
		return singleton.actions.Find(x => x.Name == name);
	}

	/// <summary>
	/// Returns non-static action by key bound.
	/// </summary>
	/// <param name="key">Action key.</param>
	private static Action GetAction(KeyCode key)
	{
		return singleton.actions.Find(x => x.AltKey == key || x.Key == key);
	}

	/// <summary>
	/// Returns static action by name.
	/// </summary>
	/// <param name="name">Action name.</param>
	public static Action GetStaticAction(string name)
	{
		return singleton.staticActions.Find (x => x.Name == name);
	}

	/// <summary>
	/// Sets the key for non-static action with the given name.
	/// </summary>
	/// <param name="name">Action name.</param>
	/// <param name="key">New action key.</param>
	/// <param name="isAlternative">If set to <c>true</c> then action altKey will be set instead of main key.</param>
	public static void SetActionKey(string name, KeyCode key, bool isAlternative = false)
	{
		// if action not found, do nothing
		Action actionToSet = GetAction (name);
		if (actionToSet == null) { return; }

		// if action with the same key bound was found, unbind this key from this action
		Action actionToUnbind = GetAction (key);
		if (actionToUnbind != null)
		{
			if (actionToUnbind.Key == key) 
			{ 
				actionToUnbind.Key = KeyCode.None; 
			} 
			else 
			{ 
				actionToUnbind.AltKey = KeyCode.None; 
			}
		}

		// set key for the target action
		if (!isAlternative) 
		{ 
			actionToSet.Key = key; 
		} 
		else 
		{ 
			actionToSet.AltKey = key; 
		}
		
	}

	/// <summary>
	/// Creates new non-static action to use.
	/// </summary>
	/// <param name="name">Action name.</param>
	/// <param name="key">Action key.</param>
	public static void CreateAction(string name, KeyCode key)
	{
		if (GetAction (name) != null) 
		{
			Debug.LogWarning ("Cannot create non-static action named " + name + ", non-static action with the same name already exists!");
			return;
		}
		singleton.actions.Add (new Action(name, key));
	}

	/// <summary>
	/// Creates new static action to use.
	/// </summary>
	/// <param name="name">Action name.</param>
	/// <param name="key">Action key.</param>
	public static void CreateStaticAction(string name, KeyCode key)
	{
		if (GetStaticAction (name) != null) 
		{
			Debug.LogWarning ("Cannot create static action named " + name + ", static action with the same name already exists!");
			return;
		}
		singleton.staticActions.Add (new Action(name, key));
	}
	
	private void Update()
	{
		// proccess mouse events
		if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) 
		{
			if (OnMouseDrag != null) 
			{
				OnMouseDrag (new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
			}
		}

		// proccess static actions
		foreach(Action action in staticActions)
		{
			action.Proccess ();
		}
		// proccess non-static actions
		foreach(Action action in actions)
		{
			action.Proccess ();
		}

		// if any key down, call OnAnyKeyDown event
		if (!Input.anyKeyDown)
		{
			return;
		}
		foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
		{
			if (!Input.GetKeyDown (key)) 
			{
				continue;
			}
			if (OnAnyKeyDown != null) {
				OnAnyKeyDown (key);
			}
		}
	}

	// saves changes to PlayerPrefs
	private void Save()
	{
		foreach(Action action in actions)
		{
			PlayerPrefs.SetInt("Action_" + action.Name + "_Key", (int)action.Key);
			PlayerPrefs.SetInt("Action_" + action.Name + "_AltKey", (int)action.AltKey);
		}
	}
	
	// loads changes from PlayerPrefs
	private void Load()
	{
		foreach(Action action in actions)
		{
			if (PlayerPrefs.HasKey ("Action_" + action.Name + "Key")) 
			{
				action.Key = (KeyCode)PlayerPrefs.GetInt ("Action_" + action.Name + "Key");
			}
			if (PlayerPrefs.HasKey ("Action_" + action.Name + "AltKey")) 
			{
				action.AltKey = (KeyCode)PlayerPrefs.GetInt ("Action_" + action.Name + "AltKey");
			}
		}
	}
	
	private void OnEnable()
	{
		if (saveChanges) { Load (); }
	}
	
	private void OnDisable()
	{
		if (saveChanges) { Save (); }
	}

	// simulates action key release on program window focus lost
	// this is done because unity built-in input doesn't handle GetKeyUp events out of focus, 
	// which can move to strange results, for example, in character controllers that store movement direction in a variable.
	// with this feature, all user input will be handled correctly and no input will be lost out of focus.
	private void OnApplicationFocus(bool status){
		// if window focus is lost then release keys for all actions that are in pressed state
		if (!status) 
		{
			foreach(Action action in staticActions)
			{
				if (action.Pressed) {
					action.SimulateKeyRelease ();
				}
			}
			foreach(Action action in actions)
			{
				if (action.Pressed) {
					action.SimulateKeyRelease ();
				}
			}
		}
	}
}