using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectActivator : TutorialBase
{
    [SerializeField] private GameObject targetObject; // Ȱ��ȭ�� ������Ʈ
    [SerializeField] private Animator animator; // �ִϸ��̼��� ������ �ִϸ�����
    [SerializeField] private string firstAnimationState; // ù ��° �ִϸ��̼� ���� �̸�
    [SerializeField] private string nextAnimationState; // ���̵� ���� �ִϸ��̼� ���� �̸�

    private bool isCompleted = false;

    public override void Enter()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true); // ������Ʈ Ȱ��ȭ
        }
    }

    public override void Execute(TutorialController controller)
    {
        if (!isCompleted && animator != null)
        {
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

            // ù ��° �ִϸ��̼ǿ��� �ٸ� �ִϸ��̼����� ���̵Ǿ����� Ȯ��
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
