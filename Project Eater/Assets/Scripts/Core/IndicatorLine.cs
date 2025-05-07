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
            // ����� ��ŭ �ε������͸� �̵����� Tail �κ��� �÷��̾� ��ü�� ���Բ� �Ѵ�. 
            transform.localPosition = new Vector3(localPositionXP, 0f, 0f); // 1 : 0.55, 2 : 1.05, 3 : 1.55 ...
        }
    }

    // Indicator�� �θ�� �����Ͽ� ����ٴ� Target
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

    // angle, radius, fillAmount, traceTarget�� �����ϴ� �ʱ�ȭ �Լ�
    public void Setup(float length, Transform traceTarget = null)
    {
        TraceTarget = traceTarget;

        Length = length;
    }

    private void Update()
    {
        var mousePosition = HelperUtilities.GetMouseWorldPosition();

        // ���콺 �������� �÷��̾� �����ʿ� �ִ� ���
        if (mousePosition.x > TraceTarget.position.x)
        {
            // �÷��̾� ���� �ٶ�
            if (Mathf.Approximately(TraceTarget.localScale.x, 1f))
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                transform.localPosition = new Vector3(localPositionXP, 0f, 0f);
            }
            // �÷��̾� ������ �ٶ�
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                transform.localPosition = new Vector3(localPositionXM, 0f, 0f);
            }
        }
        // ���콺 �������� �÷��̾� ���ʿ� �ִ� ��� 
        else
        {
            // �÷��̾� ���� �ٶ�
            if (Mathf.Approximately(TraceTarget.localScale.x, 1f))
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                transform.localPosition = new Vector3(localPositionXM, 0f, 0f);
            }
            // �÷��̾� ������ �ٶ�
            else
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                transform.localPosition = new Vector3(localPositionXP, 0f, 0f);
            }
        }
    }
}
