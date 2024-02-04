using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCursor : MonoBehaviour
{
    private void Awake()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        // 마우스 포인트 가져오기 
        transform.position = Input.mousePosition;   
    }
}
