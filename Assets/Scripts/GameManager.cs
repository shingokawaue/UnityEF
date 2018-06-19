using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

using UnityEngine.PostProcessing;
using UnityEngine.Video;

using Common;//自作定数宣言クラス
using Hint;
using System.Linq;

public class GameManager : MonoBehaviour
{
	//----------------メンバ宣言---------------------------------------------------------------------------------------
	ScreenInfo[] screenInfo = new ScreenInfo[(int)EScreenNo.SCREEN_NUM];
	public GameObject mainCamera, subCamera, subsubCamera;
	GameObject inputManager, audioManager, valueShareManager;//使い方違う
	public GameObject videoPlayerMonkey, videoPlayerRedKey, videoPlayerCutKey, videoPlayerDog, videoPlayerScissors, videoPlayerMonko, videoPlayerMonkeyLove;
	public GameObject pencilAnimation;
	public GameObject panelWalls, panelZooms, panelZoomZooms, panelWall1;
	//-------------------------------------------------------------------------------------------------------
	//UI系
	//-------------------------------------------------------------------------------------------------------
	public GameObject buttonUILeft, buttonUIRight, buttonUIBack, buttonMessage, buttonItemView;
	int lr;
	int wallNo;//slide中に右左ボタンが押されたとき　右＋＋　左ーー
	int screenNo;//現在向いている方向

	//アイテムゲットに際して、スクリプト操作が必要なもの
	public GameObject imageGetGlassDoor, clickAreaPuzzleClose, imageGetFlowerYellow;

	//ゲーム進行中に置かれたりセットされたりして見えるようになるもの。
	public GameObject imageWallFrontDogSet, imageBookShelfDogSet, imageWallFrontDogBoneSet, imageBookShelfDogBoneSet, imagePutChair;
	public GameObject[] imageBanana1 = new GameObject[3];
	public GameObject[] imageBanana2 = new GameObject[4];
	public GameObject[] imageOsara = new GameObject[4];
	public GameObject[] imageGinger1 = new GameObject[4];
	public GameObject[] imageGinger2 = new GameObject[4];
	public GameObject[] imageMonko = new GameObject[3];
	public GameObject[] imageFlower = new GameObject[5];
	public GameObject[] imageFlower2 = new GameObject[4];
	public GameObject[] imageFlowerYellow = new GameObject[4];

	//プレイヤーにゲットされてなくなったように見せるため上からかぶせるイメージ
	public GameObject[] imageNoScissors = new GameObject[2];
	public GameObject imageNoChair;

	//仕掛けのボタンたち
	public GameObject[] buttonDirection = new GameObject[4];
	public GameObject[] buttonColors = new GameObject[3];
	public GameObject[] buttonNumbers = new GameObject[4];
	public GameObject[] buttonLR = new GameObject[2];

	//GameManagerでの操作が必要なしかけ
	public GameObject imageDirectionPuzzle, imagePaint;

	bool[] lrJudge = new bool[7];//LRボタンの押された順番記録用
	int[] cornerJudge = new int[6];//paintCorner

	//ゲームフラグ 置くPut_XXX 使うUse_XXX 仕掛けを開けるOpen_XXX という感じでstringを追加
	List<string> gameFlag = new List<string>();

	private struct ScreenInfo//スクリーン一つ分の情報
	{
		public float xLocation;
		public int zoomStage;
		public int backScreen;
		public string inSound;
		public string outSound;

		public ScreenInfo(float xl, int zs, int bs = -1, string ins = "", string os = "")
		{
			xLocation = xl;//screenのパネルのx座標
			zoomStage = zs;//ズーム段階
			backScreen = bs;//戻るボタンで戻るスクリーン
			inSound = ins;
			outSound = os;
		}
	}

	//ScreenNo
	enum EScreenNo
	{
		SCREEN_WALLLEFT,
		//0↑
		SCREEN_WALLFRONT,
		SCREEN_WALLRIGHT,
		SCREEN_WALLBACK,
		SCREEN_ZOOMSTAGE_DESKLEFT,
		SCREEN_ZOOMSTAGE_DESKRIGHT,
		//5↑
		SCREEN_ZOOMZOOMSTAGE_DESKLEFTTOP,
		SCREEN_ZOOMZOOMSTAGE_DESKLEFTMIDDLE,
		SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOM,
		SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOMOPEN,
		SCREEN_ZOOMSTAGE_4X2SHELF,
		//10↑
		SCREEN_ZOOMZOOMSTAGE_STARBOX,
		SCREEN_ZOOMSTAGE_LOOKUP_SHELF,
		SCREEN_ADDSTAGE_4X2SHELFLEFT,
		SCREEN_ZOOMZOOMSTAGE_HIGHSHELFZOOM,
		SCREEN_ADDSTAGE_4X2SHELFRIGHT,
		//15↑
		SCREEN_ZOOMSTAGE_TRASHBOX,
		SCREEN_ZOOMZOOMSTAGE_TRASHBOXINSIDE,
		SCREEN_ZOOMSTAGE_KITCHENSHELF,
		SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEY,
		SCREEN_ZOOMSTAGE_TRASHBOXUP,
		//20↑
		SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN,
		SCREEN_ZOOMSTAGE_4X2SHELFHINT,
		SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEYEYES,
		SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFTOPOPEN,
		SCREEN_ZOOMSTAGE_DESKBOOKSHELF,
		//25↑
		SCREEN_ZOOMSTAGE_4X2SHELFDOGGRAFITY,
		SCREEN_ZOOMZOOMSTAGE_DESKRTOPEN,
		SCREEN_ZOOMZOOMSTAGE_DESKRBOPEN,
		SCREEN_ZOOMZOOMSTAGE_DOGSTANDOPEN,
		SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFBOTTOMOPEN,
		//30↑
		SCREEN_VIDEO,
		SCREEN_ZOOMSTAGE_DIRECTIONPUZZLE,
		SCREEN_ADDSTAGE_DIRECTIONPUZZLEOPEN,
		SCREEN_ZOOMZOOMSTAGE_ZOOMMONKO,
		SCREEN_ZOOMSTAGE_MONKOHEAD,
		//35↑
		SCREEN_ZOOMSTAGE_PAINT,
		SCREEN_ZOOMSTAGE_WALLBACKDOORZOOM,

		SCREEN_NUM
	}
	//------------------------------------------------------------
	//初期化
	//------------------------------------------------------------
	private void Awake()
	{

	}


	void Start()
	{
		//ApplySaveData();
		imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().OnPuzzlePieceMoved.AddListener(() => OnPuzzlePieceMoved());

		screenInfo[(int)EScreenNo.SCREEN_WALLLEFT] = new ScreenInfo(0, Define.ZOOMSTAGE_NON);
		screenInfo[(int)EScreenNo.SCREEN_WALLFRONT] = new ScreenInfo(1125, Define.ZOOMSTAGE_NON);
		screenInfo[(int)EScreenNo.SCREEN_WALLRIGHT] = new ScreenInfo(2250, Define.ZOOMSTAGE_NON);
		screenInfo[(int)EScreenNo.SCREEN_WALLBACK] = new ScreenInfo(3375, Define.ZOOMSTAGE_NON);

		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_4X2SHELF] = new ScreenInfo(-1125, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLLEFT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT] = new ScreenInfo(0, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLFRONT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_DESKRIGHT] = new ScreenInfo(1125, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLFRONT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_LOOKUP_SHELF] = new ScreenInfo(2250, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLRIGHT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_TRASHBOX] = new ScreenInfo(-2250, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLLEFT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_TRASHBOXUP] = new ScreenInfo(-3375, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLLEFT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_4X2SHELFHINT] = new ScreenInfo(-4450, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFLEFT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_DESKBOOKSHELF] = new ScreenInfo(-5625, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLFRONT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_4X2SHELFDOGGRAFITY] = new ScreenInfo(4500, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFRIGHT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_KITCHENSHELF] = new ScreenInfo(3375, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLRIGHT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_DIRECTIONPUZZLE] = new ScreenInfo(5625, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFBOTTOMOPEN, Define.SOUND_AVOID, Define.SOUND_AVOID);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_MONKOHEAD] = new ScreenInfo(6750, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_ZOOMMONKO);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_PAINT] = new ScreenInfo(-6750, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLBACK);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMSTAGE_WALLBACKDOORZOOM] = new ScreenInfo(7875, Define.ZOOMSTAGE_ZOOM, (int)EScreenNo.SCREEN_WALLBACK);

		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOX] = new ScreenInfo(-1125, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFRIGHT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTTOP] = new ScreenInfo(0, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT, Define.SOUND_DRAWER01, Define.SOUND_DRAWERCLOSE02);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTMIDDLE] = new ScreenInfo(1125, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT, Define.SOUND_DRAWER04, Define.SOUND_DRAWERCLOSE02);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOM] = new ScreenInfo(2250, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOMOPEN] = new ScreenInfo(3375, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKLEFT, Define.SOUND_DRAWER06, Define.SOUND_DRAWERCLOSE06);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_HIGHSHELFZOOM] = new ScreenInfo(-2250, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_WALLRIGHT);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN] = new ScreenInfo(-4500, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFRIGHT, Define.SOUND_GOTON01);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_TRASHBOXINSIDE] = new ScreenInfo(-3375, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_TRASHBOX);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEY] = new ScreenInfo(4500, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_KITCHENSHELF);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEYEYES] = new ScreenInfo(5625, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEY);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFTOPOPEN] = new ScreenInfo(-5625, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_KITCHENSHELF, Define.SOUND_DRAWER11, Define.SOUND_DRAWERCLOSE15);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKRTOPEN] = new ScreenInfo(-6750, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKRIGHT, Define.SOUND_DRAWER08, Define.SOUND_DRAWERCLOSE05);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKRBOPEN] = new ScreenInfo(-7875, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKRIGHT, Define.SOUND_DRAWER06, Define.SOUND_DRAWERCLOSE06);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DOGSTANDOPEN] = new ScreenInfo(6750, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_DESKBOOKSHELF, Define.SOUND_DRAWER14, Define.SOUND_DRAWERCLOSE19);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFBOTTOMOPEN] = new ScreenInfo(7875, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_KITCHENSHELF, Define.SOUND_FURNITUREDOOROPEN04, Define.SOUND_FURNITUREDOORCLOSE02);
		screenInfo[(int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_ZOOMMONKO] = new ScreenInfo(9000, Define.ZOOMSTAGE_ZOOMZOOM, (int)EScreenNo.SCREEN_ZOOMSTAGE_KITCHENSHELF);

		screenInfo[(int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFLEFT] = new ScreenInfo(-1125, Define.ZOOMSTAGE_ADD, (int)EScreenNo.SCREEN_ZOOMSTAGE_4X2SHELF);
		screenInfo[(int)EScreenNo.SCREEN_ADDSTAGE_4X2SHELFRIGHT] = new ScreenInfo(-2250, Define.ZOOMSTAGE_ADD, (int)EScreenNo.SCREEN_ZOOMSTAGE_4X2SHELF);
		screenInfo[(int)EScreenNo.SCREEN_ADDSTAGE_DIRECTIONPUZZLEOPEN] = new ScreenInfo(-3375, Define.ZOOMSTAGE_ADD, (int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFBOTTOMOPEN, Define.SOUND_AVOID, Define.SOUND_AVOID);

		screenInfo[(int)EScreenNo.SCREEN_VIDEO] = new ScreenInfo(0, Define.VIDEOSTAGE);

		inputManager = GameObject.FindWithTag("InputManager");
		audioManager = GameObject.FindWithTag("AudioManager");
		valueShareManager = GameObject.FindWithTag("ValueShareManager");

		GameObject[] panels = GameObject.FindGameObjectsWithTag("ScreenPanel");
		foreach (GameObject obj in panels)
		{//画面に対してのスクリーン位置の設定
			obj.transform.localPosition = new Vector3(obj.transform.localPosition.x,
													  valueShareManager.GetComponent<ValueShareManager>().ScreenPanel_posY);
		}
		GameObject rawimagevideo = GameObject.FindWithTag("RawImageVideo");
		rawimagevideo.transform.localPosition = new Vector3(0.0f, valueShareManager.GetComponent<ValueShareManager>().ScreenPanel_posY);

		//SEの個別のボリューム調整 既定値は0.8
		audioManager.GetComponent<AudioManager>().SetSEIndivisualVolume(Define.SOUND_CURSOR7, 0.3f);
		audioManager.GetComponent<AudioManager>().SetSEIndivisualVolume(Define.SOUND_DRAWER01, 0.5f);
		audioManager.GetComponent<AudioManager>().SetSEIndivisualVolume(Define.SOUND_DRAWER04, 0.6f);
		audioManager.GetComponent<AudioManager>().SetSEIndivisualVolume(Define.SOUND_DRAWER11, 0.5f);
		audioManager.GetComponent<AudioManager>().SetSEIndivisualVolume(Define.SOUND_DUKTWCHIN, 0.6f);
		audioManager.GetComponent<AudioManager>().SetSEIndivisualVolume(Define.SOUND_GLASS08, 0.35f);
		audioManager.GetComponent<AudioManager>().SetSEIndivisualVolume(Define.SOUND_KTON01, 0.4f);
		buttonMessage.SetActive(false);
		wallNo = Define.WALL_FRONT;
		screenNo = (int)EScreenNo.SCREEN_WALLFRONT;
		for (int i = 0; i < cornerJudge.Length; ++i)
		{
			cornerJudge[i] = -1;
		}

		lr = 0;
		// リストで指定された数の配列を作成する
		UpdateUIButtons();

		GetComponent<CameraSwitchCrossFade>().Initialize();
	}

	void Update()
	{
		if (panelWalls.GetComponent<PanelSlider>().isSliding == false)
		{//ズームしてない視点の回転移動（スライド
			if ((int)panelWalls.transform.localPosition.x == (int)-4500.0f)
			{//ダミーへ移動したあとの処理
				panelWalls.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				panelWall1.transform.localPosition = new Vector3(0.0f, valueShareManager.GetComponent<ValueShareManager>().ScreenPanel_posY, 0.0f);
			}
			if (lr != 0)
			{
				if (lr > 0)
				{
					WallToRight();
				}
				else
				{
					WallToLeft();
				}
			}
		}

	}

	//ロードされたセーブデータを反映させる
	public void ApplySaveData()
	{
		gameFlag = SaveData.Instance.gameFlag;
		buttonItemView.GetComponent<ButtonItemView>().ApplySaveData();
		if (gameFlag.Contains(Define.FLAG_GET_CHAIR)) imageNoChair.GetComponent<Image>().enabled = true;
		if (gameFlag.Contains(Define.FLAG_PUT_CHAIR)) imagePutChair.GetComponent<Image>().enabled = true;
		if (gameFlag.Contains(Define.FLAG_PUT_BANANA))
		{
			foreach (GameObject obj in imageBanana1)
			{
				obj.GetComponent<Image>().enabled = true;
			}
		}
		if (gameFlag.Contains(Define.FLAG_PUT_BANANA2))
		{
			foreach (GameObject obj in imageBanana2)
			{
				obj.GetComponent<Image>().enabled = true;
			}
		}
		if (gameFlag.Contains(Define.FLAG_PUT_MONKO))
		{
			foreach (GameObject obj in imageMonko)
			{
				obj.GetComponent<Image>().enabled = true;
			}
		}
		if (gameFlag.Contains(Define.FLAG_PUT_OSARA))
		{
			foreach (GameObject obj in imageOsara)
			{
				obj.GetComponent<Image>().enabled = true;
			}
		}
		if (gameFlag.Contains(Define.FLAG_PUT_ONEGINGER))
		{
			foreach (GameObject obj in imageGinger1)
			{
				obj.GetComponent<Image>().enabled = true;
			}
		}
		if (gameFlag.Contains(Define.FLAG_PUT_TWOGINGER))
		{
			foreach (GameObject obj in imageGinger2)
			{
				obj.GetComponent<Image>().enabled = true;
			}
		}
		if (gameFlag.Contains(Define.FLAG_PUT_CUTFLOWER))
		{
			foreach (GameObject obj in imageFlower)
			{
				obj.GetComponent<Image>().enabled = true;
			}
		}
		if (gameFlag.Contains(Define.FLAG_PUT_CUTFLOWERYELLOW))
		{
			foreach (GameObject obj in imageFlowerYellow)
			{
				obj.GetComponent<Image>().enabled = true;
			}
		}
		if (gameFlag.Contains(Define.FLAG_PUT_CUBEDOG))
		{
			imageBookShelfDogSet.GetComponent<Image>().enabled = true;
			imageWallFrontDogSet.GetComponent<Image>().enabled = true;
		}
		if (gameFlag.Contains(Define.FLAG_PUT_BONE))
		{
			imageBookShelfDogBoneSet.GetComponent<Image>().enabled = true;
			imageWallFrontDogBoneSet.GetComponent<Image>().enabled = true;
		}
		if (gameFlag.Contains(Define.FLAG_OPEN_DIRBOX))
		{
			foreach (GameObject btn in buttonDirection)
			{
				btn.GetComponent<ButtonDirectionManager>().canpush = false;
			}
			buttonDirection[0].GetComponent<ButtonDirectionManager>().direction = Define.UP;
			buttonDirection[1].GetComponent<ButtonDirectionManager>().direction = Define.LEFT;
			buttonDirection[2].GetComponent<ButtonDirectionManager>().direction = Define.DOWN;
			buttonDirection[3].GetComponent<ButtonDirectionManager>().direction = Define.RIGHT;
		}
		if (gameFlag.Contains(Define.FLAG_OPEN_STARBOX))
		{
			foreach (GameObject btn in buttonColors)
			{
				btn.GetComponent<ButtonWithNumber>().canpush = false;
			}
			buttonColors[0].GetComponent<ButtonWithNumber>().Text.GetComponent<Text>().text = "2";
			buttonColors[1].GetComponent<ButtonWithNumber>().Text.GetComponent<Text>().text = "1";
			buttonColors[2].GetComponent<ButtonWithNumber>().Text.GetComponent<Text>().text = "4";

		}
		if (gameFlag.Contains(Define.FLAG_OPEN_NUMBERBOX))
		{
			foreach (GameObject btn in buttonNumbers)
			{
				btn.GetComponent<ButtonWithNumber>().canpush = false;
			}
			buttonNumbers[0].GetComponent<ButtonWithNumber>().Text.GetComponent<Text>().text = "3";
			buttonNumbers[1].GetComponent<ButtonWithNumber>().Text.GetComponent<Text>().text = "7";
			buttonNumbers[2].GetComponent<ButtonWithNumber>().Text.GetComponent<Text>().text = "8";
			buttonNumbers[3].GetComponent<ButtonWithNumber>().Text.GetComponent<Text>().text = "1";
		}
		if (gameFlag.Contains(Define.FLAG_OPEN_LRBOX))
		{
			buttonLR[0].GetComponent<ButtonFlash>().canpush = false;
			buttonLR[1].GetComponent<ButtonFlash>().canpush = false;
		}

		imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().holes[0] = SaveData.Instance.holes[0];
		imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().holes[1] = SaveData.Instance.holes[1];
		imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().holes[2] = SaveData.Instance.holes[2];
		imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().holes[3] = SaveData.Instance.holes[3];
		imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().PickedPiece = SaveData.Instance.pickedPiece;
		imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().PickedHoleNo = SaveData.Instance.pickedHoleNumber;
		imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().UpdatePieceImage();
	}

	#region...アイテムゲット
	public void PushGetItemButton(string name)
	{
		buttonItemView.GetComponent<ButtonItemView>().GetItem(name);
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
		AddGameFlag("Get_" + name);
	}

	public void PushButtonChair()
	{
		if (gameFlag.Contains("Get_Chair") == false)
		{
			buttonItemView.GetComponent<ButtonItemView>().GetItem(Define.ITEM_CHAIR);
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
			AddGameFlag("Get_Chair");
			imageNoChair.GetComponent<Image>().enabled = true;
		}
	}


	public void PushButtonMessage()
	{
		buttonMessage.SetActive(false);
	}

	public void ClickItemGetMonko()
	{
		PushGetItemButton(Define.ITEM_MONKO);
		imageGetGlassDoor.GetComponent<BoxCollider2D>().enabled = true;
	}

	public void ClickItemGetScissors()
	{
		PushGetItemButton(Define.ITEM_SCISSORS);
		imageNoScissors[0].GetComponent<Image>().enabled = true;
		imageNoScissors[1].GetComponent<Image>().enabled = true;
		clickAreaPuzzleClose.GetComponent<BoxCollider2D>().enabled = true;
	}

	public void GetGinger1()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().DoesHave(Define.ITEM_GINGER2))
		{//クッキー二つ持ってれば一つにまとめる
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_GINGER2);
			PushGetItemButton(Define.ITEM_TWOGINGERS);
		}
		else
		{
			PushGetItemButton(Define.ITEM_GINGER1);
		}
	}

	public void GetGinger2()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().DoesHave(Define.ITEM_GINGER1))
		{//クッキー二つ持ってれば一つにまとめる
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_GINGER1);
			PushGetItemButton(Define.ITEM_TWOGINGERS);
		}
		else
		{
			PushGetItemButton(Define.ITEM_GINGER2);
		}
	}
	#endregion


	#region...UI


	public void PushButtonUILeft()//ズームしてない視点の回転移動（スライド
	{
		--lr;
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_CURSOR7);
	}

	public void PushButtonUIRight()//ズームしてない視点の回転移動（スライド
	{
		++lr;
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_CURSOR7);
	}

	public void PushButtonUIBack()
	{
		MovingScreen(screenInfo[screenNo].backScreen);
	}

	void UpdateUIButtons()
	{
		if (screenInfo[screenNo].zoomStage == Define.ZOOMSTAGE_NON)
		{
			buttonUIBack.SetActive(false);
			buttonUILeft.SetActive(true);
			buttonUIRight.SetActive(true);
		}
		else
		{
			buttonUIBack.SetActive(true);
			buttonUILeft.SetActive(false);
			buttonUIRight.SetActive(false);
		}
	}

	private void SetMoveButtonFalse()
	{
		buttonUIBack.SetActive(false);
		buttonUILeft.SetActive(false);
		buttonUIRight.SetActive(false);
	}

	public void ItemViewClicked()
	{
		//鉛筆削る
		if (buttonItemView.GetComponent<ButtonItemView>().shown == Define.ITEM_PENCILSHARPNER
		   && buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_PENCIL)
		{
			buttonItemView.GetComponent<ButtonItemView>().RemoveItem(Define.ITEM_PENCIL);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItem(Define.ITEM_PENCILSHARPNER);
			buttonItemView.GetComponent<ButtonItemView>().IconPosUpdate();
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_KEZURI);
			buttonItemView.GetComponent<ButtonItemView>().cantap = false;
			StartCoroutine(DelayMethod(1.0f, () =>
			{
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
				buttonItemView.GetComponent<ButtonItemView>().GetItem(Define.ITEM_SHARPEDPENCIL);
				buttonItemView.GetComponent<ButtonItemView>().cantap = true;
			}));
		}

		//紙に鉛筆で書く
		if (buttonItemView.GetComponent<ButtonItemView>().shown == Define.ITEM_PAPER
		   && buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_SHARPEDPENCIL)
		{
			buttonItemView.GetComponent<ButtonItemView>().RemoveItem(Define.ITEM_SHARPEDPENCIL);
			buttonItemView.GetComponent<ButtonItemView>().IconPosUpdate();
			pencilAnimation.SetActive(true);
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_SHAKASHAKA);
			StartCoroutine(buttonItemView.GetComponent<ButtonItemView>().ViewItemChangeTo(4.8f, Define.ITEM_PAPERDRAW));

			StartCoroutine(DelayMethod(4.8f, () =>
			{
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
				pencilAnimation.SetActive(false);
			}));
		}

		//CutKey合体
		if (
			(buttonItemView.GetComponent<ButtonItemView>().shown == Define.ITEM_CUTKEY1
			  && buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_CUTKEY2)
		   ||
			(buttonItemView.GetComponent<ButtonItemView>().shown == Define.ITEM_CUTKEY2
			  && buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_CUTKEY1)
		   )
		{
			buttonItemView.GetComponent<ButtonItemView>().RemoveItem(Define.ITEM_CUTKEY1);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItem(Define.ITEM_CUTKEY2);
			buttonItemView.GetComponent<ButtonItemView>().IconPosUpdate();
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
			buttonItemView.GetComponent<ButtonItemView>().GetItem(Define.ITEM_CUTKEY);
		}

		//Flower切る
		if (
			buttonItemView.GetComponent<ButtonItemView>().shown == Define.ITEM_FLOWER
			  && buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_SCISSORS
		   )
		{
			if (gameFlag.Contains(Define.FLAG_CUT_FLOWERYELLOW))
			{
				buttonItemView.GetComponent<ButtonItemView>().RemoveItem(Define.ITEM_SCISSORS);
			}
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_SCISSORS02);
			AddGameFlag(Define.ITEM_CUTFLOWER);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItem(Define.ITEM_FLOWER);
			buttonItemView.GetComponent<ButtonItemView>().IconPosUpdate();
			inputManager.GetComponent<InputManager>().SetEventSystemEnable(false);
			StartCoroutine(DelayMethod(0.5f, () =>
			 {
				 audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
				 buttonItemView.GetComponent<ButtonItemView>().GetItem(Define.ITEM_CUTFLOWER);
				 inputManager.GetComponent<InputManager>().SetEventSystemEnable(true);
			 }));
		}

		//FlowerYellow切る
		if (
			buttonItemView.GetComponent<ButtonItemView>().shown == Define.ITEM_FLOWERYELLOW
			  && buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_SCISSORS
		   )
		{
			if (gameFlag.Contains(Define.FLAG_CUT_FLOWER))
			{
				buttonItemView.GetComponent<ButtonItemView>().RemoveItem(Define.ITEM_SCISSORS);
			}
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_SCISSORS02);
			AddGameFlag(Define.ITEM_CUTFLOWERYELLOW);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItem(Define.ITEM_FLOWERYELLOW);
			buttonItemView.GetComponent<ButtonItemView>().IconPosUpdate();
			inputManager.GetComponent<InputManager>().SetEventSystemEnable(false);
			StartCoroutine(DelayMethod(0.5f, () =>
			{
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
				buttonItemView.GetComponent<ButtonItemView>().GetItem(Define.ITEM_CUTFLOWERYELLOW);
				inputManager.GetComponent<InputManager>().SetEventSystemEnable(true);
			}));

		}
	}


	public void HintButtonClicked()
	{
		string[] flagArray = HintString.hintTable.Keys.ToArray();

		foreach (string flag in flagArray)
		{
			if (!gameFlag.Contains(flag))
			{
				DisplayMessage(HintString.hintTable[flag]);
				return;
			}
		}
	}


	#endregion



	#region...スクリーン移動
	void WallToRight()//ズームしてない視点の回転移動（スライド
	{
		--lr;
		++wallNo;
		ClearButtons();
		if (wallNo > 4)
		{
			wallNo = Define.WALL_LEFT;
			//右端から右に移動するときはダミーへ移動する
			panelWall1.transform.localPosition = new Vector3(4500f, valueShareManager.GetComponent<ValueShareManager>().ScreenPanel_posY, 0.0f);
			panelWalls.GetComponent<PanelSlider>().SetSlide(new Vector3(-4500f, 0.0f, 0.0f), 0.15f);
			return;
		}
		DisplayWall();
		ClearButtons();
	}

	void WallToLeft()//ズームしてない視点の回転移動（スライド
	{
		++lr;
		--wallNo;
		ClearButtons();
		if (wallNo < 1)
		{
			wallNo = Define.WALL_BACK;
			//左端から左に移動するときは右のダミーに移動しておく
			panelWall1.transform.localPosition = new Vector3(4500f, valueShareManager.GetComponent<ValueShareManager>().ScreenPanel_posY, 0.0f);
			panelWalls.transform.localPosition = new Vector3(-4500f, 0.0f, 0.0f);
		}
		DisplayWall();
	}

	void DisplayWall()//ズームしてない視点の回転移動（スライド
	{
		switch (wallNo)
		{
			case Define.WALL_LEFT:
				panelWall1.transform.localPosition = new Vector3(0.0f, valueShareManager.GetComponent<ValueShareManager>().ScreenPanel_posY, 0.0f);
				panelWalls.GetComponent<PanelSlider>().SetSlide(new Vector3(0.0f, 0.0f, 0.0f), 0.15f);
				break;
			case Define.WALL_FRONT:
				panelWalls.GetComponent<PanelSlider>().SetSlide(new Vector3(-1125f, 0.0f, 0.0f), 0.15f);
				break;
			case Define.WALL_RIGHT:
				panelWalls.GetComponent<PanelSlider>().SetSlide(new Vector3(-2250f, 0.0f, 0.0f), 0.15f);
				break;
			case Define.WALL_BACK:
				panelWalls.GetComponent<PanelSlider>().SetSlide(new Vector3(-3375f, 0.0f, 0.0f), 0.15f);
				break;
		}
	}

	//-------------------------------------------------------------------------------------------------------
	//スクリーン移動（スライドなし
	//-------------------------------------------------------------------------------------------------------

	public void MovingScreen(int srNo)
	{//スライドしない移動　（ズーム変化

		if (screenInfo[screenNo].outSound != "" && screenInfo[screenNo].outSound != Define.SOUND_AVOID && screenInfo[srNo].inSound != Define.SOUND_AVOID)
		{//スクリーンを出る時の音
			audioManager.GetComponent<AudioManager>().PlaySE(screenInfo[screenNo].outSound);
		}

		if (screenInfo[srNo].inSound != "" && screenInfo[srNo].inSound != Define.SOUND_AVOID && screenInfo[screenNo].outSound != Define.SOUND_AVOID)
		{//スクリーンに入る時の音
			audioManager.GetComponent<AudioManager>().PlaySE(screenInfo[srNo].inSound);
		}

		MoveScreen(srNo);
	}

	public void MovingScreenWithoutSound(int srNo)
	{//MovingScreenに引数を二つつけるとインスペクターから呼び出せなくなるので、このようにした。
		MoveScreen(srNo);
	}

	private void MoveScreen(int srNo)
	{

		switch (screenInfo[srNo].zoomStage)
		{
			case Define.ZOOMSTAGE_NON:
				panelWalls.transform.localPosition = new Vector3(-(screenInfo[srNo].xLocation), 0.0f, 0.0f);
				GetComponent<CameraSwitchCrossFade>().SwitchCamera(Define.CAMERA_MAIN);
				//0.5秒後に実行する
				StartCoroutine(DelayMethod(0.4f, () =>
				{
					mainCamera.GetComponent<PostProcessingBehaviour>().profile.motionBlur.enabled = true;
				}));
				break;
			case Define.ZOOMSTAGE_ZOOM:
				panelZooms.transform.localPosition = new Vector3(-(screenInfo[srNo].xLocation), 0.0f, 0.0f);
				GetComponent<CameraSwitchCrossFade>().SwitchCamera(Define.CAMERA_SUB);
				mainCamera.GetComponent<PostProcessingBehaviour>().profile.motionBlur.enabled = false;
				break;
			case Define.ZOOMSTAGE_ZOOMZOOM:
				panelZoomZooms.transform.localPosition = new Vector3(-(screenInfo[srNo].xLocation), 0.0f, 0.0f);
				GetComponent<CameraSwitchCrossFade>().SwitchCamera(Define.CAMERA_SUBSUB);
				mainCamera.GetComponent<PostProcessingBehaviour>().profile.motionBlur.enabled = false;
				break;
			case Define.ZOOMSTAGE_ADD:
				panelWalls.transform.localPosition = new Vector3(-(screenInfo[srNo].xLocation), 0.0f, 0.0f);
				GetComponent<CameraSwitchCrossFade>().SwitchCamera(Define.CAMERA_MAIN);
				break;
			case Define.VIDEOSTAGE:
				GetComponent<CameraSwitchCrossFade>().SwitchCamera(Define.CAMERA_VIDEO);
				break;
		}
		screenNo = srNo;
		ClearButtons();
		UpdateUIButtons();
	}
	#endregion



	#region...仕掛けのボタンを押したとき

	public void PushButtonDirection()//方角の仕掛け
	{
		if (buttonDirection[0].GetComponent<ButtonDirectionManager>().canpush == false)
			return;

		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION22);

		if (buttonDirection[0].GetComponent<ButtonDirectionManager>().direction == Define.UP &&
			buttonDirection[1].GetComponent<ButtonDirectionManager>().direction == Define.LEFT &&
			buttonDirection[2].GetComponent<ButtonDirectionManager>().direction == Define.DOWN &&
			buttonDirection[3].GetComponent<ButtonDirectionManager>().direction == Define.RIGHT)
		{
			inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
			AddGameFlag(Define.FLAG_OPEN_DIRBOX);
			foreach (GameObject btn in buttonDirection)
			{
				btn.GetComponent<ButtonDirectionManager>().canpush = false;
			}

			//0.5秒後に実行する
			StartCoroutine(DelayMethod(0.3f, () =>
			{
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_KEY_IN2);
			}));
			StartCoroutine(DelayMethod(1.0f, () =>
			{
				MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOMOPEN);
				inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);

			}));
		}
	}

	public void PushButtonColors()
	{


		if (gameFlag.Contains(Define.FLAG_OPEN_STARBOX))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN);
			return;
		}

		if (buttonColors[0].GetComponent<ButtonWithNumber>().canpush == false)
			return;

		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_PI03);

		if (buttonColors[0].GetComponent<ButtonWithNumber>().number == 2 &&
			buttonColors[1].GetComponent<ButtonWithNumber>().number == 1 &&
			buttonColors[2].GetComponent<ButtonWithNumber>().number == 4)
		{
			inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
			AddGameFlag(Define.FLAG_OPEN_STARBOX);
			foreach (GameObject btn in buttonColors)
			{
				btn.GetComponent<ButtonWithNumber>().canpush = false;
			}

			//0.5秒後に実行する
			StartCoroutine(DelayMethod(0.3f, () =>
			{
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_SWITCH04);
			}));
			StartCoroutine(DelayMethod(1.0f, () =>
			{
				MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN);
				inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
			}));
		}

	}

	public void PushButtonNumbers()
	{
		if (gameFlag.Contains(Define.FLAG_OPEN_NUMBERBOX))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKRTOPEN);
			return;
		}
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION22);
		if (buttonNumbers[0].GetComponent<ButtonWithNumber>().number == 3 &&
			buttonNumbers[1].GetComponent<ButtonWithNumber>().number == 7 &&
			buttonNumbers[2].GetComponent<ButtonWithNumber>().number == 8 &&
			buttonNumbers[3].GetComponent<ButtonWithNumber>().number == 1)
		{
			Cursor.lockState = CursorLockMode.Locked;
			AddGameFlag(Define.FLAG_OPEN_NUMBERBOX);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_PAPERDRAW);
			foreach (GameObject btn in buttonNumbers)
			{
				btn.GetComponent<ButtonWithNumber>().canpush = false;
			}
			StartCoroutine(DelayMethod(0.3f, () =>
			{
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_KEY_IN2);
			}));
			StartCoroutine(DelayMethod(1.0f, () =>
			{
				MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKRTOPEN);
				Cursor.lockState = CursorLockMode.Confined;
			}));
		}
	}


	public void PushButtonLR(bool isRight)
	{
		if (gameFlag.Contains(Define.FLAG_OPEN_LRBOX) == true)
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKRBOPEN);
			return;
		}

		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION22);
		for (int i = lrJudge.Length - 1; i > 0; --i)
		{
			lrJudge[i] = lrJudge[i - 1];
		}
		lrJudge[0] = isRight;

		if (lrJudge[0] == false &&
		   lrJudge[1] == false &&
		   lrJudge[2] == false &&
		   lrJudge[3] == true &&
		   lrJudge[4] == false &&
		   lrJudge[5] == true &&
		   lrJudge[6] == false)
		{

			inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
			AddGameFlag(Define.FLAG_OPEN_LRBOX);
			StartCoroutine(DelayMethod(0.3f, () =>
			{
				foreach (GameObject obj in buttonLR)
				{
					obj.GetComponent<ButtonFlash>().canpush = false;
				}
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_KEY_IN2);
			}));
			StartCoroutine(DelayMethod(1.0f, () =>
			{
				MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKRBOPEN);
				inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
			}));

		}
	}

	#endregion

	///------------------------------------------------------------

	//クリックエリア押した時 clickarea

	///------------------------------------------------------------

	#region..ClickArea_開いてたら移動

	public void Click_AreaDeskLeftBottom()//机左下
	{
		if (gameFlag.Contains(Define.FLAG_OPEN_DIRBOX))
		{//空いてる時
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOMOPEN);
		}
		else
		{//空いてない時
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKLEFTBOTTOM);
		}
	}

	public void Click_AreaDeskLeftBottomCantOpen()
	{
		audioManager.GetComponent<AudioManager>().PlaySEdontOverRap(Define.SOUND_CANTOPEN02);
		buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(1.5f, "開かない。");
	}

	public void Click_AreaLookUpShelf()
	{

		buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(1.5f, "高くてとどかない。");
	}

	public void ClickAreaHighShelf()
	{
		if (gameFlag.Contains(Define.FLAG_PUT_CHAIR))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_HIGHSHELFZOOM);
		}
		else
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMSTAGE_LOOKUP_SHELF);
		}

	}

	public void Click_AreaStarBoxOpened()
	{
		if (gameFlag.Contains(Define.FLAG_OPEN_STARBOX))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_STARBOXOPEN);
		}
	}

	public void Click_AreaDeskRT()
	{
		if (gameFlag.Contains(Define.FLAG_OPEN_NUMBERBOX))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKRTOPEN);
		}
		else
		{
			audioManager.GetComponent<AudioManager>().PlaySEdontOverRap(Define.SOUND_CANTOPEN04);
			buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(1.5f, "開かない。");
		}
	}

	public void Click_AreaDeskRB()
	{
		if (gameFlag.Contains(Define.FLAG_OPEN_LRBOX))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DESKRBOPEN);
		}
		else
		{
			audioManager.GetComponent<AudioManager>().PlaySEdontOverRap(Define.SOUND_CANTOPEN03);
			buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(1.5f, "開かない。");
		}
	}

	public void Click_AreaKitchenShelfTop()
	{
		if (gameFlag.Contains(Define.FLAG_USE_REDKEY))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFTOPOPEN);
			return;
		}

		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_REDKEY)
		{
			AddGameFlag(Define.FLAG_USE_REDKEY);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_REDKEY);

			StartCoroutine(PlayRedkeyVideo());

			return;
		}

		audioManager.GetComponent<AudioManager>().PlaySEdontOverRap(Define.SOUND_FURNITURECANTOPEN02);
		buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(1.5f, "カギがかかっている。");
		//カタカタ鳴らす
	}

	public void Click_AreaKitchenShelfBottom()
	{
		if (gameFlag.Contains(Define.FLAG_USE_CUTKEY))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFBOTTOMOPEN);
			return;
		}
		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_CUTKEY)
		{
			AddGameFlag(Define.FLAG_USE_CUTKEY);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_CUTKEY);

			StartCoroutine(PlayCutkeyVideo());

			return;
		}

		audioManager.GetComponent<AudioManager>().PlaySEdontOverRap(Define.SOUND_FURNITURECANTOPEN04);
		buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(1.5f, "カギがかかっている。");
		//カタカタ鳴らす
	}

	public void Click_AreaDoor()
	{
		if (gameFlag.Contains(Define.FLAG_OPENDOOR))
		{

		}
		else
		{
			audioManager.GetComponent<AudioManager>().PlaySEdontOverRap(Define.SOUND_DUKTWCHIN);
			buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(1.5f, "カギがかかってる。");
		}

	}

	public void Click_AreaDogStand()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_CUBEDOG)
		{
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_PATA01);
			AddGameFlag(Define.FLAG_PUT_CUBEDOG);
			imageBookShelfDogSet.GetComponent<Image>().enabled = true;
			imageWallFrontDogSet.GetComponent<Image>().enabled = true;
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_CUBEDOG);

			StartCoroutine(DelayMethod(0.3f, () => { audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4); }));
		}

		if (gameFlag.Contains(Define.FLAG_PUT_CUBEDOG))
		{
			Click_AreaCubeDog();
		}
		else
		{
			buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(1.5f, "何かの台かな？");
		}
	}
	#endregion

	#region..ClickArea_置く
	public void Click_AreaPutChair()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_CHAIR)
		{
			imagePutChair.GetComponent<Image>().enabled = true;
			AddGameFlag(Define.FLAG_PUT_CHAIR);
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_CURSOR1);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_CHAIR);
		}
	}
	#endregion

	#region..ClickArea 置いたり、なんかしたり Dog Monkey
	public void Click_AreaHeart()
	{
		if (gameFlag.Contains(Define.FLAG_PUT_HEART))
		{

		}
		else
		{
			buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(2.0f, "ハート型のくぼみがある。");
		}
	}

	public void Click_AreaCubeDog()
	{
		if (gameFlag.Contains(Define.FLAG_PUT_CUBEDOG) && buttonItemView.GetComponent<ButtonItemView>().selected == "Bone")
		{
			AddGameFlag(Define.FLAG_PUT_BONE);
			imageBookShelfDogBoneSet.GetComponent<Image>().enabled = true;
			imageWallFrontDogBoneSet.GetComponent<Image>().enabled = true;
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_BONE);
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);

			StartCoroutine(DelayMethod(0.5f, () =>
			{
				StartCoroutine(PlayDogVideo());
			}));
		}

		if (gameFlag.Contains(Define.FLAG_PLAY_DOGVIDEO))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DOGSTANDOPEN);
		}
	}



	public void Click_AreaKitchenShelfZoomMonkey()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_BANANA
			|| buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_BANANA2)
		{
			if (gameFlag.Contains(Define.FLAG_PUT_BANANA) == false)
			{
				foreach (GameObject obj in imageBanana1)
				{
					obj.GetComponent<Image>().enabled = true;
				}
				AddGameFlag(Define.FLAG_PUT_BANANA);
			}
			else
			{
				foreach (GameObject obj in imageBanana2)
				{
					obj.GetComponent<Image>().enabled = true;
				}
				AddGameFlag(Define.FLAG_PUT_BANANA2);
			}

			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(buttonItemView.GetComponent<ButtonItemView>().selected);

			if (MonkeyLoveCheckAndPlay()) return;
			if (gameFlag.Contains(Define.FLAG_PUT_BANANA2) && !gameFlag.Contains(Define.FLAG_OPEN_LRBOX) && !gameFlag.Contains(Define.FLAG_PUT_MONKO))
			{
				StartCoroutine(PlayMonkeyVideo());
				return;
			}
			else
			{
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
			}
		}

		//置かずにクリックした時
		if (gameFlag.Contains(Define.FLAG_PUT_BANANA2) && !gameFlag.Contains(Define.FLAG_OPEN_LRBOX) && !gameFlag.Contains(Define.FLAG_PUT_MONKO))
		{
			StartCoroutine(PlayMonkeyVideo());
		}

	}

	#endregion

	#region..ClickArea置いたり、なんかしたり_Monko

	public void Click_AreaMonko()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_MONKO)
		{
			foreach (GameObject obj in imageMonko)
			{
				obj.GetComponent<Image>().enabled = true;
			}
			imageMonko[0].GetComponent<BoxCollider2D>().enabled = true;
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_MONKO);
			AddGameFlag(Define.FLAG_PUT_MONKO);
			return;
		}

		if (gameFlag.Contains(Define.FLAG_PUT_MONKO))
		{
			MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_ZOOMMONKO);
		}
	}

	public void Click_AreaMonkoOsara()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_OSARA
		   && gameFlag.Contains(Define.FLAG_PUT_MONKO))
		{
			foreach (GameObject obj in imageOsara)
			{
				obj.GetComponent<Image>().enabled = true;
			}
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_OSARA);
			AddGameFlag(Define.FLAG_PUT_OSARA);
		}

		if (gameFlag.Contains(Define.FLAG_PUT_OSARA))
		{//お皿が置かれていたら、ジンジャーブレッドマン設置の判定をする
			if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_GINGER1
			   || buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_GINGER2)
			{
				if (!gameFlag.Contains(Define.FLAG_PUT_ONEGINGER))
				{//1個目おく
					foreach (GameObject obj in imageGinger1)
					{
						obj.GetComponent<Image>().enabled = true;
					}
					audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
					AddGameFlag(Define.FLAG_PUT_ONEGINGER);
				}
				else
				{//2個目置く
					foreach (GameObject obj in imageGinger2)
					{
						obj.GetComponent<Image>().enabled = true;
					}
					audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
					AddGameFlag(Define.FLAG_PUT_TWOGINGER);
				}
				buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(
					buttonItemView.GetComponent<ButtonItemView>().selected);
				if (gameFlag.Contains(Define.FLAG_PUT_TWOGINGER) && gameFlag.Contains(Define.FLAG_PUT_BANANA2)
					&& gameFlag.Contains(Define.FLAG_PUT_CUTFLOWER) && !gameFlag.Contains(Define.FLAG_OPEN_PAINT))
				{//条件が整っていれば動画再生
					StartCoroutine(PlayMonkoVideo());
				}
				MonkeyLoveCheckAndPlay();
			}
			if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_TWOGINGERS)
			{//2個一気に置く
				foreach (GameObject obj in imageGinger1)
				{
					obj.GetComponent<Image>().enabled = true;
				}
				foreach (GameObject obj in imageGinger2)
				{
					obj.GetComponent<Image>().enabled = true;
				}
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
				AddGameFlag(Define.FLAG_PUT_ONEGINGER);
				AddGameFlag(Define.FLAG_PUT_TWOGINGER);
				buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_TWOGINGERS);

			}

		}

		if (gameFlag.Contains(Define.FLAG_PUT_TWOGINGER) && gameFlag.Contains(Define.FLAG_PUT_CUTFLOWER)
			 && gameFlag.Contains(Define.FLAG_PUT_BANANA2) && !gameFlag.Contains(Define.FLAG_OPEN_PAINT))
		{//条件が整っていれば動画再生
			StartCoroutine(PlayMonkoVideo());
		}
	}

	public void Click_AreaMonkoRightHole()
	{
		PutFlower();

		if (!gameFlag.Contains(Define.FLAG_PUT_CUTFLOWER))
		{
			buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(2.0f, "何かを差しこめそうな穴がある。");
		}

		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_FLOWER)
		{
			buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(2.0f, "長すぎて差し込めない。");
		}
	}

	void PutFlower()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_CUTFLOWER)//花を置く
		{
			foreach (GameObject obj in imageFlower)
			{
				obj.GetComponent<Image>().enabled = true;
			}
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
			AddGameFlag(Define.FLAG_PUT_CUTFLOWER);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_CUTFLOWER);
			if (gameFlag.Contains(Define.FLAG_PUT_TWOGINGER) && gameFlag.Contains(Define.FLAG_PUT_BANANA2)
				&& !gameFlag.Contains(Define.FLAG_PUT_CUTFLOWERYELLOW) && !gameFlag.Contains(Define.FLAG_OPEN_PAINT))
			{//条件が整っていれば動画再生
				StartCoroutine(PlayMonkoVideo());
			}
			MonkeyLoveCheckAndPlay();
			return;
		}
	}

	void PutFlowerYellow()
	{
		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_CUTFLOWERYELLOW)//花を置く
		{
			foreach (GameObject obj in imageFlowerYellow)
			{
				obj.GetComponent<Image>().enabled = true;
			}
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
			AddGameFlag(Define.FLAG_PUT_CUTFLOWERYELLOW);
			MonkeyLoveCheckAndPlay();
			return;
		}
	}
	public void Click_AreaMonkoLeftHole()
	{
		PutFlowerYellow();
		if (!gameFlag.Contains(Define.FLAG_PUT_CUTFLOWERYELLOW))
		{
			buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(2.0f, "何かを差しこめそうな穴がある。");
		}

		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_FLOWERYELLOW)
		{
			buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(2.0f, "長すぎて差し込めない。");
		}

	}

	public void Click_AreaKichenMonkoRightHead()
	{
		PutFlower();
	}

	public void Click_AreaKichenMonkoLefttHead()
	{
		PutFlowerYellow();
	}

	public void Click_AreaZoomMonko()
	{
		if (gameFlag.Contains(Define.FLAG_PUT_TWOGINGER) && gameFlag.Contains(Define.FLAG_PUT_BANANA2)
																&& !gameFlag.Contains(Define.FLAG_OPEN_PAINT))
		{//条件が整っていれば動画再生
			StartCoroutine(PlayMonkoVideo());
		}
	}
	#endregion

	#region..ClickArea_dontClassify
	public void Click_AreaPaint()
	{//絵を戻してから移動
		imagePaint.transform.localPosition = Vector3.zero;
		MovingScreen((int)EScreenNo.SCREEN_ZOOMSTAGE_PAINT);
		if (gameFlag.Contains(Define.FLAG_OPEN_PAINT))
		{
			imagePaint.GetComponent<BoxCollider2D>().enabled = true;
			imageGetFlowerYellow.GetComponent<BoxCollider2D>().enabled = false;
		}
	}

	public void Click_OpenedPaint()
	{//絵、開けた後にクリック
		imagePaint.transform.localPosition = new Vector3(0.0f, -330.0f);
		StartCoroutine(DelayMethod(0.05f, () =>
		{
			imagePaint.transform.localPosition = new Vector3(0.0f, -400.0f);
		}));
		imagePaint.GetComponent<BoxCollider2D>().enabled = false;
		if (imageGetFlowerYellow.GetComponent<Image>().enabled)
		{
			imageGetFlowerYellow.GetComponent<BoxCollider2D>().enabled = true;
		}

	}

	public void Click_AreaDirectionPuzzle()//ピースを置いてから移動
	{
		imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().PutPickedPiece();
		MovingScreen((int)EScreenNo.SCREEN_ZOOMSTAGE_DIRECTIONPUZZLE);
	}


	#endregion


	#region..ClickArea仕掛け押す

	public void Click_AreaDirectionPuzzleHole(int holeNo)
	{
		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_GLASSDOOR)
		{
			imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().PutNewPiece(Define.GLASSDOOR, holeNo);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_GLASSDOOR);
		}

		if (buttonItemView.GetComponent<ButtonItemView>().selected == Define.ITEM_GLASSTRASHBOX)
		{
			imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().PutNewPiece(Define.GLASSTRASHBOX, holeNo);
			buttonItemView.GetComponent<ButtonItemView>().RemoveItemAndUpdatePos(Define.ITEM_GLASSTRASHBOX);
		}

	}
	//自作イベント
	void OnPuzzlePieceMoved()
	{
		SaveData.Instance.holes[0] = imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().holes[0];
		SaveData.Instance.holes[1] = imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().holes[1];
		SaveData.Instance.holes[2] = imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().holes[2];
		SaveData.Instance.holes[3] = imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().holes[3];
		SaveData.Instance.pickedPiece = imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().PickedPiece;
		SaveData.Instance.pickedHoleNumber = imageDirectionPuzzle.GetComponent<ReplacementPuzzle4>().PickedHoleNo;
		SaveData.Instance.Save();
	}

	public void Click_AreaPaintsCorner(int corner)
	{
		if (gameFlag.Contains(Define.FLAG_OPEN_PAINT))
		{
			return;//kaizou
		}

		if (corner == cornerJudge[0]) return;

		switch (corner)
		{
			case Define.CORNER_UPPERLEFT:
				imagePaint.transform.localPosition = new Vector3(2.0f, -2.0f);
				RotatePaint();
				break;
			case Define.CORNER_UPPERRIGHT:
				imagePaint.transform.localPosition = new Vector3(-2.0f, -2.0f);
				RotatePaint();
				break;
			case Define.CORNER_LOWERRIGHT:
				imagePaint.transform.localPosition = new Vector3(-2.0f, 2.0f);
				RotatePaint();
				break;
			case Define.CORNER_LOWERLEFT:
				imagePaint.transform.localPosition = new Vector3(2.0f, 2.0f);
				RotatePaint();
				break;
		}

		for (int i = cornerJudge.Length - 1; i > 0; --i)
		{
			cornerJudge[i] = cornerJudge[i - 1];
		}
		cornerJudge[0] = corner;

		if (cornerJudge[5] == Define.CORNER_UPPERLEFT &&
			cornerJudge[4] == Define.CORNER_UPPERRIGHT &&
			cornerJudge[3] == Define.CORNER_LOWERRIGHT &&
			cornerJudge[2] == Define.CORNER_UPPERLEFT &&
			cornerJudge[1] == Define.CORNER_LOWERLEFT &&
			cornerJudge[0] == Define.CORNER_UPPERRIGHT)
		{//仕掛け解いたら
			AddGameFlag(Define.FLAG_OPEN_PAINT);
			inputManager.GetComponent<InputManager>().SetEventSystemEnable(false);
			//0.5秒後に実行する
			StartCoroutine(DelayMethod(0.3f, () =>
			{
				audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_PATA01);
			}));
			StartCoroutine(DelayMethod(1.2f, () =>
			{
				imagePaint.transform.localPosition = new Vector3(0.0f, -330.0f);
			}));
			StartCoroutine(DelayMethod(1.25f, () =>
			{
				imagePaint.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
				imagePaint.transform.localPosition = new Vector3(0.0f, -400.0f);
				inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
			}));
			imageGetFlowerYellow.GetComponent<BoxCollider2D>().enabled = true;
		}

	}

	void RotatePaint()//Click_AreaPaintsCorner用
	{
		if ((int)imagePaint.transform.rotation.eulerAngles.z == 2)
		{
			imagePaint.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -2.0f);
		}
		else
		{
			imagePaint.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 2.0f);
		}
		float r = UnityEngine.Random.Range(0.0f, 1.0f);
		if (r < 0.33)
		{
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_KTON01);
		}
		else if (r < 0.66)
		{
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_KTON07);
		}
		else
		{
			audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_TOKO01);
		}
	}

	#endregion




	public void Puzzle4Aligned()
	{
		AddGameFlag(Define.FLAG_OPEN_DIRECTIONPUZZULE);
		StartCoroutine(PlayScissorsVideo());
	}

	#region・・動画再生

	private IEnumerator PlayMonkeyVideo()
	{
		videoPlayerMonkey.SetActive(true);
		SetMoveButtonFalse();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
		videoPlayerMonkey.GetComponent<VideoPlayer>().Play();
		videoPlayerMonkey.GetComponent<VideoPlayer>().Pause();
		while (videoPlayerMonkey.GetComponent<VideoPlayer>().isPrepared == false)
		{
			videoPlayerMonkey.GetComponent<VideoPlayer>().Prepare(); yield return null;
		}
		MovingScreen((int)EScreenNo.SCREEN_VIDEO);
		yield return new WaitForSeconds(0.2f);//movingscreen待ち
		videoPlayerMonkey.GetComponent<VideoPlayer>().Play();
		while (videoPlayerMonkey.GetComponent<VideoPlayer>().isPlaying == false) yield return null;//再生待ち
																								   //ここに音楽
		while (videoPlayerMonkey.GetComponent<VideoPlayer>().isPlaying)
		{//終わりまで再生
			yield return null;
		}
		MovingScreen((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFMONKEY);
		UpdateUIButtons();
		videoPlayerMonkey.GetComponent<VideoPlayer>().Stop();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
		videoPlayerMonkey.SetActive(false);
	}

	private IEnumerator PlayRedkeyVideo()
	{
		videoPlayerRedKey.SetActive(true);
		SetMoveButtonFalse();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
		videoPlayerRedKey.GetComponent<VideoPlayer>().Play();
		videoPlayerRedKey.GetComponent<VideoPlayer>().Pause();
		while (videoPlayerRedKey.GetComponent<VideoPlayer>().isPrepared == false)
		{
			videoPlayerRedKey.GetComponent<VideoPlayer>().Prepare(); yield return null;
		}
		MovingScreen((int)EScreenNo.SCREEN_VIDEO);
		yield return new WaitForSeconds(0.3f);//１コマ目でしばらく止める
		videoPlayerRedKey.GetComponent<VideoPlayer>().Play();
		while (videoPlayerRedKey.GetComponent<VideoPlayer>().isPlaying == false) yield return null;//再生待ち
		yield return new WaitForSeconds(1.9f);//1.9秒あたりに音挿入
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_KACHAN05);
		while (videoPlayerRedKey.GetComponent<VideoPlayer>().isPlaying == true)
		{
			if ((int)videoPlayerRedKey.GetComponent<VideoPlayer>().frame >=
				(int)videoPlayerRedKey.GetComponent<VideoPlayer>().frameCount)
			{
				videoPlayerCutKey.GetComponent<VideoPlayer>().Pause();
				break;
			}
			yield return null;//再生終わり待ち               
		}
		yield return new WaitForSeconds(0.8f);
		MovingScreenWithoutSound((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFTOPOPEN);
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DRAWER11);
		yield return new WaitForSeconds(0.2f);//最終フレームでちょい待ち
		UpdateUIButtons();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
		videoPlayerRedKey.GetComponent<VideoPlayer>().Stop();
		videoPlayerRedKey.SetActive(false);

	}

	private IEnumerator PlayCutkeyVideo()
	{
		videoPlayerCutKey.SetActive(true);
		SetMoveButtonFalse();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
		videoPlayerCutKey.GetComponent<VideoPlayer>().Play();
		videoPlayerCutKey.GetComponent<VideoPlayer>().Pause();
		while (videoPlayerCutKey.GetComponent<VideoPlayer>().isPrepared == false)//再生準備待ち
		{
			videoPlayerCutKey.GetComponent<VideoPlayer>().Prepare(); yield return null;
		}
		MovingScreen((int)EScreenNo.SCREEN_VIDEO);
		yield return new WaitForSeconds(0.3f);//１コマ目でしばらく止める
		videoPlayerCutKey.GetComponent<VideoPlayer>().Play();
		while (videoPlayerCutKey.GetComponent<VideoPlayer>().isPlaying == false) yield return null;//再生待ち
		yield return new WaitForSeconds(1.5f);//音挿入
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_KACHAN05);
		while (videoPlayerCutKey.GetComponent<VideoPlayer>().isPlaying == true)
		{
			if ((int)videoPlayerCutKey.GetComponent<VideoPlayer>().frame ==
			   (int)videoPlayerCutKey.GetComponent<VideoPlayer>().frameCount)
			{
				videoPlayerCutKey.GetComponent<VideoPlayer>().Pause();
				break;
			}
			yield return null;//再生終わり待ち               
		}
		yield return new WaitForSeconds(0.8f);//最終フレームでちょい待ち
		MovingScreenWithoutSound((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_KITCHENSHELFBOTTOMOPEN);
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_FURNITUREDOOROPEN04);
		yield return new WaitForSeconds(0.8f);
		UpdateUIButtons();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
		videoPlayerCutKey.GetComponent<VideoPlayer>().Stop();
		videoPlayerCutKey.SetActive(false);

	}

	private IEnumerator PlayDogVideo()
	{
		videoPlayerDog.SetActive(true);
		SetMoveButtonFalse();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);

		videoPlayerDog.GetComponent<VideoPlayer>().Play();
		videoPlayerDog.GetComponent<VideoPlayer>().Pause();//1コマ目で止める
		videoPlayerDog.GetComponent<VideoPlayer>().time = 0;
		while (videoPlayerDog.GetComponent<VideoPlayer>().isPrepared == false)//再生準備待ち
		{
			videoPlayerDog.GetComponent<VideoPlayer>().Prepare(); yield return null;
		}
		MovingScreen((int)EScreenNo.SCREEN_VIDEO);
		yield return new WaitForSeconds(0.6f);
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_HUWHUWHLLL);
		yield return new WaitForSeconds(0.8f);//
		videoPlayerDog.GetComponent<VideoPlayer>().Play();
		yield return new WaitForSeconds(0.1f);
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DRAWER14);
		while (videoPlayerDog.GetComponent<VideoPlayer>().isPlaying == false) yield return null;//再生待ち
		while (videoPlayerDog.GetComponent<VideoPlayer>().isPlaying == true) yield return null;//再生終わり待ち

		yield return new WaitForSeconds(0.6f);//最終フレームでちょい待ち
		MovingScreenWithoutSound((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_DOGSTANDOPEN);
		UpdateUIButtons();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
		videoPlayerDog.GetComponent<VideoPlayer>().Stop();
		videoPlayerDog.SetActive(false);
		AddGameFlag(Define.FLAG_PLAY_DOGVIDEO);
	}

	private IEnumerator PlayScissorsVideo()
	{
		videoPlayerScissors.SetActive(true);
		SetMoveButtonFalse();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
		videoPlayerScissors.GetComponent<VideoPlayer>().Play();
		videoPlayerScissors.GetComponent<VideoPlayer>().Pause();
		while (videoPlayerScissors.GetComponent<VideoPlayer>().isPrepared == false)//再生準備待ち
		{
			videoPlayerScissors.GetComponent<VideoPlayer>().Prepare(); yield return null;
		}
		MovingScreen((int)EScreenNo.SCREEN_VIDEO);
		yield return new WaitForSeconds(0.5f);//１コマ目でしばらく止める
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_SWITCH04);
		yield return new WaitForSeconds(0.8f);
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
		yield return new WaitForSeconds(0.8f);
		videoPlayerScissors.GetComponent<VideoPlayer>().Play();
		while (videoPlayerScissors.GetComponent<VideoPlayer>().isPlaying == false) yield return null;//再生待ち
		yield return new WaitForSeconds(0.3f);//音挿入
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_SHAAAA);
		while (videoPlayerScissors.GetComponent<VideoPlayer>().isPlaying == true)
		{
			if ((int)videoPlayerScissors.GetComponent<VideoPlayer>().frame ==
				(int)videoPlayerScissors.GetComponent<VideoPlayer>().frameCount)
			{
				videoPlayerScissors.GetComponent<VideoPlayer>().Pause();
				break;
			}
			yield return null;//再生終わり待ち               
		}
		//yield return new WaitForSeconds(0.8f);//最終フレームでちょい待ち
		MovingScreenWithoutSound((int)EScreenNo.SCREEN_ADDSTAGE_DIRECTIONPUZZLEOPEN);
		yield return new WaitForSeconds(0.8f);
		UpdateUIButtons();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
		videoPlayerScissors.GetComponent<VideoPlayer>().Stop();
		videoPlayerScissors.SetActive(false);
	}

	private IEnumerator PlayMonkoVideo()
	{
		videoPlayerMonko.SetActive(true);
		SetMoveButtonFalse();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
		//videoPlayerMonko.GetComponent<VideoPlayer>().Play();
		//videoPlayerMonko.GetComponent<VideoPlayer>().Pause();
		while (videoPlayerMonko.GetComponent<VideoPlayer>().isPrepared == false)//再生準備待ち
		{
			videoPlayerMonko.GetComponent<VideoPlayer>().Prepare(); yield return null;
		}
		MovingScreen((int)EScreenNo.SCREEN_VIDEO);
		yield return new WaitForSeconds(0.8f);
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DECISION4);
		yield return new WaitForSeconds(0.8f);
		videoPlayerMonko.GetComponent<VideoPlayer>().Play();
		while (videoPlayerMonko.GetComponent<VideoPlayer>().isPlaying == false) yield return null;//再生待ち
		yield return new WaitForSeconds(0.3f);//音挿入
		while (videoPlayerMonko.GetComponent<VideoPlayer>().isPlaying == true)
		{
			if ((int)videoPlayerMonko.GetComponent<VideoPlayer>().frame ==
				(int)videoPlayerMonko.GetComponent<VideoPlayer>().frameCount)
			{
				videoPlayerMonko.GetComponent<VideoPlayer>().Pause();
				break;
			}
			yield return null;//再生終わり待ち               
		}
		//yield return new WaitForSeconds(0.8f);//最終フレームでちょい待ち
		MovingScreenWithoutSound((int)EScreenNo.SCREEN_ZOOMZOOMSTAGE_ZOOMMONKO);
		yield return new WaitForSeconds(0.8f);
		UpdateUIButtons();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
		videoPlayerMonko.GetComponent<VideoPlayer>().Stop();
		videoPlayerMonko.SetActive(false);
	}

	private IEnumerator PlayMonkeyLoveVideo()
	{
		videoPlayerMonkeyLove.SetActive(true);
		SetMoveButtonFalse();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(false);
		videoPlayerMonkeyLove.GetComponent<VideoPlayer>().Play();
		videoPlayerMonkeyLove.GetComponent<VideoPlayer>().Pause();
		while (videoPlayerMonkeyLove.GetComponent<VideoPlayer>().isPrepared == false)
		{
			videoPlayerMonkeyLove.GetComponent<VideoPlayer>().Prepare(); yield return null;
		}
		MovingScreen((int)EScreenNo.SCREEN_VIDEO);
		yield return new WaitForSeconds(0.3f);//１コマ目でしばらく止める
		videoPlayerMonkeyLove.GetComponent<VideoPlayer>().Play();
		while (videoPlayerMonkeyLove.GetComponent<VideoPlayer>().isPlaying == false) yield return null;//再生待ち
		while (videoPlayerMonkeyLove.GetComponent<VideoPlayer>().isPlaying == true)
		{
			if ((int)videoPlayerMonkeyLove.GetComponent<VideoPlayer>().frame >=
				(int)videoPlayerMonkeyLove.GetComponent<VideoPlayer>().frameCount)
			{
				videoPlayerMonkeyLove.GetComponent<VideoPlayer>().Pause();
				break;
			}
			yield return null;//再生終わり待ち               
		}
		yield return new WaitForSeconds(0.8f);
		MovingScreenWithoutSound((int)EScreenNo.SCREEN_ZOOMSTAGE_KITCHENSHELF);
		audioManager.GetComponent<AudioManager>().PlaySE(Define.SOUND_DRAWER11);
		yield return new WaitForSeconds(0.2f);//最終フレームでちょい待ち
		UpdateUIButtons();
		inputManager.GetComponent<InputManager>().SetEventSystemEnableAndCanSet(true);
		videoPlayerMonkeyLove.GetComponent<VideoPlayer>().Stop();
		videoPlayerMonkeyLove.SetActive(false);

	}
	#endregion

	//------------------------------------------------------------
	//その他
	//------------------------------------------------------------
	bool MonkeyLoveCheckAndPlay()
	{
		if (gameFlag.Contains(Define.FLAG_PUT_BANANA2) &&
		   gameFlag.Contains(Define.FLAG_PUT_TWOGINGER) &&
		   gameFlag.Contains(Define.FLAG_PUT_CUTFLOWER) &&
		   gameFlag.Contains(Define.FLAG_PUT_CUTFLOWERYELLOW)
		  )
		{
			return true;
		}
		return false;
	}

	void ClearButtons()
	{
		buttonMessage.GetComponent<DisplayMessageManager>().CloseMessage();

	}

	public void DisplayMessage(string str)
	{
		buttonMessage.GetComponent<DisplayMessageManager>().DisplayMessage(str);
	}



	void ChangeButtonColor(int buttonNo)
	{
		//unnecessary
	}
	void AddGameFlag(string flag)
	{
		gameFlag.Add(flag);
		SaveData.Instance.gameFlag = gameFlag;
		SaveData.Instance.Save();
	}

	#region..機能メソッド
	/// <summary>
	/// 渡された処理を指定時間後に実行する
	/// </summary>
	/// <param name="waitTime">遅延時間[ミリ秒]</param>
	/// <param name="action">実行したい処理</param>
	/// <returns></returns>
	IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
	#endregion


}
