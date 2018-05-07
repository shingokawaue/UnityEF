using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMoveUICamera : MonoBehaviour {

    public void OnDrag()
    {
        Vector3 TapPos = Input.mousePosition;
        TapPos.z = 10f;
        GameObject UICamObj = GameObject.FindGameObjectWithTag("UICamera");
        Camera cam = UICamObj.GetComponent<Camera>();

        transform.position = cam.ScreenToWorldPoint(TapPos);


    }
}
