using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DescargarImagenes : MonoBehaviour {

	public string list_url;

	[HideInInspector] public int cantidadPuzzlesTotal;
	[HideInInspector] public bool cargaronEnlacesDeInternet;
	string[] listaEnlacesImagen;
	UnityWebRequest[] wwwImagen;
	public UnityWebRequest wwwEnlacesImagenes;
	public AsyncOperation async;
	public List<Texture2D> puzzleImageList;
	ControlUI controlUI;
	[HideInInspector] public bool conectadoPorWifi;
	[HideInInspector] public bool sinConexion;
	bool descargaTimedOut;


	void Start(){
		controlUI = GetComponent<ControlUI> ();
		cantidadPuzzlesTotal = 0;//PlayerPrefs.GetInt ("cantidadPuzzlesTotal", 0);
		StartCoroutine(RutinaCheckInternet ());
	}

	IEnumerator RutinaCheckInternet(){
		while (true) {
			CheckInternet ();
			yield return new WaitForSeconds (60);
		}
	}

	public bool CheckInternet(){
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			sinConexion = true;
			conectadoPorWifi = false;
			return false;
		} else {
			sinConexion = false;
			if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
				conectadoPorWifi = true;
			} else {
				conectadoPorWifi = false;
			}
			return true;
		}
	}

	public void CargarLista(){CargarPack ();}

	public void CargarPack () {
			cargaronEnlacesDeInternet = true;
		string listaBrutaEnlaces = list_url;
			listaEnlacesImagen = listaBrutaEnlaces.Split (';');
			if (listaEnlacesImagen [listaEnlacesImagen.Length - 1].Trim() == "") {
				List<string> listaEnlacesAux = new List<string>();
				for (int i = 0; i < listaEnlacesImagen.Length - 1; i++) {
					listaEnlacesAux.Add (listaEnlacesImagen [i]);
				}
				listaEnlacesImagen = new string[(listaEnlacesImagen.Length - 1)];
				for (int i = 0; i < listaEnlacesImagen.Length; i++) {
					listaEnlacesImagen [i] = listaEnlacesAux [i];
				}
			}
			wwwImagen = new UnityWebRequest[listaEnlacesImagen.Length];
			cantidadPuzzlesTotal = listaEnlacesImagen.Length;
			//PlayerPrefs.SetInt("cantidadPuzzlesTotal", cantidadPuzzlesTotal);
			StartCoroutine (DescargarPack ());
	}

	Texture2D CargarImagenPlayerPrefs(int numeroPuzzle){
		Texture2D texturaCargada = ReadTextureFromPlayerPrefs (numeroPuzzle);
		return texturaCargada;
	}


	public void WriteTextureToPlayerPrefss (int numeroPuzzle, Texture2D tex){
		byte[] texByte = tex.EncodeToJPG ();
		string base64Tex = System.Convert.ToBase64String (texByte);
		PlayerPrefs.SetString ("puzzleGuardado" + numeroPuzzle, base64Tex);
		PlayerPrefs.Save ();
		Debug.Log("Imagen "+ numeroPuzzle +" guardada");
	}

	Texture2D ReadTextureFromPlayerPrefs (int numeroPuzzle){
		string base64Tex = PlayerPrefs.GetString ("puzzleGuardado" + numeroPuzzle, null);
		if (!string.IsNullOrEmpty (base64Tex)) {
			byte[] texByte = System.Convert.FromBase64String (base64Tex);
			Texture2D tex = new Texture2D (2, 2);
			if (tex.LoadImage (texByte)) {
				return tex;
			}
		}
		return null;
	}

    [System.Obsolete]
    IEnumerator DescargarPack(){
		int cantidad = 4 + Mathf.Clamp(controlUI.packsCargados,0,1);

		//Si empezó offline, sin lista, aquí la descarga cuando hay internet
		if (!cargaronEnlacesDeInternet && CheckInternet()) {
			CargarLista ();
			yield break;
		}

		/*
		while (controlUI.posicionPrimero + cantidad > cantidadPuzzlesTotal) {
			cantidad--;
			if (cantidad <= 0) {
				Debug.Log ("Its trying to load more puzzles than we have.");
			}
		}*/
		controlUI.imagenBoton[0].gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive (false);
		controlUI.imagenBoton[1].gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive (false);
		controlUI.imagenBoton[2].gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive (false);
		for (int i = 0; i < controlUI.posicionPrimero + cantidad; i++) {
			
			if (i > cantidadPuzzlesTotal - 1) {
				break;
			}
			//if (CheckInternet () && PlayerPrefs.GetString ("puzzleGuardado" + i, "") == "") {
				listaEnlacesImagen [i] = listaEnlacesImagen [i].Trim ();
				Debug.Log("img "+listaEnlacesImagen[i]);
				wwwImagen [i] = new UnityWebRequest (listaEnlacesImagen [i]);
				StartCoroutine (LimiteTiempoDescarga (i));
				yield return wwwImagen [i];
				if (descargaTimedOut || !string.IsNullOrEmpty(wwwImagen[i].error)) {
					if (!string.IsNullOrEmpty (wwwImagen [i].error)) {
						Debug.Log ("Download error: " + wwwImagen [i].error+". Image "+i+" has not been loaded.");
					} else {
						Debug.Log ("It took so long to load. Image "+i+" has not been loaded.");
					}
					descargaTimedOut = false;
					for (int borrarErrores = i-1; borrarErrores >= controlUI.posicionPrimero; borrarErrores--) {
						puzzleImageList.RemoveAt (borrarErrores);
						controlUI.imagenPuzzleGrises.RemoveAt (borrarErrores);
					}
					controlUI.ErrorDuranteDescarga ();
					yield break;
				}

				Debug.Log ("Downloading image " + i + " from that link: " + listaEnlacesImagen [i]);
				//WriteTextureToPlayerPrefs (i, wwwImagen [i].texture);
				//puzzleImageList.Add (wwwImagen [i].texture);
				controlUI.TexturaABoton (i, puzzleImageList [i]);
				controlUI.imagenBoton[i].gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive (true);
			/*} else {
				if (PlayerPrefs.GetString ("puzzleGuardado" + i, "") != "") {
					puzzleImageList.Add (CargarImagenPlayerPrefs (i));

					Debug.Log ("Image " + i + " loaded from prefs.");
					controlUI.TexturaABoton (i, puzzleImageList [i]);
				} else {
					Debug.Log ("There is no internet nor a saved image. This should never happen.");
				}
			}*/
			yield return null;
		}
		controlUI.ActivarAnimBoton ();
	}

	IEnumerator LimiteTiempoDescarga (int i){
		float timer = 0;
		float timeOut = 10f; //Tiempo maximo para poder descargar una imagen
		descargaTimedOut = false;

		while (!wwwImagen [i].isDone) {
			if (timer > timeOut) {
				descargaTimedOut = true;
				wwwImagen [i].Dispose ();
				break;
			}
			timer += Time.deltaTime;
			yield return null;
		}
	}

	IEnumerator LimiteTiempoDescargaLista (){
		float timer = 0;
		float timeOut = 10f; //Tiempo maximo para poder descargar una imagen
		descargaTimedOut = false;

		while (!wwwEnlacesImagenes.isDone) {
			if (timer > timeOut) {
				descargaTimedOut = true;
				wwwEnlacesImagenes.Dispose ();
				break;
			}
			timer += Time.deltaTime;
			yield return null;
		}

		if (descargaTimedOut) {
			controlUI.ErrorDuranteDescarga ();
		}
		yield return null;
	}
}
