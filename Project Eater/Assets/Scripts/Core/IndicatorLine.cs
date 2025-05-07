using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorLine : MonoBehaviour
{
    [SerializeField]
    private RectTransform canvas;

    private float length;
    private float localPositionXP;
    private float localPositionXM;

    public float Length
    {
        get => length;
        set
        {
            length = Mathf.Max(value, 0f);
            localPositionXP = length * 0.5f + 0.05f;
            localPositionXM = -localPositionXP;

            canvas.localScale = new Vector3(canvas.localScale.x, canvas.localScale.y * length, 1);
            // 길어진 만큼 인디케이터를 이동시켜 Tail 부분이 플레이어 몸체로 오게끔 한다. 
            transform.localPosition = new Vector3(localPositionXP, 0f, 0f); // 1 : 0.55, 2 : 1.05, 3 : 1.55 ...
        }
    }

    // Indicator가 부모로 설정하여 따라다닐 Target
    public Transform TraceTarget
    {
        get => transform.parent;
        set
        {
            transform.parent = value;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    // angle, radius, fillAmount, traceTarget을 설정하는 초기화 함수
    public void Setup(float length, Transform traceTarget = null)
    {
        TraceTarget = traceTarget;

        Length = length;
    }

    private void Update()
    {
        var mousePosition = HelperUtilities.GetMouseWorldPosition();

        // 마우스 조준점이 플레이어 오른쪽에 있는 경우
        if (mousePosition.x > TraceTarget.position.x)
        {
            // 플레이어 왼쪽 바라봄
            if (Mathf.Approximately(TraceTarget.localScale.x, 1f))
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                transform.localPosition = new Vector3(localPositionXP, 0f, 0f);
            }
            // 플레이어 오른쪽 바라봄
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                transform.localPosition = new Vector3(localPositionXM, 0f, 0f);
            }
        }
        // 마우스 조준점이 플레이어 왼쪽에 있는 경우 
        else
        {
            // 플레이어 왼쪽 바라봄
            if (Mathf.Approximately(TraceTarget.localScale.x, 1f))
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                transform.localPosition = new Vector3(localPositionXM, 0f, 0f);
            }
            // 플레이어 오른쪽 바라봄
            else
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                transform.localPosition = new Vector3(localPositionXP, 0f, 0f);
            }
        }
    }
}
