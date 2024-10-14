using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour // 투사체 관련 Class
{
    // 투사체가 Hit 했을 때, Spawn해줄 시각 효과 Prefab
    [SerializeField]
    private GameObject impactPrefab;
    [SerializeField]
    private bool isPenetration;

    // 투사체를 발사한 Entity 
    private Entity owner;
    // 투사체의 Rigidbody2D
    private new Rigidbody2D rigidbody2D;
    // 투사체 속도 
    private float speed;
    // 투사체를 맞은 대상에게 적용할 Skill
    private Skill skill;
    // 투사체 사거리 
    private float range;
    // 투사체 발사 방향
    private Vector2 fireDirectionVector; 

    public void Setup(Entity owner, float speed, Vector2 direction, float range, Skill skill)
    {
        this.owner = owner;
        this.speed = speed;
        this.range = range;
        fireDirectionVector = direction;

        // 투사체가 direction 방향을 보도록 한다. 
        transform.right = direction;

        this.skill = skill;
    }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        Destroy(skill);
    }

    private void Update()
    {
        Vector2 distanceVector = fireDirectionVector * speed * Time.deltaTime;

        range -= distanceVector.magnitude;
        if (range < 0f)
            gameObject.SetActive(false);
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
        {
            entity.SkillSystem.Apply(skill);
        }

        if (!isPenetration)
            gameObject.SetActive(false);
    }
}
