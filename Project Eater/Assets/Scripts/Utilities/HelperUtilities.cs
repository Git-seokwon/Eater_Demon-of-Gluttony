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

    // Get the camera viewport lower and upper bounds
    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLB, out Vector2Int cameraWorldPositionUB, Camera camera)
    {
        // x, y ��ǥ�� �������� 
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopRight   = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 1f));

        cameraWorldPositionLB = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUB = new Vector2Int((int)worldPositionViewportTopRight.x, (int)worldPositionViewportTopRight.y);
    }
}
