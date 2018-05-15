using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
/// <summary>
/// Button item view UIとして画面の手前で使う感じです
/// buttonDarkとbuttonCloseはPrefabに置いておく
/// アセットの　Image/ItemPanel/ にアイテム画像を入れておいてファイル名で読み込む
/// </summary>
public class ButtonItemView : MonoBehaviour
{
	private GameObject gameManager;//Startメソッドで初期化

	public GameObject buttonItemIcon;
	//プレハブからアタッチ
	public bool cantap = true;

    //適当な画像の入ってないImageをアタッチ
	public float duration = 0.5f;
    
	public const float SIZE_VIEW = 800.0f;


	public const float SIZE_ICON = 144.0f;
    public const float ICONMARGIN = 4.0f;

    public float DOUBLETAPTIME = 0.30f;
    private float PreviousTapTime;//ダブルタップ判定用
	//ボタンの大きさと合わせる
	public GameObject imageFlame;//子として作っておきアタッチ
	public GameObject imageForViewChange;//子として作っておきアタッチ
	public GameObject buttonClose;//プレハブをアタッチ、Constantiate
	public GameObject buttonDark;//プレハブをアタッチ、Constantiate
	//public Button[] icons;
	//アイテム個数分の要素があって、あらかじめシーンにボタンを作ってアタッチ。アイテムを手に入れたらImageを設定する

	private List<GameObject> items = new List<GameObject> ();
	public AnimationCurve animCurve = AnimationCurve.Linear (0, 0, 1, 1);
	//アイテムゲットエフェクトのイージング用

	private Vector3 iconPos = new Vector3 (480.0f, 570.0f, 0.0f);
	//アイコンの追加される位置
	private bool getFlag;
	public string selected = "";
    public string shown = "";//ビューに表示されてるアイテム


	private bool getNow;
	private bool isShowing;
	//表示しているか

	void Awake ()
	{
		buttonDark = Instantiate (buttonDark);
		buttonDark.GetComponent<Button> ().onClick.AddListener (() => PushButtonDark ());
		buttonDark.transform.SetParent (this.transform.root.gameObject.transform);
		buttonDark.transform.localScale = Vector3.one;
		buttonDark.transform.localPosition = Vector3.zero;
		buttonDark.transform.SetSiblingIndex (transform.GetSiblingIndex());

	}
	// Use this for initialization
	void Start ()
	{
		gameManager = GameObject.Find("GameManager");
		getFlag = false;
		isShowing = false;
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (SIZE_VIEW, SIZE_VIEW);
		imageForViewChange.GetComponent<RectTransform>().sizeDelta = new Vector2(SIZE_VIEW, SIZE_VIEW);
		buttonClose = Instantiate (buttonClose);
		buttonClose.transform.SetParent (this.transform.root.gameObject.transform);
		buttonClose.transform.localScale = Vector3.one;
		buttonClose.transform.localPosition = new Vector3 (SIZE_VIEW / 2.0f - 50.0f, SIZE_VIEW / 2.0f - 50.0f);
		buttonClose.GetComponent<Button> ().onClick.AddListener (() => PushButtonClose ());
		GetComponent<Button> ().transition = Selectable.Transition.None;//大きいビューは押したエフェクトなし
	}
	
	// Update is called once per frame
	public void Update ()
	{
  
        if (isShowing && (int)GetComponent<RectTransform>().sizeDelta.x == (int)SIZE_VIEW)
        {
            buttonClose.GetComponent<Image>().enabled = true; //ビューが最大化されたら閉じるボタン表示
        }
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
		this.GetComponent<RectTransform> ().sizeDelta = new Vector2 (SIZE_VIEW, SIZE_VIEW);

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
    
	public IEnumerator ViewItemChangeTo(float changetime ,string name){//表示されているビューのアイテムを引数のアイテムに変える
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

		ItemChangeTo(shown,name);

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
		SetIconCanDrag(false);//アイコンをドラッグ出来なくする
		GameObject newb = Instantiate (buttonItemIcon);
		newb.name = name;
		newb.GetComponent<Button> ().image.sprite = spr;
		newb.GetComponent<Button> ().onClick.AddListener (() => IconClicked (newb.gameObject));//onClickのイベントリスナー取り付け

        //inspectorで追加したイベントトリガーにコールバックを設定
        EventTrigger.Entry found = newb.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.EndDrag);
        found.callback.AddListener( (BaseEventData) => IconDragEnd(newb) );

		newb.GetComponent<RectTransform> ().sizeDelta = new Vector2 (SIZE_ICON, SIZE_ICON);
		newb.transform.SetParent (this.transform.root.gameObject.transform);
		newb.transform.localScale = Vector3.one;    //親子関係セット直後はサイズがおかしくなるので、ローカルサイズを１にセット
        newb.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
		items.Add (newb);

		ShowItem (name);
	}

	public void RemoveItemAndUpdatePos(string name){
		RemoveItem(name);
		IconPosUpdate();
	}

    public void RemoveItem(string name)
    {
        int i;
        i = items.FindIndex(item => item.name == name);
        if (i == -1) {
            Debug.Log(name + "というアイテムがないので削除できません");
            return;
        }

        Destroy(items[i]);
        items.RemoveAt(i);
        if (selected == name){
            selected = "";
        }

       
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

	public void IconPosUpdate(){
		for (int i = 0; i < items.Count; ++i)
        {//アイコンの位置整理

            if ((int)items[i].transform.localPosition.x != (int)(iconPos.x + (SIZE_ICON + ICONMARGIN) * (items.Count - (i + 1))))
            {
                StartCoroutine(
                    MovePos(items[i], new Vector3(iconPos.x + -(SIZE_ICON + ICONMARGIN) * (items.Count - (i + 1))
                                                  , iconPos.y, iconPos.z))
                );
            }

        }
	}

    //--------------------------------------------------------------------
    //                    input系
	//--------------------------------------------------------------------
	public void IconClicked (GameObject obj)
	{
		if (cantap == false) return;
        if (Time.time - PreviousTapTime < DOUBLETAPTIME)
        {
            PreviousTapTime = Time.time;
			if (getFlag == true) return;
            ShowItem(obj.name);//ダブルタップされたらその項目を表示
            return;
        }
        PreviousTapTime = Time.time;

        if (obj.transform.localPosition.y < (iconPos.y - SIZE_ICON))//下にドラッグされてたら何もしない。
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
		if (obj.transform.localPosition.y < (iconPos.y - SIZE_ICON)){//アイコンを下にドラッグして離した時
            transform.localPosition = obj.transform.localPosition;
            GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;
            GetComponent<Image>().enabled = true;
            StartCoroutine(MovePos(this.gameObject , Vector3.zero));//真ん中へ
            GetComponent<RectTransform>().sizeDelta = new Vector2(SIZE_ICON,SIZE_ICON);
            StartCoroutine(ChangeSize(this.gameObject, new Vector2(SIZE_VIEW,SIZE_VIEW)));
			buttonDark.GetComponent<Image>().enabled = true;
            shown = obj.name;
            isShowing = true;
        }
            StartCoroutine(//元の場所に戻す
                    MovePos(obj, new Vector3(iconPos.x + -(SIZE_ICON + ICONMARGIN) 
                                             * (items.Count - (items.FindIndex(x => x.name == obj.name)  + 1)), iconPos.y, iconPos.z))
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
	//--------------------------------------------------------------------
    //                    子ルーチン系
    //--------------------------------------------------------------------


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
		//ButtonItemViewのImageを縮小しながらiconの位置へ移動させてエフェクトとする(もっといい方法あると
		//思うけどめんどくさい。
		float startTime = Time.time;    // 開始時間
		float startSize = GetComponent<RectTransform> ().sizeDelta.x;  // 開始位置
		Vector3 startPos = transform.localPosition;
		float changeSize = SIZE_ICON - startSize; // 変化量
		Vector3 moveDistance = (iconPos - startPos);
		float easingSize;


		Vector3[] startPosIcon = new Vector3[items.Count];//iconのStartPos
		for (int i = 0; i < (items.Count - 1); ++i) {//既存のicon(末尾以外のアイコン)のStartPos設定
			startPosIcon [i] = items [i].transform.localPosition;
		}
		Vector3 moveDistanceIcon = new Vector3 (-(SIZE_ICON + ICONMARGIN), 0.0f, 0.0f);

		GetComponent<SmoothlyMover> ().begin ();
		while ((Time.time - startTime) < duration) {
			
			for (int i = 0; i < (items.Count - 1); ++i) {//既存のアイコン(末尾以外のアイコン)を左へずらす
				items [i].transform.localPosition = 
					startPosIcon [i] + moveDistanceIcon * animCurve.Evaluate ((Time.time - startTime) / duration);			
			}

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
		for (int i = 0; i < (items.Count - 1); ++i) {//既存iconのイージング完遂
			items [i].transform.localPosition = startPosIcon [i] + moveDistanceIcon;
		}
			
		items [items.Count - 1].GetComponent<Button> ().image.enabled = true;//icon表示
		//items [items.Count - 1].GetComponent<Image> ().enabled = true;
		items [items.Count - 1].transform.localPosition = iconPos;//iconの初期位置に配置
		//items [items.Count - 1].transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);//iconの初期位置に配置
		this.GetComponent<Image> ().enabled = false;//ButtonItemViewを非表示
		yield return null;//1フレーム後に再開　これを入れないと１行下で真ん中に戻った時に真ん中に表示される
		this.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);//ButtonItemViewを元の位置にもどす
        
		getFlag = false;
		SetIconCanDrag(true);
		isShowing = false;
	}

	public void SetIconCanDrag(bool b){
		foreach(GameObject obj in items){
			obj.GetComponent<DragMoveUICamera>().candrag = b;
		}
	}


}
