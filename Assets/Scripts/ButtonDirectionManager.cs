using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// <summary>
/// Button direction manager.
/// 押すと方向の表示が変わるボタン
/// ボタン一つ一つにアタッチ
/// ボタンにイベントトリガーPointerUp PointerDownを追加してアタッチ
/// EasingLerpsクラスを使用。
/// </summary>/

//
public class ButtonDirectionManager : MonoBehaviour
{
	private const int DOWN = 0;
	private const int LEFT = 1;
	private const int UP = 2;
	private const int RIGHT = 3;


	public float easingTime = 0.3f;
	public int direction = 0;
	public GameObject directionImage;

	public bool canpush = true;
	//方向を示す画像


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
	//ボタン画像のalpha値
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
		
		if (IsPress == true && alpha < 1.0f) {//押されていたら画像のalphaを上げて光っているように見せる
			if ((Time.time - startTime) > easingTime) {//イージングタイムオーバー
				alpha = 1.0f;
			} else {
				alpha = EasingLerps.EasingLerp (EasingLerps.EasingLerpsType.Circ,
					EasingLerps.EasingInOutType.EaseOut,
					(Time.time - startTime) / easingTime, valueA, 1.0f);
			}
			GetComponent<Image> ().color = new Color (red, green, blue, alpha);
		}

		if (IsPress == false && alpha > 0.0f) {//押されていなかったらalphaを下げる
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
		if (canpush == false)
			return;
		
		IsPress = true;
		startTime = Time.time;
		valueA = GetComponent<Image> ().color.a;

		++direction;
		if (direction == 4) {
			direction = DOWN;
		}

		UpdateDirectionImage ();
	}

	public void PointerUp ()
	{
		if (canpush == false)
			return;

		IsPress = false;
		startTime = Time.time;
		valueB = GetComponent<Image> ().color.a;
	}


	void UpdateDirectionImage ()
	{		
		switch (direction) {
		case DOWN:			
			directionImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			break;
		case LEFT:
			directionImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -90));
			break;
		case UP:
			directionImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -180));
			break;
		case RIGHT:
			directionImage.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -270));
			break;
		}
	}
}
