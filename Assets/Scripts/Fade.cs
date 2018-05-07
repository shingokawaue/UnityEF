using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour {

	GameObject fadeObj;

	float startTime;

	public int fadeTime;
	Color alpha;
	string fadeStart;


	// Use this for initialization
	void Start () {
		fadeObj = GameObject.Find ("Fade");
		startTime = Time.time;
		fadeStart = "FadeIn";
	}
	
	// Update is called once per frame
	void Update () {
		switch (fadeStart) {
		case"FadeIn":
			alpha.a = 1.0f - (Time.time - startTime) / fadeTime;
			fadeObj.GetComponent<Image>().color = new Color (0, 0, 0, alpha.a);
			break;
		case"FadeOut":
			alpha.a = (Time.time - startTime) / fadeTime;
			fadeObj.GetComponent<Image>().color = new Color (0, 0, 0, alpha.a);
			break;
		}

		if (Input.GetButtonDown ("Jump")) {
			fadeStart = "fadeOut";
			startTime = Time.time;
			Invoke ("Load", 1.5f);//1.5秒後にLoad method launch

		}
	}


	public void Load(){
		//Application.LoadScene (Application.loadedLevelName);

	}



}
