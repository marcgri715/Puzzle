/*
 * Klasa zawierająca metody pozwalające na poruszanie i obracanie puzzlami.
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PieceControl : MonoBehaviour {

	private Vector3 screenPoint;
	private Vector3 offset;
	private Vector3 curPosition;
	public GameObject gameLogic;
	private float size;
	//Zmienna mówiąca o tym, jak łatwo dopasować puzzle do innego
	private float hitbox;



	void Start () {
		size = transform.localScale.x; //Zawsze kwadratowe
		hitbox = size * 0.2f;
	}

	void Update () {
	
	}


	void OnMouseDown() {
		screenPoint = Camera.main.WorldToScreenPoint (transform.parent.gameObject.transform.position);
		float oldDepth = transform.parent.gameObject.transform.position.z;
		Vector3 newPosition;
		foreach (Transform child in transform.parent.transform) {
			if (child.tag == "DragBox") {
				//Wszystkie dragboxy wędrują na wierzch
				newPosition = child.transform.position;
				newPosition.z = oldDepth;
				child.transform.position = newPosition;
			}
		}
		//wartość z jest równa 0 - puzzle wędruje na wierzch
		offset = transform.parent.gameObject.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, 
		                                                                                                       Input.mousePosition.y, 
		                                                                                                       screenPoint.z + oldDepth));
		GameObject[] allGroups = GameObject.FindGameObjectsWithTag ("Group");
		//przesunięcie wszystkich grup puzzli które znajdowały się wyżej w dół
		foreach (GameObject group in allGroups) {
			newPosition = group.transform.position;
			float groupDepth = newPosition.z;
			if (groupDepth < oldDepth) {
				groupDepth += 1.0f;
				newPosition.z = groupDepth;
				group.transform.position = newPosition;
				foreach (Transform child in group.transform) {
					if (child.tag == "DragBox") {
						//Dragboxy znajdują się nad pozostałymi elementami
						newPosition = child.position;
						newPosition.z = groupDepth/1000.0f;
						child.position = newPosition;
					}
				}
			}
		}
		Cursor.visible = false;
	}

	void OnMouseDrag() {
		//Poruszanie grupą nadrzędną, a nie pojedynczym puzzlem lub jego dragboxem
		curPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)) + offset;
		transform.parent.transform.position = curPosition;
	}

	void OnMouseUp() {
		Cursor.visible = true;
		CheckForConnection ();
	}


	void OnMouseOver() {
		if (Input.GetMouseButtonDown (1)) {
			//Obrót puzzla po kliknięciu prawym przyciskiem myszy
			transform.parent.transform.Rotate(Vector3.forward * -90);
			//Obrócenie wszystkich puzzli
			List<int> toRotate = new List<int>();
			foreach (Transform child in transform.parent.transform) {
				if (child.gameObject.tag == "text") {
					//Tekst obracany w drugą stronę, by był czytelny dla gracza
					child.transform.Rotate(Vector3.forward * 90);
				}
				else if (child.gameObject.tag == "DragBox") {
					toRotate.Add(int.Parse(child.gameObject.name));
				}
			}
			//Obrót wszystkimi puzzlami w odpowiadających im klasach
			gameLogic.GetComponent<GameLogicController>().RotatePiece(toRotate);
			CheckForConnection();
		}
	}

	//Sprawdzenie, czy po zakończeniu ruchu/obrocie puzzle może połączyć się z innym
	void CheckForConnection () {
		Transform oldParent = transform.parent;
		List<Transform> pieces = new List<Transform> ();
		foreach (Transform child in oldParent.transform) {
			if (child.gameObject.tag == "DragBox") {
				pieces.Add (child);
			}
		}
		//Sprawdzenie każdego z puzzli w poruszanej grupie
		foreach (Transform dragBox in pieces) {
			//sprawdzenie puzzli wokół
			Collider[] colliders = Physics.OverlapSphere (dragBox.position, size * 1.5f);
			foreach (Collider piece in colliders) {
				//sprawdzenie rodziców puzzli znajdujących się blisko siebie
				oldParent = transform.parent;
				Transform newParent = piece.transform.parent;
				if (oldParent == newParent) 
					continue;
				
				int side = -1;
				float yDistance = dragBox.position.y - piece.transform.position.y;
				float xDistance = dragBox.position.x - piece.transform.position.x;

				//Sprawdzenie czy puzzle leżą dość blisko siebie, by się połączyć i określenie ściany
				//Sprawdzenie górnej i dolnej ściany:
				if (Mathf.Abs (xDistance) < hitbox) {
					if (Mathf.Abs (yDistance) - size < hitbox) {
						if (yDistance < 0)
							side = 0;
						else
							side = 2;
					}
				//Sprawdzenie prawej i lewej ściany:
				} else if (Mathf.Abs (yDistance) < hitbox) {
					if (Mathf.Abs (xDistance) - size < hitbox) {
						if (xDistance < 0) 
							side = 1;
						else
							side = 3;
					}
				}
				//jeśli są dość blisko
				if (side != -1) {
					//Sprawdzenie czy do siebie pasują
					if (gameLogic.GetComponent<GameLogicController> ().CheckPieces (int.Parse (dragBox.gameObject.name),
					                                                                side,
					                                                                int.Parse (piece.gameObject.name))) {
						//Wczytanie wszystkich potomków znajdujących się w obecnej grupie - bez rodzica
						Transform[] children = oldParent.transform.Cast<Transform> ().ToArray ();
						Vector3 newPosition = piece.transform.position;
						newPosition.z = newParent.transform.position.z;
						//ustawienie pozycji względem nowego rodzica
						switch (side) {
						case 0:
							newPosition.y -= size;
							break;
						case 1:
							newPosition.x -= size;
							break;
						case 2:
							newPosition.y += size;
							break;
						case 3:
							newPosition.x += size;
							break;
						}
						newPosition.x += oldParent.position.x - dragBox.position.x;
						newPosition.y += oldParent.position.y - dragBox.position.y;
						oldParent.position = newPosition;
						foreach (Transform child in children) {
							//zmiana rodzica na nowego - połączenie dwóch grup
							child.parent = newParent;
							if (child.tag == "DragBox") {
								//korekta wysokości dragboxa
								newPosition = child.position;
								newPosition.z /= 1000.0f;
								child.position = newPosition;
							}
						}
						//usunięcie pustej grupy
						Destroy (oldParent.gameObject);
					}
				}
			}
		}
	}
}
