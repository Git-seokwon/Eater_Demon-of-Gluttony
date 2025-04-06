using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectActivator : TutorialBase
{
    [SerializeField] private GameObject targetObject; // 활성화할 오브젝트
    [SerializeField] private Animator animator; // 애니메이션을 실행할 애니메이터
    [SerializeField] private string firstAnimationState; // 첫 번째 애니메이션 상태 이름
    [SerializeField] private string nextAnimationState; // 전이될 다음 애니메이션 상태 이름
    [SerializeField] private SoundEffectSO createSlab; // 석판 생성 시 재생할 사운드 

    private bool isCompleted = false;

    public override void Enter()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true); // 오브젝트 활성화

            if (createSlab != null)
                SoundEffectManager.Instance.PlaySoundEffect(createSlab);
        }
    }

    public override void Execute(TutorialController controller)
    {
        if (!isCompleted && animator != null)
        {
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

            // 첫 번째 애니메이션에서 다른 애니메이션으로 전이되었는지 확인
            if (currentState.IsName(nextAnimationState))
            {
                isCompleted = true;
            }
        }

        if (isCompleted)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}
