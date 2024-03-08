using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LatentSkill : MonoBehaviour
{
    [SerializeField] protected LatentSkillSO latentSkill;

    protected Player player;

    // 해방 스킬 특성이 종류를 bool로 할지, enum으로 할지 고민 중...


    protected virtual void Start()
    {
        player = GameManager.Instance.player;
    }

    // 해방 스킬 특성 적용
    public virtual void ApplyLatentSkillTrait()
    {

    }

    // 시작 스킬 
    public virtual void StartSkillFunc()
    {

    }

    // 궁극 스킬 사용
    public virtual void UltimateSkillFunc()
    {


    }

}
