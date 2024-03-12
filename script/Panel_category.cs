using UnityEngine;
using UnityEngine.UI;

public class Panel_category : MonoBehaviour {
	public Text txt_name;
	public Text txt_desc;
	public Image img_bk;
	public string url_img;

	void Start(){
		
	}

	public void click(){

	} 

	public void play_game1(){
		GameObject.Find ("app_wall").GetComponent<App_wall>().play_game1(this.url_img);
	}

	public void play_game2(){
		GameObject.Find ("app_wall").GetComponent<App_wall>().play_game2(this.url_img,this.url_img);
	}
}
