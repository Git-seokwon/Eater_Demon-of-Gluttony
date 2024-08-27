using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TargetSearchAction : ICloneable
{
    // Indicator�� �����ִ� Module
    [Header("Indicator")]
    [SerializeReference, SubclassSelector]
    private IndicatorViewAction indicatorViewAction;

    // Range�� Scale�� ������ �� ���� 
    [Header("Option")]
    [SerializeField]
    private bool isUseScale;

    private float scale;

    public float Scale
    {
        get => scale;
        set
        {
            if (scale == value)
                return;

            scale = value;
            indicatorViewAction?.SetFillAmount(scale);
            OnScaleChanged(scale);
        }
    }

    public abstract float Range { get; }
    public abstract float ScaledRange { get; }
    public abstract float Angle { get; }
    public float ProperRange => isUseScale ? ScaledRange : Range;
    public bool IsUseScale => isUseScale;

    #region ������
    public TargetSearchAction() { }
    public TargetSearchAction(TargetSearchAction copy)
    {
        indicatorViewAction = copy.indicatorViewAction?.Clone() as IndicatorViewAction;
        isUseScale = copy.isUseScale;
    }
    #endregion

    // �������� ���� ���� Target�� �˻��ϴ� �Լ� 
    // �� selectResult�� ������� Target�� ã����, �˻� ����� �ﰢ ��ȯ 
    // �� �ش� Class�� ��ӹ޴� Module���� SelectionResult�� �������� ���� Target���� ã�Ƽ� ����� return �Ѵ�. 
    public abstract TargetSearchResult Search(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject,
        TargetSelectionResult selectResult);

    public abstract object Clone(); 

    public virtual void ShowIndicator(TargetSearcher targetSearcher, GameObject requestObject, float fillAmount)
        => indicatorViewAction?.ShowIndicator(targetSearcher, requestObject, Range, Angle, fillAmount);

    public virtual void HideIndicator() => indicatorViewAction?.HideIndicator();

    public string BuildDescription(string description, string prefixKetword)
        => TextReplacer.Replace(description, prefixKetword + ".searchAction", GetStringsByKeyword());

    protected virtual IReadOnlyDictionary<string, string> GetStringsByKeyword() => null;

    // Scale ���� �����Ǿ��� ���� ó���� �ϴ� �Լ� 
    protected virtual void OnScaleChanged(float newScale) { } 
}
