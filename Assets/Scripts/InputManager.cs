using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// EventSystemへの操作などを行うクラス。
/// EventSystemへは直接操作せず、このクラスを通して行うようにする
/// </summary>
/// 
/// // Singletonパターン
/// sealed 修飾子をクラスに適用すると、それ以外のクラスが、そのクラスから継承できなくなります。 
public class InputManager : SingletonMonoBehaviour<InputManager>
{
	//=================================================================================
	//初期化
	//=================================================================================

	void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);
		//ここまでシングルトンパターン
	}


	GameObject eventSystem;


	private bool canSetEventSystemEnable = true;//入力を受け付けたくない時にfalseにする。
                                   //他のクラスでEventSystemを扱う時にこれを参照する

	private void Start()
	{
		eventSystem = GameObject.Find("EventSystem");
	}
 
	public bool CanSetEventSystemEnable
    {
		set { this.canSetEventSystemEnable = value; }
		get { return this.canSetEventSystemEnable; }
    }

	public void SetEventSystemEnableAndCanSet (bool b){
		CanSetEventSystemEnable = b;
		eventSystem.GetComponent<EventSystem>().enabled = b;
	}

	public void SetEventSystemEnable(bool b){
		if (canSetEventSystemEnable){
			eventSystem.GetComponent<EventSystem>().enabled = b;
		}
		else{
			return;
		}
	}

}