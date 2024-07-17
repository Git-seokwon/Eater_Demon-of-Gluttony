using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TargetSelectionAction : ICloneable
{
	#region Events
	// 검색에 성공했을 때, 검색 결과를 전달해주는 Event
	public delegate void SelectCompletedHandler(TargetSelectionResult reuslt);
	#endregion

	[Header("Indicator")]
	[SerializeReference, SubclassSelector]
	private IndicatorViewAction indicatorViewAction;

	// Range에 Scale을 적용할 지 여부 
	[Header("Option")]
	[SerializeField]
	private bool isUseScale;

	// Range에 적용되어 Range 값을 조절할 때 사용
	// → Skill의 Charge 정도에 따라 검색 범위를 다르게 할 때 활용
	// Ex) Charge Skill의 경우, Charge 정도에 따라 검색 범위가 달라지게 할 수 있다. 
	//     최대 검색 범위가 10이고 50%가 충전되었다면 10 * 0.5를 하여 range가 5가 되도록 설정할 수 있다. 
	private float scale;

	public float Scale
	{
		get => scale;
		set
		{
			if (scale == value)
				return;

			scale = value;
			// scale에 따라 적절한 FillAmount 처리가 되도록 한다. 
			indicatorViewAction?.SetFillAmount(scale);
            OnScaleChange(scale);
		}
	}

    // 탐색 범위 
    // → 자식 Class에서 정의하도록 abstract, 추상 Property로 만듬
    public abstract float Range { get; }
	// Range에 Scale이 적용된 값 
	public abstract float ScaledRange { get; }
    // 탐색 각도 
    // → 자식 Class에서 정의하도록 abstract, 추상 Property로 만듬
    public abstract float Angle { get; }

    // isUseScale 값에 따라, 일반 Range 혹은 ScaledRange를 반환
    public float ProperRange => isUseScale ? ScaledRange : Range;
	public bool IsUseScale => isUseScale;

	#region 생성자
	// 기본 생성자
	public TargetSelectionAction() { }

	// 복사 생성자
	public TargetSelectionAction(TargetSelectionAction copy)
	{
		indicatorViewAction = copy.indicatorViewAction?.Clone() as IndicatorViewAction;
		isUseScale = copy.isUseScale;
	}
    #endregion

    // Player가 검색을 요청했을 때, 즉시 기준점을 찾는 함수 
    // ※ requestEntity : 검색을 요청한 Entity
    // ※ requsetObject : 검색을 요청한 gameObject
    // → requestEntity와 requsetObject는 같은 Object가 아닐 수 있다. 
    // Ex) 기준점 검색을 요청한 Object가 Skill Object인 Meteo나 Tornado일 수도 있다.
    protected abstract TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher, Entity requestEntity,
		GameObject requsetObject, Vector2 position);

	// Enemy가 검색을 요청했을 때, 즉시 기준점을 찾는 함수 
	protected abstract TargetSelectionResult SelectImmediateByEnemy(TargetSearcher targetSearcher, Entity requestEntity,
		GameObject requestObject, Vector2 position);

    // Entity가 Player 인지, Enemy 인지에 따라서 위 두 함수를 중 적합한 함수를 실행
    public TargetSelectionResult SelectImmediate(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject,
		Vector2 position)
		=> requestEntity.IsPlayer 
		? SelectImmediateByPlayer(targetSearcher, requestEntity, requestObject, position) 
		: SelectImmediateByEnemy(targetSearcher, requestEntity, requestObject, position);

    // 비동기로 기준점을 찾는 함수 
    // → 검색을 실행하여 기준점 결과가 나올 때까지 기다렸다가 기준점이 나오면 그때, onSelectCompleted callBack 함수의 검색 결과를 
    //    return 한다. 
    // Ex) Player의 경우, Mouse로 Click을 할 때까지 검색 결과가 안나온다. 그러다가 Player가 마우스로 Click을 하면 그때 검색 결과로 Click한
    //     위치가 나오게 될 것이다. 이때, onSelectCompleted에 검색 결과를 넘겨주면서 검색이 완료된다. 
    public abstract void Select(TargetSearcher targetsearcher, Entity requestEntity, GameObject requestObject,
		SelectCompletedHandler onSelectCompleted);

	// Select 함수로 기준점 검색 중일 때, 검색을 취소하는 함수 
	public abstract void CancleSelect(TargetSearcher targetSearcher);

	// 인자로 받은 좌표가 기준점 검색 범위 안에 있는지 확인하는 함수 
	public abstract bool IsInRange(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, Vector2 targetPosition);

	public abstract object Clone();

	public virtual void ShowIndicator(TargetSearcher targetSearcher, GameObject requestObject, float fillAmount)
		=> indicatorViewAction?.ShowIndicator(targetSearcher, requestObject, Range, Angle, fillAmount);

	public virtual void HideIndicator() => indicatorViewAction?.HideIndicator();

    // prefixKeyword.selectionAction.keyword → 해당 형태로 출력된다. 
    // ex. targetSearcher.selectionAction.range
    public string BuildDescription(string decription, string prefixKeyword)
		=> TextReplacer.Replace(decription, prefixKeyword + ".selectionAction", GetStringsByKetword());

    // keyword Dictionary를 만드는 함수
    protected virtual IReadOnlyDictionary<string, string> GetStringsByKetword() => null;

    // Scale 값이 수정되었을 때, 처리 함수 
    protected virtual void OnScaleChange(float newScale) { }
}
