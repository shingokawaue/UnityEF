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
	GameObject inputManager;

	public int cameraAndCanvasNum = 2;
	public float duration = 0.3f;
	//フェードにかける時間
	public GameObject[] cameras;
	public GameObject[] canvases;


	public int selectedCamera = 0;
	//開始時間、開始値、終了時間、終了値
	public AnimationCurve animCurve;


	// Use this for initialization
	void Start()
	{
		inputManager = GameObject.FindWithTag("InputManager");
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Initialize()
	{
		for (int i = 0; i < cameraAndCanvasNum; ++i)
		{
			if (i == selectedCamera)
			{
				cameras[i].GetComponent<Camera>().enabled = true;
				//cameras[i].SetActive(true);

				cameras[i].GetComponent<Camera>().depth = 0;
				if (cameras[i].GetComponent<Physics2DRaycaster>() != null)
				{
					cameras[i].GetComponent<Physics2DRaycaster>().enabled = true;
				}
				canvases[i].GetComponent<Canvas>().sortingOrder = 1;
				canvases[i].GetComponent<CanvasGroup>().interactable = true;
				canvases[i].GetComponent<CanvasGroup>().alpha = 1.0f;
			}
			else
			{
				canvases[i].GetComponent<CanvasGroup>().interactable = false;
				canvases[i].GetComponent<CanvasGroup>().alpha = 0.0f;
				cameras[i].GetComponent<Camera>().enabled = false;

				cameras[i].GetComponent<Camera>().depth = -1;
				if (cameras[i].GetComponent<Physics2DRaycaster>() != null)
				{
					cameras[i].GetComponent<Physics2DRaycaster>().enabled = false;
				}
				//cameras[i].SetActive(false);


			}
		}

	}

	public void SwitchCamera(int id)
	{
		if (id == selectedCamera)
			return;

		StartCoroutine(FadeOut(selectedCamera));
		StartCoroutine(FadeIn(id));

	}

	private IEnumerator FadeOut(int id)
	{
		
		inputManager.GetComponent<InputManager>().SetEventSystemEnable(false);
		float startTime = Time.time;
		canvases[id].GetComponent<CanvasGroup>().interactable = false;
		canvases[id].GetComponent<CanvasGroup>().blocksRaycasts = false;

		while (Time.time - startTime < duration)
		{
			canvases[id].GetComponent<CanvasGroup>().alpha =
			1.0f - animCurve.Evaluate((Time.time - startTime) / duration);

			yield return null;
		}
		canvases[id].GetComponent<CanvasGroup>().alpha = 0.0f;
		canvases[id].GetComponent<Canvas>().sortingOrder = 0;
		cameras[id].GetComponent<Camera>().enabled = false;
		if (cameras[id].GetComponent<Physics2DRaycaster>() != null)
		{
			cameras[id].GetComponent<Physics2DRaycaster>().enabled = false;
		}
		cameras[id].GetComponent<Camera>().depth = -1;
        
		//cameras[id].SetActive(false);
		//canvases[id].SetActive(false);
  
		inputManager.GetComponent<InputManager>().SetEventSystemEnable(true);
	}
	private IEnumerator FadeIn (int id)
	{
		float startTime = Time.time;

		//cameras[id].SetActive(true);
		//canvases[id].SetActive(true);
		cameras [id].GetComponent<Camera> ().enabled = true;
		while (Time.time - startTime < duration) {
			canvases [id].GetComponent<CanvasGroup> ().alpha =
				0.0f + animCurve.Evaluate ((Time.time - startTime) / duration);
			
			yield return null;
		}

		cameras [id].GetComponent<Camera> ().depth = 0;
		canvases[id].GetComponent<CanvasGroup>().interactable = true;
		canvases[id].GetComponent<CanvasGroup>().blocksRaycasts = true;
		if (cameras[id].GetComponent<Physics2DRaycaster>() != null)
		{
			cameras[id].GetComponent<Physics2DRaycaster>().enabled = true;
		}
		canvases [id].GetComponent<Canvas> ().sortingOrder = 1;
		canvases[id].GetComponent<CanvasGroup>().alpha = 1.0f;
		selectedCamera = id;
	}



}
