﻿using UnityEngine;
using UnityEngine.UI;
using Carrot;
using System.Collections;
using System;

public class App_wall : MonoBehaviour{

    [Header("Config Api")]
    public string key_api_searchcustomer = "";
    public string key_search_engines = "";

    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Manager_wall wall;
    public Data_Offline data_offline;
    public Carrot_DeviceOrientationChange app_scene;

    [Header("Obj App ui")]
    public GameObject panel_game2;
    public GameObject panel_game2_win;

	public GameObject prefab_category;
    public GameObject prefab_title;
    public GameObject prefab_loading;

	public Transform area_body;
	public Skybox skyBk;

	private Texture2D data_texture;
	public Texture texture_Default;

    public GameObject game2_Tile;

    [Header("Obj View")]
    public GameObject btn_game2_storage;
    [Header("Sound Game")]
    public AudioSource[] sound;

    private int game2_size=0;
    private IDictionary data_wal_cur;
    private bool is_play_game1 = false;

	void Start () {

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        this.carrot.Load_Carrot(this.check_app_exit);
        this.carrot.shop.onCarrotPaySuccess += this.onBySuccessPayCarrot;
        this.carrot.shop.onCarrotRestoreSuccess += this.onRestoreSuccessPayCarrot;

        this.carrot.game.load_bk_music(this.sound[8]);

        this.carrot.act_after_delete_all_data+=this.delete_all_data;

        this.carrot.game.Add_type_rank("Puzzler", data_offline.icon_game1);
        this.carrot.game.Add_type_rank("jigsaw", data_offline.icon_game2);

        this.panel_game2.gameObject.SetActive(false);
        this.panel_game2_win.SetActive(false);

        this.wall.On_load();
        this.data_offline.On_load();
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
        if (this.GetComponent<ControlUI>().panelSeleccion.activeInHierarchy)
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
        this.carrot.ads.show_ads_Interstitial();
    }

	public void play_game_in_category(string url_list){
        this.play_sound(0);
        this.area_body.parent.gameObject.SetActive (true);
		this.GetComponent<DescargarImagenes> ().list_url = url_list;
		this.GetComponent<DescargarImagenes> ().puzzleImageList.Clear ();
		this.GetComponent<ControlUI> ().CargarMasPuzzles ();
	}

	public void back_home(){
        this.play_sound(0);
        this.area_body.parent.gameObject.SetActive (true);
        this.panel_game2.SetActive(false);
        this.carrot.ads.show_ads_Interstitial();
        this.clear_game_2();
        if(this.is_play_game1) this.carrot.ads.create_banner_ads();
    }

	public void play_game_cur_bk(){
        this.play_sound(0);
        this.GetComponent<ControlUI> ().show_select_by_texture (this.data_texture);
        this.GetComponent<DescargarImagenes> ().puzzleImageList.Clear ();
		this.GetComponent<DescargarImagenes> ().puzzleImageList.Add (this.data_texture);
		this.GetComponent<ControlUI> ().SeleccionarImagen (0);
		this.GetComponent<ControlUI> ().show_select_by_texture (this.data_texture);
	}

    public void play_game_1(Texture data_img,IDictionary data)
    {
        this.is_play_game1 = true;
        carrot.ads.Destroy_Banner_Ad();
        this.data_wal_cur = data;
        this.data_texture = (Texture2D)data_img;
        this.play_game_cur_bk();
    } 

	public void save_bk(){
        this.play_sound(0);
	}

    public void play_game_2_bk()
    {
        this.play_sound(0);
        this.show_game_2(this.carrot.get_tool().ResampleAndCrop(this.data_texture,300,300));
    }

    public void play_game_2(Texture2D data_img,IDictionary data)
    {
        this.is_play_game1 = false;
        this.data_wal_cur = data;
        this.play_sound(0);
        this.show_game_2(data_img);
    }

    public void storage_bk_image()
    {
        this.play_sound(0);
        this.data_offline.Add(data_wal_cur);
    }

	[ContextMenu ("delete data")]
	public void delete_all_data(){
        this.carrot.stop_all_act();
        this.data_offline.On_load();
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

    public void show_background_save()
    {
        this.play_sound(0);
        this.data_offline.show_data();
    }

    public void show_game_2(Texture2D data_img)
    {
        this.show_game_2_customer(data_img, 3, 3);
    }

    public void play_game_2_in_offline_and_view_bk(Texture2D data_img,string url_download){
        this.show_game_2_customer(this.carrot.get_tool().ResampleAndCrop(data_img,300,300), 3, 3);
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
        this.panel_game2.SetActive(true);
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
        Carrot_Box box_setting=this.carrot.Create_Setting();

        if (wall.Get_status_buy_all_wall()==false)
        {
            Carrot_Box_Item item_buy_all_img = box_setting.create_item_of_index("item_buy_all", 0);
            item_buy_all_img.set_icon(carrot.icon_carrot_database);
            item_buy_all_img.set_title("Unlock all photos");
            item_buy_all_img.set_tip("Buy and use all images included in the game");
            item_buy_all_img.set_act(() => carrot.buy_product(wall.index_buy_all_wall));
            item_buy_all_img.set_type(Box_Item_Type.box_nomal);
            item_buy_all_img.check_type();

            Carrot_Box_Btn_Item btn_buy = item_buy_all_img.create_item();
            btn_buy.set_icon(carrot.icon_carrot_buy);
            btn_buy.set_color(carrot.color_highlight);
            Destroy(btn_buy.GetComponent<Button>());
        }
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
        if (id_product==this.carrot.shop.get_id_by_index(wall.index_buy_one_wall))
        {
            this.carrot.show_msg("Successful purchase", "Thank you for purchasing the product, you can use it for gaming or storage!", Carrot.Msg_Icon.Success);
            wall.On_pay_one_success();
        }

        if (id_product==this.carrot.shop.get_id_by_index(wall.index_buy_all_wall))
        {
            this.carrot.show_msg("Successful purchase", "The function to update the image link to save to the device has been activated, you can download any image in the application", Carrot.Msg_Icon.Success);
            wall.On_pay_all_success();
        }
    }

    private void onRestoreSuccessPayCarrot(string[] arr_id)
    {
        for(int i = 0; i < arr_id.Length; i++)
        {
            string id_product = arr_id[i];
            if (id_product == this.carrot.shop.get_id_by_index(wall.index_buy_all_wall)) wall.On_pay_all_success();
        }
    }

    public void play_sound(int index){
       if(this.carrot.get_status_sound()) this.sound[index].Play();
    }

    public void btn_camera_offline(){
        this.play_sound(0);
        this.carrot.camera_pro.show_camera(act_show_img_in_camera);
    }

    private void act_show_img_in_camera(Texture2D data_img){
        string s_id_new_photo = "photo"+carrot.generateID();
        IDictionary data_new = (IDictionary) Json.Deserialize("{}");
        data_new["id"] = s_id_new_photo;
        data_new["name"] = DateTime.Now.ToString();
        data_new["buy"] = "0";
        data_new["icon"] = "Camera";
        data_new["index"] = data_offline.get_length_data().ToString();
        carrot.get_tool().PlayerPrefs_Save_by_data("wall"+s_id_new_photo, data_img.EncodeToPNG());
        this.data_offline.Add_data(data_new);
        if(!this.carrot.is_online()){
            this.data_offline.Load_data_in_home();
        }
        wall.Show_select_game(data_img, data_new);
    }

    public void Scroll_on_Top()
    {
        this.GetComponent<ControlUI>().panelInicial.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
    }

    public void Act_server_fail(string s_error)
    {
        this.carrot.show_msg("Error", s_error, Msg_Icon.Error);
    }

    public void add_obj_title(string s_title)
    {
        GameObject obj_title = Instantiate(this.prefab_title);
        obj_title.transform.SetParent(this.area_body);
        obj_title.transform.localScale = new Vector3(1f, 1f, 0f);
        obj_title.transform.localPosition = Vector3.zero;
        obj_title.transform.localRotation = Quaternion.identity;

        obj_title.GetComponent<Carrot_Box_Item>().txt_name.text = s_title;
    }

    public void add_obj_loading()
    {
        GameObject obj_title = Instantiate(this.prefab_loading);
        obj_title.transform.SetParent(this.area_body);
        obj_title.transform.localScale = new Vector3(1f, 1f, 0f);
        obj_title.transform.localPosition = Vector3.zero;
        obj_title.transform.localRotation = Quaternion.identity;
    }

    public void Add_loading_and_clear_body()
    {
        carrot.clear_contain(this.area_body);
        this.add_obj_loading();
    }

    [ContextMenu("Test Updload scores Game 1")]
    public void Act_test_upload_scores_1()
    {
        this.carrot.game.update_scores_player(UnityEngine.Random.Range(1, 20), 0);
    }

    [ContextMenu("Test Updload scores Game 2")]
    public void Act_test_upload_scores_2()
    {
        this.carrot.game.update_scores_player(UnityEngine.Random.Range(1, 20), 1);
        this.carrot.play_vibrate();
    }
}
