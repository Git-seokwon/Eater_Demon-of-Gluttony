using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledUpSkillObject : MonoBehaviour
{
    [SerializeField]
    private float maxSize;

    private float currentDuration;

    // Skill의 소유주
    public Entity Owner { get; private set; }
    // 맞은 대상에게 적용할 Skill
    public Skill Skill { get; private set; }

    public float Duration { get; private set; }
    public float Speed { get; private set; }

    // SkillObject가 Destroy되는 시간
    public float DestroyTime { get; private set; }

    public void Setup(Entity owner, float speed, float duration, Skill skill)
    {
        Owner = owner;
        Speed = speed;
        Duration = duration;

        Skill = skill;

        DestroyTime = Duration;
    }

    private void Update()
    {
        currentDuration += Time.deltaTime;

        transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), Speed * Time.deltaTime);

        if (currentDuration >= DestroyTime)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Entity>() == Owner)
            return;

        var entity = collision.GetComponent<Entity>();
        if (entity)
            entity.SkillSystem.Apply(Skill);
    }
}
