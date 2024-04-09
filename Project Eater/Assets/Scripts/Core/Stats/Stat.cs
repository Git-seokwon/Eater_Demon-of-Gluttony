using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Stat : IdentifiedObject
{
    #region Event delegate
    // ※ ValueChangedHandler : Value Change 이벤트
    // ※ stat : Event를 호출한 Stat
    // ※ currentValue : currentValue
    // ※ preValue : prevValue
    public delegate void ValueChangedHandler(Stat stat, float currentValue, float prevValue);
    #endregion

    #region Variable
    [SerializeField]
    // % type인가? (ex, 1 => 100%, 0 => 0%)
    // ex) 치명타 확률, 공격 명중률
    private bool isPercentType;
    [SerializeField]
    private float maxValue;
    [SerializeField]
    private float minValue;
    [SerializeField]
    private float defaultValue;

    // 기본 stat 외의 bonus stat을 저장하는 dictionary
    // ex) 장비를 입거나 물약을 마시면 Stat이 올라감
    // ※ key 값 : bonus stat을 준 대상
    // ex) 장비가 bonus Stat을 주었다면 그 장비가 key값이 됨
    // ※ value 값을 Dictionary로 한 이유 : Sub Key를 등록하기 위함 
    // ※ Sub Key : mainKey가 bonus stat을 여러번 줄 때 각 bonus 값을 구분하기 위한 용도
    // ex) Skill이 3 Stack일 때, 힘 5를 Bonus로 주고, 5 Stack일 때 추가로 힘 10을 Bonus로 준다
    //     이때, 어떤 Stack이 Bonus를 줬는지 각 Bonus 값을 구분하기 위한 용도 
    // → Sub Key는 상황에 따라서 있을 수도 있고, 없을 수도 있다.
    //    subKey가 필요없을 경우 Sub Key에 string.empty로 해서 Value를 넣음
    private Dictionary<object, Dictionary<object, float>> bonusValuesByKey = new();

    public bool IsPercentType => isPercentType;

    public float MaxValue
    {
        get => maxValue;
        set => maxValue = value; // 입력받은 value로 설정
    }

    public float MinValue
    {
        get => minValue;
        set => minValue = value;
    }

    public float DefaultValue
    {
        get => defaultValue;
        set
        {
            // 값 변경 전 이전 값 저장
            float prevValue = Value;
            defaultValue = Mathf.Clamp(value, minValue, maxValue);

            TryInvokeValueChangedEvent(Value, prevValue);
        }
    }

    // bonusValuesByKey에 저장된 bonus value의 합
    public float BonusValue { get; private set; }

    // 현재 총 value
    public float Value => Mathf.Clamp(defaultValue + BonusValue, minValue, maxValue);
    #endregion

    #region EVENT
    // 앞에서 만든 delegate로 Event들을 선언 
    public event ValueChangedHandler onValueChanged; // value 값이 바뀌었을 때, 호출
    public event ValueChangedHandler onValueMax;     // 값이 최대가 되었을 때, 호출
    public event ValueChangedHandler onValueMin;     // 값이 최소가 되었을 때, 호출
    #endregion

    // ※ TryInvokeValueChangedEvent : 현재 값과 이전 값이 다르다면(value가 변했을 시) 값이 바뀌었다고 event로 알림
    // → 현재 값이 Max 값이라면 onValueMax 이벤트를 현재 값이 최소값이라면 onValueMin 이벤트 호출
    private void TryInvokeValueChangedEvent(float currnetValue, float prevValue)
    {
        // ※ Approximately : 근사 
        if (!Mathf.Approximately(currnetValue, prevValue))
        {
            onValueChanged?.Invoke(this, currnetValue, prevValue);

            if (Mathf.Approximately(currnetValue, MaxValue))
                onValueMax?.Invoke(this, currnetValue, prevValue);
            else if (Mathf.Approximately(currnetValue, prevValue))
                onValueMin?.Invoke(this, currnetValue, prevValue);
        }
    }

    #region SetBonusValue
    public void SetBonusValue(object key, object subKey, float value)
    {
        // mainKey 존재하는지 확인
        // → 없다면 Main Key로 Dictionary 생성
        if (!bonusValuesByKey.ContainsKey(key))
            bonusValuesByKey[key] = new Dictionary<object, float>();
        // → 있다면 Bonus Property에 더해져있는 현재 Bonus값을 빼줌
        else
            BonusValue -= bonusValuesByKey[key][subKey];
        // 기존의 BonusValue 값이 수정된 값으로 BonusValue에 더해지기 위해 기존 값은 지운다. 
        // ex) BonusValue에 20이라는 값이 저장되어 있다고 하자. (bonusValuesByKey[key][subKey] 값은 5라고 하자)
        //     그리고 bonusValuesByKey[key][subKey]을 10으로 변경했다고 하자.
        //     그러면 총 BonusValue값은 25가 되어야 한다. 하지만, 아래 기존 값을 지우는 과정을 포함하지 않는다면,
        //     BonusValue의 값은 30이 된다. 이는 잘못된 수치이며 이를 방지하기 위해 수정되기 전 값은 미리 지워준다.

        float prevValue = Value;
        bonusValuesByKey[key][subKey] = value;
        BonusValue += value;

        TryInvokeValueChangedEvent(Value, prevValue);
    }

    // SetBonusValue의 SubKey가 없는 오버로딩 함수
    public void SetBonusValue(object key, float value)
        => SetBonusValue(key, string.Empty, value);
    #endregion

    #region GetBonusValue
    public float GetBonusValue(object key)
        // MainKey에 할당 된 Value(Dictionary)를 bonusValuesByKey 변수로 가져온다.
        => bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubkey)
        // MainKey가 존재한다면, 해당 MainKey의 Dictionary가 가진 value들의 합을 return 
        // 없으면 0을 return 
        ? bonusValuesBySubkey.Sum(x => x.Value) : 0f;

    // 특정 SubKey의 BonusValue를 가져오는 GetBonusValue 오버로딩 함수
    public float GetBonusValue(object key, object subKey)
    {
        if (bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubkey))
        {
            if (bonusValuesBySubkey.TryGetValue(subKey, out var value))
                return value;
        }

        return 0f;
    }
    #endregion

    #region RemoveValue
    // MainKey로 BonusValue를 지우는 함수 
    public bool RemoveBonusValue(object key)
    {
        if (bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubkey))
        {
            // 현재 총 값을 임시 저장 
            float prevValue = Value;
            // Dictionary에 저장된 BonusValue의 합을 BonusValue Property에서 빼주기
            BonusValue -= bonusValuesBySubkey.Values.Sum();
            bonusValuesByKey.Remove(key);

            TryInvokeValueChangedEvent(Value, prevValue);
            return true;
        }
        return false;
    }

    // SubKey로 value를 지우는 RemoveBonusValue의 오버로딩
    public bool RemoveBonusValue(object key, object subKey)
    {
        if (bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubkey))
        {
            // 찾아온 Dictionary에서 Remove 함수를 사용해서 SubKey가 지워졌다면 
            if (bonusValuesBySubkey.Remove(subKey, out var value))
            {
                var prevValue = Value;
                BonusValue -= value;
                TryInvokeValueChangedEvent(Value, prevValue);
                return true;
            }
        }

        return false;
    }
    #endregion

    #region ContainKey
    // Dictionary에 Key가 존재하는지 확인하는 함수들 
    public bool ContainBonusValue(object key)
    => bonusValuesByKey.ContainsKey(key);

    public bool ContainBonusValue(object key, object subKey)
        => bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubkey) ? bonusValuesBySubkey.ContainsKey(subKey) : false;
    #endregion
}
