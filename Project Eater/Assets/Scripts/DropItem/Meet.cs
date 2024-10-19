using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meet : MonoBehaviour
{
    [SerializeField]
    private float healFullness;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag)
        {
            // TODO : 플레이어의 Fullness를 어느정도 채워주기 
            var stats = collision.GetComponent<PlayerEntity>().Stats;
            stats.IncreaseDefaultValue(stats.FullnessStat, healFullness);

            GameManager.Instance.GetExp();

            gameObject.SetActive(false);
        }
    }
}
