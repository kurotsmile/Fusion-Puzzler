using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Data_Offline : MonoBehaviour
{
    [Header("Main obj")]
    public App_wall app;

    [Header("Data obj")]
    public Sprite icon;
    public Sprite icon_history;
    public Sprite icon_rank;
    public Sprite icon_game1;
    public Sprite icon_game2;
    int length_data = 0;
    int length_history = 0;
    private Carrot_Box box;

    public void On_load()
    {
        this.app.carrot=this.GetComponent<App_wall>().carrot;
        this.length_data = PlayerPrefs.GetInt("length_data", 0);
        this.length_history=PlayerPrefs.GetInt("length_history",0);
    }

    public void Add(IDictionary data, UnityAction after_add_act = null)
    {
        this.Add_data(data, after_add_act);
        this.app.carrot.show_msg("Storage", "Successful offline storage, you can build and use this picture without an internet connection", Carrot.Msg_Icon.Success);
    }

    public void Add_data(IDictionary data,UnityAction after_add_act=null)
    {
        data["type"] = "offline";
        PlayerPrefs.SetString("data_wall_" + this.length_data, Json.Serialize(data));
        this.length_data++;
        PlayerPrefs.SetInt("length_data", length_data);
        after_add_act?.Invoke();
    }

    public void delete(int index,UnityAction act_after_delete=null)
    {
        string s_data = PlayerPrefs.GetString("data_wall_" + index, "");
        if(s_data!="") {
            IDictionary data_img = (IDictionary)Json.Deserialize(s_data);
            string s_id = data_img["id"].ToString();
            PlayerPrefs.DeleteKey("wall" + s_id);
        }
        PlayerPrefs.DeleteKey("data_wall_" + index);
        this.app.play_sound(5);
        if (box != null) box.close();
        act_after_delete?.Invoke();
    }

    public void show_data()
    {
        this.app.play_sound(0);
        if (this.length_data == 0)
        {
            this.app.carrot.show_msg("Offline Storage", "No items have been stored offline yet!",Carrot.Msg_Icon.Alert);
        }else{
            this.box=this.app.carrot.Create_Box("Store", this.icon);
            for(int i = length_data-1; i >=0; i--)
            {
                string s_data = PlayerPrefs.GetString("data_wall_" + i,"");
                if (s_data != "")
                {
                    IDictionary data_img = (IDictionary) Json.Deserialize(s_data);
                    data_img["index"] = i;
                    string s_id_wall = "wall" + data_img["id"].ToString();
                    Carrot_Box_Item item_img = box.create_item("item_img_" + i);
                    Texture2D tex = app.carrot.get_tool().get_texture2D_to_playerPrefs(s_id_wall);
                    item_img.set_title(data_img["name"].ToString());
                    item_img.set_tip(data_img["icon"].ToString());
                    if (tex != null)
                        item_img.set_icon_white(app.carrot.get_tool().Texture2DtoSprite(tex));
                    else
                        app.carrot.get_img_and_save_playerPrefs(data_img["url"].ToString(), item_img.img_icon, s_id_wall);

                    Carrot_Box_Btn_Item btn_game1 = item_img.create_item();
                    btn_game1.set_icon(icon_game1);
                    btn_game1.set_color(app.carrot.color_highlight);
                    btn_game1.set_act(() => play_game(tex, data_img, true));

                    Carrot_Box_Btn_Item btn_game2 = item_img.create_item();
                    btn_game2.set_icon(icon_game2);
                    btn_game2.set_color(app.carrot.color_highlight);
                    btn_game2.set_act(() => play_game(tex, data_img, false));

                    item_img.set_act(() => app.wall.Show_select_game(tex, data_img));
                }
            }
            this.box.update_color_table_row();
        }
    }

    private void play_game(Texture2D tex,IDictionary data,bool is_game_1)
    {
        if (box != null) box.close();
        if (is_game_1)
            this.app.play_game_1(tex, data);
        else
            this.app.play_game_2(tex, data);
    }

    [ContextMenu("Show data in home")]
    public void show_data_in_home(){
        StartCoroutine(act_data_offline_home());
    }

    IEnumerator act_data_offline_home()
    {
        yield return new WaitForSeconds(2);
        this.Load_data_in_home();
        this.StopAllCoroutines();
    }

    public void Load_data_in_home(){
        this.app.carrot.clear_contain(this.app.area_body);
        this.app.add_obj_title("Archived photos");
        for (int i = length_data - 1; i >= 0; i--)
        {
            string s_data = PlayerPrefs.GetString("data_wall_" + i, "");
            if (s_data != "")
            {
                IDictionary data_img = (IDictionary)Json.Deserialize(s_data);
                data_img["index"] = i;
                app.wall.Add_item_to_list(data_img);
            }
        }
        app.Scroll_on_Top();
    }

    public void add_history(int score,int type){
        PlayerPrefs.SetInt("h_score_"+this.length_history,score);
        PlayerPrefs.SetInt("h_type_"+this.length_history,type);
        PlayerPrefs.SetString("h_date_"+this.length_history,System.DateTime.Now.ToString("HH:mm dd MMMM, yyyy"));
        this.length_history++;
        PlayerPrefs.SetInt("length_history",length_history);
        this.app.carrot.game.update_scores_player(get_total_scores(), type);
    }

    private int get_total_scores(){
        int total_scores=0;
        for(int i=0;i<this.length_history;i++) total_scores+=PlayerPrefs.GetInt("h_score_"+i);
        return total_scores;
    }

    public void show_list_history(){
        this.app.play_sound(0);
        if(this.length_history==0){
            this.app.carrot.show_msg("Your winning history","You have never won");
        }else{
            Carrot_Box box_history=this.app.carrot.Create_Box("Your winning history",this.icon_history);
            int total_scores=get_total_scores();

            box_history.set_title("Your total score:" + total_scores.ToString());
            box_history.set_icon(this.icon_rank);

            for (int i=this.length_history-1;i>=0;i--){
                Carrot_Box_Item item_history = box_history.create_item("item_history_" + i);
                if (PlayerPrefs.GetInt("h_type_"+i)==0)
                    item_history.set_icon(this.icon_game1);
                else
                    item_history.set_icon(this.icon_game2);
                item_history.set_title("Scores:"+PlayerPrefs.GetInt("h_score_"+i).ToString());
                item_history.set_tip(PlayerPrefs.GetString("h_date_"+i).ToString());
            }
            this.app.carrot.game.update_scores_player(total_scores);
        }
    }

    public void show_list_rank(){
        this.app.carrot.game.Show_List_Top_player();
    }

    public void Close_box()
    {
        if (box != null) box.close();
    }
}