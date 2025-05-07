using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LineIndicatorViewAction : IndicatorViewAction
{
    [SerializeField]
    private GameObject indicatorPrefab;

    // ������ Indicator�� length �� 
    [SerializeField]
    private float indicatoLengthOverride;

    // ������ Indicator�� requestObject�� �ڽ����� �����ؼ� ����ٴϰ� �������� ���� ���� 
    [SerializeField]
    private bool isAttachIndicatorToRequester;

    // ShowIndicator �Լ��� ������ Indicator�� �����ϴ� ���� 
    private IndicatorLine spawnIndicator;

    public override void ShowIndicator(TargetSearcher targetSearcher, GameObject requestObject, float range, float angle, float fillAmount)
    {
        // �̹� Indicator�� �����ְ� �ִٸ� ���� Hide ó���� ���� 
        HideIndicator();

        float length = Mathf.Approximately(indicatoLengthOverride, 0f) ? range : indicatoLengthOverride;
        var attachTarget = isAttachIndicatorToRequester ? requestObject.transform : null;

        spawnIndicator = PoolManager.Instance.ReuseGameObject(indicatorPrefab, Vector3.zero, Quaternion.identity).GetComponent<IndicatorLine>();
        spawnIndicator.Setup(length, attachTarget);
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

    }

    public override object Clone()
    {
        return new LineIndicatorViewAction()
        {
            indicatorPrefab = indicatorPrefab,
            isAttachIndicatorToRequester = isAttachIndicatorToRequester
        };
    }
}
