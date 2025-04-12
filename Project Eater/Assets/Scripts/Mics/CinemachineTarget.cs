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

    private void Update()
    {
        // ���콺 Ŀ�� ��ġ ����
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
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

        // CinemachineTargetGroup.Target ����ü (new�� �ʱ�ȭ) - Cursor
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
        // �� �ʱ� �������� �ʿ�, ������ AddMember ���� �ʰ�, �迭�� ����ִ� ����
        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    // �ܺο��� ȣ���� �ڷ�ƾ (���� �Ա� �����)
    public void StartFocusSequence(Transform focusTarget, float duration, float focusWeight = 3f, float focusRadius = 1f)
    {
        StartCoroutine(FocusSequenceCoroutine(focusTarget, duration, focusWeight, focusRadius));
    }

    private IEnumerator FocusSequenceCoroutine(Transform focusTarget, float duration, float focusWeight, float focusRadius)
    {
        this.enabled = false;

        yield return new WaitUntil(() => DialogManager.Instance.UpdateDialog(1, DialogCharacter.EVENTS));
        DialogManager.Instance.DeActivate();

        this.enabled = true;

        // 1. Cursor Ÿ�� ����
        cinemachineTargetGroup.RemoveMember(cursorTarget);

        // 2. Focus Ÿ�� �߰�
        cinemachineTargetGroup.AddMember(focusTarget, focusWeight, focusRadius);

        // 3. ���
        yield return new WaitForSeconds(duration);

        // 4. Focus Ÿ�� ����
        cinemachineTargetGroup.RemoveMember(focusTarget);

        // 5. Cursor Ÿ�� ����
        cinemachineTargetGroup.AddMember(cursorTarget, cursorWeight, cursorRadius);

        PlayerController.Instance.enabled = true;
        PlayerController.Instance.IsInterActive = false;
    }
}
