using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRoom : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // 0.75초 간격으로 0.5초 동안 반복 실행
        InvokeRepeating("EnableRoom", 0.5f, 0.75f);
    }

    private void EnableRoom()
    {
        HelperUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLB,
                                                  out Vector2Int mainCameraWorldPositionUB,
                                                  mainCamera);

        foreach (Room room in StageManager.Instance.Rooms)
        {
            if (true) // 조건 작성 해야 한다. 
            {
                room.gameObject.SetActive(true);
                room.ActivateEnvironmentGameObject();
            }
            {
                room.gameObject.SetActive(false);
                room.DeActivateEnvironmentGameObject();
            }
        }
    }
}
