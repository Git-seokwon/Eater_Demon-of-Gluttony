using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera mainCamera;

    // 마우스 포인트 월드 좌표 가져오기 
    // 1. 마우스의 Screen 좌표 가져오기 
    // 2. 마우스의 Screen 좌표를 World Space의 좌표로 변환하여 가져오기
    public static Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        // 스크린 사이즈에 맞게 마우스 position 조정
        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }

    // Get the camera viewport lower and upper bounds
    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLB, out Vector2Int cameraWorldPositionUB, Camera camera)
    {
        // x, y 좌표만 가져오기 
        Vector3 worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 worldPositionViewportTopRight   = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 1f));

        cameraWorldPositionLB = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUB = new Vector2Int((int)worldPositionViewportTopRight.x, (int)worldPositionViewportTopRight.y);
    }

    // Get the angle in degrees from a direction vector 
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }

    // Get AimDirection enum value from the pased in angleDegrees
    public static AimDirection GetAimDirection(float angleDegrees)
    {
        AimDirection aimDirection;

        // Set player direction 
        // Aim Right
        if (angleDegrees < 90f && angleDegrees >= -90f)
        {
            aimDirection = AimDirection.Right;
        }
        // Aim Left
        else
        {
            aimDirection = AimDirection.Left;
        }

        return aimDirection;
    }

    // 거리 순 정렬
    public static void QuickSortByDistance(DistanceCollider[] array, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(array, low, high);
            QuickSortByDistance(array, low, pivotIndex - 1);
            QuickSortByDistance(array, pivotIndex + 1, high);
        }
    }

    private static int Partition(DistanceCollider[] array, int low, int high)
    {
        float pivot = array[high].Distance;
        int i = low - 1;
        for (int j = low; j < high; j++)
        {
            if (array[j].Distance <= pivot)
            {
                i++;
                Swap(array, i, j);
            }
        }
        Swap(array, i+1, high);
        return i + 1;
    }

    private static void Swap(DistanceCollider[] array, int i, int j)
    {
        DistanceCollider temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}
