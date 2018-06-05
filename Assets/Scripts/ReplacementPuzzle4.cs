using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;//(EventArgs
using Common;
//４つのアナを持つ方角パズル。操作出来る拡大ビューと少し引いたビュー２つのビューに対応する
//スクリプトをパズルの台のイメージにアタッチ
//ピース画像用イメージにそれぞれのhollsアタッチ　（ピース画像に２Dコライダー,イベントトリガー,pointerClickイベント追加)
//spritsHole spritsHoleUpに全てのピース画像をアタッチ
//インスペクターでholesにピース初期配置を設定しておく


public class ReplacementPuzzle4 : MonoBehaviour
{
	public event EventHandler PuzzlePieceMoved;

	protected virtual void OnPuzzlePieceMoved(EventArgs e)//派生クラスの拡張性を損なわないために、引数をつけ、protectedをつける。
	{//イベントを発生させるクラスにプロテクト メソッドを提供します。 メソッドに OnEventName という名前を付けます。
		//このメソッド内で対象のイベントを発生させます。 C# コードでは、イベントを発生させる前にイベントが null かどうかを確認する必要があることに注意してください。
		//これによって、イベント ハンドラーが関連付けられていない場合にイベントの発生時にスローされる NullReferenceException を処理する必要がなくなります。
		if (PuzzlePieceMoved != null)
			PuzzlePieceMoved(this,e);
    }

	GameObject gameManager;

	int pickedPiece = -1;//0~3持っているピース　何も持ってないときは-1
	public int PickedPiece{
		set { pickedPiece = value; }
		get { return pickedPiece; }
	}

	int pickedHoleNo = -1;//0~3持っているピースの入っていた穴　何も持ってないときは-1
	public int PickedHoleNo{
		set { pickedHoleNo = value; }
		get { return PickedHoleNo; }
	}

	public string[] putSound = new string[4];
	public const int PIECENUM = 4;
	GameObject audioManager;
    

	public class Piece{//一つのピース
		public string name = "";
		public Sprite[] sprits = new Sprite[4];
	}

	public GameObject[] holeImages;//イメージにアタッチ
	public GameObject[] holeMiniImages;//引いたビューのイメージにアタッチ


	public Sprite [] spritsHole1 = new Sprite[PIECENUM];
	public Sprite[] spritsHole2 = new Sprite[PIECENUM];
	public Sprite [] spritsHole3 = new Sprite[PIECENUM];
	public Sprite[] spritsHole4 = new Sprite[PIECENUM];
	public Sprite[] spritsHole1Up = new Sprite[PIECENUM];
    public Sprite[] spritsHole2Up = new Sprite[PIECENUM];
    public Sprite[] spritsHole3Up = new Sprite[PIECENUM];
    public Sprite[] spritsHole4Up = new Sprite[PIECENUM];

	public Sprite[] spritsHole1Mini = new Sprite[PIECENUM];
	public Sprite[] spritsHole2Mini = new Sprite[PIECENUM];
	public Sprite[] spritsHole3Mini = new Sprite[PIECENUM];
	public Sprite[] spritsHole4Mini = new Sprite[PIECENUM];

	public int[] answer = new int[4];//正解
	public int[] holes = new int[PIECENUM];//置かれているピース番号を保持、何もないときは-1 inspectorで初期値を設定したいのでpublic
	bool putNew = false;

	//private void piece
	// Use this for initialization
	void Start()
	{

		gameManager = GameObject.Find("GameManager");
		audioManager = GameObject.Find("AudioManager");

		//inspectorで追加したイベントトリガーにコールバックを設定
		EventTrigger.Entry found;
		found = holeImages[0].GetComponent<EventTrigger>()
				.triggers.Find(x => x.eventID == EventTriggerType.PointerClick);
		found.callback.AddListener((BaseEventData) => HoleClicked(0));
		found = holeImages[1].GetComponent<EventTrigger>()
                .triggers.Find(x => x.eventID == EventTriggerType.PointerClick);
        found.callback.AddListener((BaseEventData) => HoleClicked(1));
		found = holeImages[2].GetComponent<EventTrigger>()
                .triggers.Find(x => x.eventID == EventTriggerType.PointerClick);
        found.callback.AddListener((BaseEventData) => HoleClicked(2));
		found = holeImages[3].GetComponent<EventTrigger>()
                .triggers.Find(x => x.eventID == EventTriggerType.PointerClick);
        found.callback.AddListener((BaseEventData) => HoleClicked(3));

		UpdatePieceImage();
	}

	// Update is called once per frame
	void Update () {
		
	}

	#region  //click Put
	public void HoleClicked(int holeNo){

			int buf;
		buf = pickedPiece;
		if (pickedPiece != -1)
		{//何か持ち上げてる時
			if(holes[holeNo] == -1)
			{//クリックした穴には何もない
				holes[holeNo] = buf;
			}else{//クリックした穴にもピースが入ってる
				holes[pickedHoleNo] = holes[holeNo];
                holes[holeNo] = buf;
			}
			pickedPiece = -1;
            pickedHoleNo = -1;
			PlayPutSound();
			CheckAlign();
		}
		else
		{//手元に何もない
			pickedPiece = holes[holeNo];
			if (pickedPiece != -1)
			{//何か持った時は入っていた穴を記憶
				pickedHoleNo = holeNo;
			}
			else
			{
				pickedHoleNo = -1;
			}
			holes[holeNo] = buf;
		}

		if (putNew) {//新しいものを置く時にも、このメソッドが呼ばれて持ち上げてしまうので、降ろす。
			putNew = false;
			PutPickedPiece();
		}
		//OnPuzzlePieceMoved(EventArgs.Empty);
		UpdatePieceImage();
	}

	public void PutPickedPiece(){//パズル画面に入る時などに、持ち上げているものを降ろす処理GameManagerから呼ぶ
		if(pickedPiece != -1){
			holes[pickedHoleNo] = pickedPiece;
			pickedPiece = -1;
			UpdatePieceImage();
		}
		OnPuzzlePieceMoved(EventArgs.Empty);
	}

	public void PutNewPiece(int pieceNo ,int holeNo){//GameManagerから呼ぶ
		PutPickedPiece();
		if (holes[holeNo] == -1)
		{
			holes[holeNo] = pieceNo;
		}else{//空いてない穴をクリックされたときは、空いている穴に置く
			for (int i = 0; i < holes.Length; ++i){
				if(holes[i] == -1){
					holes[i] = pieceNo;
					//PutPickedPiece();//HoleClickedが呼ばれて持ち上げた状態になっているので
					break;
				}
			}
            
		}
		PlayPutSound();
		putNew = true;
		//HoleClickedも呼ばれるのでそっちでも処理をする
	}
	#endregion

	#region//check系
	void CheckAlign(){//揃ったかどうかチェック
		if(holes[0] == answer[0] &&
		   holes[1] == answer[1] &&
		   holes[2] == answer[2] &&
		   holes[3] == answer[3] ){//揃ったら
			SetColliderFalse();
			gameManager.GetComponent<GameManager>().Puzzle4Aligned();
		}

	}

	public void UpdatePieceImage(){
		
		if (holes[0] != -1)//Left画像
			{//何か入っている時
				holeImages[0].GetComponent<Image>().sprite = spritsHole1[holes[0]];
			holeMiniImages[0].GetComponent<Image>().sprite = spritsHole1Mini[holes[0]];
			holeImages[0].GetComponent<Image>().enabled = true;
			holeMiniImages[0].GetComponent<Image>().enabled = true;
			}else{//何も入ってない時
    			if (pickedHoleNo == 0)
			    {//持ち上げてる時
				holeImages[0].GetComponent<Image>().sprite = spritsHole1Up[pickedPiece];
				holeMiniImages[0].GetComponent<Image>().sprite = spritsHole1Mini[pickedPiece];
				holeMiniImages[0].GetComponent<Image>().enabled = true;
    			}
    			else
    			{
    				holeImages[0].GetComponent<Image>().enabled = false;
				holeMiniImages[0].GetComponent<Image>().enabled = false;
    			}
			}

		if (holes[1] != -1)//Front画像
		{//何か入っている時
			holeImages[1].GetComponent<Image>().sprite = spritsHole2[holes[1]];
			holeMiniImages[1].GetComponent<Image>().sprite = spritsHole2Mini[holes[1]];
			holeImages[1].GetComponent<Image>().enabled = true;
			holeMiniImages[1].GetComponent<Image>().enabled = true;
		}
		else
		{//何も入ってない時
			if (pickedHoleNo == 1)
			{//持ち上げてる時
				holeImages[1].GetComponent<Image>().sprite = spritsHole2Up[pickedPiece];
                holeMiniImages[1].GetComponent<Image>().sprite = spritsHole2Mini[pickedPiece];
				holeMiniImages[1].GetComponent<Image>().enabled = true;
			}
			else
			{
				holeImages[1].GetComponent<Image>().enabled = false;
				holeMiniImages[1].GetComponent<Image>().enabled = false;
			}
		}

		if (holes[2] != -1)//Right画像
		{//何か入っている時
			holeImages[2].GetComponent<Image>().sprite = spritsHole3[holes[2]];
			holeMiniImages[2].GetComponent<Image>().sprite = spritsHole3Mini[holes[2]];
			holeImages[2].GetComponent<Image>().enabled = true;
			holeMiniImages[2].GetComponent<Image>().enabled = true;
		}
		else
		{//何も入ってない時
			if (pickedHoleNo == 2)
			{//持ち上げてる時
				holeImages[2].GetComponent<Image>().sprite = spritsHole3Up[pickedPiece];
                holeMiniImages[2].GetComponent<Image>().sprite = spritsHole3Mini[pickedPiece];
				holeMiniImages[2].GetComponent<Image>().enabled = true;
			}
			else
			{
				holeImages[2].GetComponent<Image>().enabled = false;
				holeMiniImages[2].GetComponent<Image>().enabled = false;
			}
		}

		if (holes[3] != -1)//Back画像
		{//何か入っている時
			holeImages[3].GetComponent<Image>().sprite = spritsHole4[holes[3]];
			holeMiniImages[3].GetComponent<Image>().sprite = spritsHole4Mini[holes[3]];
			holeImages[3].GetComponent<Image>().enabled = true;
			holeMiniImages[3].GetComponent<Image>().enabled = true;
		}
		else
		{//何も入ってない時
			if (pickedHoleNo == 3)
			{//持ち上げてる時
				holeImages[3].GetComponent<Image>().sprite = spritsHole4Up[pickedPiece];
                holeMiniImages[3].GetComponent<Image>().sprite = spritsHole4Mini[pickedPiece];
				holeMiniImages[3].GetComponent<Image>().enabled = true;
			}
			else
			{
				holeImages[3].GetComponent<Image>().enabled = false;
				holeMiniImages[3].GetComponent<Image>().enabled = false;
			}
		}
		CheckAlign();
	}//UpdatePieceImage()
	#endregion

	void SetColliderFalse(){//パズルをクリックできなくする
		foreach(GameObject obj in holeImages){
			obj.GetComponent<Collider2D>().enabled = false;
		}
	}

	void PlayPutSound(){
		int random = UnityEngine.Random.Range(1, 5);//1~4の数random生成
		switch(random){
			case 1: audioManager.GetComponent<AudioManager>().PlaySE(putSound[0]);
				break;
			case 2:
                audioManager.GetComponent<AudioManager>().PlaySE(putSound[1]);
                break;
			case 3:
                audioManager.GetComponent<AudioManager>().PlaySE(putSound[2]);
                break;
			case 4:
                audioManager.GetComponent<AudioManager>().PlaySE(putSound[3]);
                break;
		}
	}

}//class
