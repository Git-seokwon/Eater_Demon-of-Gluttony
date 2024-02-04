using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    protected Player player;

    protected virtual void Start()
    {
        player = GameManager.Instance.player;
    }

    protected virtual void Update()
    {

    }

    public virtual void SkillFunction()
    {
        // do some skill special things
    }

    // TODO : 엑티브, 패시브 둘 다 중복되는 기능 부모 클래스로 빼두기
}
