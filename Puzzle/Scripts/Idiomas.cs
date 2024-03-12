using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Idiomas : MonoBehaviour {


	string idioma;

	public Text play;
	public Text adv;
	public Text areYouSure;
	public Text avisoTutorial;
	public Text cancel;
	public Text clasificar;
	public Text complete;
	public Text connectInternet1;
	public Text connectInternet2;
	public Text download;
	public Text easy;
	public Text epic;
	public Text guide;
	public Text hard;
	public Text medio;
	public Text preview;
	public Text regroup;
	public Text start;
	public Text yes;



	void Start () {
		idioma = Application.systemLanguage.ToString();

		if (idioma == "Spanish") {	//Si no es Spanish no hace nada, ya que por defecto viene en inglés.
			PonerEnEsp ();
		} 
	}


	void PonerEnEsp(){
		play.text = "JUGAR";
		play.fontSize = 52;
		areYouSure.text = "Vas a volver al menu inicial.\n\nEstas de acuerdo?";
		avisoTutorial.text = "Hey! Has visto el menu de abajo?\n\nPuedes separar las piezas del borde de las centrales.";
		cancel.text = "Cancelar";
		clasificar.text = "BORDES";
		clasificar.fontSize = 22;
		complete.text = "PUZLE\nCOMPLETADO!";
		complete.fontSize = 34;
		connectInternet1.text = "Por favor, conectate a internet para continuar.";
		connectInternet2.text = "Por favor, conectate a internet para continuar.";
		download.text = "Descargar";
		easy.text = "FACIL\n16pcs";
		medio.text = "MEDIO\n24pcs";
		adv.text = "DIFICIL\n60pcs";
		hard.text = "PRO\n112pcs";
		epic.text = "EPICO\n160pcs";
		guide.text = "GUIA";
		preview.text = "VISTA PREVIA";
		preview.fontSize = 22;
		regroup.text = "REAGRUPAR";
		regroup.fontSize = 22;
		start.text = "INICIAR";
		start.fontSize = 36;
		yes.text = "SI";
	}

}
