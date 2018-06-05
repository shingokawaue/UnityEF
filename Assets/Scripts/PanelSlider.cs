using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelSlider : MonoBehaviour
{
	// ここでゲームオーバー処理とクリア処理
	// ---------------------------
	/// <summary>
	/// panelにアタッチするスクリプト
	/// </summary>
	/// 
	public bool isSliding = false;
	public AnimationCurve animCurve = AnimationCurve.Linear (0, 0, 1, 1);
	private Vector3 startPos;
	private Vector3 finishPos;
	private float duration;
	//何秒スライドし終わるか
 
	public void SetSlide (Vector3 to, float dur)
	{
		finishPos = to;
		duration = dur;
		isSliding = true;
		StartCoroutine (StartSlidePanel ());
	}



	private IEnumerator StartSlidePanel ()
	{//
		float startTime = Time.time;    // 開始時間
		startPos = transform.localPosition;  // 開始位置
		Vector3 moveDistance;            // 移動距離および方向

		moveDistance = (finishPos - startPos);

		while ((Time.time - startTime) < duration) {
			transform.localPosition = startPos + moveDistance * animCurve.Evaluate ((Time.time - startTime) / duration);
			yield return 0;        // 1フレーム後、再開
		}
		transform.localPosition = startPos + moveDistance;
		if (finishPos == new Vector3 (-3200f, 0.0f , 0.0f)) {//右端からダミーに移動した後の処理
			transform.localPosition = new Vector3 (0.0f, 0.0f , 0.0f);
		}

		isSliding = false;
	}

}