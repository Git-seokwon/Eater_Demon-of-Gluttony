using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterIndicator : MonoBehaviour
{
    [SerializeField] private Transform transform;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Transform playerTransform;

    public void Rotate(Vector3 targetPos)
    {
        targetPos.z = 0;
        Vector3 dir = targetPos - playerTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // ���� ȸ���� ���� �����
        rectTransform.rotation = Quaternion.Euler(0f, 0f, angle+90f);
    }

    public void Rotate(Vector2 targetPos) => Rotate(new Vector3(targetPos.x, targetPos.y, 0));
    public void Rotate(Transform targetPos)
    {
        if (targetPos == null) 
            return;
        
        Rotate(targetPos.position);
    }


    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    /*
    // �׽�Ʈ�� ���콺 Ȧ��
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f; // z�� ����
            Rotate(mouseWorldPos);
        }
    }
    */
}
