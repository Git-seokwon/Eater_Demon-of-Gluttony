using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatentSkill : MonoBehaviour
{
    [SerializeField] protected LatentSkillSO latentSkill;

    protected Player player;

    // �ع� ��ų Ư���� ������ bool�� ����, enum���� ���� ��� ��...


    protected virtual void Start()
    {
        player = GameManager.Instance.player;
    }

    // �ع� ��ų Ư�� ����
    public virtual void ApplyLatentSkillTrait()
    {

    }

    // ���� ��ų 
    public virtual void StartSkillFunc()
    {

    }

    // �ñ� ��ų ���
    public virtual void UltimateSkillFunc()
    {


    }

}
