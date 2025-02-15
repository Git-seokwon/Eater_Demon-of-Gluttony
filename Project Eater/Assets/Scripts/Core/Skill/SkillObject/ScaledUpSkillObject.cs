using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledUpSkillObject : MonoBehaviour
{
    [SerializeField]
    private float maxScale;

    private float targetScale = 2f; // ��ǥ ������

    // Skill�� ������
    public Entity Owner { get; private set; }
    // ���� ��󿡰� ������ Skill
    public Skill Skill { get; private set; }
    public float Speed { get; private set; }

    public void Setup(Entity owner, float speed, Skill skill)
    {
        Owner = owner;
        Speed = speed;

        Skill = skill;

        // �ʱ� ũ�⸦ 0.01�� ����
        transform.localScale = new Vector2(0.01f, 0.01f);
    }

    private void OnDisable() => Clear();

    private void Update()
    {
        transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxScale, maxScale), Speed * Time.deltaTime);

        // ���� ������ ��ǥ ������ �����ϸ� ��Ȱ��ȭ
        if (transform.localScale.x >= targetScale)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Entity>() == Owner)
            return;

        var entity = collision.GetComponent<Entity>();
        if (entity)
            entity.SkillSystem.Apply(Skill);
    }

    private void Clear()
    {
        Owner = null;
        Skill = null;
    }
}
