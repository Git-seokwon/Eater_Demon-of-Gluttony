using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    #region ToolTip
    [Tooltip("Populate with the CursorTarget gameObject")]
    #endregion
    [SerializeField] private Transform cursorTarget;

    #region ToolTip
    [Tooltip("Set Weight and Radius of CinemachineTarget")]
    #endregion
    [SerializeField] private float playerWeight;
    [SerializeField] private float playerRadius;
    [SerializeField] private float cursorWeight;
    [SerializeField] private float cursorRadius;

    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    // 시네머신 카메라 타겟 그룹 세팅
    private void SetCinemachineTargetGroup()
    {
        // CinemachineTargetGroup.Target 구조체 (new로 초기화) - Player
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target
        {
            weight = playerWeight,
            radius = playerRadius,
            target = GameManager.Instance.player.transform
        };

        // CinemachineTargetGroup.Target 구조체 (new로 초기화) - Player
        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target
        {
            weight = cursorWeight,
            radius = cursorRadius,
            target = cursorTarget
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[]
        {
            cinemachineGroupTarget_player,
            cinemachineGroupTarget_cursor
        };

        // 위에 생성한 Target을 최종적으로 CinemachineTargetGroup에 넣기
        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    private void Update()
    {
        // 마우스 커서 위치 추적
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
