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

    // �� TargetSelectionAction : Target �˻� �������� ã�� Module
    [Header("Select Action")]
	[SerializeReference, SubclassSelector]
	private TargetSelectionAction selectionAction;

    // �� TargetSearchAction : TargetSelectionAction���� ã�� �������� ���� Search�ϴ� Module
    [Header("Search Action")]
    [SerializeReference, SubclassSelector]
    private TargetSearchAction searchAction;

    // TargetSelectionAction�� Select�� �Ϸ����� ��, ȣ���� CallBack �Լ�
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

    // ���� TargetSearcher�� �˻� �������� ��Ÿ���� ���� 
    public bool IsSearching { get; private set; }

    // SelectionResult�� SearchResult�� ���� �� ��ȯ�ϴ� Result Property
    public TargetSelectionResult SelectionResult { get; private set; }
    public TargetSearchResult SearchResult { get; private set; }

    #region ������
    public TargetSearcher() { }
    public TargetSearcher(TargetSearcher copy)
    {
        selectionAction = copy.selectionAction?.Clone() as TargetSelectionAction;
        searchAction = copy.searchAction?.Clone() as TargetSearchAction;
        Scale = copy.scale;
    }
    #endregion

    // TargetSelectionAction Module�� ����ؼ� �������� ã�� �Լ� 
    public void SelectTarget(Entity requestEntity, GameObject requestObject, SelectionCompletedHandler onSelectionCompleted)
    {
        // �̹� �˻� ���̶�� ���� �˻��� ����Ѵ�. 
        CancelSelect();

        IsSearching = true;
        // �ݹ��Լ� ����
        this.onSelectionCompleted = onSelectionCompleted;

        //            �ڡڡ� �Ʒ� �� ���� delegate�� �ٸ���! �ڡڡ�
        // 1. TargetSelectionAction.SelectCompletedHandler : OnSelectCompleted
        // �� TargetSearcher���� ����
        // 2. TargetSearcher.SelectionCompletedHandler : onSelectionCompleted
        // �� TargetSearcher�� �ƴ� �ܺ� Ŭ�������� ����
        selectionAction.Select(this, requestEntity, requestObject, OnSelectCompleted);
    }

    // TargetSelectionAction ����� �ﰢ������ ��ȯ�ϴ� �Լ� 
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

    // TargetSearchAction Module�� ����Ͽ� Target���� ã�� �Լ� 
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

    // selectionAction�� IsInRange �Լ��� ����� ��ȯ�Ѵ�. 
    public bool IsInRange(Entity requestEntity, GameObject requestObject, Vector2 targetPosition)
		=> selectionAction.IsInRange(this, requestEntity, requestObject, targetPosition);

    public string BuildDescription(string description, string prefixKeyword = "")
    {
        // prefixKeyword�� Empty��� �׳� targetSearcher��� ������ ���̰�, ���� ���� �ִٸ� �ڿ� ".targetSearcher"��� ������ ���δ�. 
        // ex) skill�� ���, skill.targetSearcher
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
