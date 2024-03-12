using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuntarPiezas : MonoBehaviour {

	public Transform[,] piezasPuzzle;
	public List <GameObject> piezasSinColocar;

	float margenSnap;
	int anchoPuzzle;
	int altoPuzzle;
	public Vector3[,] posicionInicial;
	GameObject objetoControlador;
	MoverPiezas moverPiezas;
	ControlUI controlUI;

	DistanciaEntrePiezas[,] distanciaEntrePiezas;
	public struct DistanciaEntrePiezas {
		public float superior;
		public float inferior;
		public float derecha;
		public float izquierda;
	}


	void Start () {
		objetoControlador = GameObject.Find ("app_wall");
		moverPiezas = objetoControlador.GetComponent<MoverPiezas> ();
		controlUI = objetoControlador.GetComponent<ControlUI> ();

		piezasSinColocar = new List<GameObject> ();
		string dimensionesPuzzle = this.gameObject.name.Substring (5, this.gameObject.name.Length - 5);
		string[] dosDimensiones = dimensionesPuzzle.Split('x');
		anchoPuzzle = int.Parse(dosDimensiones [0]);
		altoPuzzle = int.Parse(dosDimensiones [1]);

		piezasPuzzle = new Transform[anchoPuzzle, altoPuzzle];
		posicionInicial = new Vector3[anchoPuzzle, altoPuzzle];
		distanciaEntrePiezas = new DistanciaEntrePiezas[anchoPuzzle, altoPuzzle];

		margenSnap = 0.2f - (anchoPuzzle * 0.01f); //Va desde 0.16 (El puzzle de 4) hasta 0.10 (El puzzle de 10)

		for (int i = 0; i < altoPuzzle; i++) {
			for (int k = 0; k < anchoPuzzle; k++) {
				piezasPuzzle [k, i] = transform.GetChild (i * anchoPuzzle + k);
				posicionInicial [k,i] = piezasPuzzle [k, i].position;
				piezasSinColocar.Add (piezasPuzzle [k, i].gameObject);
			}
		}
		for (int i = 0; i < altoPuzzle; i++) {
			for (int k = 0; k < anchoPuzzle; k++) {
				if (k > 0) {
					distanciaEntrePiezas [k, i].izquierda = Mathf.Abs(piezasPuzzle [k, i].position.x - piezasPuzzle [k - 1, i].position.x);
				}
				if (k < anchoPuzzle - 1) {
					distanciaEntrePiezas [k, i].derecha = Mathf.Abs(piezasPuzzle [k + 1, i].position.x - piezasPuzzle [k, i].position.x);
				}
				if (i > 0) {
					distanciaEntrePiezas [k, i].superior = Mathf.Abs(piezasPuzzle [k, i - 1].position.y - piezasPuzzle [k, i].position.y);
				}
				if (i < altoPuzzle - 1) {
					distanciaEntrePiezas [k, i].inferior = Mathf.Abs(piezasPuzzle [k, i].position.y - piezasPuzzle [k, i + 1].position.y);
				}
			}
		}
	}

	public void ReajustarPiezas(int k, int i){
		bool tieneIzquierda = false;
		bool tieneDerecha = false;
		bool tieneSuperior = false;
		bool tieneInferior = false;

		if (k > 0 && distanciaEntrePiezas [k, i].izquierda <= Mathf.Abs (piezasPuzzle [k, i].position.x - piezasPuzzle [k - 1, i].position.x) + margenSnap
		     && distanciaEntrePiezas [k, i].izquierda >= Mathf.Abs (piezasPuzzle [k, i].position.x - piezasPuzzle [k - 1, i].position.x) - margenSnap
		     && piezasPuzzle [k, i].position.y <= piezasPuzzle [k - 1, i].position.y + margenSnap
		     && piezasPuzzle [k, i].position.y >= piezasPuzzle [k - 1, i].position.y - margenSnap
			 && piezasPuzzle [k, i].position.x > piezasPuzzle [k - 1, i].position.x) {
			tieneIzquierda = true;

			Vector3 posicionAnterior = piezasPuzzle [k, i].transform.position;
			piezasPuzzle [k, i].transform.position = new Vector3 (piezasPuzzle [k - 1, i].position.x + distanciaEntrePiezas [k - 1, i].derecha, piezasPuzzle [k - 1, i].position.y, piezasPuzzle [k - 1, i].position.z);
			Vector3 posicionActual = piezasPuzzle [k, i].transform.position;

			if (piezasPuzzle [k, i].parent.name == "GrupoPiezas") {
				piezasPuzzle [k, i].transform.position = posicionAnterior;
				piezasPuzzle [k, i].parent.transform.position += posicionActual - posicionAnterior;
			}

			AgruparPiezas (k, i, "Izquierda");
		}
		if (k < anchoPuzzle - 1 && distanciaEntrePiezas [k, i].derecha <= Mathf.Abs (piezasPuzzle [k + 1, i].position.x - piezasPuzzle [k, i].position.x) + margenSnap
		     && distanciaEntrePiezas [k, i].derecha >= Mathf.Abs (piezasPuzzle [k + 1, i].position.x - piezasPuzzle [k, i].position.x) - margenSnap
		     && piezasPuzzle [k, i].position.y <= piezasPuzzle [k + 1, i].position.y + margenSnap
		     && piezasPuzzle [k, i].position.y >= piezasPuzzle [k + 1, i].position.y - margenSnap
			 && piezasPuzzle [k, i].position.x < piezasPuzzle [k + 1, i].position.x) {
			tieneDerecha = true;

			Vector3 posicionAnterior = piezasPuzzle [k, i].transform.position;
			piezasPuzzle [k, i].transform.position = new Vector3 (piezasPuzzle [k + 1, i].position.x - distanciaEntrePiezas [k + 1, i].izquierda, piezasPuzzle [k + 1, i].position.y, piezasPuzzle [k + 1, i].position.z);
			Vector3 posicionActual = piezasPuzzle [k, i].transform.position;

			if (piezasPuzzle [k, i].parent.name == "GrupoPiezas") {
				piezasPuzzle [k, i].transform.position = posicionAnterior;
				piezasPuzzle [k, i].parent.transform.position += posicionActual - posicionAnterior;
			}
			AgruparPiezas (k, i, "Derecha");
		}
		if (i > 0 && distanciaEntrePiezas [k, i].superior <= Mathf.Abs (piezasPuzzle [k, i - 1].position.y - piezasPuzzle [k, i].position.y) + margenSnap
		     && distanciaEntrePiezas [k, i].superior >= Mathf.Abs (piezasPuzzle [k, i - 1].position.y - piezasPuzzle [k, i].position.y) - margenSnap
		     && piezasPuzzle [k, i].position.x <= piezasPuzzle [k, i - 1].position.x + margenSnap
			 && piezasPuzzle [k, i].position.x >= piezasPuzzle [k, i - 1].position.x - margenSnap
			 && piezasPuzzle [k, i].position.y < piezasPuzzle [k, i - 1].position.y) {
			tieneSuperior = true;

			Vector3 posicionAnterior = piezasPuzzle [k, i].transform.position;
			piezasPuzzle [k, i].transform.position = new Vector3 (piezasPuzzle [k, i - 1].position.x, piezasPuzzle [k, i - 1].position.y - distanciaEntrePiezas [k, i - 1].inferior, piezasPuzzle [k, i - 1].position.z);
			Vector3 posicionActual = piezasPuzzle [k, i].transform.position;

			if (piezasPuzzle [k, i].parent.name == "GrupoPiezas") {
				piezasPuzzle [k, i].transform.position = posicionAnterior;
				piezasPuzzle [k, i].parent.transform.position += posicionActual - posicionAnterior;
			}
			AgruparPiezas (k, i, "Arriba");
		}
		if (i < altoPuzzle - 1 && distanciaEntrePiezas [k, i].inferior <= Mathf.Abs (piezasPuzzle [k, i].position.y - piezasPuzzle [k, i + 1].position.y) + margenSnap
		     && distanciaEntrePiezas [k, i].inferior >= Mathf.Abs (piezasPuzzle [k, i].position.y - piezasPuzzle [k, i + 1].position.y) - margenSnap
		     && piezasPuzzle [k, i].position.x <= piezasPuzzle [k, i + 1].position.x + margenSnap
			 && piezasPuzzle [k, i].position.x >= piezasPuzzle [k, i + 1].position.x - margenSnap
			 && piezasPuzzle [k, i].position.y > piezasPuzzle [k, i + 1].position.y) {
			tieneInferior = true;

			Vector3 posicionAnterior = piezasPuzzle [k, i].transform.position;
			piezasPuzzle [k, i].transform.position = new Vector3 (piezasPuzzle [k, i + 1].position.x, piezasPuzzle [k, i + 1].position.y + distanciaEntrePiezas [k, i + 1].superior, piezasPuzzle [k, i + 1].position.z);
			Vector3 posicionActual = piezasPuzzle [k, i].transform.position;

			if (piezasPuzzle [k, i].parent.name == "GrupoPiezas") {
				piezasPuzzle [k, i].transform.position = posicionAnterior;
				piezasPuzzle [k, i].parent.transform.position += posicionActual - posicionAnterior;
			}
			AgruparPiezas (k, i, "Abajo");
		}
		AjustarPiezaAlLienzo ();
		CheckearCompleted ();
	}

	void AjustarPiezaAlLienzo(){
		bool colocaAlgunaPieza = false;
		for (int i = 0; i < altoPuzzle; i++) {
			for (int k = 0; k < anchoPuzzle; k++) { //Si no es PiezaColocada, o si lo es, pero está mal posicionada
				if (piezasPuzzle [k, i].tag != "PiezaColocada" ||
				   (piezasPuzzle [k, i].tag != "PiezaColocada"
				   && piezasPuzzle [k, i].position.x != posicionInicial [k, i].x
				   && piezasPuzzle [k, i].position.y != posicionInicial [k, i].y)) {
					if (Vector2.Distance (piezasPuzzle [k, i].position, posicionInicial [k, i]) < margenSnap) { 
						piezasPuzzle [k, i].tag = "PiezaColocada";
						piezasSinColocar.Remove (piezasPuzzle [k, i].gameObject);
						piezasPuzzle [k, i].GetChild (0).GetComponent<MeshRenderer> ().enabled = false;
						colocaAlgunaPieza = true;
						if (piezasPuzzle [k, i].parent.name == "GrupoPiezas") {
							Transform padreADestruir = piezasPuzzle [k, i].parent;
							while (padreADestruir.childCount > 0) {
								padreADestruir.GetChild (0).SetParent (padreADestruir.parent);
							}
							if (padreADestruir.childCount == 0) {
								Destroy (padreADestruir.gameObject);
							}
						}
						piezasPuzzle [k, i].position = new Vector3 (posicionInicial [k, i].x, posicionInicial [k, i].y, 1);
					}
				} 
			}
		}

		if (colocaAlgunaPieza) {
			GameObject.Find("app_wall").GetComponent<App_wall>().play_sound(4);
		}
	}

	void CheckearCompleted(){
		if (piezasSinColocar.Count == 0) {
			switch (altoPuzzle) {
			case 4: //Noob
				if (PlayerPrefs.GetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 0) < 1) {
					PlayerPrefs.SetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 1);
				}
				break;
			case 6: //Facil
				if (PlayerPrefs.GetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 0) < 2) {
					PlayerPrefs.SetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 2);
				}
				break;
			case 10: //Medio
				if (PlayerPrefs.GetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 0) < 3) {
					PlayerPrefs.SetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 3);
				}
				break;
			case 14: //Dificil
				if (PlayerPrefs.GetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 0) < 4) {
					PlayerPrefs.SetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 4);
				}
				break;
			case 16: //Epico
				if (PlayerPrefs.GetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 0) < 5) {
					PlayerPrefs.SetInt ("puzzleCompleto" + controlUI.puzzlePreseleccionado, 5);
				}
				break;
			}

			GameObject.Find("app_wall").GetComponent<App_wall>().play_sound(1);
			controlUI.ActivarPanelCompleto ();
		}
	}

	void AgruparPiezas(int k, int i, string tieneLado){
		int kk = k;
		int ii = i;
		if (tieneLado == "Izquierda") {
			kk = kk - 1;
		} else if (tieneLado == "Derecha") {
			kk = kk + 1;
		} else if (tieneLado == "Arriba") {
			ii = ii - 1;
		} else if (tieneLado == "Abajo") {
			ii = ii + 1;
		}


			//Si AMBOS tienen grupos diferentes
			if (piezasPuzzle [k, i].parent.name == "GrupoPiezas"
				&& piezasPuzzle [kk, ii].parent.name == "GrupoPiezas"
				&& piezasPuzzle [k, i].parent != piezasPuzzle [kk, ii].parent) {

				Transform antiguoParent = piezasPuzzle [kk, ii].parent;
				Transform[] hijosAReorganizar = antiguoParent.GetComponentsInChildren<Transform> ();
				for (int j = 0; j < hijosAReorganizar.Length; j++) {
					if (hijosAReorganizar [j].name != "Sombra") {
						hijosAReorganizar [j].SetParent (piezasPuzzle [k, i].parent);
					}
				}
			if(!controlUI.fixGrupoSFX.isPlaying && !controlUI.fixFondoSFX.isPlaying) controlUI.fixGrupoSFX.Play ();
				Destroy (antiguoParent.gameObject);
			} 

			//Si solo ESTE tiene grupo
			else if (piezasPuzzle [k, i].parent.name == "GrupoPiezas"
				&& piezasPuzzle [kk, ii].parent.name != "GrupoPiezas") {

				piezasPuzzle [kk, ii].SetParent (piezasPuzzle [k, i].parent);
			if(!controlUI.fixGrupoSFX.isPlaying && !controlUI.fixFondoSFX.isPlaying) controlUI.fixGrupoSFX.Play ();
			} 
			//Si solo el OTRO tiene grupo
			else if (piezasPuzzle [k, i].parent.name != "GrupoPiezas"
				&& piezasPuzzle [kk, ii].parent.name == "GrupoPiezas") {

				piezasPuzzle [k, i].SetParent (piezasPuzzle [kk, ii].parent);
			if(!controlUI.fixGrupoSFX.isPlaying && !controlUI.fixFondoSFX.isPlaying) controlUI.fixGrupoSFX.Play ();
			} 
			//Si NINGUNO tiene grupo
			else if (piezasPuzzle [k, i].parent.name != "GrupoPiezas"
				&& piezasPuzzle [kk, ii].parent.name != "GrupoPiezas"
				&& piezasPuzzle[k,i].tag != "PiezaColocada"
				&& piezasPuzzle[kk,ii].tag != "PiezaColocada") {

				GameObject nuevoGrupo = new GameObject();
				piezasPuzzle [k, i].SetParent (nuevoGrupo.transform);
				piezasPuzzle [kk, ii].SetParent (nuevoGrupo.transform);
				nuevoGrupo.name = "GrupoPiezas";
				nuevoGrupo.transform.SetParent (this.transform);
			if(!controlUI.fixGrupoSFX.isPlaying && !controlUI.fixFondoSFX.isPlaying) controlUI.fixGrupoSFX.Play ();
			}
	}
}
