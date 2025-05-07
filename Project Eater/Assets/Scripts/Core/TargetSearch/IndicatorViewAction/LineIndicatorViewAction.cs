using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LineIndicatorViewAction : IndicatorViewAction
{
    [SerializeField]
    private GameObject indicatorPrefab;

    // 생성한 Indicator의 length 값 
    [SerializeField]
    private float indicatoLengthOverride;

    // 생성한 Indicator를 requestObject의 자식으로 설정해서 따라다니게 만들지에 대한 여부 
    [SerializeField]
    private bool isAttachIndicatorToRequester;

    // ShowIndicator 함수로 생성한 Indicator를 저장하는 변수 
    private IndicatorLine spawnIndicator;

    public override void ShowIndicator(TargetSearcher targetSearcher, GameObject requestObject, float range, float angle, float fillAmount)
    {
        // 이미 Indicator를 보여주고 있다면 먼저 Hide 처리를 해줌 
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
