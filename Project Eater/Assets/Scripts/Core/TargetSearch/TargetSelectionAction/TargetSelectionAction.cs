using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TargetSelectionAction : ICloneable
{
	#region Events
	// �˻��� �������� ��, �˻� ����� �������ִ� Event
	public delegate void SelectCompletedHandler(TargetSelectionResult reuslt);
	#endregion

	[Header("Indicator")]
	[SerializeReference, SubclassSelector]
	private IndicatorViewAction indicatorViewAction;

	// Range�� Scale�� ������ �� ���� 
	[Header("Option")]
	[SerializeField]
	private bool isUseScale;

	// Range�� ����Ǿ� Range ���� ������ �� ���
	// �� Skill�� Charge ������ ���� �˻� ������ �ٸ��� �� �� Ȱ��
	// Ex) Charge Skill�� ���, Charge ������ ���� �˻� ������ �޶����� �� �� �ִ�. 
	//     �ִ� �˻� ������ 10�̰� 50%�� �����Ǿ��ٸ� 10 * 0.5�� �Ͽ� range�� 5�� �ǵ��� ������ �� �ִ�. 
	private float scale;

	public float Scale
	{
		get => scale;
		set
		{
			if (scale == value)
				return;

			scale = value;
			// scale�� ���� ������ FillAmount ó���� �ǵ��� �Ѵ�. 
			indicatorViewAction?.SetFillAmount(scale);
            OnScaleChange(scale);
		}
	}

    // Ž�� ���� 
    // �� �ڽ� Class���� �����ϵ��� abstract, �߻� Property�� ����
    public abstract float Range { get; }
	// Range�� Scale�� ����� �� 
	public abstract float ScaledRange { get; }
    // Ž�� ���� 
    // �� �ڽ� Class���� �����ϵ��� abstract, �߻� Property�� ����
    public abstract float Angle { get; }

    // isUseScale ���� ����, �Ϲ� Range Ȥ�� ScaledRange�� ��ȯ
    public float ProperRange => isUseScale ? ScaledRange : Range;
	public bool IsUseScale => isUseScale;

	#region ������
	// �⺻ ������
	public TargetSelectionAction() { }

	// ���� ������
	public TargetSelectionAction(TargetSelectionAction copy)
	{
		indicatorViewAction = copy.indicatorViewAction?.Clone() as IndicatorViewAction;
		isUseScale = copy.isUseScale;
	}
    #endregion

    // Player�� �˻��� ��û���� ��, ��� �������� ã�� �Լ� 
    // �� requestEntity : �˻��� ��û�� Entity
    // �� requsetObject : �˻��� ��û�� gameObject
    // �� requestEntity�� requsetObject�� ���� Object�� �ƴ� �� �ִ�. 
    // Ex) ������ �˻��� ��û�� Object�� Skill Object�� Meteo�� Tornado�� ���� �ִ�.
    protected abstract TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher, Entity requestEntity,
		GameObject requsetObject, Vector2 position);

	// Enemy�� �˻��� ��û���� ��, ��� �������� ã�� �Լ� 
	protected abstract TargetSelectionResult SelectImmediateByEnemy(TargetSearcher targetSearcher, Entity requestEntity,
		GameObject requestObject, Vector2 position);

    // Entity�� Player ����, Enemy ������ ���� �� �� �Լ��� �� ������ �Լ��� ����
    public TargetSelectionResult SelectImmediate(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject,
		Vector2 position)
		=> requestEntity.IsPlayer 
		? SelectImmediateByPlayer(targetSearcher, requestEntity, requestObject, position) 
		: SelectImmediateByEnemy(targetSearcher, requestEntity, requestObject, position);

    // �񵿱�� �������� ã�� �Լ� 
    // �� �˻��� �����Ͽ� ������ ����� ���� ������ ��ٷȴٰ� �������� ������ �׶�, onSelectCompleted callBack �Լ��� �˻� ����� 
    //    return �Ѵ�. 
    // Ex) Player�� ���, Mouse�� Click�� �� ������ �˻� ����� �ȳ��´�. �׷��ٰ� Player�� ���콺�� Click�� �ϸ� �׶� �˻� ����� Click��
    //     ��ġ�� ������ �� ���̴�. �̶�, onSelectCompleted�� �˻� ����� �Ѱ��ָ鼭 �˻��� �Ϸ�ȴ�. 
    public abstract void Select(TargetSearcher targetsearcher, Entity requestEntity, GameObject requestObject,
		SelectCompletedHandler onSelectCompleted);

	// Select �Լ��� ������ �˻� ���� ��, �˻��� ����ϴ� �Լ� 
	public abstract void CancleSelect(TargetSearcher targetSearcher);

	// ���ڷ� ���� ��ǥ�� ������ �˻� ���� �ȿ� �ִ��� Ȯ���ϴ� �Լ� 
	public abstract bool IsInRange(TargetSearcher targetSearcher, Entity requestEntity, GameObject requestObject, Vector2 targetPosition);

	public abstract object Clone();

	public virtual void ShowIndicator(TargetSearcher targetSearcher, GameObject requestObject, float fillAmount)
		=> indicatorViewAction?.ShowIndicator(targetSearcher, requestObject, Range, Angle, fillAmount);

	public virtual void HideIndicator() => indicatorViewAction?.HideIndicator();

    // prefixKeyword.selectionAction.keyword �� �ش� ���·� ��µȴ�. 
    // ex. targetSearcher.selectionAction.range
    public string BuildDescription(string decription, string prefixKeyword)
		=> TextReplacer.Replace(decription, prefixKeyword + ".selectionAction", GetStringsByKetword());

    // keyword Dictionary�� ����� �Լ�
    protected virtual IReadOnlyDictionary<string, string> GetStringsByKetword() => null;

    // Scale ���� �����Ǿ��� ��, ó�� �Լ� 
    protected virtual void OnScaleChange(float newScale) { }
}
