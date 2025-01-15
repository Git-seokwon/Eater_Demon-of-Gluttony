using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollControl : MonoBehaviour
{
    [SerializeField] private RectTransform scrollRect;
    [SerializeField] private RectTransform scrollbarRect;

    private RectTransform ownRect;
    private float prePos;
    private float ownprePos;
    

    private void Start()
    {
        prePos = scrollRect.localPosition.y;
        ownRect = GetComponent<RectTransform>();
        ownprePos = ownRect.localPosition.y;
    }

    void Update()
    {
        ownRect.localPosition = new Vector3(ownRect.localPosition.x, 
            ownprePos - (scrollbarRect.rect.height * ((scrollRect.localPosition.y - prePos) / (scrollRect.rect.height - 500))),
            ownRect.localPosition.z);
    }
}
