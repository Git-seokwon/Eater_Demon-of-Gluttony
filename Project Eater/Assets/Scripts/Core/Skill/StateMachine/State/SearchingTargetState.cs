using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Skill�� ����� �� ���� ������ �˻��� �ʿ��� ��쿡 ���̵Ǵ� State
// Ex) Skill Button�� ������ ��� Skill�� ���� �������� �����ϰ� Skill�� ���
public class SearchingTargetState : State<Skill>
{
    public override void Enter()
    {
        Debug.Log("SearchingTargetState ����");

        Entity.SelectTarget();
    }
    public override void Exit()
    {
        Debug.Log("SearchingTargetState ����");
        Entity.CancelSelectTarget();
    }
}
