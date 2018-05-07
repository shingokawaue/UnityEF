using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ItemGetButton : MonoBehaviour
{
	public void OnClicked ()
	{
		GetComponent<Image> ().enabled = false;
	}
}
