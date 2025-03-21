using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    void Start()
    {
        PlayerController.Instance.SetPlayerMode(PlayerMode.Devil);
    }
}
