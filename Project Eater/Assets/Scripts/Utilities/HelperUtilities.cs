using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    // 크리티컬 적용 함수 
    public static float GetApplyCritDamage(float damage, float critRate, float critDamage = 0f)
    {
        int critRateInt = Mathf.FloorToInt(critRate * 100f);

        if (UnityEngine.Random.Range(0, 100) <= critRateInt)
            damage += damage * critDamage;

        return damage;
    }

    // 특정 오브젝트에서 자식 오브젝트를 이름으로 찾는 함수 
    // → recursive 여부를 통해 자식의 자식까지 찾아 줄지 정한다. 
    public static T FindChild<T>(GameObject go, string name = "", bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (!recursive)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = "", bool recursive = false)
    {
        var transform = FindChild<Transform>(go, name, recursive);

        if (transform != null)
            return transform.gameObject;
        else
            return null;
    }
}
