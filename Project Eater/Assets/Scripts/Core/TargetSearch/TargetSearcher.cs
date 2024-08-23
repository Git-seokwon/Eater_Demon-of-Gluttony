using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetSearcher
{
	#region Events
	public delegate void SelectionCompletedHandler(TargetSearcher targetSearcher, TargetSelectionResult result);
    #endregion

    // ※ TargetSelectionAction : Target 검색 기준점을 찾는 Module
    [Header("Select Action")]
	[SerializeReference, SubclassSelector]
	private TargetSelectionAction selectionAction;

    // ※ TargetSearchAction : TargetSelectionAction으로 찾은 기준점을 토대로 Search하는 Module
    [Header("Search Action")]
    [SerializeReference, SubclassSelector]
    private TargetSearchAction searchAction;

    // TargetSelectionAction이 Select를 완료했을 때, 호출할 CallBack 함수
    private SelectionCompletedHandler onSelectionCompleted;

    private float scale = 1f;
    public float Scale
    {
        get => scale;
        set
        {
            scale = Mathf.Clamp01(value);
            selectionAction.Scale = scale;
            searchAction.Scale = scale;
        }
    }

    public float SelectionRange => selectionAction.Range;
    public float SelectionScaledRange => selectionAction.ScaledRange;
    public float SelectionProperRange => selectionAction.ProperRange;
    public float SelectionAngle => selectionAction.Angle;

    public float SearchRange => searchAction.Range;
    public float SearchScaledRange => searchAction.ScaledRange;
    public float SearchProperRange => searchAction.ProperRange;
    public float SearchAngle => searchAction.Angle;

    // 현재 TargetSearcher가 검색 중인지를 나타내는 변수 
    public bool IsSearching { get; private set; }

    // SelectionResult와 SearchResult를 저장 및 반환하는 Result Property
    public TargetSelectionResult SelectionResult { get; private set; }
    public TargetSearchResult SearchResult { get; private set; }

    #region 생성자
    public TargetSearcher() { }
    public TargetSearcher(TargetSearcher copy)
    {
        selectionAction = copy.selectionAction?.Clone() as TargetSelectionAction;
        searchAction = copy.searchAction?.Clone() as TargetSearchAction;
        Scale = copy.scale;
    }
    #endregion

    // TargetSelectionAction Module을 사용해서 기준점을 찾는 함수 
    public void SelectTarget(Entity requestEntity, GameObject requestObject, SelectionCompletedHandler onSelectionCompleted)
    {
        // 이미 검색 중이라면 기존 검색을 취소한다. 
        CancelSelect();

        IsSearching = true;
        // 콜백함수 저장
        this.onSelectionCompleted = onSelectionCompleted;

        //            ★★★ 아래 두 개의 delegate는 다르다! ★★★
        // 1. TargetSelectionAction.SelectCompletedHandler : OnSelectCompleted
        // → TargetSearcher에서 지정
        // 2. TargetSearcher.SelectionCompletedHandler : onSelectionCompleted
        // → TargetSearcher가 아닌 외부 클래스에서 지정
        selectionAction.Select(this, requestEntity, requestObject, OnSelectCompleted);
    }

    // TargetSelectionAction 결과를 즉각적으로 반환하는 함수 
    public TargetSelectionResult SelectImmediate(Entity requestEntity, GameObject requestObject, Vector2 position)
    {
        CancelSelect();

        SelectionResult = selectionAction.SelectImmediate(this, requestEntity, requestObject, position);
        return SelectionResult;
    }

    public void CancelSelect()
    {
        if (!IsSearching)
            return;

        IsSearching = true;
        selectionAction.CancelSelect(this);
    }

    // TargetSearchAction Module을 사용하여 Target들을 찾는 함수 
    public TargetSearchResult SearchTargets(Entity requestEntity, GameObject requestObject)
    {
        SearchResult = searchAction.Search(this, requestEntity, requestObject, SelectionResult);
        return SearchResult;
    }

    public void ShowIndicator(GameObject requestObject)
    {
        HideIndicator();

        selectionAction.ShowIndicator(this, requestObject, scale);
        searchAction.ShowIndicator(this, requestObject, scale);
    }

    public void HideIndicator()
    {
        selectionAction.HideIndicator();
        searchAction.HideIndicator();
    }

    // selectionAction의 IsInRange 함수의 결과를 반환한다. 
    public bool IsInRange(Entity requestEntity, GameObject requestObject, Vector2 targetPosition)
		=> selectionAction.IsInRange(this, requestEntity, requestObject, targetPosition);

    public string BuildDescription(string description, string prefixKeyword = "")
    {
        // prefixKeyword가 Empty라면 그냥 targetSearcher라는 문장을 붙이고, 무언가 값이 있다면 뒤에 ".targetSearcher"라는 문장을 붙인다. 
        // ex) skill인 경우, skill.targetSearcher
        prefixKeyword += string.IsNullOrEmpty(prefixKeyword) ? "targetSearcher" : ".targetSearcher";
        description = selectionAction.BuildDescription(description, prefixKeyword);
        description = searchAction.BuildDescription(description, prefixKeyword);
        return description;
    }

    #region CallBack
    private void OnSelectCompleted(TargetSelectionResult selectReuslt)
    {
        IsSearching = false;
        SelectionResult = selectReuslt;
        onSelectionCompleted?.Invoke(this, selectReuslt);
    }
    #endregion
}
