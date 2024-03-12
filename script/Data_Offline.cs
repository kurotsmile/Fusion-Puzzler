using Carrot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Offline : MonoBehaviour
{
    private Carrot.Carrot carrot;
    public Sprite icon;
    public Sprite icon_history;
    public Sprite icon_rank;
    public Sprite[] icon_rank_index;
    public Sprite icon_game1;
    public Sprite icon_game2;
    int length_data = 0;
    int length_history = 0;
    private List<Data_offline_item> list_item;
    public GameObject prefab_data_offline_item;
    public GameObject prefab_data_offline_home;
    public GameObject prefab_item_history;
    public GameObject prefab_item_rank;
    public void Start()
    {
        this.carrot=this.GetComponent<App_wall>().carrot;
        this.list_item = new List<Data_offline_item>();
        this.length_data = PlayerPrefs.GetInt("length_data", 0);
        this.length_history=PlayerPrefs.GetInt("length_history",0);
        this.load_data();
    }

    public void load_data()
    {
        Debug.Log("Load data offilie");
        list_item.Clear();
        if (this.length_data > 0)
        {
            for (int i = 0; i < this.length_data; i++)
            {
                string url_data=PlayerPrefs.GetString("url_" + i, "");
                if (url_data!= "")
                {
                    Data_offline_item item_img = new Data_offline_item();
                    string name_file_bk = "";
                    if (Application.isEditor)
                    {
                        name_file_bk = Application.dataPath + "/" + "img_" + i + ".png";
                    }
                    else
                    {
                        name_file_bk = Application.persistentDataPath + "/" + "img_" + i + ".png";
                    }
                    Debug.Log("file:" + name_file_bk);
                    if (System.IO.File.Exists(name_file_bk))
                    {
                        Texture2D load_s01_texture;
                        byte[] bytes;
                        bytes = System.IO.File.ReadAllBytes(name_file_bk);
                        load_s01_texture = new Texture2D(1, 1);
                        load_s01_texture.LoadImage(bytes);
                        item_img.data_img=load_s01_texture;
                        load_s01_texture=this.GetComponent<App_wall>().ResampleAndCrop(load_s01_texture,60,60);
                        item_img.icon_thumb = Sprite.Create(load_s01_texture, new Rect(0.0f, 0.0f, load_s01_texture.width, load_s01_texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                        item_img.url=url_data;
                    }
                    item_img.index = i;
                    this.list_item.Add(item_img);
                }
            }
        }
    }

    public void add_data(string url,byte[] data_save,bool is_show_list_done)
    {
        PlayerPrefs.SetString("url_" + this.length_data, url);
        //this.GetComponent<App_wall>().carrot.save_file("img_" + this.length_data + ".png",data_save);
        this.length_data++;
        PlayerPrefs.SetInt("length_data", length_data);
        this.load_data();
        if(is_show_list_done)this.show_data();
    }

    public void delete(int index,bool is_home)
    {
        string name_file_bk = "";
        if (Application.isEditor)
        {
            name_file_bk = Application.dataPath + "/" + "img_" + index + ".png";
        }
        else
        {
            name_file_bk = Application.persistentDataPath + "/" + "img_" + index + ".png";
        }
        if (System.IO.File.Exists(name_file_bk))
        {
            System.IO.File.Delete(name_file_bk);
        }
        PlayerPrefs.DeleteKey("url_" + index);
        this.load_data();
        if(is_home) 
            this.Load_data_in_home();
        else
            this.show_data();
        this.GetComponent<App_wall>().play_sound(5);
    }


    public void show_data()
    {
        this.GetComponent<App_wall>().play_sound(0);
        if (this.list_item.Count == 0)
        {
            this.carrot.show_msg("Offline Storage", "No items have been stored offline yet!",Carrot.Msg_Icon.Alert);
        }else{
            this.carrot.Create_Box("Store", this.icon);
            for(int i = 0; i < this.list_item.Count; i++)
            {
                GameObject item_data_img = Instantiate(this.prefab_data_offline_item);
                //item_data_img.transform.SetParent(this.GetComponent<App_wall>().carrot.area_body_box);
                item_data_img.transform.localPosition = new Vector3(item_data_img.transform.localPosition.x, item_data_img.transform.localPosition.y, 0f);
                item_data_img.transform.localScale = new Vector3(1f, 1f, 1f);
                item_data_img.GetComponent<Panel_item_data_offline_img>().index = this.list_item[i].index;
                item_data_img.GetComponent<Panel_item_data_offline_img>().is_home=false;
                item_data_img.GetComponent<Panel_item_data_offline_img>().data_img=this.list_item[i].data_img;
                item_data_img.GetComponent<Panel_item_data_offline_img>().img_Image.sprite = this.list_item[i].icon_thumb;
                item_data_img.GetComponent<Panel_item_data_offline_img>().url = this.list_item[i].url;
            }
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
        for(int i = 0; i < this.list_item.Count; i++)
        {
            GameObject item_data_img = Instantiate(this.prefab_data_offline_home);
            item_data_img.transform.SetParent(this.GetComponent<App_wall>().area_body);
            item_data_img.transform.localPosition = new Vector3(item_data_img.transform.localPosition.x, item_data_img.transform.localPosition.y, 0f);
            item_data_img.transform.localScale = new Vector3(1f, 1f, 1f);
            item_data_img.GetComponent<Panel_item_data_offline_img>().index = this.list_item[i].index;
            item_data_img.GetComponent<Panel_item_data_offline_img>().is_home=true;
            item_data_img.GetComponent<Panel_item_data_offline_img>().data_img=this.list_item[i].data_img;
            item_data_img.GetComponent<Panel_item_data_offline_img>().img_Image.sprite = this.list_item[i].icon_thumb;
            item_data_img.GetComponent<Panel_item_data_offline_img>().url = this.list_item[i].url;
        }
    }

    public void add_history(int score,int type){
        PlayerPrefs.SetInt("h_score_"+this.length_history,score);
        PlayerPrefs.SetInt("h_type_"+this.length_history,type);
        PlayerPrefs.SetString("h_date_"+this.length_history,System.DateTime.Now.ToString("HH:mm dd MMMM, yyyy"));
        this.length_history++;
        PlayerPrefs.SetInt("length_history",length_history);
        this.carrot.game.update_scores_player(get_total_scores());
    }

    private int get_total_scores(){
        int total_scores=0;
        for(int i=0;i<this.length_history;i++) total_scores+=PlayerPrefs.GetInt("h_score_"+i);
        return total_scores;
    }

    public void show_list_history(){
        this.GetComponent<App_wall>().play_sound(0);
        if(this.length_history==0){
            this.carrot.show_msg("Your winning history","You have never won");
        }else{
            Carrot_Box box_history=this.carrot.Create_Box("Your winning history",this.icon_history);
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
            this.carrot.game.update_scores_player(total_scores);
        }
    }

    public void show_list_rank(){
        this.carrot.game.Show_List_Top_player();
    }
}

public class Data_offline_item
{
    public string url;
    public Sprite icon_thumb;
    public Texture2D data_img;
    public int index; 
}
