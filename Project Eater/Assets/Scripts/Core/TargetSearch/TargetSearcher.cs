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

    // selectionAction�� IsInRange �Լ��� ����� ��ȯ�Ѵ�. 
    public bool IsInRange(Entity requestEntity, GameObject requestObject, Vector3 targetPosition)
		=> selectionAction.IsInRange(this, requestEntity, requestObject, targetPosition);
}
