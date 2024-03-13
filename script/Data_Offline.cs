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
    public GameObject prefab_data_offline_item;
    public GameObject prefab_data_offline_home;
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
        Debug.Log(Json.Serialize(data));
        PlayerPrefs.SetString("data_wall_" + this.length_data, Json.Serialize(data));
        this.length_data++;
        PlayerPrefs.SetInt("length_data", length_data);
        after_add_act?.Invoke();
    }

    public void delete(int index,UnityAction act_after_delete)
    {
        PlayerPrefs.DeleteKey("data_wall_" + index);
        this.app.play_sound(5);
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
                    Debug.Log(s_data);
                    IDictionary data_img = (IDictionary) Json.Deserialize(s_data);
                    string s_id_wall = "wall" + data_img["id"].ToString();
                    Carrot_Box_Item item_img = box.create_item("item_img_" + i);
                    Sprite sp_icon = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_wall);
                    item_img.set_title("Image " + i);
                    item_img.set_tip(data_img["name"].ToString());
                    if (sp_icon != null)
                        item_img.set_icon_white(sp_icon);
                    else
                        app.carrot.get_img_and_save_playerPrefs(data_img["url"].ToString(), item_img.img_icon, s_id_wall);
                }
            }
            this.box.update_color_table_row();
        }
    }

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
        this.GetComponent<App_wall>().carrot.clear_contain(this.GetComponent<App_wall>().area_body);
        for(int i = 0; i < this.length_data; i++)
        {
            GameObject item_data_img = Instantiate(this.prefab_data_offline_home);
            item_data_img.transform.SetParent(this.GetComponent<App_wall>().area_body);
            item_data_img.transform.localPosition = new Vector3(item_data_img.transform.localPosition.x, item_data_img.transform.localPosition.y, 0f);
            item_data_img.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void add_history(int score,int type){
        PlayerPrefs.SetInt("h_score_"+this.length_history,score);
        PlayerPrefs.SetInt("h_type_"+this.length_history,type);
        PlayerPrefs.SetString("h_date_"+this.length_history,System.DateTime.Now.ToString("HH:mm dd MMMM, yyyy"));
        this.length_history++;
        PlayerPrefs.SetInt("length_history",length_history);
        this.app.carrot.game.update_scores_player(get_total_scores());
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
}