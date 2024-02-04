using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera mainCamera;

    // ���콺 ����Ʈ ���� ��ǥ �������� 
    // 1. ���콺�� Screen ��ǥ �������� 
    // 2. ���콺�� Screen ��ǥ�� World Space�� ��ǥ�� ��ȯ�Ͽ� ��������
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        // ��ũ�� ����� �°� ���콺 position ����
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }
}
