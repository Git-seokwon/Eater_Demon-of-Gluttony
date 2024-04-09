using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// Stats : Entity�� ���� Stat���� �������ִ� Class
[RequireComponent(typeof(Entity))]
public class Stats : MonoBehaviour
{
    #region Variable
    [SerializeField]
    private Stat hungerStat; // ��⵵ Stat (��� Entity���� �������� ������ ����)

    [Space]
    [SerializeField]
    private StatOverride[] statOverrides;
    // �� StatOverride�� ���� Stat���� Stats�� ��ϵǰ� �ȴ�.
    // �� �ٸ��� ���ϸ�, StatOverride �迭�� ���� ���� Stat���� �ش� Entity�� �� ����. 

    // ��ϵ� Stat���� Stats �迭�� ����ȴ�. 
    private Stat[] stats;

    public Entity Owner { get; private set; }
    public Stat HungerStat { get; private set; }
    // �� �߰� Tip!
    // �� Serialize ���� hungerStat�� Property HungerStat�� �ٸ� ���̴�. 
    // �� Serialize ���� stat���� Stat Data�� �����̰�, Property���� Stats�� ��ϵ� �纻 Stat�� ������ ������.
    #endregion

    #region Stat Set Up
    public void SetUp(Entity entity)
    {
        Owner = entity;

        // �� Linq.Select : �������� ��� �׸��� ������ ������ Ư�� �׸� ������ ������ ������ �� �ִ� ���� (���⼭�� ��� �׸� ����)
        // 1) statOverrides �迭�� �� statOverride ��ҿ� ����
        // 2) CreateStat �޼��� �����ؼ� Stat �纻�� �����
        // 3) �迭 ���·� ��ȯ
        stats = statOverrides.Select(x => x.CreateStat()).ToArray();

        // �� hpStat ������ null�� �ƴ϶�� GetStat �Լ��� �纻 HP Stat�� ã�ƿͼ� Property�� Set ���ش�.
        HungerStat = hungerStat ? GetStat(hungerStat) : null;
    }
    #endregion

    #region Get Stat
    // �� GetStat : ���ڷ� ���� stat�� Stats�� ����� stat�� ID�� ���ؼ� ã�ƿ��� �Լ� 
    private Stat GetStat(Stat stat)
    {
        // null check
        Debug.Assert(stat != null, $"Stats::GetStat - stat�� null�� �� �� �����ϴ�.");

        // �� Linq.FirstOrDefault : �־��� ������ �����ϴ� ���������� ù ��° ��Ҹ� �˻��ϴ� LINQ�� �޼���
        return stats.FirstOrDefault(x => x.ID == stat.ID);
    }

    // �� TryGetStat : Stats�� �����Ѵٸ�, outStat�� ã�� Stat�� �־��ְ�, �˻��� ���� ���� ���� ��ȯ�ϴ� �Լ�
    public bool TryGetStat(Stat stat, out Stat outStat)
    {
        // null check
        Debug.Assert(stat != null, $"Stats::TryGetStat - stat�� null�� �� �� �����ϴ�.");

        outStat = stats.FirstOrDefault(x => x.ID == stat.ID);
        return outStat != null;
    }

    #endregion

    #region Set & Get Stats Value
    public void SetDefaultValue(Stat stat, float value) => GetStat(stat).DefaultValue = value;

    public float GetDefaultValue(Stat stat) => GetStat(stat).DefaultValue;

    public void IncreaseDefaultValue(Stat stat, float value) => GetStat(stat).DefaultValue += value;

    public void SetBonusValue(Stat stat, object key, float value) => GetStat(stat).SetBonusValue(key, value);

    public void SetBonusValue(Stat stat, object key, object subKey, float value) => GetStat(stat).SetBonusValue(key, subKey, value);

    public float GetBonusValue(Stat stat) => GetStat(stat).BonusValue;

    public float GetBonusValue(Stat stat, object key) => GetStat(stat).GetBonusValue(key);

    public float GetBonusValue(Stat stat, object key, object subKey) => GetStat(stat).GetBonusValue(key, subKey);

    public void RemoveBonusValue(Stat stat, object key) => GetStat(stat).RemoveBonusValue(key);

    public void RemoveBonusValue(Stat stat, object key, object subKey) => GetStat(stat).RemoveBonusValue(key, subKey);

    public void ContainBonusValue(Stat stat, object key) => GetStat(stat).ContainBonusValue(key);

    public void ContainBonusValue(Stat stat, object key, object subKey) => GetStat(stat).ContainBonusValue(key, subKey);
    #endregion

    #region OnGUI
    // �� OnGUI : Dubugging �뵵�� Player�� ���� Stat���� ȭ�鿡 ����ֱ� ����
    // �� SkillSystemWindow
    // �� EditorWindow�� ��ӹ޾ұ� ������ Editor Window���� OnGUI�� ����
    // �� Stats
    // �� MonoBehaviour�� ��ӹ޾ұ� ������ Game â���� OnGUI�� ����
    private void OnGUI()
    {
        if (!Owner.IsPlayer)
            return;

        // ���� ��ܿ� ���� Box�� �׷���
        GUI.Box(new Rect(2f, 2f, 250f, 250f), string.Empty);

        // �ڽ� �� �κп� Player Stat Text�� �߿���
        GUI.Label(new Rect(4f, 2f, 100f, 30f), "Player Stat");

        /* ���� �׸� ��Ȳ
         _______________
        |  Player Stat  |
        |               |
        |_______________| 

        */

        // "Player Stat" Text �ٷ� �Ʒ������� Stat ������ �׸�
        // �� �ٸ� ��ġ���� ���ΰ� y���� ��� �÷��ִ� ������ �Ʒ��� �� ��, �� �� �׷��ش�. 
        var textRect = new Rect(4f, 22f, 200f, 30f); // ���� Rect

        // Stat ������ ���� + Button�� ���� ��ġ
        // �� textRect�� �׷��� �ٷ� �����ʿ� �׷��� 
        var plusButtonRect = new Rect(textRect.x + textRect.width, textRect.y, 20f, 20f);

        // Stat ���Ҹ� ���� - Button�� ���� ��ġ
        // �� x������ �� �о + Button �����ʿ� ��ġ�ϵ��� �Ѵ�. 
        var minusButtonRect = plusButtonRect;
        minusButtonRect.x += 22f;

        foreach (var stat in stats)
        {
            // Stat�� DefaultValue�� string���� ����� 
            // �� ���� % Type�̸� ���ϱ� 100�� �ؼ� 0~100���� ���
            // �� ���� ���� 
            // �� :0.##;-0.## format�� �Ҽ��� 2��°¥������ ����ϵ�
            //    ����� �״�� ���(���� ���� ���), ������ -�� �ٿ��� ����϶�� ��(������ ���� ���)
            // �� �ؿ��� �� ������ ToString�� �⺻ �����̱� ������ ���� ����� �ʿ䰡 ����.
            string defaultValueAsString = stat.IsPercentType ?
                $"{stat.DefaultValue * 100f:0.##;-0.##}%" :
                stat.DefaultValue.ToString();

            string bonusValueAsString = stat.IsPercentType ?
                $"{stat.BonusValue * 100f:0.##;-0.##}%" :
                stat.DefaultValue.ToString();

            // �տ��� ���� Value ���ڿ����� �׷��ش�. 
            GUI.Label(textRect, $"{stat.DisplayName} : {defaultValueAsString} ({bonusValueAsString})");

            // Stat ������ ���� Button���� �׷��ֱ�  
            if (GUI.Button(plusButtonRect, "+")) // + Button�� ������ Stat ����
            {
                if (stat.IsPercentType)
                    stat.DefaultValue += 0.01f;
                else
                    stat.DefaultValue += 1f;
            }

            if (GUI.Button(minusButtonRect, "-")) // - Button�� ������ Stat ����
            {
                if (stat.IsPercentType)
                    stat.DefaultValue -= 0.01f;
                else
                    stat.DefaultValue -= 1f;
            }

            // ���� Stat ���� ����� ���� y������ ��ĭ ����
            textRect.y += 22f;
            plusButtonRect.y = minusButtonRect.y = textRect.y;
        }
    }
    #endregion

    public float GetValue(Stat stat)
        => GetStat(stat).Value;

    // Stats ������Ʈ�� ������ ��, ����
    private void OnDestroy()
    {
        foreach (var stat in stats)
            Destroy(stat);

        stats = null;
    }

    #region Load Stat
#if UNITY_EDITOR
    // Context Menu�� ����
    [ContextMenu("LoadStats")]

    private void LoadStats()
    {
        // Resources ������ Stat ������ �ִ� ��� Stat �������� ID ������������ �����ϱ� 
        // �� Database���� Stat�� ����� Resources -> Stat ������ Stat�� �����
        //    ��, Database���� ���� Stat�� �������� ���� �ȴ�. 
        var stats = Resources.LoadAll<Stat>("Stat").OrderBy(x => x.ID);

        // 1) ������ Stat�� Select�� ��ȸ
        // 2) StatOverride ��ü ���� �� ���ڷ� stat�� �޴� ������
        // 3) ������ StatOverride ��ü���� statOverrides �迭�� ����
        statOverrides = stats.Select(x => new StatOverride(x)).ToArray();
    }
#endif
    #endregion
}
