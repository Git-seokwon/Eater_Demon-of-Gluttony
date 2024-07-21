using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TargetSearchAction : ICloneable
{
    // Indicator를 보여주는 Module
    [Header("Indicator")]
    [SerializeReference, SubclassSelector]
    private IndicatorViewAction indicatorViewAction;

    // Range에 Scale을 적용할 지 여부 
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

    #region 생성자
    public TargetSearchAction() { }
    public TargetSearchAction(TargetSearchAction copy)
    {
        indicatorViewAction = copy.indicatorViewAction?.Clone() as IndicatorViewAction;
        isUseScale = copy.isUseScale;
    }
    #endregion

    // 기준점을 토대로 실제 Target을 검색하는 함수 
    // → selectResult를 기반으로 Target을 찾으며, 검색 결과를 즉각 반환 
    // → 해당 Class를 상속받는 Module들은 SelectionResult를 기준으로 실제 Target들을 찾아서 결과를 return 한다. 
    public abstract TargetSearchResult Search(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject,
        TargetSelectionResult selectResult);

    public abstract object Clone(); 

    public virtual void ShowIndicator(TargetSearcher targetSearcher, GameObject requestObject, float fillAmount)
        => indicatorViewAction?.ShowIndicator(targetSearcher, requestObject, Range, Angle, fillAmount);

    public virtual void HideIndicator() => indicatorViewAction?.HideIndicator();

    public string BuildDescription(string description, string prefixKetword)
        => TextReplacer.Replace(description, prefixKetword + ".searchAction", GetStringsByKeyword());

    protected virtual IReadOnlyDictionary<string, string> GetStringsByKeyword() => null;

    // Scale 값이 수정되었을 때의 처리를 하는 함수 
    protected virtual void OnScaleChanged(float newScale) { } 
}
