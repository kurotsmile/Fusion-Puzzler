using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_search : MonoBehaviour
{
    public Image icon;
    public string url;
    public Text txt_name;
    public void click(){
        GameObject.Find("app_wall").GetComponent<App_wall>().show_view_by_url_image(this.url);
    }
}
