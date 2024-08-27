using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour // ����ü ���� Class
{
    // ����ü�� Hit ���� ��, Spawn���� �ð� ȿ�� Prefab
    [SerializeField]
    private GameObject impactPrefab;

    // ����ü�� �߻��� Entity 
    private Entity owner;
    // ����ü�� Rigidbody2D
    private new Rigidbody2D rigidbody2D;
    // ����ü �ӵ� 
    private float speed;
    // ����ü�� ���� ��󿡰� ������ Skill
    private Skill skill;

    public void Setup(Entity owner, float speed, Vector2 direction, Skill skill)
    {
        this.owner = owner;
        this.speed = speed;

        // ����ü�� direction ������ ������ �Ѵ�. 
        transform.right = direction;

        // ���� ��ų�� Level ������ �����ϱ� ���� Clone�� ����
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

        // �ε��� ����� �ڱ� �ڽ�(owner)�� �ƴ϶�� Impact�� ����� ����ü�� ���� ��ġ�� Spawn �Ѵ�. 
        var impact = PoolManager.Instance.ReuseGameObject(impactPrefab, transform.position, Quaternion.identity);
        impact.transform.right = -transform.right;

        // �ε��� ��ü�� Entity��� �ش� ��ü�� SkillSystem�� ����ü�� ���� Skill�� ���� 
        // Ex) ��ų�� �������� �ִ� ��ų�̶�� �ε��� Entity�� �������� �Դ´�. 
        var entity = collision.GetComponent<Entity>();
        if (entity)
            entity.SkillSystem.Apply(skill);

        impact.SetActive(false);
    }
}
