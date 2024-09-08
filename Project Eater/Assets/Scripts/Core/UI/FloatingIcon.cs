using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingIcon : MonoBehaviour
{
    [SerializeField]
    private List<Image> CCSprites = new List<Image>();

    public void SetActiveCCSprite(int index) => CCSprites[index].gameObject.SetActive(true);
    public void SetDeActiveCCSprite(int index) => CCSprites[index].gameObject.SetActive(false);
}
