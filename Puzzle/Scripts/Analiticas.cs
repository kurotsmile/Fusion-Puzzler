using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Analiticas : MonoBehaviour {


	public void PuzzleSeleccionado (int numPuzzle) {
		Analytics.CustomEvent("Puzzles",  new Dictionary<string, object>
			{
				{ "PuzzleSeleccionado", "Puzzle"+numPuzzle }
			});
	}

	public void DificultadSeleccionada (int nivelDificultad) {
		Analytics.CustomEvent("Puzzles",  new Dictionary<string, object>
			{
				{ "DificultadSeleccionada", "D "+nivelDificultad }
			});
	}

	public void DificultadCompletada (int nivelDificultad) {
		Analytics.CustomEvent("Puzzles",  new Dictionary<string, object>
			{
				{ "DificultadCompletada", "D "+nivelDificultad }
			});
	}

	public void UsaAyudaBG () {
		Analytics.CustomEvent("Puzzles",  new Dictionary<string, object>
			{
				{ "AyudaBG", 1 }
			});
	}

	public void UsaAyudaBordes () {
		Analytics.CustomEvent("Puzzles",  new Dictionary<string, object>
			{
				{ "AyudaBordes", 1 }
			});
	}

	public void UsaAyudaSeparar () {
		Analytics.CustomEvent("Puzzles",  new Dictionary<string, object>
			{
				{ "AyudaSeparar (Siempre hay una al iniciar Puzzle)", 1 }
			});
	}

}
