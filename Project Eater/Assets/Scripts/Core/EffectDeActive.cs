using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDeActive : MonoBehaviour
{
    private void DeActivate()
    {
        if (gameObject.transform.parent != null)
            gameObject.transform.SetParent(null);
        
        gameObject.SetActive(false);
    }
}
