using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.PostProcessing;

//delegate用

public class GameManager : MonoBehaviour
{
	//------------------------------------------------------------
	//定数宣言　列挙子定義
	//------------------------------------------------------------
	//CameraNo
	public const int CAMERA_MAIN = 0;
	public const int CAMERA_SUB = 1;
	public const int CAMERA_SUBSUB = 2;
	//wallNo
	public const int WALL_LEFT = 1;
	public const int WALL_FRONT = 2;
	public const int WALL_RIGHT = 3;
	public const int WALL_BACK = 4;

	//colors
	public const int COLOR_GREEN = 0;
	public const int COLOR_RED = 1;
	public const int COLOR_BLUE = 2;
	public const int COLOR_WHITE = 3;

	//soundsClipNo
	public const string SOUND_DOOROPEN1 = "door-open1";
	public const string SOUND_DRAWEROPEN1 = "drawer-open1";
	public const string SOUND_DECISION22 = "decision22";
	public const string SOUND_DECISION4 = "decision4";
	public const string SOUND_KEY_IN2 = "key-in2";
	public const string SOUND_CURSOR7 = "cursor7";
    public const string SOUND_CURSOR1 = "cursor1";
	//direction
	public const int DOWN = 0;
	public const int LEFT = 1;
	public const int UP = 2;
	public const int RIGHT = 3;


		
	//GameFlag
	enum EGameFlag : int
	{
		//アイテム（下のconst intと同じ順番
		DOESGETCHAIR,
		DOESGETTONKACHI,
		DOESGETGINGER1,
		DOESGETPENCIL,

		DOESGETHAMMER,
		DOESGETKEY,

		USE_CHAIR,
		OPEN_DIRBOX,
		OPEN_STARBOX,

		NUMOFFLAG
	}


	//------------------------------------------------------------
	//構造体定義
	//------------------------------------------------------------
	private struct Screen
	{
		public float xLocation;
		public int zoomStage;
		public int backScreen;
		public string inSound;
		public string outSound;

		public Screen (float xl, int zs, int bs = -1, string ins = "", string os = "")
		{
			xLocation = xl;//screenのパネルのx座標
			zoomStage = zs;//ズーム段階
			backScreen = bs;//戻るボタンで戻るスクリーン
			inSound = ins;
			outSound = os;
		}

	}
	//------------------------------------------------------------
	//メンバ宣言
	//------------------------------------------------------------

	Screen[] screen = new Screen[(int)EScreenNo.SCREEN_NUM];
	public GameObject mainCamera;
	public GameObject subCamera;
	public GameObject subsubCamera;

	public GameObject audioManager;

	public GameObject panelWalls;
	public GameObject panelZooms;
	public GameObject panelZoomZooms;

	public GameObject panelWall1;

	public GameObject buttonUILeft;
	public GameObject buttonUIRight;
	public GameObject buttonUIBack;

	public GameObject buttonItemView;

	public GameObject buttonMessage;
	public GameObject buttonMessageText;

	public GameObject buttonHammer;
	public GameObject buttonHammerIcon;

	public GameObject buttonKey;
	public GameObject imageKeyIcon;

	public GameObject imageNoChair;
    public GameObject imagePutChair;

    public GameObject imageWallRightBanana1;
    public GameObject imageKitchenShelfBanana1;
    public GameObject imageKitchenShelfZoomMonkeyBanana1;

	public GameObject[] buttonDirection = new GameObject[4];
	public GameObject[] buttonColors = new GameObject[3];



	public Sprite[] buttonPicture = new Sprite[4];
	public Sprite hammerPicture;
	public Sprite KeyPicture;

	private int lr;
	//slide中に右左ボタンが押されたとき　右＋＋　左ーー
	private int wallNo;
	//現在向いている方向

	private int screenNo;


	private int[] buttonColor = new int[3];



    //ZoomStage
    public const int ZOOMSTAGE_NON = 0;
    public const int ZOOMSTAGE_ZOOM = 1;
    public const int ZOOMSTAGE_ZOOMZOOM = 2;
    public const int ZOOMSTAGE_ADD = 3;

	List<string> gameFlag = new List<string>();//ゲームフラグ
    //GetXXX UseXXX OpenXXX というstringを追加する



    //ScreenNo
    enum EScreenNo
    {
        SCREEN_WALLLEFT,
        //0
        SCREEN_WALLFRONT,
        SCREEN_WALLRIGHT,
        SCREEN_WALLBACK,
        SCREEN_ZOOMSTAGE_DESKLEFT,
        SCREEN_ZOOMSTAGE_DESKRIGHT,
        //5
        SCREEN_ZOOMZOOMSTAGE_DESKLEFTTOP,
        SCREEN_ZOOMZOOMSTAGE_DESKLEFTMIDDLE,
        SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOM,
        SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOMOPEN,
        SCREEN_ZOOMSTAGE_4X2SHELF,
        //10
        SCREEN_ZOOMZOOMSTAGE_STARBOX,
        SCREEN_ZOOMSTAGE_LOOKUP_SHELF,
        SCREEN_ADDSTAGE_4X2SHELFLEFT,
        SCREEN_ZOOMZOOMSTAGE_HIGHSHELFZOOM,
        SCREEN_ADDSTAGE_4X2SHELFRIGHT,
        //15
        SCREEN_ZOOMSTAGE_TRASHBOX,
        SCREEN_ZOOMZOOMSTAGE_TRASHBOXINSIDE,
        SCREEN_ZOOMSTAGE_KITCHENSHELF,
        SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEY,
		SCREEN_ZOOMSTAGE_TRASHBOXUP,
        //20
		SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN,
		SCREEN_ZOOMSTAGE_4X2SHELFHINT,
        SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEYEYES,

        SCREEN_NUM
    }
	//------------------------------------------------------------
	//初期化
	//------------------------------------------------------------
	void Start ()
	{
		wallNo = WALL_FRONT;
		screenNo = (int)EScreenNo.SCREEN_WALLFRONT;
		lr = 0;
		// リストで指定された数の配列を作成する


		screen [(int)EScreenNo.SCREEN_WALLLEFT] = new Screen (0, ZOOMSTAGE_NON);
		screen [(int)EScreenNo.SCREEN_WALLFRONT] = new Screen (1125, ZOOMSTAGE_NON);
		screen [(int)EScreenNo.SCREEN_WALLRIGHT] = new Screen (2250, ZOOMSTAGE_NON);
		screen [(int)EScreenNo.SCREEN_WALLBACK] = new Screen (3375, ZOOMSTAGE_NON);

		screen [(int)EScreenNo.SCREEN_ZOOMSTAGE_4X2SHELF] = new Screen (-1125, ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLLEFT);
		screen [(int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT] = new Screen (0, ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLFRONT);
		screen [(int)EScreenNo.SCREEN_ZOOMSTAGE_DESKRIGHT] = new Screen (1125, ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLFRONT);
		screen [(int)EScreenNo.SCREEN_ZOOMSTAGE_LOOKUP_SHELF] = new Screen (2250, ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLRIGHT);
        screen[(int)EScreenNo.SCREEN_ZOOMSTAGE_TRASHBOX] = new Screen(-2250, ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLLEFT);
		screen[(int)EScreenNo.SCREEN_ZOOMSTAGE_TRASHBOXUP] = new Screen(-3375, ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLLEFT);
		screen [(int)EScreenNo.SCREEN_ZOOMSTAGE_4X2SHELFHINT] = new Screen (-4450, ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFLEFT);


        screen [(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOX] = new Screen (-1125, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFRIGHT);
		screen [(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTTOP] = new Screen (0, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT, SOUND_DRAWEROPEN1, SOUND_DRAWEROPEN1);
		screen [(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTMIDDLE] = new Screen (1125, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT, SOUND_DRAWEROPEN1, SOUND_DRAWEROPEN1);
		screen [(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOM] = new Screen (2250, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT);
		screen [(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOMOPEN] = new Screen (3375, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT, SOUND_DRAWEROPEN1, SOUND_DRAWEROPEN1);
        screen[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_HIGHSHELFZOOM] = new Screen(-2250, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_WALLRIGHT);
		screen[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN] = new Screen(-4500, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFRIGHT);
		screen[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_TRASHBOXINSIDE] = new Screen(-3375, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_TRASHBOX);
		screen[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEY] = new Screen(4500, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_KITCHENSHELF);
        screen[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEYEYES] = new Screen(5625, ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEY);

		screen [(int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFLEFT] = new Screen (-1125, ZOOMSTAGE_ADD, (int)EScreenNo.SCREEN_ZOOMSTAGE_4X2SHELF);
        screen[(int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFRIGHT] = new Screen(-2250, ZOOMSTAGE_ADD, (int)EScreenNo.SCREEN_ZOOMSTAGE_4X2SHELF);

        screen[(int)EScreenNo.SCREEN_ZOOMSTAGE_KITCHENSHELF] = new Screen(3375, ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLRIGHT);

		buttonColor [0] = COLOR_GREEN;
		buttonColor [1] = COLOR_RED;
		buttonColor [2] = COLOR_BLUE;

		UpdateUIButtons ();

		GetComponent<CameraSwitchCrossFade> ().Initialize ();


	}

	//------------------------------------------------------------
	//
	//------------------------------------------------------------
	void Update ()
	{
		

		if (panelWalls.GetComponent<PanelSlider> ().isSliding == false) {//ズームしてない視点の回転移動（スライド
			if (panelWalls.transform.localPosition.x == -4500.0f) {//ダミーへ移動したあとの処理
				panelWalls.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
				panelWall1.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
			}

			if (lr != 0) {
				if (lr > 0) {
					WallToRight ();
				} else {
					WallToLeft ();
				}
			}
		} else {

				
		
		}

	}


	//------------------------------------------------------------
	//アイテムゲット
	//------------------------------------------------------------
	public void PushGetItemButton (string name)
	{
		buttonItemView.GetComponent<ButtonItemView> ().GetItem (name);
		audioManager.GetComponent<AudioManager> ().PlaySE (SOUND_DECISION4);
		gameFlag.Add ("Get_" + name);
	}

	public void PushButtonChair ()
	{
		if (gameFlag.Contains ("Get_Chair") == false) {
			buttonItemView.GetComponent<ButtonItemView> ().GetItem ("Chair");
			audioManager.GetComponent<AudioManager> ().PlaySE (SOUND_DECISION4);
			gameFlag.Add ("Get_Chair");
			imageNoChair.GetComponent<Image> ().enabled = true;
		}
	}

	public void PushButtonHammer ()
	{
		buttonHammer.SetActive (false);
	}

	public void PushButtonMessage ()
	{
		buttonMessage.SetActive (false);
	}

	//------------------------------------------------------------
	//UI
	//------------------------------------------------------------

	public void PushButtonUILeft ()//ズームしてない視点の回転移動（スライド
	{
		--lr;
		audioManager.GetComponent<AudioManager> ().PlaySE (SOUND_CURSOR7);
	}

	public void PushButtonUIRight ()//ズームしてない視点の回転移動（スライド
	{
		++lr;
		audioManager.GetComponent<AudioManager> ().PlaySE (SOUND_CURSOR7);
	}

	public void PushButtonUIBack ()
	{
		MovingScreen (screen [screenNo].backScreen);
	}

	void UpdateUIButtons ()
	{
		if (screen [screenNo].zoomStage == ZOOMSTAGE_NON) {
			buttonUIBack.SetActive (false);
			buttonUILeft.SetActive (true);
			buttonUIRight.SetActive (true);
		} else {
			buttonUIBack.SetActive (true);
			buttonUILeft.SetActive (false);
			buttonUIRight.SetActive (false);
		}
	}
    
	public void ItemViewClicked(){
		if(buttonItemView.GetComponent<ButtonItemView>().shown == "PencilSharpner"
		   && buttonItemView.GetComponent<ButtonItemView>().selected == "Pencil"){
			buttonItemView.GetComponent<ButtonItemView>().RemoveItem("Pencil");
			buttonItemView.GetComponent<ButtonItemView>().RemoveItem("PencilSharpner");
			buttonItemView.GetComponent<ButtonItemView>().IconPosUpdate();
			audioManager.GetComponent<AudioManager>().PlaySE(SOUND_DECISION4);
			buttonItemView.GetComponent<ButtonItemView>().GetItem("SharpedPencil");
		}
	}
	//------------------------------------------------------------
	//スクリーン移動（スライド
	//------------------------------------------------------------
	void WallToRight ()//ズームしてない視点の回転移動（スライド
	{
		--lr;
		++wallNo;
		ClearButtons ();
		if (wallNo > 4) {
			wallNo = WALL_LEFT;
			//右端から右に移動するときはダミーへ移動する
			panelWall1.transform.localPosition = new Vector3 (4500f, 0.0f, 0.0f);
			panelWalls.GetComponent<PanelSlider> ().SetSlide (new Vector3 (-4500f, 0.0f, 0.0f), 0.15f);
			return;
		}
		DisplayWall ();
	}

	void WallToLeft ()//ズームしてない視点の回転移動（スライド
	{
		++lr;
		--wallNo;
		ClearButtons ();
		if (wallNo < 1) {
			wallNo = WALL_BACK;
			//左端から左に移動するときは右のダミーに移動しておく
			panelWall1.transform.localPosition = new Vector3 (4500f, 0.0f, 0.0f);
			panelWalls.transform.localPosition = new Vector3 (-4500f, 0.0f, 0.0f);
		}
		DisplayWall ();
	}

	void DisplayWall ()//ズームしてない視点の回転移動（スライド
	{
		switch (wallNo) {
		case WALL_LEFT:
			panelWall1.transform.localPosition = new Vector3 (0.0f, 0.0f, 0.0f);
			panelWalls.GetComponent<PanelSlider> ().SetSlide (new Vector3 (0.0f, 0.0f, 0.0f), 0.15f);
			break;
		case WALL_FRONT:
			panelWalls.GetComponent<PanelSlider> ().SetSlide (new Vector3 (-1125f, 0.0f, 0.0f), 0.15f);
			break;
		case WALL_RIGHT:
			panelWalls.GetComponent<PanelSlider> ().SetSlide (new Vector3 (-2250f, 0.0f, 0.0f), 0.15f);
			break;
		case WALL_BACK:
			panelWalls.GetComponent<PanelSlider> ().SetSlide (new Vector3 (-3375f, 0.0f, 0.0f), 0.15f);
			break;
		}
	}

	//------------------------------------------------------------
	//スクリーン移動（スライドなし
	//------------------------------------------------------------
	public void MovingScreen (int srNo)
	{//スライドしない移動　（ズーム変化

		if (screen [screenNo].outSound != "") {//スクリーンを出る時の音
			audioManager.GetComponent<AudioManager> ().PlaySE (screen [screenNo].outSound);
		}
		if (screen [srNo].inSound != "") {//スクリーンに入る時の音
			audioManager.GetComponent<AudioManager> ().PlaySE (screen [srNo].outSound);
		}

		switch (screen [srNo].zoomStage) {
		case ZOOMSTAGE_NON:
			panelWalls.transform.localPosition = new Vector3 (-(screen [srNo].xLocation), 0.0f, 0.0f);
			GetComponent<CameraSwitchCrossFade> ().SwitchCamera (CAMERA_MAIN);
			//0.5秒後に実行する
			StartCoroutine (DelayMethod (0.4f, () => {
				mainCamera.GetComponent<PostProcessingBehaviour> ().profile.motionBlur.enabled = true;
			}));
			break;
		case ZOOMSTAGE_ZOOM:
			panelZooms.transform.localPosition = new Vector3 (-(screen [srNo].xLocation), 0.0f, 0.0f);
			GetComponent<CameraSwitchCrossFade> ().SwitchCamera (CAMERA_SUB);
			mainCamera.GetComponent<PostProcessingBehaviour> ().profile.motionBlur.enabled = false;
			break;
		case ZOOMSTAGE_ZOOMZOOM:
			panelZoomZooms.transform.localPosition = new Vector3 (-(screen [srNo].xLocation), 0.0f, 0.0f);
			GetComponent<CameraSwitchCrossFade> ().SwitchCamera (CAMERA_SUBSUB);
			mainCamera.GetComponent<PostProcessingBehaviour> ().profile.motionBlur.enabled = false;
			break;
		case ZOOMSTAGE_ADD:
			panelWalls.transform.localPosition = new Vector3 (-(screen [srNo].xLocation), 0.0f, 0.0f);
			GetComponent<CameraSwitchCrossFade> ().SwitchCamera (CAMERA_MAIN);
			break;
		}
		screenNo = srNo;
		UpdateUIButtons ();
	}



	//------------------------------------------------------------
	//ボタン押した時
	//------------------------------------------------------------

	public void PushButtonKey ()
	{
		buttonKey.SetActive (false);
	}

	public void PushButtonLamp1 ()
	{
		ChangeButtonColor (0);
	}

	public void PushButtonLamp2 ()
	{
		ChangeButtonColor (1);
	}

	public void PushButtonLamp3 ()
	{
		ChangeButtonColor (2);
	}

	public void PushButtonDirection ()//方角の仕掛け
	{
		if (buttonDirection [0].GetComponent<ButtonDirectionManager> ().enabled == false)
			return;

		audioManager.GetComponent<AudioManager> ().PlaySE (SOUND_DECISION22);

		if (buttonDirection [0].GetComponent<ButtonDirectionManager> ().direction == UP &&
		    buttonDirection [1].GetComponent<ButtonDirectionManager> ().direction == LEFT &&
		    buttonDirection [2].GetComponent<ButtonDirectionManager> ().direction == DOWN &&
		    buttonDirection [3].GetComponent<ButtonDirectionManager> ().direction == RIGHT) {
			gameFlag.Add ("Open_DirBox");
			foreach (GameObject btn in buttonDirection) {
				btn.GetComponent<ButtonDirectionManager> ().enabled = false;
			}

			//0.5秒後に実行する
			StartCoroutine (DelayMethod (0.3f, () => {
				audioManager.GetComponent<AudioManager> ().PlaySE (SOUND_KEY_IN2);
			}));
			StartCoroutine (DelayMethod (1.0f, () => {
				MovingScreen ((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOMOPEN);
				audioManager.GetComponent<AudioManager> ().PlaySE (SOUND_DRAWEROPEN1);
			}));
		}
	}

	public void PushButtonColors ()
	{


        if (gameFlag.Contains("Open_StarBox"))
        {
            MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN);
			return;
        }

		if (buttonColors [0].GetComponent<ButtonWithNumber> ().enabled == false)
			return;

		audioManager.GetComponent<AudioManager> ().PlaySE (SOUND_DECISION22);

		if (buttonColors [0].GetComponent<ButtonWithNumber> ().number == 2 &&
		    buttonColors [1].GetComponent<ButtonWithNumber> ().number == 1 &&
		    buttonColors [2].GetComponent<ButtonWithNumber> ().number == 4) {
			gameFlag.Add ("Open_StarBox");

			foreach (GameObject btn in buttonColors) {
				btn.GetComponent<ButtonWithNumber> ().enabled = false;
			}

			//0.5秒後に実行する
			StartCoroutine (DelayMethod (0.3f, () => {
				audioManager.GetComponent<AudioManager> ().PlaySE (SOUND_KEY_IN2);
			}));
            StartCoroutine(DelayMethod(1.0f, () => {
				MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN);
            }));
		}

	}


	//------------------------------------------------------------
	//クリックエリア押した時
	//------------------------------------------------------------
	public void Click_AreaDeskLeftBottom ()//机左下
	{
		if (gameFlag.Contains ("Open_DirBox")) {//空いてる時
			MovingScreen ((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOMOPEN);
		} else {//空いてない時
			MovingScreen ((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOM);
		}
	}

	public void Click_AreaLookUpShelf ()
	{
			DisplayMessage ("高すぎて見えないよバカヤロウ");
	}

	public void Click_AreaPutChair ()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().selected == "Chair") {
            imagePutChair.GetComponent<Image>().enabled = true;
            gameFlag.Add("Use_Chair");
            audioManager.GetComponent<AudioManager>().PlaySE(SOUND_CURSOR1);
            buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos("Chair");
		}
	}

    public void ClickAreaHighShelf()
    {
        if (gameFlag.Contains("Use_Chair"))
        {
            MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_HIGHSHELFZOOM);
        }
        else
        {
            MovingScreen((int)EScreenNo.SCREEN_ZOOMSTAGE_LOOKUP_SHELF);
        }

    }

    public void Click_AreaKitchenShelfZoomMonkey(){
        if (buttonItemView.GetComponent<ButtonItemView>().selected == "Banana")
        {
            imageKitchenShelfZoomMonkeyBanana1.GetComponent<Image>().enabled = true;
            imageKitchenShelfBanana1.GetComponent<Image>().enabled = true;
            imageWallRightBanana1.GetComponent<Image>().enabled = true;
            gameFlag.Add("Use_Banana");
            audioManager.GetComponent<AudioManager>().PlaySE(SOUND_DECISION4);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos("Banana");
        }
    }

	public void Click_AreaStarBoxOpened(){
		if(gameFlag.Contains("Open_StarBox")){
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN);
        }
	}
	//------------------------------------------------------------
	//その他
	//------------------------------------------------------------
	void ClearButtons ()
	{
		buttonMessage.SetActive (false);
	
	}




	void DisplayMessage (string mes)
	{
		buttonMessage.SetActive (true);
		buttonMessageText.GetComponent<Text> ().text = mes;
	}

	void ChangeButtonColor (int buttonNo)
	{
		//		++buttonColor [buttonNo];
		//
		//		if (buttonColor [buttonNo] > COLOR_WHITE)
		//			buttonColor [buttonNo] = COLOR_GREEN;
		//		buttonLamp [buttonNo].GetComponent<Image> ().sprite = //ボタンの画像変更
		//			buttonPicture [buttonColor [buttonNo]];
		//
		//
		//		if ((buttonColor [0] == COLOR_BLUE) &&
		//		    (buttonColor [1] == COLOR_WHITE) &&
		//		    (buttonColor [2] == COLOR_RED)) {
		//			if (gameFlag[(int)EGameFlag.DOESGETHAMMER] == false) {//色があっていてハンマ持っていなければ手に入れる。
		//				DisplayMessage ("金庫の中にトンカチが入っていた。");
		//				buttonHammerIcon.GetComponent<Image> ().sprite = hammerPicture;
		//
		//				buttonHammer.SetActive (true);
		//				gameFlag[(int)EGameFlag.DOESGETHAMMER] = true;
		//			}
		//
		//		}
	}

	/// <summary>
	/// 渡された処理を指定時間後に実行する
	/// </summary>
	/// <param name="waitTime">遅延時間[ミリ秒]</param>
	/// <param name="action">実行したい処理</param>
	/// <returns></returns>
	private IEnumerator DelayMethod (float waitTime, Action action)
	{
		yield return new WaitForSeconds (waitTime);
		action ();
	}



}
