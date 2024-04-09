using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Stat : IdentifiedObject
{
    #region Event delegate
    // �� ValueChangedHandler : Value Change �̺�Ʈ
    // �� stat : Event�� ȣ���� Stat
    // �� currentValue : currentValue
    // �� preValue : prevValue
    public delegate void ValueChangedHandler(Stat stat, float currentValue, float prevValue);
    #endregion

    #region Variable
    [SerializeField]
    // % type�ΰ�? (ex, 1 => 100%, 0 => 0%)
    // ex) ġ��Ÿ Ȯ��, ���� ���߷�
    private bool isPercentType;
    [SerializeField]
    private float maxValue;
    [SerializeField]
    private float minValue;
    [SerializeField]
    private float defaultValue;

    // �⺻ stat ���� bonus stat�� �����ϴ� dictionary
    // ex) ��� �԰ų� ������ ���ø� Stat�� �ö�
    // �� key �� : bonus stat�� �� ���
    // ex) ��� bonus Stat�� �־��ٸ� �� ��� key���� ��
    // �� value ���� Dictionary�� �� ���� : Sub Key�� ����ϱ� ���� 
    // �� Sub Key : mainKey�� bonus stat�� ������ �� �� �� bonus ���� �����ϱ� ���� �뵵
    // ex) Skill�� 3 Stack�� ��, �� 5�� Bonus�� �ְ�, 5 Stack�� �� �߰��� �� 10�� Bonus�� �ش�
    //     �̶�, � Stack�� Bonus�� ����� �� Bonus ���� �����ϱ� ���� �뵵 
    // �� Sub Key�� ��Ȳ�� ���� ���� ���� �ְ�, ���� ���� �ִ�.
    //    subKey�� �ʿ���� ��� Sub Key�� string.empty�� �ؼ� Value�� ����
    private Dictionary<object, Dictionary<object, float>> bonusValuesByKey = new();

    public bool IsPercentType => isPercentType;

    public float MaxValue
    {
        get => maxValue;
        set => maxValue = value; // �Է¹��� value�� ����
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
            // �� ���� �� ���� �� ����
            float prevValue = Value;
            defaultValue = Mathf.Clamp(value, minValue, maxValue);

            TryInvokeValueChangedEvent(Value, prevValue);
        }
    }

    // bonusValuesByKey�� ����� bonus value�� ��
    public float BonusValue { get; private set; }

    // ���� �� value
    public float Value => Mathf.Clamp(defaultValue + BonusValue, minValue, maxValue);
    #endregion

    #region EVENT
    // �տ��� ���� delegate�� Event���� ���� 
    public event ValueChangedHandler onValueChanged; // value ���� �ٲ���� ��, ȣ��
    public event ValueChangedHandler onValueMax;     // ���� �ִ밡 �Ǿ��� ��, ȣ��
    public event ValueChangedHandler onValueMin;     // ���� �ּҰ� �Ǿ��� ��, ȣ��
    #endregion

    // �� TryInvokeValueChangedEvent : ���� ���� ���� ���� �ٸ��ٸ�(value�� ������ ��) ���� �ٲ���ٰ� event�� �˸�
    // �� ���� ���� Max ���̶�� onValueMax �̺�Ʈ�� ���� ���� �ּҰ��̶�� onValueMin �̺�Ʈ ȣ��
    private void TryInvokeValueChangedEvent(float currnetValue, float prevValue)
    {
        // �� Approximately : �ٻ� 
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
        // mainKey �����ϴ��� Ȯ��
        // �� ���ٸ� Main Key�� Dictionary ����
        if (!bonusValuesByKey.ContainsKey(key))
            bonusValuesByKey[key] = new Dictionary<object, float>();
        // �� �ִٸ� Bonus Property�� �������ִ� ���� Bonus���� ����
        else
            BonusValue -= bonusValuesByKey[key][subKey];
        // ������ BonusValue ���� ������ ������ BonusValue�� �������� ���� ���� ���� �����. 
        // ex) BonusValue�� 20�̶�� ���� ����Ǿ� �ִٰ� ����. (bonusValuesByKey[key][subKey] ���� 5��� ����)
        //     �׸��� bonusValuesByKey[key][subKey]�� 10���� �����ߴٰ� ����.
        //     �׷��� �� BonusValue���� 25�� �Ǿ�� �Ѵ�. ������, �Ʒ� ���� ���� ����� ������ �������� �ʴ´ٸ�,
        //     BonusValue�� ���� 30�� �ȴ�. �̴� �߸��� ��ġ�̸� �̸� �����ϱ� ���� �����Ǳ� �� ���� �̸� �����ش�.

        float prevValue = Value;
        bonusValuesByKey[key][subKey] = value;
        BonusValue += value;

        TryInvokeValueChangedEvent(Value, prevValue);
    }

    // SetBonusValue�� SubKey�� ���� �����ε� �Լ�
    public void SetBonusValue(object key, float value)
        => SetBonusValue(key, string.Empty, value);
    #endregion

    #region GetBonusValue
    public float GetBonusValue(object key)
        // MainKey�� �Ҵ� �� Value(Dictionary)�� bonusValuesByKey ������ �����´�.
        => bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubkey)
        // MainKey�� �����Ѵٸ�, �ش� MainKey�� Dictionary�� ���� value���� ���� return 
        // ������ 0�� return 
        ? bonusValuesBySubkey.Sum(x => x.Value) : 0f;

    // Ư�� SubKey�� BonusValue�� �������� GetBonusValue �����ε� �Լ�
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
    // MainKey�� BonusValue�� ����� �Լ� 
    public bool RemoveBonusValue(object key)
    {
        if (bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubkey))
        {
            // ���� �� ���� �ӽ� ���� 
            float prevValue = Value;
            // Dictionary�� ����� BonusValue�� ���� BonusValue Property���� ���ֱ�
            BonusValue -= bonusValuesBySubkey.Values.Sum();
            bonusValuesByKey.Remove(key);

            TryInvokeValueChangedEvent(Value, prevValue);
            return true;
        }
        return false;
    }

    // SubKey�� value�� ����� RemoveBonusValue�� �����ε�
    public bool RemoveBonusValue(object key, object subKey)
    {
        if (bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubkey))
        {
            // ã�ƿ� Dictionary���� Remove �Լ��� ����ؼ� SubKey�� �������ٸ� 
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
    // Dictionary�� Key�� �����ϴ��� Ȯ���ϴ� �Լ��� 
    public bool ContainBonusValue(object key)
    => bonusValuesByKey.ContainsKey(key);

    public bool ContainBonusValue(object key, object subKey)
        => bonusValuesByKey.TryGetValue(key, out var bonusValuesBySubkey) ? bonusValuesBySubkey.ContainsKey(subKey) : false;
    #endregion
}
