using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRoom : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // 0.75�� �������� 0.5�� ���� �ݺ� ����
        InvokeRepeating("EnableRoom", 0.5f, 0.75f);
    }

    private void EnableRoom()
    {
        HelperUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLB,
                                                  out Vector2Int mainCameraWorldPositionUB,
                                                  mainCamera);

        foreach (Room room in GameManager.Instance.roomArray)
        {
            if (true)
            {
                room.gameObject.SetActive(true);
                room.ActivateEnvironmentGameObject();
            }
            else
            {
                room.gameObject.SetActive(false);
                room.DeActivateEnvironmentGameObject();
            }
        }
    }
}
