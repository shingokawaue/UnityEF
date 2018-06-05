using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

//GameManagerのDisplayMessageButtonにアタッチするものとして開発開始
//子オブジェクトText,OnClickイベントリスナー は、スクリプトで。
public class DisplayMessageManager : MonoBehaviour {

	bool isShowing = false;
	float startTime;
	float duration = 0;//0ならずっと表示
	GameObject text;
	// Use this for initialization
	void Start () {
		Transform buf;
		buf = gameObject.transform.Find("Text");
		text = buf.transform.gameObject;

		GetComponent<Button>().onClick.AddListener(() => CloseMessage());//onClickのイベントリスナー
	}
	
	// Update is called once per frame
	void Update () {
		if (isShowing && duration > 0 && ((Time.time - startTime) > duration)){
			this.gameObject.SetActive(false);
			isShowing = false;
		}
	}

	public void DisplayMessage(string mes)
    {
		isShowing = true;
		duration = 0;
		this.gameObject.SetActive(true);
		this.gameObject.GetComponent<Text>().text = mes;
    }

	public void DisplayMessage(float time, string mes)
    {
		isShowing = true;
		startTime = Time.time;
		duration = time;
		this.gameObject.SetActive(true);
		text.GetComponent<Text>().text = mes;
    }
    
	public void CloseMessage(){
		this.gameObject.SetActive(false);
        isShowing = false;
	}

}
