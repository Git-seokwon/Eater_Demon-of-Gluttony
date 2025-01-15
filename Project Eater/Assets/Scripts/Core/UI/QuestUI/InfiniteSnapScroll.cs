using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class InfiniteSnapScroll : MonoBehaviour
{
    [Header("Need To Fill")]
    [SerializeField] private ScrollRect scrollrect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private RectTransform sampleListItem;
    [SerializeField] private DogamUI dogamUI;
    [SerializeField] private VerticalLayoutGroup VLG;
    
    private bool isSnapped;

    [Header("Option")]
    [SerializeField] private float snapForce;
    [SerializeField] private float snapSpeed;

    private Vector3 prePos;
    private int prevItem;

    private Vector2 prevMousePosition;

    void Start()
    {
        isSnapped = false;

        prePos = contentPanel.localPosition;
        prevItem = 0;
    }

    void Update()
    {
        int currentItem = Mathf.RoundToInt((contentPanel.localPosition.y - prePos.y) / (sampleListItem.rect.height + VLG.spacing));
        //Debug.Log(currentItem);
        //DogamUI에 currentItem에 대한 정보를 보내는 부분 추가.
        if(prevItem != currentItem)
        {
            prevItem = currentItem;
            dogamUI.CurrentIndex = currentItem;
        }

        if(scrollrect.velocity.magnitude < 200 && !isSnapped)
        {
            scrollrect.velocity = Vector3.zero;
            snapSpeed += snapForce * Time.deltaTime;

            contentPanel.localPosition = new Vector3(prePos.x,
                Mathf.MoveTowards(contentPanel.localPosition.y, (prePos.y + (currentItem*(sampleListItem.rect.height + VLG.spacing))), snapSpeed),
                contentPanel.localPosition.z);

            if (contentPanel.localPosition.y == (currentItem * (sampleListItem.rect.height + VLG.spacing)))
            {
                isSnapped = true;
            }

        }
        if (scrollrect.velocity.magnitude > 200)
        {
            isSnapped = false;
            snapSpeed = 0;
        }

        // 임시방편.
        if(Input.GetMouseButtonDown(0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(scrollrect.GetComponent<RectTransform>(), Input.mousePosition, null, out prevMousePosition);
        }

        if(Input.GetMouseButtonUp(0))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(scrollrect.GetComponent<RectTransform>(), Input.mousePosition, null, out Vector2 mousePosition);
            if (prevMousePosition.Equals(mousePosition))
            {
                contentPanel.localPosition = new Vector3(contentPanel.localPosition.x, contentPanel.localPosition.y - mousePosition.y, contentPanel.localPosition.z);
            }
        }
    }

}
