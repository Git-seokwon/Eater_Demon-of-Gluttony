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

    // TODO : ��Ƽ��, �нú� �� �� �ߺ��Ǵ� ��� �θ� Ŭ������ ���α�
}
