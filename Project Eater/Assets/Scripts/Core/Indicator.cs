using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private RectTransform canvas;

    [Header("Main")]
    [SerializeField]
    private Image mainImage;
    [SerializeField]
    private Image chargeImage;

    [Header("Border")]
    [SerializeField]
    private RectTransform leftBorder;
    [SerializeField]
    private RectTransform rightBorder;

    private float radius;
    private float angle = 360f;
    private float fillAmount;

    #region ������Ƽ
    public float Radius
    {
        get => radius;
        set
        {
            radius = Mathf.Max(value, 0f);
            // ���� : 2r
            // �� canvas�� �⺻ Scale ���� 0.01 * 2 * radius = 0.01 * 2r = ����
            canvas.localScale = Vector2.one * 0.01f * 2 * radius;
        }
    }

    public float Angle
    {
        get => angle;
        set
        {
            angle = Mathf.Clamp(value, 0f, 360f);

            // �� Image�� fillAmount ���� (Angle / 360)���� Normalize �Ͽ� Set
            mainImage.fillAmount = angle / 360f;
            chargeImage.fillAmount = mainImage.fillAmount;

            // (- angle * 0.5)�� ���ָ� ������ ������ ���� �ȴ�.
            canvas.transform.eulerAngles = new Vector3(0f, 0f, angle * 0.5f);

            // mainImage�� fillAmount�� 1, �� ������ �����̶�� ��輱���� ���ش�.
            if (Mathf.Approximately(mainImage.fillAmount, 1f))
            {
                leftBorder.gameObject.SetActive(false);
                rightBorder.gameObject.SetActive(false);
            }
            // �������̶�� ��輱���� ���ֱ�
            else
            {
                leftBorder.gameObject.SetActive(true);
                rightBorder.gameObject.SetActive(true);
                // rightBorder�� angle��ŭ ȸ��
                rightBorder.transform.localEulerAngles = new Vector3(0f, 0f, -90f - angle);
            }
        }
    }

    // ������� Indicator�� ���� ä��� �뵵
    public float FillAmout
    {
        get => fillAmount;
        set
        {
            fillAmount = Mathf.Clamp01(value);
            // Charge ������ ���� Image�� ���ΰ� �������� ��ó�� �������� �����ش�.
            chargeImage.transform.localScale = Vector3.one * fillAmount;
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
    #endregion

    // angle, radius, fillAmount, traceTarget�� �����ϴ� �ʱ�ȭ �Լ�
    public void Setup(float angle, float radius, float fillAmount = 0f, Transform traceTarget = null)
    {
        Angle = angle;
        Radius = radius;
        TraceTarget = traceTarget;
        FillAmout = fillAmount;

        if (traceTarget == null)
            TraceCursor();
    }

    private void Update()
    {
        if (TraceTarget == null)
            TraceCursor();

        if (Vector2.Angle(HelperUtilities.GetMouseWorldPosition(), transform.right) < 45f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else
            transform.localScale = new Vector3(-1f, -1f, 1f);
    }

    private void LateUpdate()
    {
        // Indicator�� ������ ��, rotation ���� �����ؼ� TraceTarget�� ȸ���� �ص� Indicator�� ȸ������ �ʵ��� ���ش�.
        if (Mathf.Approximately(angle, 360f))
            transform.rotation = Quaternion.identity;
    }

    // Indicator�� Mouse�� ��ġ�� ����
    private void TraceCursor()
    {
        var position = HelperUtilities.GetMouseWorldPosition();
        transform.position = position;
    }
}
