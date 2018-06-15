using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Common;
using UnityEngine.EventSystems;
//LeftBottomPointは自分で作った概念、画面の左下を基準にしたPNG画像のピクセル数のサイズに合わせた座標
public class CalcCoordinate : SingletonMonoBehaviour<CalcCoordinate>
{

	float a;
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
	}


	public Vector2 CalcWorldToLeftBottomPoint(Vector3 world)
	{
		float x, y;

		x = world.x; y = world.y;
		//Debug.Log("world:x:" + x + " y:" + y);
		x = x + ValueShareManager.Instance.WorldWidth;
		y = y + Define.CAMERA_SIZE;
		//Debug.Log("worldEdit:x:" + x + " y:" + y);
		x = x * (Define.CANBUS_WIDTH / (ValueShareManager.Instance.WorldWidth * 2));
		y = y * (ValueShareManager.Instance.ScreenHeightPixel / (Define.CAMERA_SIZE * 2));

		return new Vector2(x, y);
	}
}
