﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void PushStartButton ()
	{
		FadeManager.Instance.LoadScene ("GameScene", 0.3f);
		//SceneManager.LoadScene ("GameScene");
	}
}
