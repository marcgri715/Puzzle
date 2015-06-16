/*
 * Klasa opisująca poszczególny puzzle.
 */

using UnityEngine;
using System.Collections;

public class PieceClass {

	//obecny obrót puzzla
	private int rotation;

	private int[] edges; // [0] - góra, [1] - prawo, [2] - dół, [3] - lewo
	// Wartość krawędzi to liczba znajdująca się z boku puzzla, jeśli jest mniejsza od 1 - puzzle leży na krawędzi


	public PieceClass (int pRow, int pCol, int pSize) {
		rotation = Random.Range(0,3);
		edges = new int[4];

		//Obliczanie wartości górnej krawędzi
		if (pRow == 0) 
			edges [0] = 0;
		else
			edges [0] = (2 * pSize - 1) * (pRow - 1) + 2 * (pCol + 1);

		//Obliczanie wartości dolnej krawędzi
		if (pRow == pSize - 1)
			edges [2] = 0;
		else 
			edges [2] = (2 * pSize - 1) * pRow + 2 * (pCol + 1);

		//Obliczanie wartości prawej krawędzi + mała poprawka dla górnych i dolnych krawędzi przy prawym krańcu obrazka
		if (pCol == pSize - 1) {
			edges [0]--;
			edges [1] = 0;
			edges [2]--;
		}
		else
			edges [1] = (2 * pSize - 1) * pRow + 2 * pCol + 1;

		//Obliczanie wartości lewej krawędzi
		if (pCol == 0) 
			edges [3] = 0;
		else 
			edges [3] = (2 * pSize - 1) * pRow + 2 * (pCol - 1) + 1;
	}

	//Zwraca wartość danej krawędzi
	public int GetEdge (int pIndex) {
		return (edges [(pIndex + rotation) % 4]);
	}

	//Obrót klocka
	public void RotateRight () {
		if (rotation == 0)
			rotation = 3;
		else
			rotation--;
	}

	//Zwraca kąt, pod którym puzzle jest obecnie odwrócony
	public float GetRotation () {
		float eulerRotation = rotation*90-180;
		return eulerRotation;
	}
}
