using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

//

/// <summary>
/// Camera switch cross fade.
/// シーンにカメラとキャンバスの組み合わせが複数ある時に、それをクロスフェードで切り替える
/// カメラ一つとキャンバスをセットにして考える。
/// カメラにはphysics
/// 
/// これをGameManagerにアタッチ。カメラとキャンバスをこれにアタッチ(canvasはcanvas groupを追加しておく
/// physics2Draycasterをカメラに追加して　ボックスコライダー２Dを配置すると、canvasgroupの interactiveを
/// falseにしても反応してしまうので、physics2Draycasterのenableをfalseに。
/// </summary>
public class CameraSwitchCrossFade : MonoBehaviour
{
	public int cameraAndCanvasNum = 2;
	public float duration = 0.3f;
	//フェードにかける時間
	public GameObject[] cameras;
	public GameObject[] canvases;


	public int selectedCamera = 0;
	//開始時間、開始値、終了時間、終了値
	public AnimationCurve animCurve;


	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Initialize ()
	{
		for (int i = 0; i < cameraAndCanvasNum; ++i) {
			if (i == selectedCamera) {
				cameras [i].GetComponent<Camera> ().enabled = true;
				cameras [i].GetComponent<Camera> ().depth = 0;
				cameras [i].GetComponent<Physics2DRaycaster> ().enabled = true;

				canvases [i].GetComponent<Canvas> ().sortingOrder = 1;
				canvases [i].GetComponent<CanvasGroup> ().interactable = true;
				canvases [i].GetComponent<CanvasGroup> ().alpha = 1.0f;
			} else {
				cameras [i].GetComponent<Camera> ().enabled = false;
				cameras [i].GetComponent<Camera> ().depth = -1;
				cameras [i].GetComponent<Physics2DRaycaster> ().enabled = false;

				canvases [i].GetComponent<CanvasGroup> ().interactable = false;
				canvases [i].GetComponent<CanvasGroup> ().alpha = 0.0f;
			}
		}

	}

	public void SwitchCamera (int id)
	{
		if (id == selectedCamera)
			return;

		StartCoroutine (FadeOut (selectedCamera));
		StartCoroutine (FadeIn (id));

	}

	private IEnumerator FadeOut (int id)
	{
		float startTime = Time.time;
		canvases [id].GetComponent<CanvasGroup> ().interactable = false;
		while (Time.time - startTime < duration) {
			canvases [id].GetComponent<CanvasGroup> ().alpha =
			1.0f - animCurve.Evaluate ((Time.time - startTime) / duration);
						
			yield return null;
		}
		canvases [id].GetComponent<CanvasGroup> ().alpha = 0.0f;
		cameras [id].GetComponent<Camera> ().enabled = false;
		cameras [id].GetComponent<Physics2DRaycaster> ().enabled = false;
		cameras [id].GetComponent<Camera> ().depth = -1;
		canvases [id].GetComponent<Canvas> ().sortingOrder = 0;
	}

	private IEnumerator FadeIn (int id)
	{
		float startTime = Time.time;

		cameras [id].GetComponent<Camera> ().enabled = true;
		while (Time.time - startTime < duration) {
			canvases [id].GetComponent<CanvasGroup> ().alpha =
				0.0f + animCurve.Evaluate ((Time.time - startTime) / duration);
			
			yield return null;
		}
		canvases [id].GetComponent<CanvasGroup> ().alpha = 1.0f;
		cameras [id].GetComponent<Camera> ().depth = 0;
		canvases [id].GetComponent<CanvasGroup> ().interactable = true;
		cameras [id].GetComponent<Physics2DRaycaster> ().enabled = true;
		canvases [id].GetComponent<Canvas> ().sortingOrder = 1;
		selectedCamera = id;
	}



}
