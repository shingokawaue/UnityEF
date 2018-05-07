using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ButtonWithNumber : MonoBehaviour
{

	public int MAXNO = 9;
	public int MINNO = 0;
	public int number;
	public bool enabled = true;

	private GameObject text;
	//子

	// Use this for initialization
	void Start ()
	{
		number = MINNO;
		text = transform.Find ("Text").gameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void PointerDown ()
	{
		if (enabled == false)
			return;

		GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		++number;

		if (number > MAXNO) {
			number = MINNO;
		}
		text.GetComponent<Text> ().text = number.ToString ();
	}

	public void PointerUp ()
	{
		GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
	}

}
