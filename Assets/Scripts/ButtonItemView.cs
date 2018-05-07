using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
/// <summary>
/// Button item view UIとして画面の手前で使う感じです 
/// ButtonDarkをButtonItemViewの上に置いてアタッチする//(非親子)
/// ImageFlameと、ButtonCloseをボタンの子として作ってアタッチ
/// spriteItemsにアイテムの画像をアタッチ
/// 小さいアイコンのボタンをButtonItemViewの下に作ってアタッチ(非親子)
/// 
/// </summary>
public class ButtonItemView : MonoBehaviour
{
	public GameObject buttonItemIcon;
	//プレハブからアタッチ

	public float duration = 0.5f;

	public const float SIZE_VIEW = 800.0f;


	public const float SIZE_ICON = 144.0f;
    public const float ICONMARGIN = 4.0f;
	//ボタンの大きさと合わせる
	public GameObject imageFlame;
	public GameObject buttonClose;
	public GameObject buttonDark;
	//public Button[] icons;
	//アイテム個数分の要素があって、あらかじめシーンにボタンを作ってアタッチ。アイテムを手に入れたらImageを設定する

	private List<GameObject> items = new List<GameObject> ();
	public AnimationCurve animCurve = AnimationCurve.Linear (0, 0, 1, 1);
	//アイテムゲットエフェクトのイージング用

	private Vector3 iconPos = new Vector3 (480.0f, 570.0f, 0.0f);
	//アイコンの追加される位置
	private bool getFlag;
	public string selected = "";

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
		buttonDark.transform.SetSiblingIndex (0);
	}
	// Use this for initialization
	void Start ()
	{


		getFlag = false;
		this.enabled = false;
		isShowing = false;
		GetComponent<RectTransform> ().sizeDelta = new Vector2 (SIZE_VIEW, SIZE_VIEW);
		buttonClose = Instantiate (buttonClose);
		buttonClose.transform.SetParent (this.transform.root.gameObject.transform);
		buttonClose.transform.localScale = Vector3.one;
		buttonClose.transform.localPosition = new Vector3 (SIZE_VIEW / 2.0f - 50.0f, SIZE_VIEW / 2.0f - 50.0f);
		buttonClose.GetComponent<Button> ().onClick.AddListener (() => PushButtonClose ());
		GetComponent<Button> ().transition = Selectable.Transition.None;//大きいビューは押したエフェクトなし

	}
	
	// Update is called once per frame
	void Update ()//なぜか呼ばれない。謎
	{
       
       // && GetComponent<RectTransform>().sizeDelta.x > (SIZE_VIEW - 1.0f)
        if(isShowing){
            imageFlame.GetComponent<Image>().enabled = true;
            buttonClose.GetComponent<Image>().enabled = true;
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

		selected = name;
		isShowing = true;

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

		selected = "";

	}

	public void GetItem (string name)
	{
		getFlag = true;

		Sprite spr = Resources.Load ("Image/ItemPanel/" + name, typeof(Sprite)) as Sprite;

		if (spr == null) {
			Debug.Log (name + "というアイテムはありません");
		}

		GameObject newb = Instantiate (buttonItemIcon);
		newb.name = name;
		newb.GetComponent<Button> ().image.sprite = spr;
		newb.GetComponent<Button> ().onClick.AddListener (() => IconClicked (name));//onClickのイベントリスナー取り付け

        //inspectorで追加したイベントトリガーにコールバックを設定
        EventTrigger.Entry found = newb.GetComponent<EventTrigger>().triggers.Find(x => x.eventID == EventTriggerType.EndDrag);
        found.callback.AddListener( (BaseEventData) => IconDragEnd(newb) );

		newb.GetComponent<RectTransform> ().sizeDelta = new Vector2 (SIZE_ICON, SIZE_ICON);
		newb.transform.SetParent (this.transform.root.gameObject.transform);
		newb.transform.localScale = Vector3.one;    //親子関係セット直後はサイズがおかしくなるので、ローカルサイズを１にセット
		items.Add (newb);



		ShowItem (name);
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

        for (i = 0; i < items.Count; ++i){
            
            if( (int)items[i].transform.localPosition.x != (int)(iconPos.x + (SIZE_ICON + ICONMARGIN) * (items.Count - (i +1)) )  )
            {
                StartCoroutine(
                    MovePos(items[i], new Vector3(iconPos.x + -(SIZE_ICON + ICONMARGIN) * (items.Count - (i + 1))
                                                  , iconPos.y, iconPos.z))
                );
            }

        }
    }

	public void IconClicked (string name)
	{
		if (isShowing == true) {
			if (name == selected) {
				HideItem ();
			} else {
				ShowItem (name);
			}
		} else {
			ShowItem (name);
		}
	}

    public void IconDragEnd (GameObject obj){

        if (obj.transform.localPosition.y < (iconPos.y - SIZE_ICON)){//アイコン一つ分より下にドラッグして離した時
            transform.localPosition = obj.transform.localPosition;
            GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;
            GetComponent<Image>().enabled = true;
            StartCoroutine(MovePos(this.gameObject , Vector3.zero));//真ん中へ
            GetComponent<RectTransform>().sizeDelta = new Vector2(SIZE_ICON,SIZE_ICON);
            StartCoroutine(ChangeSize(this.gameObject, new Vector2(SIZE_VIEW,SIZE_VIEW)));
            isShowing = true;
        }
            StartCoroutine(//元の場所に戻す
                    MovePos(obj, new Vector3(iconPos.x + -(SIZE_ICON + ICONMARGIN) 
                                             * (items.Count - (items.FindIndex(x => x.name == obj.name)  + 1)), iconPos.y, iconPos.z))
                );
    }



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
		isShowing = false;
	}

	public void PushButtonDark ()
	{
		HideItem ();
	}

	public void PushButtonClose ()
	{
		HideItem ();
	}

	public void OnClick ()
	{
		//ご自由にお使いください
	}
}
