using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Panel_category : MonoBehaviour {
	public Text txt_name;
	public Text txt_desc;
	public Image img_bk;
	public string url_img;

	private App_wall app;
	private Texture2D texData;
	private IDictionary data;

	public void On_load(App_wall app,IDictionary data_wall)
	{
		string s_id_wall = "wall" + data_wall["id"].ToString();
        this.app= app;
		this.texData=app.carrot.get_tool().get_texture2D_to_playerPrefs(s_id_wall);
		if (this.texData != null)
			this.img_bk.sprite = app.carrot.get_tool().Texture2DtoSprite(this.texData);
		else
			this.app.carrot.get_img_and_save_playerPrefs(url_img, null, s_id_wall, this.On_load_image_done);
    }

	public void click(){
		app.play_sound(0);

		Carrot.Carrot_Box box_menu = app.carrot.Create_Box();
		box_menu.set_icon(app.carrot.icon_carrot_game);
		box_menu.set_title("Select Game");

		Carrot.Carrot_Box_Item item_game_puzzler = box_menu.create_item("game_puzzler");
		item_game_puzzler.set_title("Puzzler");
        item_game_puzzler.set_tip("The type of game that arranges the order of given puzzle pieces");
        item_game_puzzler.set_act(() => this.play_game1());
		item_game_puzzler.set_icon(app.data_offline.icon_game1);

        Carrot.Carrot_Box_Item item_game_jigsaw = box_menu.create_item("game_jigsaw");
        item_game_jigsaw.set_title("jigsaw");
        item_game_jigsaw.set_tip("The type of puzzle you have to put together piece by piece");
		item_game_jigsaw.set_icon(app.data_offline.icon_game2);
        item_game_jigsaw.set_act(() => this.play_game2());
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
		Texture2D texPic = app.carrot.get_tool().ResampleAndCrop(tex, 280, 100);
		this.img_bk.sprite = app.carrot.get_tool().Texture2DtoSprite(texPic);
	}
}
