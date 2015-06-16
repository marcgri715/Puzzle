/*
 * Klasa sterująca logiką gry.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameLogicController : MonoBehaviour {

	private GameLogicClass game;
	public GameObject piecePrefab;
	private bool finished;
	public Texture2D picture;
	private bool GUIEnabled;
	private int size;


	void CreateNewGame() {
		//Wczytanie danych z menu głównego
		size = PlayerPrefs.GetInt ("size");
		string textureName = PlayerPrefs.GetString ("texture");
		picture = (Texture2D) Resources.Load (textureName);

		game = new GameLogicClass (size);
		//Ustawienie odległości kamery od puzzli - zależne od ich liczby
		Camera.main.orthographic = true;
		Camera.main.orthographicSize = size/2;
		//Pętla tworząca nowe puzzle oraz ustawiająca parametry poszczególnych elementów puzzla
		for (int i=0; i<size*size; i++) {
			GameObject newObject = (GameObject)Instantiate(piecePrefab,
			                                               new Vector3(Random.Range(-size/2.0f, size/2.0f),
			                                                                        Random.Range(-size/2.0f, size/2.0f),
			                                                                        i),
			                                               Quaternion.Euler(0,0,0));
			foreach(Transform child in newObject.transform) {
				if (child.tag == "DragBox") {
					//Ustawienie nazwy puzzla oraz ustawienie głębokości na której się domyślnie znajduje
					child.gameObject.name = i.ToString();
					child.GetComponent<PieceControl>().gameLogic = gameObject;
					Vector3 dragBoxDepth = child.position;
					dragBoxDepth.z = i/1000.0f;
					child.position = dragBoxDepth;
				}
				if (child.tag == "text") {
					//Wyświetlenie poszczególnych numerów puzzla
					var text = child.GetComponent<TextMesh>();
					if (child.name == "TextUp") {
						text.text = game.GetPieceEdgeValue(i, 0);
					}
					else if (child.name == "TextRight") {
						text.text = game.GetPieceEdgeValue(i, 1);
					}
					else if (child.name == "TextDown") {
						text.text = game.GetPieceEdgeValue(i, 2);
					}
					else if (child.name == "TextLeft") {
						text.text = game.GetPieceEdgeValue(i, 3);
					}
				}
				if (child.tag == "Pic") {
					//Ustawienie fragmentu tekstury jako wyświetlanego obrazka
					child.gameObject.GetComponent<MeshRenderer>().material.mainTexture = picture;
					float scale = 1.0f/(float)size;
					float xOffset = (i%size)/(float)size;
					float yOffset = (size-1-(i/size))/(float)size;
					child.gameObject.GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", new Vector2(scale, scale));
					child.gameObject.GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(xOffset, yOffset));
					//Obrót obrazka w celu prawidłowego wyświetlenia go na ekranie
					Vector3 rotation = new Vector3(0,0,game.GetRotation(i));
					child.Rotate(rotation);
				}
			}
			
		}
	}

	//Sprawdzenie dwóch puzzli
	public bool CheckPieces (int pFirstPieceIndex, int pFirstPieceEdge, int pSecondPieceIndex) {
		if (game.CompareTwoPieces (pFirstPieceIndex, pFirstPieceEdge, pSecondPieceIndex)) {
			finished = game.IsGameFinished ();
			return true;
		}
		return false;
	}

	public void RotatePiece(List<int> pIndex) {
		foreach (int index in pIndex) {
			game.RotateRight (index);
		}
	}

	void Start () {
		CreateNewGame ();
		GUIEnabled = false;
		finished = false;
	}

	void Update () {
		if (finished)
			GUIEnabled = true;
	}
	
	private void OnGUI() {
		if (GUIEnabled) {
			GUI.Window (0, new Rect (Screen.width * 0.25f, Screen.height * 0.4f, Screen.width * 0.5f, Screen.height * 0.2f), restartMenuFunc, "Restart");
		}
	}

	//Menu z przyciskami wyświetlane po ułożeniu obrazka
	private void restartMenuFunc(int id) {
		if (GUILayout.Button ("Uruchom ponownie")) {
			RestartGame ();
			GUIEnabled = false;
		}
		if (GUILayout.Button ("Powróć do menu")) {
			Application.LoadLevel (0);
		}
	}

	//Ponowne uruchomienie gry z tymi samymi parametrami - usunięcie poprzednich puzzli i stworzenie nowych
	private void RestartGame() {
		GameObject toDelete = GameObject.FindGameObjectWithTag ("Group");
		Transform[] children = toDelete.transform.Cast<Transform>().ToArray();
		foreach (Transform child in children) {
			Destroy (child.gameObject);
		}
		Destroy (toDelete);
		Start ();
	}
}
