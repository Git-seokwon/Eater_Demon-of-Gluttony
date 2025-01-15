using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageControl : MonoBehaviour
{
    public InfiniteSnapScroll bg;
    public RectTransform targetRect;
    [SerializeField] private int selectedItemFontSize;
    [SerializeField] private int defaultItemFontSize;
    
    private TextMeshProUGUI textMeshPro;
    //private Vector3 ScrollField;
    private RectTransform rect;
    
    void Start()
    {
        rect = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if(Mathf.Abs(targetRect.position.y - rect.position.y)< 20)
        {
            textMeshPro.fontSize = Mathf.Lerp(textMeshPro.fontSize, selectedItemFontSize, 0.1f);
        }
        else
        {
            textMeshPro.fontSize = Mathf.Lerp(textMeshPro.fontSize, defaultItemFontSize, 0.1f);
        }

    }
}
