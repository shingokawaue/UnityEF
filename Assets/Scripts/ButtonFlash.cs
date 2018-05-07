using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ただの光るボタン。光るボタンの背景を用意して、光ってないボタンのイメージを重ねる
/// ボタンにアタッチする。親のボタンのイメージに光った状態のボタンを付けておく
/// </summary>
public class ButtonFlash : MonoBehaviour
{
	public float easingTime = 0.3f;

	private bool IsPress = false;
	private float red;
	private float green;
	private float blue;
	private float alpha = 0.0f;

	private float valueA = 0.0f;
	//イージング
	private float valueB = 1.0f;
	//イージング

	private float startTime;

	// Use this for initialization
	void Start ()
	{
		red = GetComponent<Image> ().color.r;
		green = GetComponent<Image> ().color.g;
		blue = GetComponent<Image> ().color.b;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (IsPress == false && alpha < 1.0f) {//押されていなかったらalphaを下げて光ってるように見せる
			if ((Time.time - startTime) > easingTime) {//イージングタイムオーバー
				alpha = 1.0f;
			} else {
				alpha = EasingLerps.EasingLerp (EasingLerps.EasingLerpsType.Circ,
					EasingLerps.EasingInOutType.EaseOut,
					(Time.time - startTime) / easingTime, valueA, 1.0f);
			}
			GetComponent<Image> ().color = new Color (red, green, blue, alpha);
		}

		if (IsPress == true && alpha > 0.0f) {//押されていたら画像のalphaを上げ
			if ((Time.time - startTime) > easingTime) {//イージングタイムオーバー
				alpha = 0.0f;
			} else {
				alpha = EasingLerps.EasingLerp (EasingLerps.EasingLerpsType.Circ,
					EasingLerps.EasingInOutType.EaseIn,
					(Time.time - startTime) / easingTime, 0.0f, valueB);
			}
			GetComponent<Image> ().color = new Color (red, green, blue, alpha);
		}

	}



	public void PointerDown ()
	{
		IsPress = true;
		startTime = Time.time;
		valueA = GetComponent<Image> ().color.a;
	}

	public void PointerUp ()
	{
		IsPress = false;
		startTime = Time.time;
		valueB = GetComponent<Image> ().color.a;
	}
}
