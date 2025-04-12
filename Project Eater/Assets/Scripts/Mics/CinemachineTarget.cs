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
        // 마우스 커서 위치 추적
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
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

        // CinemachineTargetGroup.Target 구조체 (new로 초기화) - Cursor
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
        // → 초기 설정에만 필요, 일일이 AddMember 하지 않고, 배열로 집어넣는 것임
        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    // 외부에서 호출할 코루틴 (던전 입구 연출용)
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

        // 1. Cursor 타겟 제거
        cinemachineTargetGroup.RemoveMember(cursorTarget);

        // 2. Focus 타겟 추가
        cinemachineTargetGroup.AddMember(focusTarget, focusWeight, focusRadius);

        // 3. 대기
        yield return new WaitForSeconds(duration);

        // 4. Focus 타겟 제거
        cinemachineTargetGroup.RemoveMember(focusTarget);

        // 5. Cursor 타겟 복원
        cinemachineTargetGroup.AddMember(cursorTarget, cursorWeight, cursorRadius);

        PlayerController.Instance.enabled = true;
        PlayerController.Instance.IsInterActive = false;
    }
}
