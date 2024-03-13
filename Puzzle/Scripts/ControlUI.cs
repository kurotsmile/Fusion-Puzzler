using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour{

	DescargarImagenes descargarImagenes;
	Analiticas analiticas;

	public GameObject panelInicial;
	public GameObject panelSeleccion;
	public GameObject panelPreGame;
	public GameObject panelInGame;
	public GameObject panelComplete;
	public GameObject panel_menu_home;

	public GameObject[] botonContinue;
	public GameObject[] packBotones;
	public Image[] imagenBoton;
	Image[] contornoBoton;
	public List<Texture2D> imagenPuzzleGrises;
	Color[] colorDificultad;

	public Button botonPlay;
	public int packsCargados = 0;
	public int posicionPrimero = 0;
	public RectTransform panelScroll;
	ScrollRect panelScrollComponent;
	public Image[] pedazoVistaPrevia;
	public Image[] tickDificultad; 
	int ultimaDificultadSeleccionada = 0;
	public int puzzlePreseleccionado;

	bool mensajeCheckeoWifi;
	public Image avisoDescarga;
	public Image avisoConectarse;
	public Image avisoConectarseInGame;
	public Image avisoTutorial;

	bool yaPidioAyudaBackground;
	bool yaPidioSepararPiezas;


	bool ayudaActivada;
	public SpriteRenderer bgAyuda;

	bool menuIngameActivado;
	public RectTransform flechitaMenuInGame;
	public Button botonMenuInvisible;

	public GameObject seguroMenu;
	int ultimoNumImagen = -1;

	Sprite spritePreseleccionado;
	public Image miniaturaDeAyuda;

	public Text completeInfo;
	float horaInicio;
	int tiempoEnPuzzle;

	float contadorTiempoAnuncio = 0;
	float tiempoParaAnuncio = 480; //8 minutos
	bool puedeMostrarAnuncio;
	bool puedeMostrarAnuncioExtra;

	public GameObject[] prefabsPuzzle;
	GameObject nuevoPuzzle;

	public GameObject tickSort;
	public GameObject tickGuia;
	public GameObject notaTachada;

	Sprite[] imagenesAColor;
	Sprite[] imagenesEnGris;

	public AudioSource fixGrupoSFX;
	public AudioSource fixFondoSFX;
	void Start () {

		descargarImagenes = GetComponent<DescargarImagenes> ();
		analiticas = GetComponent<Analiticas> ();
		panelScrollComponent = panelScroll.GetComponent<ScrollRect> ();
		contornoBoton = new Image[imagenBoton.Length];
		for (int i = 0; i < imagenBoton.Length; i++) {
			contornoBoton[i] = imagenBoton [i].transform.parent.parent.GetComponent<Image>();
		}

		imagenesAColor = new Sprite[imagenBoton.Length] ;
		imagenesEnGris = new Sprite[imagenBoton.Length] ;

		colorDificultad = new Color[6];
		colorDificultad [0] = new Color (0.93f, 0.93f, 0.93f, 1); //Gris
		colorDificultad [1] = new Color (0.73f, 1, 0.79f, 1); //Verde 
		colorDificultad [2] = new Color (1, 1, 0.73f, 1); //Amarillo 
		colorDificultad [3] = new Color (1, 0.87f, 0.73f, 1); //Naranja 
		colorDificultad [4] = new Color (1, 0.73f, 0.73f, 1); //Rojo
		colorDificultad [5] = new Color (0.87f, 0.73f, 1, 1); //Morado 


	}

	void Update(){
		contadorTiempoAnuncio += Time.deltaTime;
		if (contadorTiempoAnuncio >= tiempoParaAnuncio * 2 && !puedeMostrarAnuncioExtra) {
			puedeMostrarAnuncioExtra = true;
		} else if (contadorTiempoAnuncio >= tiempoParaAnuncio && !puedeMostrarAnuncio) {
			puedeMostrarAnuncio = true;
		}
	}

	void AjustarNuevoPack(){
		Debug.Log ("There is a total of " + descargarImagenes.cantidadPuzzlesTotal + " images");
		if (packsCargados == 1) {
			for (int i = 0; i < packBotones [packsCargados - 1].transform.childCount; i++) {
				if (i + 1 > descargarImagenes.cantidadPuzzlesTotal) {
					packBotones [0].transform.GetChild (i).gameObject.SetActive (false);
				}
			}
		} else if (packsCargados == 2) {
			for (int i = 0; i < packBotones [packsCargados - 1].transform.childCount; i++) {
				if (i + 1 + 4/*4 del primer pack*/ > descargarImagenes.cantidadPuzzlesTotal) {
					packBotones [packsCargados - 1].transform.GetChild (i).gameObject.SetActive (false);
				}
			}
		} else if (packsCargados >= 3) {
			for (int i = 0; i < packBotones [packsCargados - 1].transform.childCount; i++) {
				if (i + 1 + 4 + (5*(packsCargados-2))/*5 de los siguientes pack, -2 porque ya vamos por el pack 3*/ > descargarImagenes.cantidadPuzzlesTotal) {
					packBotones [packsCargados - 1].transform.GetChild (i).gameObject.SetActive (false);
				}
			}
		}

		AjustarLimitesScrollVertical ();
	}

	void AjustarLimitesScrollVertical(){
		if (packsCargados >= 2 &&panelScrollComponent.enabled == false) { //Activamos el mover por scroll a partir del segundo pack
			panelScrollComponent.enabled = true;
		}
		panelScroll.offsetMin = new Vector2 (0,panelScroll.offsetMax.y + (-140 * (packsCargados-1) - 20* (packsCargados -2))); //Calibrado para dejar un margen abajo
	}

	public void CargarMasPuzzles () {
		this.panel_menu_home.SetActive (true);
		packsCargados = 0;
		//if (!cargandoNuevoPack) {
			//PARA EL PACK 0
			if (packsCargados == 0) {
				botonPlay.interactable = false;
				if (!descargarImagenes.CheckInternet ()) {
					bool tienePackCompletoEnPrefs = true;
					for (int i = 0; i < 4; i++) {
						if (PlayerPrefs.GetString ("puzzleGuardado" + i, "") == "") {
							tienePackCompletoEnPrefs = false;
						}
					}
					if (tienePackCompletoEnPrefs) {
						descargarImagenes.CargarPack ();
						botonPlay.transform.GetChild (0).GetComponent<Animator> ().enabled = true;
					} else {
						Debug.Log ("No tiene todo el pack guardado");
						this.GetComponent<App_wall>().play_sound(6);
						ErrorDuranteDescarga ();
					}
					
				} else {
					
					bool tienePackCompletoEnPrefs = true;
					for (int i = 0; i < 4; i++) {
						if (PlayerPrefs.GetString ("puzzleGuardado" + i, "") == "") {
							tienePackCompletoEnPrefs = false;
						}
					}

					if (tienePackCompletoEnPrefs) {
						
						//cargandoNuevoPack = true;
						descargarImagenes.CargarPack ();
						botonPlay.transform.GetChild (0).GetComponent<Animator> ().enabled = true;
					} else if (!descargarImagenes.conectadoPorWifi && !mensajeCheckeoWifi) {
						this.GetComponent<App_wall>().play_sound(6);
						avisoDescarga.gameObject.SetActive (true);
						
					} else {
						descargarImagenes.CargarPack ();
						botonPlay.transform.GetChild (0).GetComponent<Animator> ().enabled = true;
					}
				}

			} else { //PARA EL RESTO DE PACKS
				if (!descargarImagenes.CheckInternet ()) {
					bool tienePackCompletoEnPrefs = true;
					for (int i = 0; i < 5; i++) {
						if (PlayerPrefs.GetString ("puzzleGuardado" + (i - 1 + packsCargados * 5), "") == "") {
							tienePackCompletoEnPrefs = false;
						}
					}
					if (tienePackCompletoEnPrefs) {
						descargarImagenes.CargarPack ();
						botonContinue [packsCargados - 1].transform.GetChild (0).GetComponent<Animator> ().enabled = true;
					} else {
						Debug.Log ("No tiene todo el pack guardado");
						this.GetComponent<App_wall>().play_sound(6);
						ErrorDuranteDescarga ();
					}
				} else {
					bool tienePackCompletoEnPrefs = true;
					for (int i = 0; i < 5; i++) {
						if (PlayerPrefs.GetString ("puzzleGuardado" + (i - 1 + packsCargados * 5), "") == "") {
							tienePackCompletoEnPrefs = false;
						}
					}
					if (tienePackCompletoEnPrefs) {
						descargarImagenes.CargarPack ();
						botonContinue [packsCargados - 1].transform.GetChild (0).GetComponent<Animator> ().enabled = true;
					} else if (!descargarImagenes.conectadoPorWifi && !mensajeCheckeoWifi) {
						this.GetComponent<App_wall>().play_sound(6);
						avisoDescarga.gameObject.SetActive (true);
					} else {
						descargarImagenes.CargarPack ();
						botonContinue [packsCargados - 1].transform.GetChild (0).GetComponent<Animator> ().enabled = true;
					}
				}
			}
		//}
	}

	public void AceptarDescargaWifi(){
		mensajeCheckeoWifi = true;
		if (packsCargados == 0) {
				descargarImagenes.CargarLista ();
				botonPlay.transform.GetChild (0).GetComponent<Animator> ().enabled = true;

		} else {
			descargarImagenes.CargarPack ();
			botonContinue [packsCargados - 1].transform.GetChild (0).GetComponent<Animator> ().enabled = true;
		}
		this.GetComponent<App_wall>().play_sound(0);
		avisoDescarga.gameObject.SetActive (false);
	}

	public void CancelarDescargaWifi(){
		if (packsCargados > 0) {
			botonContinue [packsCargados - 1].transform.GetChild (0).GetComponent<Animator> ().enabled = false;
			botonContinue [packsCargados - 1].transform.GetChild (0).GetChild (0).GetComponent<Text> ().enabled = true;
			botonContinue [packsCargados - 1].transform.GetChild (0).GetChild (1).GetComponent<Image> ().enabled = false;
		} else if (packsCargados == 0) {
			DevuelveBotonPlay ();
			botonPlay.interactable = true;
		}
		this.GetComponent<App_wall>().play_sound(0);
		avisoDescarga.gameObject.SetActive (false);
	}

	public void ActivarAnimBoton (){
		Invoke ("MostrarNuevoPack", 0.2f);
	}

	void MostrarNuevoPack(){
		this.GetComponent<App_wall>().play_sound(7);
		if(packsCargados > 0){ //Desactiva el boton anterior
			botonContinue [packsCargados - 1].SetActive (false);
		}
		packBotones [packsCargados].SetActive(true);
		posicionPrimero = posicionPrimero + 4 + Mathf.Clamp (packsCargados, 0, 1);
		packsCargados++;
		AjustarNuevoPack ();
		if (packsCargados == 1) { //Vuelve a escribir PLAY en el primer boton cuando ya cargó el 2º pack
			DevuelveBotonPlay ();
		}
	}

	public void TexturaABoton(int numeroBoton, Texture imagenTex){
		Texture texImagenBoton = Instantiate (imagenTex) as Texture;
		Sprite spriteImagen = ConvertirASprite(numeroBoton, texImagenBoton);
		imagenBoton[numeroBoton].sprite = spriteImagen;
		contornoBoton [numeroBoton].color = colorDificultad[PlayerPrefs.GetInt("puzzleCompleto"+numeroBoton, 0)];
	}
		
	Sprite ConvertirASprite(int numeroBoton, Texture imagenTex){
		Texture2D imagenTex2D = imagenTex as Texture2D;
		if(PlayerPrefs.GetInt("puzzleCompleto"+numeroBoton, 0) < 5){
			imagenTex2D = ConvertToGrayscale(imagenTex2D);
		}
		imagenPuzzleGrises.Add (imagenTex2D);
		Rect rect = new Rect (0, 0, imagenTex2D.width, imagenTex2D.height);
		return Sprite.Create (imagenTex2D, rect, Vector2.zero, imagenTex2D.width);
	}

	public Sprite ConversionRapidaASprite(Texture imagenTex){
		Texture2D imagenTex2D = imagenTex as Texture2D;
		Rect rect = new Rect (0, 0, imagenTex2D.width, imagenTex2D.height);
		return Sprite.Create (imagenTex2D, rect, Vector2.zero, imagenTex2D.width);
	}

	Texture2D ConvertToGrayscale(Texture2D imagenTex2D)
	{
		float lightness = 0.2f;
		Color32[] pixels = imagenTex2D.GetPixels32();
		for (int x=0;x<imagenTex2D.width;x++)
		{
			for (int y=0;y<imagenTex2D.height;y++)
			{
				Color32 pixel = pixels[x+y*imagenTex2D.width];
				int p =  ( (256 * 256 + pixel.r) * 256 + pixel.b) * 256 + pixel.g;
				int b = p % 256;
				p = Mathf.FloorToInt(p / 256);
				int g = p % 256;
				p = Mathf.FloorToInt (p / 256);
				int r = p % 256;
				float l = (0.2126f*r/255f) + 0.7152f*(g/255f) + 0.0722f*(b/255f);
				Color c = new Color(l+lightness,l+lightness,l+lightness,1); //
				imagenTex2D.SetPixel(x,y,c);
			}
		}
		imagenTex2D.Apply(true);
		return imagenTex2D;
	}

	public void CerrarAvisoConectarse(){
		this.GetComponent<App_wall>().play_sound(0);
		avisoConectarse.gameObject.SetActive (false);
		avisoConectarseInGame.gameObject.SetActive (false);
	}

	void ActivarAvisoTutorial(){
		if (panelInGame.activeSelf) {
			avisoTutorial.gameObject.SetActive (true);
		}
	}

	public void CerrarAvisoTutorialAbriendoMenuInGame(){
		CerrarAvisoTutorial ();
		ActivarMenuInGame ();
	}
	public void CerrarAvisoTutorial(){
		this.GetComponent<App_wall>().play_sound(0);
		PlayerPrefs.SetInt ("yaUsoBotonSort", 1);
		avisoTutorial.gameObject.SetActive (false);
	}

	public void ErrorDuranteDescarga() {
		if (packsCargados > 0) {
			botonContinue [packsCargados - 1].transform.GetChild (0).GetComponent<Animator> ().enabled = false;
			botonContinue [packsCargados - 1].transform.GetChild (0).GetChild (0).GetComponent<Text> ().enabled = true;
			botonContinue [packsCargados - 1].transform.GetChild (0).GetChild (1).GetComponent<Image> ().enabled = false;
		} else if (packsCargados == 0) {
			DevuelveBotonPlay ();
			botonPlay.interactable = true;
		}
		this.GetComponent<App_wall>().play_sound(6);
		avisoConectarse.gameObject.SetActive (true);
	}

	void DevuelveBotonPlay(){
		botonPlay.transform.GetChild (0).GetComponent<Animator> ().enabled = false;
		botonPlay.transform.GetChild (0).GetChild (0).GetComponent<Text> ().enabled = true;
		botonPlay.transform.GetChild (0).GetChild (1).GetComponent<Image> ().enabled = false;
	}

	public void VolverAMenu(){

		if(puedeMostrarAnuncioExtra) {
			contadorTiempoAnuncio = 0;
			puedeMostrarAnuncioExtra = false;
			puedeMostrarAnuncio = false;
			ShowRewardedAdExtra ();
		} else if(puedeMostrarAnuncio) {
			contadorTiempoAnuncio = 0;
			puedeMostrarAnuncioExtra = false;
			puedeMostrarAnuncio = false;
			ShowRewardedAdExtra ();
		}

		this.GetComponent<App_wall>().play_sound(0);
		panelInicial.SetActive (true);
		panelSeleccion.SetActive (false);
		panelPreGame.SetActive (false);
		panelInGame.SetActive (false);
		panelComplete.SetActive (false);
	}

	public void SeleccionarImagen(int numImagen){
		this.GetComponent<App_wall>().play_sound(0);
		int piezasColoreadas = 0;
		int dificultadCompletada = CargarSaveDificultades (numImagen);

		switch (dificultadCompletada) {
		case 1:
			piezasColoreadas = 2;
			break;
		case 2:
			piezasColoreadas = 5;
			break;
		case 3:
			piezasColoreadas = 7;
			break;
		case 4:
			piezasColoreadas = 10;
			break;
		case 5:
			piezasColoreadas = 12;
			break;
		default:
			break;
		}



		if (ultimoNumImagen != numImagen) {
			puzzlePreseleccionado = numImagen;
			spritePreseleccionado = ConversionRapidaASprite (descargarImagenes.puzzleImageList [puzzlePreseleccionado]);

			if (imagenesAColor [numImagen] == null || imagenesEnGris [numImagen] == null) {
				Texture2D texturaColor = Instantiate (descargarImagenes.puzzleImageList [numImagen]) as Texture2D;
				Texture2D texturaGrises = Instantiate (descargarImagenes.puzzleImageList [numImagen]) as Texture2D;

				Sprite imagenAColor = ConversionRapidaASprite (texturaColor);
				Sprite imagenEnGris = ConversionRapidaASprite (ConvertToGrayscale (texturaGrises));
				imagenesAColor [numImagen] = imagenAColor;
				imagenesEnGris [numImagen] = imagenEnGris;
			}
		}

		for (int i = 0; i < pedazoVistaPrevia.Length; i++) {
			//if (i < piezasColoreadas) {
				pedazoVistaPrevia [i].sprite = imagenesAColor [numImagen];
			//} else {
				//pedazoVistaPrevia [i].sprite = imagenesEnGris [numImagen];
			//}
		}

		panelInicial.SetActive (false);
		panelSeleccion.SetActive (true);
		panelPreGame.SetActive (false);
		panelInGame.SetActive (false);
		panelComplete.SetActive (false);

		ultimoNumImagen = numImagen;
	}

	public void show_select_by_texture(Texture txt){

		for (int i = 0; i < pedazoVistaPrevia.Length; i++) {
			//if (i < piezasColoreadas) {
			pedazoVistaPrevia [i].sprite = ConversionRapidaASprite(txt);
			//} else {
			//pedazoVistaPrevia [i].sprite = imagenesEnGris [numImagen];
			//}
		}
		spritePreseleccionado = ConversionRapidaASprite(txt);
	}

	int CargarSaveDificultades (int numImagen) {
		int dificultadCompletada = PlayerPrefs.GetInt ("puzzleCompleto" + numImagen, 0);
		for (int i = 0; i < tickDificultad.Length; i++) {
			if (i < dificultadCompletada) {
				tickDificultad [i].enabled = true;
			} else {
				tickDificultad [i].enabled = false;
			}
		}
		return dificultadCompletada;
	}

	public void IniciarPuzzle(int dificultadSeleccionada){
		this.GetComponent<App_wall>().play_sound(0);
		ultimaDificultadSeleccionada = dificultadSeleccionada;
		Vector3 posicionPuzzle = Vector3.zero; 
		switch (dificultadSeleccionada) {
		case 0:
			posicionPuzzle = new Vector3 (0.2f, -0.5f, 0);
			break;
		case 1:
			posicionPuzzle = new Vector3 (0.2f, -0.5f, 0);
			break;
		case 2:
			posicionPuzzle = Vector3.zero; 
			break;
		case 3:
			posicionPuzzle = Vector3.zero; 
			break;
		case 4:
			posicionPuzzle = Vector3.zero; 
			break;
		}

		GameObject nuevoPuzzle = Instantiate (prefabsPuzzle [dificultadSeleccionada], posicionPuzzle, prefabsPuzzle [dificultadSeleccionada].transform.rotation) as GameObject;
		nuevoPuzzle.name = prefabsPuzzle [dificultadSeleccionada].name; //Le quitamos el "(Clone)" del nombre
		this.nuevoPuzzle = nuevoPuzzle;
		panelInicial.SetActive (false);
		panelSeleccion.SetActive (false);
		panelPreGame.SetActive (true);
		panelInGame.SetActive (false);
		panelComplete.SetActive (false);
		CargarTextura ();
		PonerMiniaturaDeAyuda ();
	}

	void CargarTextura(){
		GameObject[] piezasPuzzle = GameObject.FindGameObjectsWithTag ("PiezaPuzzle");
		for (int i = 0; i < piezasPuzzle.Length; i++) {
			piezasPuzzle [i].GetComponent<Renderer> ().material.mainTexture = descargarImagenes.puzzleImageList [puzzlePreseleccionado];
		}
	}

	public void StartPuzzle(){
		MoverPiezas moverPiezas = this.gameObject.GetComponent<MoverPiezas> ();
		moverPiezas.juntarPiezas = GameObject.FindGameObjectWithTag("MatrizPuzzle").GetComponent <JuntarPiezas> ();
		DesactivarAyudaBG ();
		DesordenarPiezas (nuevoPuzzle.transform); //Barajar Piezas
		horaInicio = Time.time;
		panelInicial.SetActive (false);
		panelSeleccion.SetActive (false);
		panelPreGame.SetActive (false);
		panelInGame.SetActive (true);
		panelComplete.SetActive (false);
		Invoke ("ActivarControles", 0.5f);
		if (PlayerPrefs.GetInt("yaUsoBotonSort", 0) == 0) {
			Invoke ("ActivarAvisoTutorial", 30);
		}
		analiticas.DificultadSeleccionada (ultimaDificultadSeleccionada + 1);
		analiticas.PuzzleSeleccionado (puzzlePreseleccionado);
	}

	void ActivarControles(){
		MoverPiezas moverPiezas = this.gameObject.GetComponent<MoverPiezas> ();
		moverPiezas.puzzlePlaying = true;
	}

	void DesordenarPiezas (Transform nuevoPuzzle){ //Barajar Piezas
		this.gameObject.GetComponent<MoverPiezas> ().posicionZ = 0.0f;
		List <int> listaProfundidades = new List<int>();
		int profundidad = 0;
		foreach (Transform hijo in nuevoPuzzle) {
			profundidad = Random.Range (-nuevoPuzzle.childCount, 0);
			while (listaProfundidades.Contains (profundidad)) {
				profundidad++;
				if (profundidad > 0) {
					profundidad = -nuevoPuzzle.childCount;
				}
			}
			listaProfundidades.Add (profundidad);
			StartCoroutine (LerpHijo (hijo, profundidad * 0.001f, 2.25f));
		}
		this.GetComponent<App_wall>().play_sound(3);
	}

	IEnumerator LerpHijo (Transform pieza, float profundidad, float velocidadRecogida){
		Vector3 posInicial = pieza.position;
		Vector3 posFinal = new Vector3 (Random.Range (-2.6f, 2.6f), Random.Range (5.2f, 5.5f), profundidad);
		float t = 0;
		while(t < 0.5f){
			t += velocidadRecogida * Time.deltaTime;
			pieza.position = Vector3.Lerp (posInicial, posFinal, t*2);
			yield return null;
		}
		yield return null;
	}

	public void OrdenarPiezasRestantes(){
		this.gameObject.GetComponent<MoverPiezas> ().posicionZ = 0.0f;
		PlayerPrefs.SetInt ("yaUsoBotonSort", 1);
		List <int> listaProfundidades = new List<int>();
		int profundidad = 0;
		foreach (Transform hijo in nuevoPuzzle.transform) {
			if (hijo.tag != "PiezaColocada") {
				profundidad = Random.Range (-nuevoPuzzle.transform.childCount, 0);
				while (listaProfundidades.Contains (profundidad)) {
					profundidad++;
					if (profundidad > 0) {
						profundidad = -nuevoPuzzle.transform.childCount;
					}
				}
				listaProfundidades.Add (profundidad);
				StartCoroutine (LerpHijo (hijo, profundidad * 0.001f, 5));
			}
		}
		this.GetComponent<App_wall>().play_sound(3);
		analiticas.UsaAyudaBordes ();
	}

	public void SepararPiezasDeBorde(){
		if (yaPidioSepararPiezas) {
			SepararPiezasDeBordeAceptado ();
		} else if (descargarImagenes.CheckInternet ()) {
			if (PlayerPrefs.GetInt ("is_buy_ads", 0) == 0){
				this.GetComponent<App_wall>().carrot.ads.show_ads_Rewarded();
			}else{
				this.SepararPiezasDeBordeAceptado();
			}
		} else {
			this.GetComponent<App_wall>().play_sound(6);
			avisoConectarseInGame.gameObject.SetActive (true);
		}
	}

	public void SepararPiezasDeBordeAceptado(){
		PlayerPrefs.SetInt ("yaUsoBotonSort", 1);
		yaPidioSepararPiezas = true;
		if (!IsInvoking ("ReiniciarYaPidioSepararPiezas")) {
			Invoke ("ReiniciarYaPidioSepararPiezas", 300); //5 minutos
		}
		List <int> listaProfundidades = new List<int> ();
		int profundidad = 0;
		foreach (Transform hijo in nuevoPuzzle.transform) {
			if (hijo.tag != "PiezaColocada") {
				profundidad = Random.Range (-nuevoPuzzle.transform.childCount, 0);
				while (listaProfundidades.Contains (profundidad)) {
					profundidad++;
					if (profundidad > 0) {
						profundidad = -nuevoPuzzle.transform.childCount;
					}
				}
				listaProfundidades.Add (profundidad);
				StartCoroutine (LerpHijoBordes (hijo, profundidad * 0.001f, 5));
			}
		}
		this.GetComponent<App_wall>().play_sound(3);
		ActualizarBotonesPause ();
		analiticas.UsaAyudaBordes ();
	}

	IEnumerator LerpHijoBordes (Transform pieza, float profundidad, float velocidadRecogida){
		Vector3 posInicial = pieza.position;
		Vector3 posFinal;

		string dimensionesPuzzle = nuevoPuzzle.name.Substring (5, nuevoPuzzle.name.Length - 5);
		string[] dosDimensiones = dimensionesPuzzle.Split('x');
		int anchoPuzzle = int.Parse(dosDimensiones [0])-1;
		int altoPuzzle = int.Parse(dosDimensiones [1])-1;

		if (pieza.name.Contains ("_0x") || pieza.name.Contains ("x0") || pieza.name.Contains ("x" + anchoPuzzle) || pieza.name.Contains ("_"+altoPuzzle + "x")) {
			posFinal = new Vector3 (Random.Range (-2.6f, -1), Random.Range (5.2f, 5.5f), profundidad);
		} else {
			posFinal = new Vector3 (Random.Range (1, 2.6f), Random.Range (5.2f, 5.5f), profundidad);
		}


		float t = 0;
		while(t < 0.5f){
			t += velocidadRecogida * Time.deltaTime;
			pieza.position = Vector3.Lerp (posInicial, posFinal, t*2);
			yield return null;
		}
		yield return null;
	}

	public void VolverDesdePuzzle(bool confirmacion){
		this.GetComponent<App_wall>().play_sound(0);
		if (confirmacion) {
			MoverPiezas moverPiezas = this.gameObject.GetComponent<MoverPiezas> ();
			moverPiezas.puzzlePlaying = false;
			if (nuevoPuzzle != null) {
				Destroy (nuevoPuzzle);
			}
			SeleccionarImagen (puzzlePreseleccionado);
		}
		seguroMenu.SetActive (false);
	}

	void PonerMiniaturaDeAyuda(){
		miniaturaDeAyuda.sprite = spritePreseleccionado;
	}


	void DesactivarAyudaBG(){
		ayudaActivada = false;
		bgAyuda.sprite = null;

		ActualizarBotonesPause ();
	}

	public void ActivarAyudaBackground(){
		if (ayudaActivada) {
			ayudaActivada = false;
			bgAyuda.sprite = null;
			ActualizarBotonesPause ();
			return;
		}

		if (yaPidioAyudaBackground) {
			this.GetComponent<App_wall>().play_sound(0);
			ActivarAyudaBackgroundAceptado ();
		} else if (descargarImagenes.CheckInternet ()) {
			this.GetComponent<App_wall>().play_sound(0);
			if (PlayerPrefs.GetInt ("is_buy_ads", 0) == 0){
				this.GetComponent<App_wall>().carrot.ads.show_ads_Rewarded();
			}else{
				this.ActivarAyudaBackgroundAceptado();
			}
		} else {
			this.GetComponent<App_wall>().play_sound(6);
			avisoConectarseInGame.gameObject.SetActive (true);
		}
	}

	public void ActivarAyudaBackgroundAceptado(){
		yaPidioAyudaBackground = true;
		if (!IsInvoking ("ReiniciarYaPidioAyudaBackground")) {
			Invoke ("ReiniciarYaPidioAyudaBackground", 480); //8 minutos
		}
		ayudaActivada = !ayudaActivada;
		if (ayudaActivada) {
			bgAyuda.sprite = spritePreseleccionado;
			analiticas.UsaAyudaBG ();
		} else {
			bgAyuda.sprite = null;
		}
		ActualizarBotonesPause ();
	}

	public void ActivarMenuInGame(){
		this.GetComponent<App_wall>().play_sound(7);
		menuIngameActivado = !menuIngameActivado;
		botonMenuInvisible.gameObject.SetActive (menuIngameActivado);

		MoverPiezas moverPiezas = this.gameObject.GetComponent<MoverPiezas> ();
		moverPiezas.puzzlePlaying = !menuIngameActivado;

		if (menuIngameActivado) {
			ActualizarBotonesPause ();
			flechitaMenuInGame.rotation = Quaternion.Euler(new Vector3(0,0,180));
			if (avisoTutorial.gameObject.activeSelf) {
				CerrarAvisoTutorial ();
			}
			StartCoroutine (DesplazarMenuInGame (true));
		} else {
			flechitaMenuInGame.rotation = Quaternion.Euler(new Vector3(0,0,0));
			StartCoroutine (DesplazarMenuInGame (false));
		}
	}

	IEnumerator DesplazarMenuInGame(bool mostrarMenu){

		RectTransform panelRect = panelInGame.GetComponent<RectTransform> ();

		float posMostradoBottom = 0;
		float posMostradoTop = -235;
		float posOcultoBottom = -369;
		float posOcultoTop = -603;

		float transicion = (mostrarMenu) ? 0 : 1;
		float velocidadTransicion = 5;



		while (transicion >= 0 && transicion <= 1) {
			if (mostrarMenu) {
				transicion += Time.deltaTime * velocidadTransicion;
			} else {
				transicion -= Time.deltaTime * velocidadTransicion;
			}
			flechitaMenuInGame.anchoredPosition = new Vector2 (0, Mathf.Lerp (15, 10, transicion));
			panelRect.offsetMin = new Vector2(0,Mathf.Lerp(posOcultoBottom, posMostradoBottom, transicion));
			panelRect.offsetMax = new Vector2(0,Mathf.Lerp(posOcultoTop, posMostradoTop, transicion));
			yield return null;
		}
		yield return null;
	}

	public void VerificarBoton(string opcion){
		ActivarMenuInGame ();
		switch (opcion) {
		case "Menu":
			seguroMenu.SetActive (true);
			break;
		}
	}

	public void ActivarPanelCompleto(){
		panelInicial.SetActive (false);
		panelSeleccion.SetActive (false);
		panelPreGame.SetActive (false);
		panelInGame.SetActive (false);
		panelComplete.SetActive (true);
		tiempoEnPuzzle = Mathf.RoundToInt(Time.time - horaInicio);

		string horas = "";
		int intHoras = 0;
		string minutos = "";
		if (tiempoEnPuzzle >= 60*60) {
			horas = Mathf.Floor (tiempoEnPuzzle / 3600).ToString () + ":";
			intHoras = tiempoEnPuzzle / 3600;
		}
		if (tiempoEnPuzzle >= 60) {
			if (tiempoEnPuzzle >= 60 * 60) {
				minutos = Mathf.Floor ((tiempoEnPuzzle - (intHoras * 3600)) / 60).ToString ("00") + ":";
			} else {
				minutos = Mathf.Floor (tiempoEnPuzzle / 60).ToString () + ":";
			}
		}
		string segundos = (tiempoEnPuzzle % 60).ToString("00");

		string letraTiempo = (tiempoEnPuzzle < 3600) ? "m" : "";
		letraTiempo = (tiempoEnPuzzle < 60) ? "s" : letraTiempo;
		if (Application.systemLanguage.ToString () == "Spanish") {
			completeInfo.text = "Nivel de dificultad: " + (ultimaDificultadSeleccionada + 1).ToString () + "\n\nTiempo: " + horas + minutos + segundos + letraTiempo;
		} else {
			completeInfo.text = "Difficulty level: " + (ultimaDificultadSeleccionada + 1).ToString () + "\n\nTime: " + horas + minutos + segundos + letraTiempo;
		}
		analiticas.DificultadCompletada (ultimaDificultadSeleccionada + 1);
	}

	public void VolverAMenuTrasCompletar(){
		this.GetComponent<App_wall>().play_sound(0);
		MoverPiezas moverPiezas = this.gameObject.GetComponent<MoverPiezas> ();
		moverPiezas.puzzlePlaying = false;
		if (nuevoPuzzle != null) {
			Destroy (nuevoPuzzle);
		}
		SeleccionarImagen (puzzlePreseleccionado);

		//COLOREAR SI SE ACABA DE COMPLETAR EN DIFICIL
		contornoBoton [puzzlePreseleccionado].color = colorDificultad[PlayerPrefs.GetInt("puzzleCompleto"+puzzlePreseleccionado, 0)];
		if (PlayerPrefs.GetInt ("puzzleCompleto" + puzzlePreseleccionado, 0) == 5) {
			TexturaABoton (puzzlePreseleccionado, descargarImagenes.puzzleImageList [puzzlePreseleccionado]);
		}


		if(puedeMostrarAnuncioExtra) {
			contadorTiempoAnuncio = 0;
			puedeMostrarAnuncioExtra = false;
			puedeMostrarAnuncio = false;
			ShowRewardedAdExtra ();
		} else if(puedeMostrarAnuncio) {
			contadorTiempoAnuncio = 0;
			puedeMostrarAnuncioExtra = false;
			puedeMostrarAnuncio = false;
			ShowRewardedAdExtra ();
		}

		panelInicial.SetActive (true);
		panelSeleccion.SetActive (false);
		panelPreGame.SetActive (false);
		panelInGame.SetActive (false);
		panelComplete.SetActive (false);
	}

	void ReiniciarYaPidioAyudaBackground(){
		yaPidioAyudaBackground = false;
		ActualizarBotonesPause ();
	}
	void ReiniciarYaPidioSepararPiezas(){
		yaPidioSepararPiezas = false;
		ActualizarBotonesPause ();
	}

	public void ShowRewardedAdMostrarBG()
	{
		this.GetComponent<App_wall>().carrot.ads.show_ads_Rewarded();
	}

	public void ShowRewardedAdExtra()
	{
		this.GetComponent<App_wall>().carrot.ads.show_ads_Interstitial();
	}

	void ActualizarBotonesPause(){
		#if PLATFORM_ANDROID
		if (yaPidioSepararPiezas) {
			tickSort.SetActive (false);
		} else {
			tickSort.SetActive (true);
		}
		if (!yaPidioAyudaBackground && !ayudaActivada){
			tickGuia.SetActive (true);
		} else {
			tickGuia.SetActive (false);
		}
		#else
			tickSort.SetActive (false);
			tickGuia.SetActive (false);
		#endif
	}

	public void btn_add_socres(){
		int scrore_add=(ultimaDificultadSeleccionada+1)*2;
		GameObject.Find("app_wall").GetComponent<Data_Offline>().add_history(scrore_add,0);
	}

}