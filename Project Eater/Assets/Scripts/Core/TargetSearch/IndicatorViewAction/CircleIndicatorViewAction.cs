using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CircleIndicatorViewAction : IndicatorViewAction
{
    [SerializeField]
    private GameObject indicatorPrefab;

    // 생성한 Indicator의 radius 값 
    // → 해당 값이 0이라면 targetSearcher에서 넘어온 range 값을 대신 사용함 
    [SerializeField]
    private float indicatorRadiusOverride;
    // 생성한 Indicator의 Angle 값 
    // → 해당 값이 0이라면 targetSearcher에서 넘어온 Angle 값을 대신 사용함 
    [SerializeField]
    private float indicatorAngleOverride;

    // Indicator의 FillAmount 프로퍼티를 사용할 것인가? 
    // → 만약, Charge Skill이 Charge 정도에 따라 스킬의 범위가 달라진다 할 경우, 해당 옵션은 true로 설정한다. 
    [SerializeField]
    private bool isUseIndicatorFillAmount;

    // 생성한 Indicator를 requestObject의 자식으로 설정해서 따라다니게 만들지에 대한 여부 
    [SerializeField]
    private bool isAttachIndicatorToRequester;

    // ShowIndicator 함수로 생성한 Indicator를 저장하는 변수 
    private Indicator spawnIndicator;

    public override void ShowIndicator(TargetSearcher targetSearcher, GameObject requestObject, 
        float range, float angle, float fillAmount)
    {
        // 이미 Indicator를 보여주고 있다면 먼저 Hide 처리를 해줌 
        HideIndicator();

        fillAmount = isUseIndicatorFillAmount ? fillAmount : 0;

        var attachTarget = isAttachIndicatorToRequester ? requestObject.transform : null;

        float radius = Mathf.Approximately(indicatorRadiusOverride, 0f) ? range : indicatorRadiusOverride;

        angle = Mathf.Approximately(indicatorAngleOverride, 0f) ? angle : indicatorAngleOverride;

        // Indicator를 Reuse 하고, Setup 함수로 위에서 정한 값들을 Setting 해줌
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
