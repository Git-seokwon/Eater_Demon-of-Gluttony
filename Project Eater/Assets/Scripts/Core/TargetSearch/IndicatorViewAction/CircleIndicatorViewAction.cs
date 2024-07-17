using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CircleIndicatorViewAction : IndicatorViewAction
{
    [SerializeField]
    private GameObject indicatorPrefab;

    // ������ Indicator�� radius �� 
    // �� �ش� ���� 0�̶�� targetSearcher���� �Ѿ�� range ���� ��� ����� 
    [SerializeField]
    private float indicatorRadiusOverride;
    // ������ Indicator�� Angle �� 
    // �� �ش� ���� 0�̶�� targetSearcher���� �Ѿ�� Angle ���� ��� ����� 
    [SerializeField]
    private float indicatorAngleOverride;

    // Indicator�� FillAmount ������Ƽ�� ����� ���ΰ�? 
    // �� ����, Charge Skill�� Charge ������ ���� ��ų�� ������ �޶����� �� ���, �ش� �ɼ��� true�� �����Ѵ�. 
    [SerializeField]
    private bool isUseIndicatorFillAmount;

    // ������ Indicator�� requestObject�� �ڽ����� �����ؼ� ����ٴϰ� �������� ���� ���� 
    [SerializeField]
    private bool isAttachIndicatorToRequester;

    // ShowIndicator �Լ��� ������ Indicator�� �����ϴ� ���� 
    private Indicator spawnIndicator;

    public override void ShowIndicator(TargetSearcher targetSearcher, GameObject requestObject, 
        float range, float angle, float fillAmount)
    {
        // �̹� Indicator�� �����ְ� �ִٸ� ���� Hide ó���� ���� 
        HideIndicator();

        fillAmount = isUseIndicatorFillAmount ? fillAmount : 0;

        var attachTarget = isAttachIndicatorToRequester ? requestObject.transform : null;

        float radius = Mathf.Approximately(indicatorRadiusOverride, 0f) ? range : indicatorRadiusOverride;

        angle = Mathf.Approximately(indicatorAngleOverride, 0f) ? angle : indicatorAngleOverride;

        // Indicator�� Reuse �ϰ�, Setup �Լ��� ������ ���� ������ Setting ����
        spawnIndicator = PoolManager.Instance.ReuseGameObject(indicatorPrefab, Vector3.zero, Quaternion.identity).GetComponent<Indicator>();
        spawnIndicator.Setup(angle, radius, fillAmount, attachTarget);
    }

    public override void HideIndicator()
    {
        if (!spawnIndicator)
            return;

        if (isAttachIndicatorToRequester)
        {
            spawnIndicator.gameObject.transform.parent = null;
            spawnIndicator.gameObject.SetActive(false);
        }
        else
            spawnIndicator.gameObject.SetActive(false);
    }

    public override void SetFillAmount(float fillAmount)
    {
        if (!isUseIndicatorFillAmount || spawnIndicator == null)
            return;

        spawnIndicator.FillAmout = fillAmount;
    }

    public override object Clone()
    {
        return new CircleIndicatorViewAction()
        {
            indicatorPrefab = indicatorPrefab,
            indicatorRadiusOverride = indicatorRadiusOverride,
            indicatorAngleOverride = indicatorAngleOverride,
            isUseIndicatorFillAmount = isUseIndicatorFillAmount,
            isAttachIndicatorToRequester = isAttachIndicatorToRequester
        };
    }
}
