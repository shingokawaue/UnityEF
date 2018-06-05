using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using System;
using Common;
/// <summary>
/// Button item view UIとして画面の手前で使う感じです
/// buttonDarkとbuttonCloseはPrefabに置いておく
/// アセットの　Image/ItemPanel/ にアイテム画像を入れておいてファイル名で読み込む
/// </summary>
public class ButtonItemView : MonoBehaviour
{
	#region//宣言など
	GameObject gameManager, inputManager , valueShareManager;//Startメソッドで初期化
	public GameObject buttonItemIcon;
	//プレハブからアタッチ
	public bool cantap = true;

	//適当な画像の入ってないImageをアタッチ
	public float duration = 0.5f;
	private float buttonSize;
	private const float BUTTONMARGIN = 40.0f;
	float itemIconSize;
	private float PreviousTapTime;//ダブルタップ判定用
								  //ボタンの大きさと合わせる
	public GameObject imageFlame;//ヒエラルキーに子として作っておきアタッチ
	public GameObject imageForViewChange;//子として作っておきアタッチ
	public GameObject buttonClose;//プレハブからInstantiate
	public GameObject buttonDark;//プレハブから、Instantiate
	public GameObject imageBlackTop;//プレハブからImageBlackをアタッチ、Instantiate
	public GameObject imageBlackBottom;//プレハブからImageBlackをアタッチ、Instantiate

	public GameObject iconScrollBar;//ヒエラルキーからアタッチ

	List<GameObject> items = new List<GameObject>();
	List<string> itemsString = new List<string>();//save用

	public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
	//アイテムゲットエフェクトのイージング用

	Vector3 iconInitialPos;
	float iconInitialPosX;
	float iconInitialPosY;
	//アイコンの追加される位置
	bool getFlag;
	public string selected = "";
	public string shown = "";//ビューに表示されてるアイテム

	float scrollPos = 0.0f;//アイコンスクロール用、localpositionのxに足して使う。
	bool getNow;
	bool isShowing;//表示しているか

	float barWidth = 0.0f;
	float barPos = 0.0f;//左端0.0f 右端1.0f
	#endregion



	void Awake()//Startと分けている意味は特にない
	{
		if (Application.platform == RuntimePlatform.OSXEditor)
        {
            Debug.Log("MacOSEditor");
            buttonSize = 150.0f;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Debug.Log("iOS");
            buttonSize = Screen.dpi / 5;
        }

  
		buttonDark = Instantiate (buttonDark);
		buttonDark.GetComponent<Button> ().onClick.AddListener (() => PushButtonDark ());
		buttonDark.transform.SetParent (this.transform.root.gameObject.transform);
		buttonDark.transform.localScale = Vector3.one;
		buttonDark.transform.localPosition = Vector3.zero;
		buttonDark.transform.SetSiblingIndex (transform.GetSiblingIndex());

		buttonClose = Instantiate(buttonClose);
        buttonClose.transform.SetParent(this.transform.root.gameObject.transform);
        buttonClose.transform.localScale = Vector3.one;
		buttonClose.transform.localPosition = new Vector3(Define.ITEMVIEW_IDEALSIZE / 2 - buttonSize / 2 - BUTTONMARGIN ,
		                                                  Define.ITEMVIEW_IDEALSIZE / 2 - buttonSize / 2 - BUTTONMARGIN);
        buttonClose.GetComponent<Button>().onClick.AddListener(() => PushButtonClose());
		buttonClose.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonSize, buttonSize);
  
		gameManager = GameObject.FindWithTag("GameManager");
        inputManager = GameObject.FindWithTag("InputManager");
        valueShareManager = GameObject.FindWithTag("ValueShareManager");

        itemIconSize = valueShareManager.GetComponent<ValueShareManager>().ItemIconSize;
        iconInitialPosX = (Define.CANBUS_WIDTH / 2) - (itemIconSize / 2) - Define.ITEMICON_MARGIN;
        iconInitialPosY = (Define.CANBUS_HEIGHT / 2);
        iconInitialPos = new Vector3(iconInitialPosX, iconInitialPosY);

	}
	// Use this for initialization
	void Start ()
	{
		
		iconScrollBar.GetComponent<HorizontalScrrolBar>().OnScrollBarMoved += OnScrollBarMoved;//
		iconScrollBar.transform.localPosition = new Vector3(0.0f,iconInitialPosY - itemIconSize /2 
		                                                    - iconScrollBar.GetComponent<RectTransform>().sizeDelta.y / 2);

		imageBlackTop = Instantiate(imageBlackTop);
        imageBlackTop.transform.SetParent(this.transform.root.gameObject.transform);
        imageBlackTop.transform.localScale = Vector3.one;
		imageBlackTop.transform.localPosition = new Vector3(0.0f, (Define.CANBUS_HEIGHT / 2) - (valueShareManager.GetComponent<ValueShareManager>().ScreenSlidePixel / 2)
		                                                 + (valueShareManager.GetComponent<ValueShareManager>().ScreenSurplusHeightPixel / 4) );
		imageBlackTop.GetComponent<RectTransform>().sizeDelta = new Vector2(Define.CANBUS_WIDTH,
		                                                                 valueShareManager.GetComponent<ValueShareManager>().ScreenSurplusHeightPixel / 2
		                                                                 + valueShareManager.GetComponent<ValueShareManager>().ScreenSlidePixel);
		imageBlackTop.transform.SetSiblingIndex(transform.GetSiblingIndex());

		imageBlackBottom = Instantiate(imageBlackBottom);
		imageBlackBottom.transform.SetParent(this.transform.root.gameObject.transform);
		imageBlackBottom.transform.localScale = Vector3.one;
		imageBlackBottom.transform.localPosition = new Vector3(0.0f, -(Define.CANBUS_HEIGHT / 2) - (valueShareManager.GetComponent<ValueShareManager>().ScreenSlidePixel / 2)
                                                         - (valueShareManager.GetComponent<ValueShareManager>().ScreenSurplusHeightPixel / 4));
		imageBlackBottom.GetComponent<RectTransform>().sizeDelta = new Vector2(Define.CANBUS_WIDTH,
                                                                         valueShareManager.GetComponent<ValueShareManager>().ScreenSurplusHeightPixel / 2
                                                                         - valueShareManager.GetComponent<ValueShareManager>().ScreenSlidePixel);
		imageBlackBottom.transform.SetSiblingIndex(transform.GetSiblingIndex());

		getFlag = false;
		isShowing = false;
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (Define.ITEMVIEW_IDEALSIZE, Define.ITEMVIEW_IDEALSIZE);
		imageForViewChange.GetComponent<RectTransform>().sizeDelta = new Vector2(Define.ITEMVIEW_IDEALSIZE, Define.ITEMVIEW_IDEALSIZE);

		GetComponent<Button> ().transition = Selectable.Transition.None;//大きいビューは押したエフェクトなし


	}
	
	// Update is called once per frame
	public void Update ()
	{
  
        if (isShowing && (int)GetComponent<RectTransform>().sizeDelta.x == (int)Define.ITEMVIEW_IDEALSIZE)
        {
            buttonClose.GetComponent<Image>().enabled = true; //ビューが最大化されたら閉じるボタン表示
        }

		if(Screen.width < items.Count * itemIconSize + (items.Count - 1 ) * Define.ITEMICON_MARGIN){
			


		}else{


		}


	}



    //-----------------------------------------------------------------------------------------
    //                              メンバアクセス
    //-----------------------------------------------------------------------------------------
	public bool DoesHave(string name){//引数で指定された名前のアイテムを持ってるかどうかを返す
		foreach(GameObject obj in items){
			if(obj.name == name){
				return true;
			}
		}
		return false;
	}
	//-----------------------------------------------------------------------------------------
    //                              表示系統
    //-----------------------------------------------------------------------------------------
    
	void DisplayBar(){//アイコンが画面からはみ出すとスクロールバーを表示する
		if( (int)(items.Count * itemIconSize + items.Count * Define.ITEMICON_MARGIN) < Screen.width ){
			return;
		}
		//return (Screen.width / (items.Count * itemIconSize + items.Count * Define.ITEMICON_MARGIN)) * Screen.width;
	}



	public void ShowItem (string name)
	{
		GameObject foundItem = items.Find (item => item.name == name);//ラムダ式。意味は理解していない。
		//Findメソッドは、要素が見つからない時は、型の規定値を返す
		if (foundItem.name != name) {
			Debug.Log (name + "というアイテムはありません。");
			return;
		}
		isShowing = true;
        shown = name;
		buttonDark.GetComponent<Image> ().enabled = true;

		this.GetComponent<Image> ().sprite = foundItem.GetComponent<Button> ().image.sprite;
		this.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Define.ITEMVIEW_IDEALSIZE, Define.ITEMVIEW_IDEALSIZE);

		this.GetComponent<Image> ().enabled = true;
        buttonClose.GetComponent<Image>().enabled = true;
	}

	private void HideItem ()
	{
		isShowing = false;
		buttonDark.GetComponent<Image> ().enabled = false;
		imageFlame.GetComponent<Image> ().enabled = false;
		buttonClose.GetComponent<Image> ().enabled = false;
		if (getFlag == true) {
			StartCoroutine (GetEffect ());
			return;
		}
		this.GetComponent<Image> ().enabled = false;

	}
    


   

	//-----------------------------------------------------------------------------------------
    //                              アイテム所持の操作
    //-----------------------------------------------------------------------------------------
	public IEnumerator ViewItemChangeTo(float changetime, string name)
    {//表示されているビューのアイテムを引数のアイテムに変える
        Sprite spr = Resources.Load("Image/ItemPanel/" + name, typeof(Sprite)) as Sprite;

        if (spr == null)
        {
            Debug.Log(name + "というアイテムはありません");
            yield break;
        }
        cantap = false;

        imageForViewChange.SetActive(true);
        imageForViewChange.GetComponent<Image>().sprite = spr;
        imageForViewChange.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        float startTime = Time.time;

        while ((Time.time - startTime) < changetime)
        {
            imageForViewChange.GetComponent<Image>().color = new Color(1, 1, 1,
        0.0f + 1.0f * ((Time.time - startTime) / changetime)
                                                                      );
            yield return 0;
        }
        GetComponent<Image>().sprite = spr;
        imageForViewChange.SetActive(false);//画像のフェードが終わったらimageForViewと本体を入れ替える

        ItemChangeTo(shown, name);

        cantap = true;
    }


	public void GetItem (string name)
{
		Sprite spr = Resources.Load ("Image/ItemPanel/" + name, typeof(Sprite)) as Sprite;

		if (spr == null) {
			Debug.Log (name + "というアイテムはありません");
			return;
		}
		getFlag = true;
		if (iconScrollBar.activeSelf) iconScrollBar.GetComponent<HorizontalScrrolBar>().CanDrag = false;
		SetIconCanDrag(false);//アイコンをドラッグ出来なくする
		GameObject newb = Instantiate (buttonItemIcon);
		newb.name = name;
		newb.GetComponent<Button> ().image.sprite = spr;
		newb.GetComponent<Button> ().onClick.AddListener (() => IconClicked (newb.gameObject));//onClickのイベントリスナー取り付け

        //inspectorで追加したイベントトリガーにコールバックを設定
        EventTrigger.Entry found = newb.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.EndDrag);
        found.callback.AddListener( (BaseEventData) => IconDragEnd(newb) );

		newb.GetComponent<RectTransform> ().sizeDelta = new Vector2 (itemIconSize, itemIconSize);
		newb.transform.SetParent (this.transform.root.gameObject.transform);
		newb.transform.localScale = Vector3.one;    //親子関係セット直後はサイズがおかしくなるので、ローカルサイズを１にセット
        newb.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
		items.Add (newb);
		itemsString.Add(newb.name);
		SaveData.Instance.itemList = itemsString;
        SaveData.Instance.Save();
        
		Debug.Log("GetItem:" + name);
		ShowItem (name);
	}

	public void RemoveItemAndUpdatePos(string name){
		RemoveItem(name);
		IconPosUpdate();
	}

    public void RemoveItem(string name)
    {
        int i,j;
        i = items.FindIndex(item => item.name == name);
        if (i == -1) {
            Debug.Log(name + "というアイテムがないので削除できません");
            return;
        }
		j = itemsString.FindIndex(items => items == name);
        

        Destroy(items[i]);
        items.RemoveAt(i);
        itemsString.RemoveAt(j);
		SaveData.Instance.itemList = itemsString;
		SaveData.Instance.Save();
		Debug.Log("RemoveItem " + name);
		CheckIconWidth();
		scrollPos = 0.0f;//←０じゃない
        if (selected == name){
            selected = "";
        }
    }

    /// <summary>
    /// Applies the save data.
    /// </summary>
	public void ApplySaveData()
    {
        itemsString = SaveData.Instance.itemList;
        foreach (GameObject obj in items)
        {
            Destroy(obj);
        }
        items = new List<GameObject>();

        foreach (string itemname in itemsString)
        {
            Sprite spr = Resources.Load("Image/ItemPanel/" + itemname, typeof(Sprite)) as Sprite;
            GameObject newb = Instantiate(buttonItemIcon);
            newb.name = itemname;
            newb.GetComponent<Button>().image.sprite = spr;
            newb.GetComponent<Button>().onClick.AddListener(() => IconClicked(newb.gameObject));//onClickのイベントリスナー取り付け

            //inspectorで追加したイベントトリガーにコールバックを設定
            EventTrigger.Entry found = newb.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.EndDrag);
            found.callback.AddListener((BaseEventData) => IconDragEnd(newb));

            newb.GetComponent<RectTransform>().sizeDelta = new Vector2(itemIconSize, itemIconSize);
            newb.transform.SetParent(this.transform.root.gameObject.transform);
            newb.transform.localScale = Vector3.one;    //親子関係セット直後はサイズがおかしくなるので、ローカルサイズを１にセット
            newb.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
			newb.GetComponent<Image>().enabled = true;
            items.Add(newb);
        }
		IconPosUpdate(false);
    }

	public void ItemChangeTo(string currentname, string name)
    {
        Sprite spr = Resources.Load("Image/ItemPanel/" + name, typeof(Sprite)) as Sprite;
        if (spr == null)
        {
            Debug.Log(name + "というアイテムはありませんItemChangeTo");
            return;
        }

        GetComponent<Image>().sprite = spr;

        int id;
        id = items.FindIndex(item => item.name == currentname);

		items[id].name = name;
		items[id].GetComponent<Image>().sprite = spr;
    }

    /// <summary>
    /// Update Icons position.
    /// </summary>
    /// <param name="usecol">If set to <c>true</c> usecol.</param>
	public void IconPosUpdate(bool usecol = true){
		for (int i = 0; i < items.Count; ++i)//整列
        {//アイコンの位置整理
			if (  (int)items[i].transform.localPosition.x != (int)(iconInitialPos.x + (itemIconSize + Define.ITEMICON_MARGIN) * (items.Count - (i + 1)) + scrollPos )  )
            {
				if (usecol)
				{//ゆっくり
					StartCoroutine(
						MovePos(items[i], new Vector3(iconInitialPos.x + -(itemIconSize + Define.ITEMICON_MARGIN) * (items.Count - (i + 1)) + scrollPos
													  , iconInitialPos.y, iconInitialPos.z))
					);
				}else{//即座に
					items[i].transform.localPosition = new Vector3(iconInitialPos.x + -(itemIconSize + Define.ITEMICON_MARGIN) * (items.Count - (i + 1)) + scrollPos
					                                               , iconInitialPos.y, iconInitialPos.z);
				}
            }
        }
	}



	//--------------------------------------------------------------------
	//                    input系
	//--------------------------------------------------------------------
	#region// input系
	public void IconClicked (GameObject obj)
	{
		if (cantap == false) return;
        if (Time.time - PreviousTapTime < Define.DOUBLETAPTIME)
        {
            PreviousTapTime = Time.time;
			if (getFlag == true) return;
            ShowItem(obj.name);//ダブルタップされたらその項目を表示
            return;
        }
        PreviousTapTime = Time.time;

        if (obj.transform.localPosition.y < (iconInitialPos.y - itemIconSize))//下にドラッグされてたら何もしない。
        {
            return;
        }

        //if (isShowing == true && obj.name == shown)//ビューが表示されてて、そのアイテムをクリック
        //{
        //    HideItem();
        //    return;
        //}

        if (obj.name != selected)
        {//白いとこクリック
                if (selected != "")
                {//他のが選択されてたら白くしとく
                    items.Find(hoge => hoge.name == selected).GetComponent<Image>().color = Color.white;
                }
                obj.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1.0f);
                selected = obj.name;
                return;
         }
        else//黒いとこクリック
        {
                selected = "";
            obj.GetComponent<Image>().color = Color.white;
        }
	}

    public void IconDragEnd (GameObject obj){
		if (cantap == false) return;
		if (getFlag == true) return;
		if (obj.transform.localPosition.y < (iconInitialPos.y - itemIconSize)){//アイコンを下にドラッグして離した時
            transform.localPosition = obj.transform.localPosition;
            GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;
            GetComponent<Image>().enabled = true;
            StartCoroutine(MovePos(this.gameObject , Vector3.zero));//真ん中へ
            GetComponent<RectTransform>().sizeDelta = new Vector2(itemIconSize,itemIconSize);
            StartCoroutine(ChangeSize(this.gameObject, new Vector2(Define.ITEMVIEW_IDEALSIZE,Define.ITEMVIEW_IDEALSIZE)));
			buttonDark.GetComponent<Image>().enabled = true;
            shown = obj.name;
            isShowing = true;
        }
            StartCoroutine(//元の場所に戻す
                    MovePos(obj, new Vector3(iconInitialPos.x + -(itemIconSize + Define.ITEMICON_MARGIN) 
		                                     * (items.Count - (items.FindIndex(x => x.name == obj.name)  + 1)) + scrollPos
		                                     , iconInitialPos.y, iconInitialPos.z))
                );
    }

	public void PushButtonDark()
    {
		if (cantap == false) return;
        HideItem();
    }

    public void PushButtonClose()
    {
		if (cantap == false) return;
        HideItem();
    }

    public void OnClick()
    {
        //ご自由にお使いください
		if (cantap == false) return;
        gameManager.GetComponent<GameManager>().ItemViewClicked();
    }
	#endregion

	//--------------------------------------------------------------------
	//                    子ルーチン系
	//--------------------------------------------------------------------
	#region//コルーチン系

	private IEnumerator MovePos (GameObject obj , Vector3 finishpos){//イージングで位置を移動する
        float startTime = Time.time;
        Vector3 startPos = obj.transform.localPosition;
        Vector3 moveDistance = finishpos - startPos; // 変化量

        while((Time.time - startTime ) < duration){
            obj.transform.localPosition =
                startPos + moveDistance * animCurve.Evaluate( (Time.time - startTime) / duration );
            yield return 0;
        }
        obj.transform.localPosition = startPos + moveDistance;
    }

    private IEnumerator ChangeSize (GameObject obj , Vector2 finishsize){
        float startTime = Time.time;
        Vector2 startSize = obj.GetComponent<RectTransform>().sizeDelta;
        Vector2 changeSize = finishsize - startSize; // 変化量

        while ((Time.time - startTime) < duration)
        {
            obj.GetComponent<RectTransform>().sizeDelta =
                   startSize + changeSize * animCurve.Evaluate((Time.time - startTime) / duration);
            yield return 0;
        }
        obj.GetComponent<RectTransform>().sizeDelta = startSize + changeSize;
    }

	private IEnumerator GetEffect ()
	{
		//ピュイーんしょーる間はinput効かんようにする
		//ButtonItemViewのImageを縮小しながらiconの位置へ移動させてエフェクトとする(もっといい方法あると
		//思うけどめんどくさい。

		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
		float startTime = Time.time;    // 開始時間
		float startSize = GetComponent<RectTransform> ().sizeDelta.x;  // 開始位置
		Vector3 startPos = transform.localPosition;
		float changeSize = itemIconSize - startSize; // 変化量
		Vector3 moveDistance = (iconInitialPos - startPos);
		float easingSize;

  		for (int i = 0; i < items.Count - 1; ++i)
        {//既存のアイコンの位置整理
            if ((int)items[i].transform.localPosition.x != (int)(iconInitialPos.x + (itemIconSize + Define.ITEMICON_MARGIN) * (items.Count - (i + 1))))
            {
                StartCoroutine(
                    MovePos(items[i], new Vector3(iconInitialPos.x + -(itemIconSize + Define.ITEMICON_MARGIN) * (items.Count - (i + 1)), iconInitialPos.y, iconInitialPos.z))
                );
            }
        }

		GetComponent<SmoothlyMover> ().begin ();
		while ((Time.time - startTime) < duration) {

			//サイズのイージング
			easingSize = startSize + changeSize * animCurve.Evaluate ((Time.time - startTime) / duration);
			//this.GetComponent<RectTransform> ().sizeDelta = new Vector2 (easingSize, easingSize);
			GetComponent<SmoothlyMover> ().ChangeSize (new Vector2 (easingSize, easingSize));
			//座標のイージング
			GetComponent<SmoothlyMover> ().ChangePos (
				startPos + moveDistance * animCurve.Evaluate ((Time.time - startTime) / duration));
			
			yield return 0;        // 1フレーム後、再開
		}

		GetComponent<SmoothlyMover> ().end ();
			
		items [items.Count - 1].GetComponent<Button> ().image.enabled = true;//icon表示
		//items [items.Count - 1].GetComponent<Image> ().enabled = true;
		items [items.Count - 1].transform.localPosition = iconInitialPos;//iconの初期位置に配置
		//items [items.Count - 1].transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);//iconの初期位置に配置
		this.GetComponent<Image> ().enabled = false;//ButtonItemViewを非表示
		yield return null;//1フレーム後に再開　これを入れないと１行下で真ん中に戻った時に真ん中に表示される
		this.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);//ButtonItemViewを元の位置にもどす
        
		getFlag = false;
		if (iconScrollBar.activeSelf) iconScrollBar.GetComponent<HorizontalScrrolBar>().CanDrag = true;
		CheckIconWidth();
		SetIconCanDrag(true);
		isShowing = false;
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
	}

	#endregion

	public void SetIconCanDrag(bool b){
		foreach(GameObject obj in items){
			obj.GetComponent<DragMoveUICamera>().candrag = b;
		}
	}

    
	public void OnScrollBarMoved(float pos){
		Debug.Log("event Handled!!!!!" + "pos = " + pos.ToString());
		scrollPos = pos;
		IconPosUpdate(false);
	}

	void CheckIconWidth(){//アイテムアイコンの幅をチェックして画面からはみ出していたら、スクロールバーを表示、はみ出してなかったら非表示
		float iconwidth = items.Count * itemIconSize + (items.Count - 1) * Define.ITEMICON_MARGIN;

		Debug.Log(this.name + ":CheckIconWidth()" + "iconwidth = " + iconwidth.ToString() + "Screen.width = " 
		          + Screen.width.ToString() + "Define.CANBUS_WIDTH = " + Define.CANBUS_WIDTH.ToString()
		          + "items.Count = " + items.Count
		         );

		if( Define.CANBUS_WIDTH < iconwidth ){
			iconScrollBar.SetActive(true);
			float surplusPixels = iconwidth - Screen.width;//画面からはみ出した分のピクセル数
			float iconScrollPos = 1.0f - ((surplusPixels - scrollPos) / surplusPixels);
			iconScrollBar.GetComponent<HorizontalScrrolBar>().Display(iconwidth, iconScrollPos);
		}else{
			iconScrollBar.GetComponent<HorizontalScrrolBar>().Hide();
			iconScrollBar.SetActive(false);
			scrollPos = 0.0f;
		}

	}
    
    

 

}//class
