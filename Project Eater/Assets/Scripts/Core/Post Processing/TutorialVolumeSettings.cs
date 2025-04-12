using Cinemachine.PostFX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TutorialVolumeSettings : MonoBehaviour
{
    [SerializeField]
    private CinemachineVolumeSettings cinemachineVS;

    private ColorAdjustments colorAdjustmentsSetting;

    void Awake()
    {
        cinemachineVS?.m_Profile.TryGet(out colorAdjustmentsSetting);
    }

    void Start()
    {
        if (cinemachineVS != null)
            colorAdjustmentsSetting.postExposure.value = (GraphicManager.Instance.brightness - 100) / 25;
    }
}
