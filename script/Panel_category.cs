using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Panel_category : MonoBehaviour {
	public Text txt_name;
	public Text txt_desc;
	public Image img_bk;
	public string url_img;

	private App_wall app;

	public void On_load(App_wall app,string s_id)
	{
		string s_id_wall = "wall" + s_id;
        this.app= app;
		Sprite sp_pic=app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_wall);
		if (sp_pic != null)
			this.img_bk.sprite = sp_pic;
		else
			this.app.carrot.get_img_and_save_playerPrefs(url_img, null, s_id_wall, this.On_load_image_done);
    }

	public void click(){
		Carrot.Carrot_Box box_menu = app.carrot.Create_Box();
		box_menu.set_icon(app.carrot.icon_carrot_game);
		box_menu.set_title("Select Game");

		Carrot.Carrot_Box_Item item_game_puzzler = box_menu.create_item("game_puzzler");
		item_game_puzzler.set_title("Puzzler");
		item_game_puzzler.set_act(() => this.play_game1());
		item_game_puzzler.set_icon(app.data_offline.icon_game1);

        Carrot.Carrot_Box_Item item_game_jigsaw = box_menu.create_item("game_jigsaw");
        item_game_jigsaw.set_title("jigsaw puzzle");
        item_game_jigsaw.set_act(() => this.play_game1());
		item_game_jigsaw.set_icon(app.data_offline.icon_game2);
    } 

	public void play_game1(){
		this.app.play_game1(this.url_img);
	}

	public void play_game2(){
		this.app.play_game2(this.url_img,this.url_img);
	}

	public void load_image()
	{

	}

	public void On_load_image_done(Texture2D tex)
	{
		Texture2D texPic = app.ResizeAndCrop(tex, 280, 100);
		this.img_bk.sprite = app.carrot.get_tool().Texture2DtoSprite(texPic);
	}
}
