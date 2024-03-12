using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverPiezas : MonoBehaviour {

	bool estaSeleccionado;

	float distanciaRayo; //Al posicionar click la calculamos
	GameObject pieza; //Pieza que al hacer click se asigna y moveremos.
	Transform[] piezasMovidas;
	[HideInInspector]
	public JuntarPiezas juntarPiezas;
	[HideInInspector]
	public float posicionZ = -0.001f;
	[HideInInspector]
	public bool puzzlePlaying;
	Vector3 offset;

	void Update () {
		//PULSAR RATON
		if (puzzlePlaying) {
			if (Input.GetMouseButtonDown(0)) {
				PosicionarClick ();
			}

			//MANTENER RATON
			if (estaSeleccionado) {
				MoverPieza ();
			}

			//SOLTAR RATON
			if (Input.GetMouseButtonUp (0)) {
				SoltarPieza ();
			}
		}
	}

	void PosicionarClick(){
		Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition); 	//Rayo que va desde la camara hasta el punto
		RaycastHit hit;
		if (Physics.Raycast (rayo, out hit)) {							//Rayo y hayamos el hit si es que hay
			if (hit.collider.gameObject.tag == "PiezaPuzzle") {
				posicionZ -= 0.01f;
				distanciaRayo = hit.distance;
				pieza = hit.collider.gameObject;
				offset = hit.point-hit.collider.gameObject.transform.position;
				offset.z = 0;
				hit.collider.transform.GetChild (0).GetComponent<MeshRenderer> ().enabled = false; //Desactivamos la sombra
				if (pieza.transform.parent.name == "GrupoPiezas") {
					BoxCollider[]hijosARecolocarBox = pieza.transform.parent.GetComponentsInChildren<BoxCollider> ();
					Transform[] hijosARecolocar = new Transform[hijosARecolocarBox.Length];
					for (int i = 0; i < hijosARecolocar.Length; i++) {
						hijosARecolocar[i] = hijosARecolocarBox [i].transform;
						hijosARecolocar [i].GetChild (0).GetComponent<MeshRenderer> ().enabled = false; //Desactivamos las sombras
					}

					piezasMovidas = new Transform[hijosARecolocar.Length];
					for (int i = 0; i < piezasMovidas.Length; i++) {
						piezasMovidas [i] = hijosARecolocar [i];
					}
					hijosARecolocar = piezasMovidas; //Estas 4 lineas para evitar que se coja al padre como [0]

					Vector3[] posicionRelativa = new Vector3[hijosARecolocar.Length];
					GameObject nuevoParent = pieza.transform.parent.gameObject;

					for (int i = 0; i < hijosARecolocar.Length; i++) {
						if (hijosARecolocar [i].tag == "PiezaPuzzle" && hijosARecolocar [i] != pieza.transform) {
							posicionRelativa [i] = hijosARecolocar [i].position - pieza.transform.position;
							hijosARecolocar [i].transform.parent = nuevoParent.transform.parent;
						}
					}

					nuevoParent.transform.position = Input.mousePosition;

					for (int i = 0; i < hijosARecolocar.Length; i++) {
						if (hijosARecolocar [i].tag == "PiezaPuzzle") {
							hijosARecolocar [i].transform.parent = nuevoParent.transform;
							hijosARecolocar [i].localPosition = posicionRelativa [i];
						}
					}

					pieza = nuevoParent;
				} else {
					piezasMovidas = new Transform[1];
					piezasMovidas [0] = pieza.transform;
				}
				estaSeleccionado = true;
			}
		}
	}

	void MoverPieza(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Vector3 limiteRayo = ray.GetPoint (distanciaRayo);
		limiteRayo = new Vector3 (limiteRayo.x, limiteRayo.y, posicionZ);
		pieza.transform.position = limiteRayo - offset;
	}

	void SoltarPieza(){
		if(estaSeleccionado){
			pieza.transform.position = new Vector3 (pieza.transform.position.x, pieza.transform.position.y, posicionZ);


			for (int i = 0; i < piezasMovidas.Length; i++) {
				if(piezasMovidas [i].tag != "PiezaColocada"){
					piezasMovidas [i].GetChild (0).GetComponent<MeshRenderer> ().enabled = true; //Reactivamos las sombras
				}
				string numDePieza = piezasMovidas[i].gameObject.name.Substring (6, piezasMovidas[i].gameObject.name.Length-6);
				string[] numsPiezas = numDePieza.Split('x');
				juntarPiezas.ReajustarPiezas (int.Parse(numsPiezas [1]), int.Parse(numsPiezas [0])); //Las piezas estan numeradas al reves, primero el orden vertical, luego el horizontal
			}

			if (pieza.name == "GrupoPiezas") {
				foreach (Transform piezaHija in pieza.transform) {
					piezaHija.position = new Vector3 (piezaHija.position.x, piezaHija.position.y, posicionZ);
				}
			}

			pieza = null;
			piezasMovidas = new Transform[0];
			estaSeleccionado = false;
		}
	}
}
