using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Carrot;

public class App_wall : MonoBehaviour{

    [Header("Config Api")]
    public string key_api_searchcustomer = "";
    public string key_search_engines = "";

    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Manager_wall wall;
    public Data_Offline data_offline;

    [Header("Obj App ui")]
	public GameObject panel_menu;
	public GameObject panel_bk;
    public GameObject panel_game2;
    public GameObject panel_game2_win;

	public GameObject prefab_category;
    public GameObject prefab_search_item;

	public Transform area_body;
	public Skybox skyBk;

	private Texture2D data_texture;
	public Texture texture_Default;

	public RawImage img_bk_view;
    public GameObject game2_Tile;

    [Header("Obj View")]
    public GameObject btn_bk_view_next;
    public GameObject btn_bk_view_prev;
    public GameObject btn_buy_ads;
    public GameObject btn_bk_view_save;
    public GameObject btn_bk_view_save_to_phone;
    public GameObject btn_game2_save_to_phone;
    public GameObject btn_game2_storage;
    private List<string> list_bk_view; 
    private int count_cur_bk = 0;
    private string url_bk_view="";
    [Header("Sound Game")]
    public AudioSource[] sound;

    private int game2_size=0;

	void Start () {

        this.carrot.Load_Carrot(this.check_app_exit);
        this.carrot.shop.onCarrotPaySuccess += this.onBySuccessPayCarrot;
        this.carrot.shop.onCarrotRestoreSuccess += this.onRestoreSuccessPayCarrot;

        this.carrot.game.load_bk_music(this.sound[8]);

		this.panel_menu.SetActive (false);
		this.panel_bk.SetActive (false);
        this.panel_game2.gameObject.SetActive(false);
        this.panel_game2_win.SetActive(false);

        this.wall.On_load();
	}

    public void load_app_online(){
        this.wall.Get_list_data_background();
    }

    public void load_app_offline(){
        this.data_offline.show_data_in_home();
    }

    private void check_app_exit()
    {
        this.play_sound(0);
        if (this.panel_menu.activeInHierarchy)
        {
            this.back_home();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.GetComponent<ControlUI>().panelSeleccion.activeInHierarchy)
        {
            this.GetComponent<ControlUI>().VolverAMenu();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.GetComponent<ControlUI>().panelInGame.activeInHierarchy)
        {
            this.GetComponent<ControlUI>().VerificarBoton("menu");
            this.carrot.set_no_check_exit_app();
        }
    }

    public void refresh_home()
    {
        this.play_sound(3);
        this.wall.Get_list_data_background();
        this.check_and_show_ads();
    }

    public void btn_view_prev()
    {
        this.count_cur_bk--;
        Debug.Log("Prev :" + this.list_bk_view[this.count_cur_bk]);
        StartCoroutine(this.download_image_bk(this.list_bk_view[this.count_cur_bk]));
        this.check_show_hide_btn_view();
    }

    private void check_show_hide_btn_view()
    {
        if (this.count_cur_bk <= 0)
        {
            this.btn_bk_view_prev.SetActive(false);
        }
        else
        {
            this.btn_bk_view_prev.SetActive(true);
        }
    }

	IEnumerator download_image_bk (string url)
	{
        this.check_show_btn_save_phone(url,false);
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            this.carrot.show_loading();
            www.SendWebRequest();
            while (!www.isDone)
            {
                yield return null;
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                this.carrot.hide_loading();
            }
            else
            {
                this.carrot.hide_loading();
                Texture2D profilePic = ((DownloadHandlerTexture)www.downloadHandler).texture;
                if(this.list_bk_view!=null)this.list_bk_view.Add(url);
                this.show_view_img(profilePic);
            }
        }
	}

    private void show_view_img(Texture2D data_img){
        this.data_texture = data_img;
        this.panel_menu.SetActive (true);
        this.area_body.parent.gameObject.SetActive (false);
        this.img_bk_view.texture = this.data_texture;
        this.img_bk_view.rectTransform.sizeDelta = new Vector2 (data_img.width, data_img.height);
        this.panel_bk.SetActive (true);
        this.btn_bk_view_next.SetActive(true);
        this.btn_bk_view_prev.SetActive(true);
        this.btn_bk_view_save.SetActive(true);
        this.check_show_hide_btn_view();
    }

    public void show_view_in_offline(Texture2D data_img,string url_download){
        this.check_show_btn_save_phone(url_download,true);
        this.show_view_img(data_img);
        this.btn_bk_view_next.SetActive(false);
        this.btn_bk_view_prev.SetActive(false);
        this.btn_bk_view_save.SetActive(false);
        this.carrot.close();
    }

    public void show_view_by_url_image(string url)
    {
        StartCoroutine(this.download_image_bk(url));
    }

	public void play_game_in_category(string url_list){
        this.play_sound(0);
        this.area_body.parent.gameObject.SetActive (true);
		this.GetComponent<DescargarImagenes> ().list_url = url_list;
		this.GetComponent<DescargarImagenes> ().puzzleImageList.Clear ();
		this.GetComponent<ControlUI> ().CargarMasPuzzles ();
		this.panel_bk.SetActive (false);
	}

	public void back_home(){
        this.play_sound(0);
        this.area_body.parent.gameObject.SetActive (true);
		this.panel_menu.SetActive (false);
        this.panel_game2.SetActive(false);
        this.check_and_show_ads();
        this.clear_game_2();
    }

	public void play_game_cur_bk(){
        this.play_sound(0);
        //this.GetComponent<ControlUI> ().show_select_by_texture (this.data_texture);
        this.GetComponent<DescargarImagenes> ().puzzleImageList.Clear ();
		this.GetComponent<DescargarImagenes> ().puzzleImageList.Add (this.data_texture);
		this.GetComponent<ControlUI> ().SeleccionarImagen (0);
		this.GetComponent<ControlUI> ().show_select_by_texture (this.data_texture);
		this.panel_menu.SetActive (false);
		this.panel_bk.SetActive (false);
	}

    public void play_game_1(Texture data_img)
    {
        this.data_texture = (Texture2D)data_img;
        this.play_game_cur_bk();
        this.carrot.close();
    } 

	public void save_bk(){
        this.play_sound(0);
	}

    public void play_game_2_bk()
    {
        this.play_sound(0);
        this.show_game_2(this.ResizeAndCrop(this.data_texture,300,300));
        this.panel_menu.SetActive(false);
        this.panel_bk.SetActive(false);
    }

    public void play_game_2(Texture2D data_img)
    {
        this.play_sound(0);
        this.show_game_2(data_img);
        this.panel_menu.SetActive(false);
        this.panel_bk.SetActive(false);
    }

    public void storage_bk_image()
    {
        this.play_sound(0);
        this.carrot.show_msg("Storage", "Successful offline storage, you can build and use this picture without an internet connection",Carrot.Msg_Icon.Success);
        this.GetComponent<Data_Offline>().add_data(this.url_bk_view,this.data_texture.EncodeToPNG(),true);
    }

    public void play_game1(string url){
        this.url_bk_view=url;
        this.carrot.get_img(url,act_play_game1);
    }

    private void act_play_game1(Texture2D data_img){
        this.play_game_1(data_img);
    }

    public void play_game2(string url,string url_full){
        this.url_bk_view=url_full;
        this.carrot.get_img(url,act_play_game2);
    }

    private void act_play_game2(Texture2D data_img){
        this.play_game_2(data_img);
    }

	public void rate_app ()
	{
        this.play_sound(0);
        this.carrot.show_rate();
	}

	[ContextMenu ("delete data")]
	public void delete_all_data(){
        this.play_sound(0);
        this.GetComponent<Data_Offline>().Start();
		this.carrot.delete_all_data();
        this.Start();
	}

    public void show_more_app()
    {
        this.play_sound(0);
        this.carrot.show_list_carrot_app();
    }

    public void app_share()
    {
        this.play_sound(0);
        this.carrot.show_share();
    }

    public void search()
    {
        this.play_sound(0);
        this.wall.Show_Search();
    }

    public void act_get_list_search(string s_data)
    {
        if(s_data!=""){
            IList list = (IList)Carrot.Json.Deserialize(s_data);
            if(list.Count>0){
                this.carrot.clear_contain(this.area_body);
                list_bk_view=new List<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    IDictionary item_bk = (IDictionary)list[i];
                    GameObject item_bk_obj = Instantiate(prefab_search_item);
                    item_bk_obj.name = "item_search";
                    item_bk_obj.transform.SetParent(area_body);
                    item_bk_obj.transform.localPosition = new Vector3(0f, 0f, 0f);
                    item_bk_obj.transform.localScale = new Vector3(1f, 1f, 1f);
                    item_bk_obj.GetComponent<Item_search>().url = item_bk["id"].ToString();
                    item_bk_obj.GetComponent<Item_search>().txt_name.text=item_bk["name"].ToString();
                    this.carrot.get_img(item_bk["url"].ToString(),item_bk_obj.GetComponent<Item_search>().icon);
                    list_bk_view.Add(item_bk["id"].ToString());
                }   
            }else{
                this.carrot.show_msg("Search","No matching results were found!",Carrot.Msg_Icon.Alert);
            }
            Canvas.ForceUpdateCanvases();
        }else{
            this.carrot.show_msg("Search","Something went wrong, please search again!",Carrot.Msg_Icon.Alert);
        }
    }


    public void show_background_save()
    {
        this.play_sound(0);
        this.GetComponent<Data_Offline>().show_data();
    }

    public void show_game_2(Texture2D data_img)
    {
        this.show_game_2_customer(data_img, 3, 3);
    }

    public void play_game_2_in_offline_and_view_bk(Texture2D data_img,string url_download){
        this.check_show_btn_save_phone(url_download,true);
        this.show_game_2_customer(this.ResizeAndCrop(data_img,300,300), 3, 3);
    }

    private void check_show_btn_save_phone(string s_url_download,bool is_storager){
        this.url_bk_view=s_url_download;
        this.btn_game2_save_to_phone.SetActive(false);
        this.btn_bk_view_save_to_phone.SetActive(false);
        this.btn_game2_storage.SetActive(false);
        if(s_url_download!="camera_photo"){
            if(!is_storager){
                this.btn_bk_view_save_to_phone.SetActive(true);
                this.btn_game2_save_to_phone.SetActive(true);
                this.btn_game2_storage.SetActive(true);
                this.url_bk_view=s_url_download;
            }else{
                if(this.carrot.is_online()){
                    this.btn_game2_save_to_phone.SetActive(true);
                    this.btn_bk_view_save_to_phone.SetActive(true);
                }
            }
        }
    }

    private void show_game_2_customer(Texture2D data_img,int w,int h)
    {
        this.clear_game_2();
        this.GetComponent<ControlUI>().panelInicial.SetActive(false);
        GameObject game2 = Instantiate(this.game2_Tile);
        game2.SetActive(true);
        game2.name = "game2";
        game2.GetComponent<ST_PuzzleDisplay>().clear_all_img();
        game2.GetComponent<ST_PuzzleDisplay>().PuzzleImage = data_img;
        game2.GetComponent<ST_PuzzleDisplay>().Width = w;
        game2.GetComponent<ST_PuzzleDisplay>().Height =h;
        this.data_texture = data_img;
        game2.GetComponent<ST_PuzzleDisplay>().Start_game();
        this.carrot.close_all_window();
        this.panel_game2.SetActive(true);
        this.panel_bk.SetActive(false);
        this.play_sound(0);
    }
    public void show_game_2_3n3()
    {
        this.game2_size=3;
        this.show_game_2_customer(this.data_texture, 3, 3);
    }
    public void show_game_2_4n4()
    {
        this.game2_size=4;
        this.show_game_2_customer(this.data_texture, 4, 4);
    }

    public void show_game_2_5n5()
    {
        this.game2_size=5;
        this.show_game_2_customer(this.data_texture, 5, 5);
    }

    public void play_again_game2(){
        this.play_sound(0);
        this.panel_game2_win.SetActive(false);
        this.show_game_2_customer(this.data_texture, this.game2_size, this.game2_size);
    }

    public void close_game_2()
    {
        this.GetComponent<ControlUI>().panelInicial.SetActive(true);
        this.game2_Tile.SetActive(false);
        this.panel_game2.SetActive(false);
    }

    private void clear_game_2()
    {
        this.panel_game2_win.SetActive(false);
        GameObject game2old = GameObject.Find("game2");
        if (game2old != null)
        {
            Destroy(game2old);
        }
    }

    public void show_setting(){
        this.carrot.Create_Setting();
    }

    public void btn_show_login(){
        this.play_sound(0);
        this.carrot.show_login();
    }

    public void buy_product(int index){
        this.carrot.buy_product(index);
    }

    private void onBySuccessPayCarrot(string id_product)
    {
        if (id_product==this.carrot.shop.get_id_by_index(1))
        {
            this.carrot.show_msg("Buy background music", "Background music purchased successfully, now you can use the background music!!!", Carrot.Msg_Icon.Success);
        }

        if (id_product==this.carrot.shop.get_id_by_index(2))
        {
            this.carrot.show_msg("Successful purchase", "You can now get the link to download the image in your browser, thank you for your purchase!", Carrot.Msg_Icon.Success);
            Application.OpenURL(this.url_bk_view);
        }

        if (id_product==this.carrot.shop.get_id_by_index(3))
        {
            this.carrot.show_msg("Successful purchase", "The function to update the image link to save to the device has been activated, you can download any image in the application", Carrot.Msg_Icon.Success);
            PlayerPrefs.SetInt("is_all_img", 1);
        }

        this.carrot.check_buy_music_item_bk(id_product);
    }

    private void onRestoreSuccessPayCarrot(string[] arr_id)
    {
        for(int i = 0; i < arr_id.Length; i++)
        {
            string id_product = arr_id[i];
            if (id_product == this.carrot.shop.get_id_by_index(0)) this.act_inapp_removeads();
            if (id_product == this.carrot.shop.get_id_by_index(3)) PlayerPrefs.SetInt("is_all_img", 1);
        }
    }

    private void act_inapp_removeads()
    {
        PlayerPrefs.SetInt("is_buy_ads", 1);
        this.btn_buy_ads.SetActive(false);
    }

    public void play_sound(int index){
       if(this.carrot.get_status_sound()) this.sound[index].Play();
    }

    private void check_and_show_ads(){
        this.carrot.ads.show_ads_Interstitial();
    }

    public void ShowAd(){
        carrot.ads.show_ads_Interstitial();
	}

    public Texture2D ResizeAndCrop(Texture2D source, int targetWidth, int targetHeight)
    {
        if (source == null) return null;
         int sourceWidth = source.width;
         int sourceHeight = source.height;
         float sourceAspect = (float)sourceWidth / sourceHeight;
         float targetAspect = (float)targetWidth / targetHeight;
         int xOffset = 0;
         int yOffset = 0;
         float factor = 0f;
         if (sourceAspect > targetAspect)
         { 
             factor = (float)targetHeight / sourceHeight;
             xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
         }
         else
         { 
             factor = (float)targetWidth / sourceWidth;
             yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
         }
         Color32[] data = source.GetPixels32();
         Color32[] data2 = new Color32[targetWidth * targetHeight];
         for (int y = 0; y < targetHeight; y++)
         {
             for (int x = 0; x < targetWidth; x++)
             {
                 var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                 var c11 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                 var c12 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                 var c21 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                 var c22 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                 var f = new Vector2(Mathf.Repeat(p.x, 1f), Mathf.Repeat(p.y, 1f));
                 data2[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
             }
         }
 
         var tex = new Texture2D(targetWidth, targetHeight);
         tex.SetPixels32(data2);
         tex.Apply(true);
         return tex;
    }

    public void btn_camera_offline(){
        this.play_sound(0);
        this.carrot.camera_pro.show_camera(act_show_img_in_camera);
    }

    private void act_show_img_in_camera(Texture2D data_img){
        this.show_view_in_offline(data_img,"camera_photo");
        this.data_offline.add_data("camera_photo",data_img.EncodeToPNG(),false);
        if(!this.carrot.is_online()){
            this.data_offline.load_data();
            this.data_offline.Load_data_in_home();
        }
        this.btn_game2_save_to_phone.SetActive(false);
        this.btn_bk_view_save_to_phone.SetActive(false);
        this.btn_bk_view_save.SetActive(false);
    }

    public void btn_save_image_to_phone(){
        this.play_sound(0);
        if(PlayerPrefs.GetFloat("is_all_img",0)==0)
            this.buy_product(2);
        else
            Application.OpenURL(this.url_bk_view);
    }

    public void Scroll_on_Top()
    {
        this.GetComponent<ControlUI>().panelInicial.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
    }

    public void Act_server_fail(string s_error)
    {
        this.carrot.show_msg("Error", s_error, Msg_Icon.Error);
    }
}
