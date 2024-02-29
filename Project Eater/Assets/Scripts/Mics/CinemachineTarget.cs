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

    // �ó׸ӽ� ī�޶� Ÿ�� �׷� ����
    private void SetCinemachineTargetGroup()
    {
        // CinemachineTargetGroup.Target ����ü (new�� �ʱ�ȭ) - Player
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target
        {
            weight = playerWeight,
            radius = playerRadius,
            target = GameManager.Instance.player.transform
        };

        // CinemachineTargetGroup.Target ����ü (new�� �ʱ�ȭ) - Player
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

        // ���� ������ Target�� ���������� CinemachineTargetGroup�� �ֱ�
        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    private void Update()
    {
        // ���콺 Ŀ�� ��ġ ����
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
