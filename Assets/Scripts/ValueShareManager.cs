using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common;

public class ValueShareManager : SingletonMonoBehaviour<ValueShareManager>
{

	float screenHeightPixel;//CANBUS_WIDTHで決まる画面の縦幅(pixel
	public float ScreenHeightPixel
	{
		get { return screenHeightPixel; }
	}

	//画面の上下の余り幅
	public float ScreenSurplusHeightPixel
	{
		get { return screenHeightPixel - Define.CANBUS_HEIGHT; }
	}

	float itemIconSize;
	public float ItemIconSize
	{
		get { return itemIconSize; }
	}

	float screenPanel_posY;
	public float ScreenPanel_posY
	{
		get { return screenPanel_posY; }
	}

	float screenSlidePixel;//上にアイテムを表示するために、スクリーンを下にずらすピクセル数
	public float ScreenSlidePixel
	{
		get { return screenSlidePixel; }
	}

	float worldWidth;//CANVAS_WIDTHとCAMERA_SIZE、screenHeightPixelによって決まるワールドポイントの真ん中から左右端への距離
	public float WorldWidth
	{
		get { return worldWidth; }
	}
	//=================================================================================
	//初期化
	//=================================================================================

	void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);
		//ここまでシングルトンパターン

		screenHeightPixel = Define.CANBUS_WIDTH * ((float)Screen.height / (float)Screen.width);

		if (Define.ITEMICON_IDEALSIZE <= ScreenSurplusHeightPixel)
		{//上下の余り幅がアイテムアイコン表示のために十分ある時
			itemIconSize = Define.ITEMICON_IDEALSIZE;

		}
		else
		{//十分ない時　＊要改良
			itemIconSize = ScreenSurplusHeightPixel;
		}
		screenPanel_posY = -(itemIconSize / 2);
		screenSlidePixel = itemIconSize / 2;

		worldWidth = Define.CAMERA_SIZE / screenHeightPixel * Define.CANBUS_WIDTH;
	}


}
