using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DoubleClick : MonoBehaviour, IPointerClickHandler
{
	public void OnPointerClick (PointerEventData eventData)
	{
		if (eventData.clickCount > 1) {
			Debug.Log (eventData.clickCount);
		}
	}
}