using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothlyMover : MonoBehaviour
{

	//public Sprite forwardSprite;
	public int NUMOF_SMOOTH = 4;
	public float ALPHA = 0.3f;

	public Image[] forward;
	public Image[] back;
	public int denoiseSampleNum = 4;
	//移動距離デノイズ用サンプル数

	List<Vector3> listPreMove = new List<Vector3> ();
	//moveDisのデノイズサンプル用

	private Vector3 moveDis;
	//１フレーム前から動いた距離方角
	private Vector3 prePos;
	private Sprite previousSp;


	// Use this for initialization
	void Start ()
	{
		prePos = transform.position;

		if (GetComponent<Image> ().sprite != null) {
			foreach (Image fwd in forward) {
				fwd.sprite = GetComponent <Image> ().sprite;
			}
			foreach (Image bc in back) {
				bc.sprite = GetComponent <Image> ().sprite;
			}
		}

//		for (int i = 0; i < NUMOF_SMOOTH; ++i) {
//			forward [i].color = new Color (1.0f, 1.0f, 1.0f, (i + 1.0f) / (NUMOF_SMOOTH + 1.0f));
//			back [i].color = new Color (1.0f, 1.0f, 1.0f, (i + 1.0f) / (NUMOF_SMOOTH + 1.0f));
//		}
		for (int i = 0; i < NUMOF_SMOOTH; ++i) {
			forward [i].color = new Color (1.0f, 1.0f, 1.0f, ALPHA);
			back [i].color = new Color (1.0f, 1.0f, 1.0f, ALPHA);
		}

	}
	
	// Update is called once per frame
	void Update ()
	{
		

		if (GetComponent<Image> ().sprite != previousSp) {//親のspriteが変わったら子も変える
			foreach (Image fwd in forward) {
				fwd.sprite = GetComponent <Image> ().sprite;
			}
			foreach (Image bc in back) {
				bc.sprite = GetComponent <Image> ().sprite;
			}
		}

		if (prePos == transform.position) {

		}


//		//TEST
//		forward [0].GetComponent<RectTransform> ().sizeDelta 
//						= GetComponent<RectTransform> ().sizeDelta;




//		if (currentPos != listPrePos[0] && GetComponent<Image> ().enabled == true) {
//
//			moveDis = currentPos - listPrePos[0];
//
//			for (int i = 0; i < NUMOF_SMOOTH; ++i) {
//				forward [i].transform.localPosition
		//				= (moveDis * (i + 1.0f) / (NUMOF_SMOOTH + 1.0f));
//				back [i].transform.localPosition
		//				= -(moveDis * (i + 1.0f) / (NUMOF_SMOOTH + 1.0f));
//
//				forward [i].enabled = true;
//				back [i].enabled = true;
//
//				forward [i].GetComponent<RectTransform> ().sizeDelta 
//				= GetComponent<RectTransform> ().sizeDelta;
//				back [i].GetComponent<RectTransform> ().sizeDelta 
//				= GetComponent<RectTransform> ().sizeDelta;
//			}
//
		//			GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 0.7f);
//		} else {
//			
		//			GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
//
//			for (int i = 0; i < NUMOF_SMOOTH; ++i) {
//				forward [i].enabled = false;
//				back [i].enabled = false;
//			}
//		}

//		listPrePos[0] = transform.localPosition;
		previousSp = GetComponent<Image> ().sprite;



	}


	public void begin ()
	{



		GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, ALPHA);

		foreach (Image fwd in forward) {
			fwd.enabled = true;
		}

		foreach (Image bc in back) {
			bc.enabled = true;
		}
	}

	public void end ()
	{
		GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);

		foreach (Image fwd in forward) {
			fwd.enabled = false;
		}

		foreach (Image bc in back) {
			bc.enabled = false;
		}

		listPreMove.Clear ();
	}
	//このオブジェクトをアタッチするオブジェクトから呼び出す
	public void ChangePos (Vector3 parentPos)
	{
		
		transform.localPosition = parentPos;

		//test
		moveDis = transform.position - prePos;

		listPreMove.Insert (0, moveDis);
		if (listPreMove.Count > denoiseSampleNum) {
			listPreMove.RemoveAt (denoiseSampleNum);
		}
			
		float denoiseX = 0.0f;
		float denoiseY = 0.0f;
		for (int i = 0; i < listPreMove.Count; ++i) {
			denoiseX += listPreMove [i].x;
			denoiseY += listPreMove [i].y;
		}
		denoiseX /= listPreMove.Count;
		denoiseY /= listPreMove.Count;

		denoiseX *= 2.0f;
		denoiseY *= 2.0f;
		for (int i = 0; i < NUMOF_SMOOTH; ++i) {
			forward [i].transform.localPosition = 
				new Vector3 (denoiseX / (i + 1.0f), denoiseY / (i + 1.0f), 0.0f);
			back [i].transform.localPosition = 
				new Vector3 (-(denoiseX / (i + 1.0f)), -(denoiseY / (i + 1.0f)), 0.0f);
		}



//		Debug.Log (moveDis.ToString ());
//		Debug.Log (denoiseX.ToString ());


		prePos = transform.position;

	}

	public void ChangeSize (Vector2 size)
	{
		GetComponent<RectTransform> ().sizeDelta = size;

		foreach (Image fwd in forward) {
			fwd.GetComponent<RectTransform> ().sizeDelta = size;
		}
		foreach (Image bc in back) {
			bc.GetComponent<RectTransform> ().sizeDelta = size;
		}
	}

}
