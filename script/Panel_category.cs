using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Panel_category : MonoBehaviour {
	public Text txt_name;
	public Text txt_desc;
	public Image img_bk;
	public Image img_type;

	private App_wall app;
	private Texture2D texData;
	private IDictionary data;
    private UnityAction act_click;

	public void On_load(App_wall app,IDictionary data_wall)
	{
        string s_id= data_wall["id"].ToString();
        string s_id_wall = "wall" + s_id;
        this.app= app;
		this.data=data_wall;
		this.texData=app.carrot.get_tool().get_texture2D_to_playerPrefs(s_id_wall);
		if (this.texData != null)
			this.img_bk.sprite = this.Resize_img(this.texData);
		else
			if(data_wall["icon"]!=null) this.app.carrot.get_img_and_save_playerPrefs(data_wall["icon"].ToString(), null, s_id_wall, this.On_load_image_done);

        this.txt_name.text = data["name"].ToString();

        bool is_buy;
        if (data["buy"] != null)
        {
            if (data["buy"].ToString() == "0")
                is_buy = false;
            else
            {
                if (app.wall.Check_Wall_used(s_id))
                    is_buy = false;
                else
                    is_buy = true;
            }
        }
        else
        {
            is_buy = false;
        }

        if (is_buy)
        {
            this.img_type.sprite = app.carrot.icon_carrot_buy;
            this.txt_desc.text = "Buy";
            this.act_click = () => app.wall.Buy_wall(texData, data,s_id);
        }
        else
        {
            this.img_type.sprite = app.carrot.icon_carrot_game;
            this.txt_desc.text = "Free";
            this.act_click=()=>app.wall.Show_select_game(texData, data);
        }
    }

	public void click(){
        act_click?.Invoke();
    } 

	public void play_game1(){
		this.app.play_game_1(this.texData,this.data);
	}

	public void play_game2(){
		this.app.play_game_2(this.texData,this.data);
	}

	public void On_load_image_done(Texture2D tex)
	{
		this.texData = tex;
        this.img_bk.sprite =Resize_img(tex);
	}

	private Sprite Resize_img(Texture2D tex)
	{
        Texture2D texPic;
        if (app.app_scene.get_status_portrait())
        {
            texPic = app.carrot.get_tool().ResampleAndCrop(tex, 280, 100);
        }
        else
        {
            texPic = app.carrot.get_tool().ResampleAndCrop(tex, 480, 100);
        }
		return app.carrot.get_tool().Texture2DtoSprite(texPic);
    }
}
