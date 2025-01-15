using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogamTest : MonoBehaviour
{
    [SerializeField] private DogamUI dogamUI;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F4))
        {
            if(!dogamUI.gameObject.activeInHierarchy)
                dogamUI.Open();
            else
                dogamUI.Close();
        }
    }
}
