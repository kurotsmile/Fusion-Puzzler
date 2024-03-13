using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Manager_wall : MonoBehaviour
{
    [Header("Obj Main")]
    public App_wall app;

    private Carrot_Window_Input box_inpu_search = null;
    private string s_json_data_wall="";

    public void On_load()
    {
        if (app.carrot.is_offline()) s_json_data_wall = PlayerPrefs.GetString("s_json_data_wall");
    }

    public void Get_list_data_background()
    {
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
        PlayerPrefs.SetString("s_json_data_wall", s_data);
        this.Act_Show_list_background_by_data(s_data);
    }

    private void Act_Show_list_background_by_data(string s_data)
    {
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            this.app.carrot.clear_contain(this.app.area_body);
            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary item_bk = fc.fire_document[i].Get_IDictionary();
                this.Add_item_to_list(item_bk);
            }
            this.app.Scroll_on_Top();
        }
    }

    private void Add_item_to_list(IDictionary data)
    {
        GameObject item_category = Instantiate(app.prefab_category);
        item_category.name = "item_category_" + data["id"].ToString();
        item_category.transform.SetParent(app.area_body);
        item_category.transform.localPosition = new Vector3(0f, 0f, 0f);
        item_category.transform.localScale = new Vector3(1f, 1f, 1f);
        item_category.transform.localRotation = Quaternion.identity;

        Panel_category p_category = item_category.GetComponent<Panel_category>();
        p_category.txt_name.text = data["name"].ToString();
        if (data["buy"] != null)
        {
            if (data["buy"].ToString() == "0")
                p_category.txt_desc.text = "Free";
            else
                p_category.txt_desc.text = "Buy";
        }
        else
        {
            p_category.txt_desc.text = "Free";
        }
        p_category.url_img = data["icon"].ToString();
        p_category.On_load(this.app, data["id"].ToString());
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
            IDictionary data = (IDictionary)Json.Deserialize(www.downloadHandler.text);
            IList list_pic = (IList)data["items"];

            for (int i = 0; i < list_pic.Count; i++)
            {
                string url_pic;
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
                    IList cse_thumbnail = (IList) pagemap["cse_thumbnail"];
                    IDictionary thumb =(IDictionary) cse_thumbnail[0];
                    url_pic = thumb["src"].ToString();
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
}
