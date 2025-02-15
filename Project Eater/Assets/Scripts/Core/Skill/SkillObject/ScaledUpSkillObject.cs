using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledUpSkillObject : MonoBehaviour
{
    [SerializeField]
    private float maxScale;

    private float targetScale = 2f; // 목표 스케일

    // Skill의 소유주
    public Entity Owner { get; private set; }
    // 맞은 대상에게 적용할 Skill
    public Skill Skill { get; private set; }
    public float Speed { get; private set; }

    public void Setup(Entity owner, float speed, Skill skill)
    {
        Owner = owner;
        Speed = speed;

        Skill = skill;

        // 초기 크기를 0.01로 설정
        transform.localScale = new Vector2(0.01f, 0.01f);
    }

    private void OnDisable() => Clear();

    private void Update()
    {
        transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxScale, maxScale), Speed * Time.deltaTime);

        // 원의 지름이 목표 지름에 도달하면 비활성화
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
