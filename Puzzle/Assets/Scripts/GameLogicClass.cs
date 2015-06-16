/*
 * Klasa opisująca logikę gry.
 */

using UnityEngine;
using System.Collections;

public class GameLogicClass {

	//Wszystkie poszczególne puzzle
	private PieceClass[] pieces;
	//Obecna liczba połączonych grup puzzli
	private int currentSize;


	public GameLogicClass(int pSize) {
		currentSize = pSize*pSize;
		pieces = new PieceClass[pSize*pSize];
		for (int i=0; i<pSize; i++) {
			for (int j=0; j<pSize; j++) {
				pieces[i*pSize+j] = new PieceClass(i, j, pSize);
			}
		}
	}

	//Metoda porównująca wartości stykających się krawędzi dwóch puzzli 
	public bool CompareTwoPieces(int pFirstPieceIndex, int pFirstPieceEdgeIndex, int pSecondPieceIndex) {
		int secondPieceEdgeIndex = pFirstPieceEdgeIndex + 2;
		secondPieceEdgeIndex = secondPieceEdgeIndex % 4;
		int firstEdge = pieces [pFirstPieceIndex].GetEdge (pFirstPieceEdgeIndex);
		int secondEdge = pieces [pSecondPieceIndex].GetEdge (secondPieceEdgeIndex);
		if (firstEdge == secondEdge && firstEdge > 0) {
			currentSize--; //zmniejszenie liczby grup, jeśli 
			return true;
		}
		return false;
	}
	
	public bool IsGameFinished() {
		if (currentSize == 1) 
			return true;
		return false;
	}

	public string GetPieceEdgeValue(int pPieceIndex, int pEdge) {
		int result = pieces [pPieceIndex].GetEdge (pEdge);
		if (result < 1)
			return "";
		return result.ToString();
	}

	public void RotateRight(int pPieceIndex) {
		pieces [pPieceIndex].RotateRight ();
	}

	public float GetRotation(int pPieceIndex) {
		return pieces [pPieceIndex].GetRotation ();
	}
}
