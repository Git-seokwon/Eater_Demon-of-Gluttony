using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meet : MonoBehaviour
{
    [SerializeField]
    private float healFullness;
    [SerializeField]
    private bool isElite;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            var stats = collision.GetComponent<PlayerEntity>().Stats;
            stats.IncreaseDefaultValue(stats.FullnessStat, healFullness);

            GameManager.Instance.GetExp(isElite);

            gameObject.SetActive(false);
        }
    }
}
