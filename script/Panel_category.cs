using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_category : MonoBehaviour {
	public Text txt_name;
	public Text txt_desc;
	public Image[] img_bk;
	public string id_cat;

	float count_next_bk=0f;
	int index_bk_show=0;

	public IList list_url_game1;
	public IList list_url_game2;

	void Start(){
		this.count_next_bk = Random.Range (0f, 4f);
	}

	void Update(){
		this.count_next_bk += Time.deltaTime * 1f;
		if (this.count_next_bk > 4f) {
			this.count_next_bk = 0f;
			this.next_bk ();
		}
	}

	private void next_bk(){
		this.index_bk_show++;
		if (this.index_bk_show >= 3) {
			this.index_bk_show = 0;
		}

		this.img_bk [0].gameObject.SetActive (false);
		this.img_bk [1].gameObject.SetActive (false);
		this.img_bk [2].gameObject.SetActive (false);

		this.img_bk [index_bk_show].gameObject.SetActive (true);
	}

	public void click(){
		GameObject.Find ("app_wall").GetComponent<App_wall>().show_bk_in_category(this.id_cat);
	} 

	public void play_game1(){
		GameObject.Find ("app_wall").GetComponent<App_wall>().play_game1(this.list_url_game1[index_bk_show].ToString());
	}

	public void play_game2(){
		GameObject.Find ("app_wall").GetComponent<App_wall>().play_game2(this.list_url_game2[index_bk_show].ToString(),this.list_url_game1[index_bk_show].ToString());
	}
}
