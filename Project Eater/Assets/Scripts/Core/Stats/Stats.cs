using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// Stats : Entity의 각종 Stat들을 관리해주는 Class
public class Stats : MonoBehaviour
{
    #region Variable
    [Header("STAT OWNER")]
    [Tooltip("LoadStats 메서드로 Stat 정보를 가져오기 위한 Key\nStat의 폴더명과 이름이 똑같아야 한다.\n폴더명과 같아야 하기 때문에 대소문자 구분이 필수")]
    [SerializeField]
    private string StatOwner;

    [Space(10f)]
    [SerializeField]
    private Stat fullnessStat; // 허기도 Stat (모든 Entity들이 공통으로 가지고 있음)
    [SerializeField]
    private Stat attackStat;
    [SerializeField]
    private Stat expStat;
    [SerializeField]
    private Stat defenceStat;
    [SerializeField]
    private Stat critRateStat;
    [SerializeField]
    private Stat critDamageStat;
    [SerializeField]
    private Stat moveSpeedStat;
    [SerializeField]
    private Stat abilityHasteStat;
    [SerializeField]
    private Stat rerollStat;
    [SerializeField]
    private Stat absorptionStat;

    [Space]
    [SerializeField]
    private StatOverride[] statOverrides;
    // → StatOverride에 넣은 Stat들이 Stats에  등록되게 된다.
    // → 다르게 말하면, StatOverride 배열에 넣지 않은 Stat들은 해당 Entity가 안 쓴다. 

    // 등록된 Stat들은 Stats 배열로 저장된다. 
    private Stat[] stats;

    public Entity Owner { get; private set; }
    // serialize 변수 Stat과 Property 변수 Stat은 다르다. 
    // → Serialize 변수 Stat들은 Stat Data의 원본이고, Property들은 Stats에 등록된 사본 Stat을 값으로 가진다.
    public Stat FullnessStat { get; private set; }
    public Stat ExpStat { get; private set; }
    public Stat DefenceStat { get; private set; }
    public Stat CritRateStat { get; private set; }
    public Stat CritDamageStat { get; private set; }
    public Stat MoveSpeedStat { get; private set; }
    public Stat AttackStat { get; private set; }
    public Stat AbilityHasteStat { get; private set; }
    public Stat ReRollStat { get; private set; }
    public Stat AbsorptionStat { get; private set; }
    #endregion

    #region Stat Set Up
    public void SetUp(Entity entity)
    {
        Owner = entity;

        // ※ Linq.Select : 데이터의 모든 항목을 추출할 것인지 특정 항목만 추출할 것인지 설정할 수 있는 문법 (여기서는 모든 항목 추출)
        // 1) statOverrides 배열에 각 statOverride 요소에 접근
        // 2) CreateStat 메서드 실행해서 Stat 사본을 만들기
        // 3) 배열 형태로 반환
        stats = statOverrides.Select(x => x.CreateStat()).ToArray();

        // → hpStat 변수가 null이 아니라면 GetStat 함수로 사본 HP Stat을 찾아와서 Property에 Set 해준다.
        FullnessStat = fullnessStat ? GetStat(fullnessStat) : null;
        ExpStat = expStat ? GetStat(expStat) : null;
        DefenceStat = defenceStat ? GetStat(defenceStat) : null;
        CritRateStat = critRateStat ? GetStat(critRateStat) : null;
        CritDamageStat = critDamageStat ? GetStat(critDamageStat) : null;
        MoveSpeedStat = moveSpeedStat ? GetStat(moveSpeedStat) : null;
        AttackStat = attackStat ? GetStat(attackStat) : null;
        AbilityHasteStat = abilityHasteStat ? GetStat(abilityHasteStat) : null;
        ReRollStat = rerollStat ? GetStat(rerollStat) : null;
        AbsorptionStat = absorptionStat ? GetStat(absorptionStat) : null;
    }
    #endregion

    #region Get Stat
    // ※ GetStat : 인자로 들어온 stat과 Stats에 저장된 stat의 ID를 비교해서 찾아오는 함수 
    public Stat GetStat(Stat stat)
    {
        // null check
        Debug.Assert(stat != null, $"Stats::GetStat - stat은 null이 될 수 없습니다.");

        // ※ Linq.FirstOrDefault : 주어진 조건을 만족하는 시퀀스에서 첫 번째 요소를 검색하는 LINQ의 메서드
        return stats.FirstOrDefault(x => x.ID == stat.ID);
    }

    // ※ TryGetStat : Stats이 존재한다면, outStat에 찾은 Stat을 넣어주고, 검색에 대한 성공 여부 반환하는 함수
    public bool TryGetStat(Stat stat, out Stat outStat)
    {
        // null check
        Debug.Assert(stat != null, $"Stats::TryGetStat - stat은 null이 될 수 없습니다.");

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
    // ※ OnGUI : Dubugging 용도로 Player가 가진 Stat들을 화면에 띄워주기 위함
    // ※ SkillSystemWindow
    // → EditorWindow를 상속받았기 때문에 Editor Window에서 OnGUI가 실행
    // ※ Stats
    // → MonoBehaviour를 상속받았기 때문에 Game 창에서 OnGUI가 실행
    /*private void OnGUI()
    {
        if (!Owner.IsPlayer)
            return;

        // 좌측 상단에 넓은 Box를 그려줌
        GUI.Box(new Rect(2f, 2f, 250f, 270f), string.Empty);

        // 박스 윗 부분에 Player Stat Text를 뜨워줌
        GUI.Label(new Rect(4f, 2f, 100f, 30f), "Player Stat");

        // 현재 그린 상황
        *//*
         _______________
        |  Player Stat  |
        |               |
        |_______________| 
        *//*

        // "Player Stat" Text 바로 아래쪽으로 Stat 정보를 그림
        // → 다른 수치들은 냅두고 y값만 계속 늘려주는 것으로 아래로 한 줄, 한 줄 그려준다. 
        var textRect = new Rect(4f, 22f, 200f, 30f); // 기준 Rect

        foreach (var stat in stats)
        {
            // Stat의 DefaultValue를 string으로 만들기 
            // → 만약 % Type이면 곱하기 100을 해서 0~100으로 출력
            // ※ 서식 지정 
            // → :0.##;-0.## format은 소숫점 2번째짜리까지 출력하되
            //    양수면 그대로 출력(왼쪽 서식 출력), 음수면 -를 붙여서 출력하라는 것(오른쪽 서식 출력)
            // → 밑에서 쓴 서식은 ToString의 기본 서식이기 때문에 굳이 사용할 필요가 없다.
            string defaultValueAsString = stat.IsPercentType ?
                $"{stat.DefaultValue * 100f:0.##;-0.##}%" :
                stat.DefaultValue.ToString("0.##;-0.##");

            string bonusValueAsString = stat.IsPercentType ?
                $"{stat.BonusValue * 100f:0.##;-0.##}%" :
                stat.BonusValue.ToString("0.##;-0.##");

            // 앞에서 만든 Value 문자열들을 그려준다. 
            GUI.Label(textRect, $"{stat.DisplayName} : {defaultValueAsString} ({bonusValueAsString})");

            // 다음 Stat 정보 출력을 위해 y축으로 한칸 내림
            textRect.y += 22f;
        }
    }*/
    #endregion

    public float GetValue(Stat stat)
        => GetStat(stat).Value;

    // Stats 컴포넌트가 삭제될 때, 실행
    private void OnDestroy()
    {
        foreach (var stat in stats)
            Destroy(stat);

        stats = null;
    }

    #region Load Stat
#if UNITY_EDITOR
    // Context Menu로 실행
    [ContextMenu("LoadStats")]
    private void LoadStats()
    {
        // Resources 폴더에 Stat 폴더에 있는 모든 Stat 가져오고 ID 오름차순으로 정렬하기 
        // → Database에서 Stat을 만들면 Resources -> Stat 폴더에 Stat이 저장됨
        //    즉, Database에서 만든 Stat을 가져오는 꼴이 된다. 
        var stats = Resources.LoadAll<Stat>($"Stat/{StatOwner}").OrderBy(x => x.ID);

        // 1) 가져온 Stat을 Select로 순회
        // 2) StatOverride 객체 생성 → 인자로 stat을 받는 생성자
        // 3) 생성한 StatOverride 객체들을 statOverrides 배열에 저장
        statOverrides = stats.Select(x => new StatOverride(x)).ToArray();
    }
#endif
    #endregion
}
