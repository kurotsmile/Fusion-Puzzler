using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargarShapeSombra : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("AUTOCOLOCAR LA SOMBRA DEBE SER TEMPORAL. Más adelante sustituirlo colocando la sombra a mano en el inspector");
		GetComponent<MeshFilter> ().mesh = transform.parent.GetComponent<MeshFilter> ().mesh;
	
	}
}
