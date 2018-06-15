using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Common;
[RequireComponent(typeof(Image))]
public class TransparencyButton : MonoBehaviour, IPointerClickHandler
{

	Image image;
	TransparencyMap transparencyMap;
	void Start()
	{
		image = GetComponent<Image>();
		transparencyMap = TransparencyMap.Load(image.sprite);
	}

	public void OnPointerClick(PointerEventData eventdata)
	{
		Debug.Log("Screen.width:" + Screen.width);
		Debug.Log("Screen.height:" + Screen.height);
		Debug.Log("event.point:" + eventdata.position.x + "y:" + eventdata.position.y);
		Debug.Log("x:" + eventdata.position.x * ValueShareManager.Instance.ScreenHeightPixel / Screen.height +
				  "y:" + eventdata.position.y * Define.CANBUS_WIDTH / Screen.width);
		float mousex, mousey;
		mousex = eventdata.position.x * ValueShareManager.Instance.ScreenHeightPixel / Screen.height;
		mousey = eventdata.position.y * Define.CANBUS_WIDTH / Screen.width;

		Vector2 buf = CalcCoordinate.Instance.CalcWorldToLeftBottomPoint(image.transform.position);
		float imageStartingPointx = buf.x - (int)(GetComponent<RectTransform>().sizeDelta.x / 2);
		float imageStartingPointy = buf.y - (int)(GetComponent<RectTransform>().sizeDelta.y / 2);


		Debug.Log("startx:" + imageStartingPointx + "starty:" + imageStartingPointy);
		//Debug.Log("trax:" + imageStartingPointx + "tray:" + transform.position.y);
		if (mousex >= imageStartingPointx && mousex <= imageStartingPointx + image.sprite.texture.width
			&& mousey >= imageStartingPointy && mousey <= imageStartingPointy + image.sprite.texture.height)
		{
			float x, y;
			x = mousex - imageStartingPointx;
			y = mousey - imageStartingPointy;
			Debug.Log("x :" + x + "y: " + y);
			Debug.Log(transparencyMap.IsTransparency((int)x, (int)y));
		}
		else
		{
			Debug.Log("クリック範囲ずれ");
			return;
		}
	}


}
