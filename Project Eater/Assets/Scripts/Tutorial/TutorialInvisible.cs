using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInvisible : MonoBehaviour
{
    [SerializeField]
    private float invisibleTime = 0f;

    private void OnEnable()
    {
        Invoke("Invisible", invisibleTime);
    }

    private void Invisible() => gameObject.SetActive(false);
}
