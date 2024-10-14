using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour // ����ü ���� Class
{
    // ����ü�� Hit ���� ��, Spawn���� �ð� ȿ�� Prefab
    [SerializeField]
    private GameObject impactPrefab;
    [SerializeField]
    private bool isPenetration;

    // ����ü�� �߻��� Entity 
    private Entity owner;
    // ����ü�� Rigidbody2D
    private new Rigidbody2D rigidbody2D;
    // ����ü �ӵ� 
    private float speed;
    // ����ü�� ���� ��󿡰� ������ Skill
    private Skill skill;
    // ����ü ��Ÿ� 
    private float range;
    // ����ü �߻� ����
    private Vector2 fireDirectionVector; 

    public void Setup(Entity owner, float speed, Vector2 direction, float range, Skill skill)
    {
        this.owner = owner;
        this.speed = speed;
        this.range = range;
        fireDirectionVector = direction;

        // ����ü�� direction ������ ������ �Ѵ�. 
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

        // �ε��� ����� �ڱ� �ڽ�(owner)�� �ƴ϶�� Impact�� ����� ����ü�� ���� ��ġ�� Spawn �Ѵ�. 
        var impact = PoolManager.Instance.ReuseGameObject(impactPrefab, transform.position, Quaternion.identity);
        impact.transform.right = -transform.right;

        // �ε��� ��ü�� Entity��� �ش� ��ü�� SkillSystem�� ����ü�� ���� Skill�� ���� 
        // Ex) ��ų�� �������� �ִ� ��ų�̶�� �ε��� Entity�� �������� �Դ´�. 
        var entity = collision.GetComponent<Entity>();
        if (entity)
        {
            entity.SkillSystem.Apply(skill);
        }

        if (!isPenetration)
            gameObject.SetActive(false);
    }
}
