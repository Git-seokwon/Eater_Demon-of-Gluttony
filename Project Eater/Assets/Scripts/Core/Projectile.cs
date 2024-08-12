using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour // 투사체 관련 Class
{
    // 투사체가 Hit 했을 때, Spawn해줄 시각 효과 Prefab
    [SerializeField]
    private GameObject impactPrefab;

    // 투사체를 발사한 Entity 
    private Entity owner;
    // 투사체의 Rigidbody2D
    private new Rigidbody2D rigidbody2D;
    // 투사체 속도 
    private float speed;
    // 투사체를 맞은 대상에게 적용할 Skill
    private Skill skill;

    public void Setup(Entity owner, float speed, Vector2 direction, Skill skill)
    {
        this.owner = owner;
        this.speed = speed;

        // 투사체가 direction 방향을 보도록 한다. 
        transform.right = direction;

        // 현재 스킬의 Level 정보를 저장하기 위해 Clone을 보관
        this.skill = skill.Clone() as Skill;
    }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        Destroy(skill);
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Entity>() == owner)
            return;

        // 부딪힌 대상이 자기 자신(owner)이 아니라면 Impact를 만들어 투사체의 현재 위치에 Spawn 한다. 
        var impact = PoolManager.Instance.ReuseGameObject(impactPrefab, transform.position, Quaternion.identity);
        impact.transform.right = -transform.right;

        // 부딪힌 객체가 Entity라면 해당 객체의 SkillSystem에 투사체가 가진 Skill을 적용 
        // Ex) 스킬이 데미지를 주는 스킬이라면 부딪힌 Entity가 데미지를 입는다. 
        var entity = collision.GetComponent<Entity>();
        if (entity)
            entity.SkillSystem.Apply(skill);

        impact.SetActive(false);
    }
}
