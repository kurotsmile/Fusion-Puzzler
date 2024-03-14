using Carrot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Manager_wall : MonoBehaviour
{
    [Header("Obj Main")]
    public App_wall app;

    [Header("Obj Wall")]
    public int index_buy_one_wall;
    public int index_buy_all_wall;
    private Carrot_Window_Input box_inpu_search = null;
    private Carrot_Box box;
    private string s_json_data_wall="";

    private Texture2D buy_texture_temp = null;
    private IDictionary buy_data_temp = null;
    private string buy_s_id="";
    private bool is_buy_all_img = false;

    private Carrot_Button_Item btn_save = null;

    public void On_load()
    {
        if (app.carrot.is_offline()) s_json_data_wall = PlayerPrefs.GetString("s_json_data_wall");
        if (PlayerPrefs.GetInt("is_all_img", 0) == 0)
            is_buy_all_img = false;
        else
            is_buy_all_img = true;
    }

    public void Get_list_data_background()
    {
        this.app.Add_loading_and_clear_body();
        if (this.s_json_data_wall == "")
        {
            StructuredQuery q = new("background");
            q.Set_limit(20);
            app.carrot.server.Get_doc(q.ToJson(), Act_get_list_background_done, app.Act_server_fail);
        }
        else
        {
            this.Act_Show_list_background_by_data(this.s_json_data_wall);
        }
    }

    private void Act_get_list_background_done(string s_data)
    {
        this.s_json_data_wall = s_data;
        PlayerPrefs.SetString("s_json_data_wall", s_data);
        this.Act_Show_list_background_by_data(s_data);
    }

    private void Act_Show_list_background_by_data(string s_data)
    {
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            this.app.carrot.clear_contain(this.app.area_body);
            IList<IDictionary> list_data_bk = new List<IDictionary>();
            for (int i = 0; i < fc.fire_document.Length; i++) list_data_bk.Add(fc.fire_document[i].Get_IDictionary());
            list_data_bk=app.carrot.get_tool().Shuffle_Ilist(list_data_bk);
            for (int i = 0; i < list_data_bk.Count; i++) this.Add_item_to_list(list_data_bk[i]);
            this.app.Scroll_on_Top();
        }
    }

    public void Add_item_to_list(IDictionary data)
    {
        GameObject item_category = Instantiate(app.prefab_category);
        item_category.name = "item_category_" + data["id"].ToString();
        item_category.transform.SetParent(app.area_body);
        item_category.transform.localPosition = new Vector3(0f, 0f, 0f);
        item_category.transform.localScale = new Vector3(1f, 1f, 1f);
        item_category.transform.localRotation = Quaternion.identity;

        Panel_category p_category = item_category.GetComponent<Panel_category>();
        p_category.On_load(this.app, data);
    }

    public void Show_Search()
    {
        this.box_inpu_search = this.app.carrot.show_search(Act_Search_Done, "You can search the display photo theme to your liking");
    }

    private void Act_Search_Done(string s_key)
    {
        app.carrot.show_loading();
        StartCoroutine(Act_Search_resualt(s_key));
    }

    IEnumerator Act_Search_resualt(string s_key)
    {
        string url_api_search = "https://www.googleapis.com/customsearch/v1?key="+this.app.key_api_searchcustomer+"&cx="+this.app.key_search_engines+"&q=" + s_key;
        using UnityWebRequest www = UnityWebRequest.Get(url_api_search);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            app.carrot.hide_loading();
            if (this.box_inpu_search != null) this.box_inpu_search.close();
            this.app.carrot.clear_contain(this.app.area_body);
            this.app.add_obj_title(s_key);
            IDictionary data = (IDictionary)Json.Deserialize(www.downloadHandler.text);
            IList list_pic = (IList)data["items"];

            for (int i = 0; i < list_pic.Count; i++)
            {
                string url_pic = "";
                IDictionary pic = (IDictionary)list_pic[i];
                IDictionary pagemap = (IDictionary) pic["pagemap"];
                IList metatags = (IList) pagemap["metatags"];
                IDictionary pic_meta = (IDictionary)metatags[0];


                if (pic_meta["og:image"] != null)
                {
                    url_pic= pic_meta["og:image"].ToString();
                }
                else
                {
                    if (pagemap["cse_thumbnail"] != null)
                    {
                        IList cse_thumbnail = (IList)pagemap["cse_thumbnail"];
                        IDictionary thumb = (IDictionary)cse_thumbnail[0];
                        url_pic = thumb["src"].ToString();
                    }
                }

                if (url_pic != "")
                {
                    pic["name"] = pic["title"];
                    pic["icon"] = url_pic;
                    pic["id"] = "wall" + app.carrot.generateID();
                    this.Add_item_to_list(pic);
                }
            }
            this.app.Scroll_on_Top();
        }
        else
        {
            app.carrot.hide_loading();
        }
    }

    public void Show_select_game(Texture2D tex,IDictionary data)
    {
        app.carrot.ads.show_ads_Interstitial();
        app.play_sound(0);

        box = app.carrot.Create_Box();
        box.set_icon(app.carrot.icon_carrot_game);
        box.set_title("Select Game");

        Carrot.Carrot_Box_Item item_game_jigsaw = box.create_item("game_jigsaw");
        item_game_jigsaw.set_title("jigsaw");
        item_game_jigsaw.set_tip("The type of puzzle you have to put together piece by piece");
        item_game_jigsaw.set_icon(app.data_offline.icon_game1);
        item_game_jigsaw.set_act(() => this.play_game(tex, data,true));

        Carrot_Box_Btn_Item btn_jigsaw_play = item_game_jigsaw.create_item();
        btn_jigsaw_play.set_icon(app.carrot.game.icon_play_music_game);
        btn_jigsaw_play.set_color(app.carrot.color_highlight);
        Destroy(btn_jigsaw_play.GetComponent<Button>());

        Carrot.Carrot_Box_Item item_game_puzzler = box.create_item("game_puzzler");
        item_game_puzzler.set_title("Puzzler");
        item_game_puzzler.set_tip("The type of game that arranges the order of given puzzle pieces");
        item_game_puzzler.set_icon(app.data_offline.icon_game2);
        item_game_puzzler.set_act(() => this.play_game(tex, data, false));
        
        Carrot_Box_Btn_Item btn_puzzler_play = item_game_puzzler.create_item();
        btn_puzzler_play.set_icon(app.carrot.game.icon_play_music_game);
        btn_puzzler_play.set_color(app.carrot.color_highlight);
        Destroy(btn_puzzler_play.GetComponent<Button>());

        Carrot_Box_Btn_Panel panel_btn = box.create_panel_btn();
        if (data["index"] != null)
        {
            int index = int.Parse(data["index"].ToString());
            Carrot_Button_Item btn_del = panel_btn.create_btn("btn_delete");
            btn_del.set_icon_white(app.carrot.sp_icon_del_data);
            btn_del.set_label("Delete");
            btn_del.set_label_color(Color.white);
            btn_del.set_bk_color(Color.red);
            btn_del.set_act_click(() => Act_delete(index));
            app.btn_game2_storage.SetActive(false);
        }
        else
        {
            btn_save = panel_btn.create_btn("btn_save");
            btn_save.set_icon_white(app.data_offline.icon);
            btn_save.set_label("Archive");
            btn_save.set_label_color(Color.white);
            btn_save.set_bk_color(app.carrot.color_highlight);
            btn_save.set_act_click(() => Save_wall_to_offlie(data));
            app.btn_game2_storage.SetActive(true);
        }

        Carrot_Button_Item btn_view = panel_btn.create_btn("btn_view");
        btn_view.set_icon_white(app.carrot.icon_carrot_visible_off);
        btn_view.set_label("View Imager");
        btn_view.set_label_color(Color.white);
        btn_view.set_bk_color(app.carrot.color_highlight);
        btn_view.set_act_click(() => app.carrot.camera_pro.show_photoshop(tex));

        Carrot_Button_Item btn_close = panel_btn.create_btn("btn_close");
        btn_close.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_close.set_label("Close");
        btn_close.set_label_color(Color.white);
        btn_close.set_bk_color(app.carrot.color_highlight);
        btn_close.set_act_click(() => box.close());
    }

    private void Act_delete(int index)
    {
        app.data_offline.delete(index);
        app.carrot.show_msg("Delete", "Deleted successfully!", Msg_Icon.Success);
        if (box != null) box.close();
    }

    public void play_game(Texture2D tex,IDictionary data,bool is_game_1)
    {
        if (box != null) box.close();
        if (is_game_1)
            this.app.play_game_1(tex, data);
        else
            this.app.play_game_2(tex, data);
        app.data_offline.Close_box();
    }

    public void close_box()
    {
        if (this.box != null) this.box.close();
    }

    public void Buy_wall(Texture2D tex,IDictionary data,string s_id)
    {
        this.buy_s_id = s_id;
        this.buy_texture_temp = tex;
        this.buy_data_temp = data;
        this.app.carrot.buy_product(this.index_buy_one_wall);
    }

    public void On_pay_one_success()
    {
        if (this.buy_data_temp != null)
        {
            PlayerPrefs.SetInt("is_buy_" + this.buy_s_id,1);
            this.Show_select_game(this.buy_texture_temp, this.buy_data_temp);
            this.buy_data_temp = null;
            this.buy_texture_temp = null;
            this.buy_s_id = "";
            this.Get_list_data_background();
        }
    }

    public void On_pay_all_success()
    {
        this.is_buy_all_img = true;
        PlayerPrefs.SetInt("is_all_img", 1);
    }

    public bool Check_Wall_used(string s_id)
    {
        if (this.is_buy_all_img)
        {
            return true;
        }
        else
        {
            if (PlayerPrefs.GetInt("is_buy_" + s_id, 0) == 1)
                return true;
            else
                return false;
        }
    }

    private void Save_wall_to_offlie(IDictionary data)
    {
        app.carrot.play_sound_click();
        app.carrot.play_vibrate();
        app.data_offline.Add(data);
        if (btn_save != null) Destroy(btn_save.gameObject);
    }

    public bool Get_status_buy_all_wall()
    {
        return this.is_buy_all_img;
    }
}