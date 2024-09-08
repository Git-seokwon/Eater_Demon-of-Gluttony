using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillEffectView : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private Image blindImage;
    [SerializeField]
    private TextMeshProUGUI stackText;

    // UI�� �׷��ְ� �ִ� Effect�� Target Property
    public Effect Target {  get; private set; }

    private void OnDestroy()
    {
        if (!Target)
            return;

        Target.onStackChanged -= OnEffectStackChanged;
        Target.onReleased -= OnEffectReleased;
    }

    private void Update()
    {
        // ���ӽð��� ��Ÿ���� blindImage�� fillAmount�� (���� ���� �ð� / �ִ� ���� �ð�)�� Setting
        blindImage.fillAmount = Target.CurrentDuration / Target.Duration;
    }

    public void Setup(Effect viewEffect)
    {
        Target = viewEffect;
        iconImage.sprite = viewEffect.Icon;
        blindImage.fillAmount = 0f;
        stackText.gameObject.SetActive(false);

        Target.onStackChanged += OnEffectStackChanged;
        Target.onReleased += OnEffectReleased;

        // Target Effect�� ������ڸ��� ���� Stack�� ���̴� Effect�� ���� ������ OnEffectStackChanged �Լ���
        // stackText Update�� �õ��Ѵ�. 
        OnEffectStackChanged(viewEffect, viewEffect.CurrentStack, 0);
    }

    // ���� Stack ���� Text�� �����ִ� �Լ� 
    private void OnEffectStackChanged(Effect effect, int currentStack, int prevStack)
    {
        if (effect.MaxStack == 1)
            return;

        stackText.text = currentStack.ToString();
        stackText.gameObject.SetActive(true);
    }

    // ���� EffectView�� ���ش�. 
    private void OnEffectReleased(Effect effect)
        => Destroy(gameObject);
}
