using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_item_data_offline_img : MonoBehaviour
{
    public Image img_Image;
    public Texture2D data_img;
    public int index;
    public bool is_home;
    public string url;
    public void click()
    {
        GameObject.Find("app_wall").GetComponent<App_wall>().play_sound(0);
        GameObject.Find("app_wall").GetComponent<App_wall>().show_view_in_offline(this.data_img,this.url);
    }
    public void game1()
    {
        GameObject.Find("app_wall").GetComponent<App_wall>().play_game_1(this.data_img);
        
    }
    public void game2()
    {
        GameObject.Find("app_wall").GetComponent<App_wall>().play_game_2_in_offline_and_view_bk(this.data_img,this.url);
    }

   public void delete()
    {
        GameObject.Find("app_wall").GetComponent<Data_Offline>().delete(index,this.is_home);
    }
}
