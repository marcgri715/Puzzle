/*
 * Klasa służąca do wyboru obrazka do ułożenia oraz liczby puzzli.
 */

using UnityEngine;
using System.Collections;

public class MainMenuGUI : MonoBehaviour {

	private int selected;
	public string[] textures;
	private int size;
	private GUIStyle textstyle;


	void Start () {
		selected = 0;
		size = 4;

		//Nazwy plików z obrazkami
		textures = new string[]{"sloth", "horse", "zorza", "path"};

		textstyle = new GUIStyle ();
		textstyle.alignment = TextAnchor.MiddleCenter;
	}

	void Update () {
	
	}

	void OnGUI() {
		GUI.Window (0, new Rect (Screen.width * 0.25f,
		                         Screen.height * 0.3f,
		                         Screen.width * 0.5f,
		                         Screen.height * 0.525f),
		            SelectWindowFunc, "Wybór obrazka:");
	}

	void SelectWindowFunc (int id) {
		selected = GUILayout.SelectionGrid (selected, textures, 1);
		GUI.Label (new Rect (Screen.width * 0.2f,
		                     Screen.height * 0.25f,
		                     Screen.width * 0.1f,
		                     Screen.height * 0.05f),
		           "Wybierz liczbę puzzli w każdym rzędzie i kolumnie (2-25):",
		           textstyle);
		if (GUI.Button (new Rect (Screen.width * 0.05f,
		                          Screen.height * 0.3f,
		                          Screen.width * 0.1f,
		                          Screen.height * 0.05f),
		                "-")) {
			if (size > 2) {
				size--;
			}
		}
		GUI.Label (new Rect (Screen.width * 0.2f,
		                     Screen.height * 0.3f,
		                     Screen.width * 0.1f,
		                     Screen.height * 0.05f),
		           size.ToString (),
		           textstyle);
		if (GUI.Button (new Rect (Screen.width * 0.35f,
		                          Screen.height * 0.3f,
		                          Screen.width * 0.1f,
		                          Screen.height * 0.05f),
		                "+")) {
			if (size < 25) {
				size++;
			}
		}

		if (GUI.Button (new Rect(Screen.width * 0.1f,
		                         Screen.height*0.375f,
		                         Screen.width * 0.3f,
		                         Screen.height * 0.05f),
		                "Rozpocznij grę")) {
			PlayerPrefs.SetString ("texture", textures [selected]);
			PlayerPrefs.SetInt ("size", size);
			Application.LoadLevel (1);
		}

		if (GUI.Button (new Rect (Screen.width * 0.2f,
		                          Screen.height * 0.45f,
		                          Screen.width * 0.1f,
		                          Screen.height * 0.05f),
		                "Wyjdź z gry")) {
			Application.Quit ();
		}
	}
}
