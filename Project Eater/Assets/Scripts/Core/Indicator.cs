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

    #region 프로퍼티
    public float Radius
    {
        get => radius;
        set
        {
            radius = Mathf.Max(value, 0f);
            // 지름 : 2r
            // → canvas의 기본 Scale 값인 0.01 * 2 * radius = 0.01 * 2r = 지름
            canvas.localScale = Vector2.one * 0.01f * 2 * radius;
        }
    }

    public float Angle
    {
        get => angle;
        set
        {
            angle = Mathf.Clamp(value, 0f, 360f);

            // 각 Image의 fillAmount 값을 (Angle / 360)으로 Normalize 하여 Set
            mainImage.fillAmount = angle / 360f;
            chargeImage.fillAmount = mainImage.fillAmount;

            // (- angle * 0.5)를 해주면 원뿔이 정면을 보게 된다.
            canvas.transform.eulerAngles = new Vector3(0f, 0f, angle * 0.5f);

            // mainImage의 fillAmount가 1, 즉 완전한 원형이라면 경계선들을 꺼준다.
            if (Mathf.Approximately(mainImage.fillAmount, 1f))
            {
                leftBorder.gameObject.SetActive(false);
                rightBorder.gameObject.SetActive(false);
            }
            // 원뿔형이라면 경계선들을 켜주기
            else
            {
                leftBorder.gameObject.SetActive(true);
                rightBorder.gameObject.SetActive(true);
                // rightBorder를 angle만큼 회전
                rightBorder.transform.localEulerAngles = new Vector3(0f, 0f, -90f - angle);
            }
        }
    }

    // 만들어진 Indicator의 속을 채우는 용도
    public float FillAmout
    {
        get => fillAmount;
        set
        {
            fillAmount = Mathf.Clamp01(value);
            // Charge 정도에 따라서 Image의 내부가 차오르는 것처럼 느껴지게 보여준다.
            chargeImage.transform.localScale = Vector3.one * fillAmount;
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
    #endregion

    // angle, radius, fillAmount, traceTarget을 설정하는 초기화 함수
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
        // Indicator가 원형일 때, rotation 값을 고정해서 TraceTarget이 회전을 해도 Indicator는 회전하지 않도록 해준다.
        if (Mathf.Approximately(angle, 360f))
            transform.rotation = Quaternion.identity;
    }

    // Indicator가 Mouse의 위치를 추적
    private void TraceCursor()
    {
        var position = HelperUtilities.GetMouseWorldPosition();
        transform.position = position;
    }
}
