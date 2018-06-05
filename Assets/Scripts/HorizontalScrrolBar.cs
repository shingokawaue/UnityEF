using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity;
using UnityEngine.EventSystems;
using Common;
using System;
//画面からはみ出す画像をスクロールさせるためのバーを表示するクラス。(横のスクロール）
//スクロールしたい画像を持つクラスからスクロールしたい画像の幅のpixel数を受け取る。(その情報を元にバーを表示
//バーの位置が変わった時にイベントを発生させる。
//スクロールしたい画像を持つクラスから、OnScrollBarMovedに対してイベントハンドラを追加。

public delegate void ScrollBarMovedEventHandler(float pos);

public class HorizontalScrrolBar : MonoBehaviour
{
 //イベントデリゲート の宣言
	public event ScrollBarMovedEventHandler OnScrollBarMoved;
    //このクラスでイベントを発生させる。（このデリゲート を呼び出す


	//protected virtual void OnScrollBarMoved(EventArgs e)
  //  {
		//if (OnScrollBarMoved != null)//C＃ではイベントを発生させる際に、デリゲート がnullかどうか確認する必要がある
			//OnScrollBarMoved(this, e);//senderはこのクラス、イベントデータはEventArgs
    //}

	GameObject barLeftImage,barMiddleImage,barRightImage;//バーのイメージ、プレハブからアタッチ

	public const float BAR_SIDEWIDTH = 20.0f;

	public float yPos;
	float beginDragPosX;
	float beginDragMyLocalPosX;//どらっぐが始まった時のバーのローカルポジションx位置
	float barWidth;
	float barMoveWidth;//バーが動けるピクセル数　いるかな？
	float imageWidth;//スクロールする目的のイメージのWidth
	//動かせるバーの右端から移動最大幅の右端までの距離
    //float rightSpace; //(  ■■■動かせるバー■■■←ここから　　　　　　　　ここまで→)＊Figure①
	//float scrollPixel = 0.0f;//スクロールしたい画像のローカルポス
		
	public GameObject imageForUI;//プレハブからアタッチ Instantiate用

	public float pixelsOfSize;

	int currentRight;
	bool candrag = false;
	public bool CanDrag{
		set { candrag = value; }
	}
 
	void Awake()
	{
		pixelsOfSize = (float)(1125.0 * (float)Screen.height / (float)Screen.width / (Define.CAMERA_SIZE * 2));

		barLeftImage = Instantiate(imageForUI);
        barRightImage = Instantiate(imageForUI);
        barMiddleImage = Instantiate(imageForUI);
        barLeftImage.GetComponent<Image>().sprite = Resources.Load("Image/UI/horizontalBarLeft", typeof(Sprite)) as Sprite;
        barRightImage.GetComponent<Image>().sprite = Resources.Load("Image/UI/horizontalBarRight", typeof(Sprite)) as Sprite;
        barMiddleImage.GetComponent<Image>().sprite = Resources.Load("Image/UI/horizontalBarMiddle", typeof(Sprite)) as Sprite;
		barLeftImage.GetComponent<RectTransform>().sizeDelta = new Vector2(BAR_SIDEWIDTH,Define.ITEMICON_SCROLLBARHEIGHT);
		barRightImage.GetComponent<RectTransform>().sizeDelta = new Vector2(BAR_SIDEWIDTH, Define.ITEMICON_SCROLLBARHEIGHT);
		barMiddleImage.GetComponent<RectTransform>().sizeDelta = new Vector2(90.0f, Define.ITEMICON_SCROLLBARHEIGHT);
		GetComponent<RectTransform>().sizeDelta = new Vector2(100.0f,barMiddleImage.GetComponent<RectTransform>().sizeDelta.y );



        barMiddleImage.transform.SetParent(this.transform.root.gameObject.transform);
		barMiddleImage.transform.localPosition = new Vector3(0,transform.localPosition.y);
        barMiddleImage.transform.localScale = Vector3.one;    //親子関係セット直後はサイズがおかしくなるので、ローカルサイズを１にセット
        barMiddleImage.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
		barLeftImage.transform.SetParent(this.transform.root.gameObject.transform);
		barLeftImage.transform.localPosition = new Vector3(-(barWidth / 2) + BAR_SIDEWIDTH / 2, transform.localPosition.y);
		barLeftImage.transform.localScale = Vector3.one;    //親子関係セット直後はサイズがおかしくなるので、ローカルサイズを１にセット
		barLeftImage.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
		barRightImage.transform.SetParent(this.transform.root.gameObject.transform);
		barRightImage.transform.localPosition = new Vector3((barWidth / 2) - BAR_SIDEWIDTH / 2, transform.localPosition.y);
		barRightImage.transform.localScale = Vector3.one;    //親子関係セット直後はサイズがおかしくなるので、ローカルサイズを１にセット
		barRightImage.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        //inspectorで追加したイベントトリガーにコールバックを設定
        EventTrigger.Entry found = barLeftImage.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.Drag);
        found.callback.AddListener((BaseEventData) => OnDrag());
		found = barLeftImage.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.PointerDown);
		found.callback.AddListener((BaseEventData) => PointerDown());

        found = barRightImage.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.Drag);
        found.callback.AddListener((BaseEventData) => OnDrag());
		found = barRightImage.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.PointerDown);
		found.callback.AddListener((BaseEventData) => PointerDown());

        found = barMiddleImage.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.Drag);
        found.callback.AddListener((BaseEventData) => OnDrag());
		found = barMiddleImage.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.PointerDown);
		found.callback.AddListener((BaseEventData) => PointerDown());

		Hide();
	}

	// Use this for initialization
	void Start()
	{
  
	}

	// Update is called once per frame
	void Update()
	{

	}
 
	public void PointerDown(){
		Vector3 TapPos = Input.mousePosition;

        TapPos.z = 10f;
        GameObject UICamObj = GameObject.FindGameObjectWithTag("UICamera");//UIカメラを取得
        Camera cam = UICamObj.GetComponent<Camera>();

        beginDragPosX = cam.ScreenToWorldPoint(TapPos).x;
        beginDragMyLocalPosX = barMiddleImage.transform.localPosition.x;
		Debug.Log("pointerdown" + "world" + beginDragPosX.ToString() + "local" + barMiddleImage.transform.localPosition.x + "pixelsofsize" + pixelsOfSize.ToString());
	}

    public void OnDrag()
    {
        if (candrag == true)
        {
            Vector3 TapPos = Input.mousePosition;
            TapPos.z = 10f;
            GameObject UICamObj = GameObject.FindGameObjectWithTag("UICamera");
            Camera cam = UICamObj.GetComponent<Camera>();
			float dragPosX;
			dragPosX = cam.ScreenToWorldPoint(TapPos).x;

			float bufLocalX = beginDragMyLocalPosX + (dragPosX - beginDragPosX) * pixelsOfSize;

			Debug.Log( ((dragPosX - beginDragPosX) * pixelsOfSize).ToString()  );

			if(bufLocalX < -(barMoveWidth / 2)){
				bufLocalX = -(barMoveWidth / 2);
			}
			if (bufLocalX > (barMoveWidth / 2))
            {
                bufLocalX = (barMoveWidth / 2);
            }

			barMiddleImage.transform.localPosition = new Vector3(bufLocalX , transform.localPosition.y);
			barLeftImage.transform.localPosition = new Vector3(bufLocalX - (barWidth / 2) + BAR_SIDEWIDTH / 2, transform.localPosition.y);
			barRightImage.transform.localPosition = new Vector3(bufLocalX + (barWidth / 2) - BAR_SIDEWIDTH / 2, transform.localPosition.y);
			float scrollPixel = ((Define.CANBUS_WIDTH - barWidth) / 2 - bufLocalX) * (imageWidth / Define.CANBUS_WIDTH);
			//　　　　　　　　　　　　　(スクロールバーの右の空きスペース)　　　　　　　*(比率)
			OnScrollBarMoved(scrollPixel);//イベントを発生させる！！ (こっちのタイミングでデリゲート を実行する
   
        }

    }
    
	public void CalcBarWidth(float imagewidth){
		imageWidth = imagewidth;
		barWidth = Define.CANBUS_WIDTH * Define.CANBUS_WIDTH / imagewidth;
		Debug.Log(this.name + ":CalcBarWidth(float imagewidth)        " + "imagewidth = " + imagewidth);
		barMoveWidth = Define.CANBUS_WIDTH - barWidth;
        
		float posBuf = barMiddleImage.transform.localPosition.x;
		barMiddleImage.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth - 20.0f, Define.ITEMICON_SCROLLBARHEIGHT);
		barLeftImage.transform.localPosition = new Vector3(posBuf - (barWidth / 2) + BAR_SIDEWIDTH / 2, transform.localPosition.y);
		barRightImage.transform.localPosition = new Vector3(posBuf + (barWidth / 2) - BAR_SIDEWIDTH / 2, transform.localPosition.y);
        //バーの大きさが変わった時に両はしからはみ出さないように調整する
		if (posBuf < -(barMoveWidth / 2))
        {
			barMiddleImage.transform.localPosition = new Vector3 (-(barMoveWidth / 2),barMiddleImage.transform.localPosition.y) ;
        }
		if (posBuf > (barMoveWidth / 2))
        {
			barMiddleImage.transform.localPosition = new Vector3((barMoveWidth / 2), barMiddleImage.transform.localPosition.y);
        }
	}



	public void Display(float imagewidth , float pos){//posはバーの初期位置、0.0~1.0で0.0で左端、1.0で右端
		Debug.Log(this.name + ":Display(float imagewidth , float pos)");
		barLeftImage.SetActive(true);
		barRightImage.SetActive(true);
		barMiddleImage.SetActive(true);
		CalcBarWidth(imagewidth);
  //バーの初期位置設定
		barMiddleImage.transform.localPosition = new Vector3( -(barMoveWidth * (pos - 0.5f)),transform.localPosition.y);
		barLeftImage.transform.localPosition = new Vector3(-(barMoveWidth * (pos - 0.5f)) - (barWidth / 2) + BAR_SIDEWIDTH / 2, transform.localPosition.y);
		barRightImage.transform.localPosition = new Vector3(-(barMoveWidth * (pos - 0.5f)) + (barWidth / 2) - BAR_SIDEWIDTH / 2, transform.localPosition.y);
	}

	public void Hide(){
		barLeftImage.SetActive(false);
		barRightImage.SetActive(false);
		barMiddleImage.SetActive(false);
	}

    //実際にアタッチするクラスに渡す情報は、
	//動かせるバーの右端から移動最大幅の右端までの距離(＊Figure①)を渡せば、向こうで計算できそう、なのでそうしてみる。
}
