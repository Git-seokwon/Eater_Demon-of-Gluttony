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

    // UI가 그려주고 있는 Effect인 Target Property
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
        // 지속시간을 나타내는 blindImage의 fillAmount에 (현재 지속 시간 / 최대 지속 시간)을 Setting
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

        // Target Effect가 적용되자마자 여러 Stack이 쌓이는 Effect일 수도 있으니 OnEffectStackChanged 함수로
        // stackText Update를 시도한다. 
        OnEffectStackChanged(viewEffect, viewEffect.CurrentStack, 0);
    }

    // 현재 Stack 값을 Text로 보여주는 함수 
    private void OnEffectStackChanged(Effect effect, int currentStack, int prevStack)
    {
        if (effect.MaxStack == 1)
            return;

        stackText.text = currentStack.ToString();
        stackText.gameObject.SetActive(true);
    }

    // 현재 EffectView을 꺼준다. 
    private void OnEffectReleased(Effect effect)
        => Destroy(gameObject);
}
