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

    // selectionAction의 IsInRange 함수의 결과를 반환한다. 
    public bool IsInRange(Entity requestEntity, GameObject requestObject, Vector3 targetPosition)
		=> selectionAction.IsInRange(this, requestEntity, requestObject, targetPosition);
}
