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
            var player = collision.GetComponent<PlayerEntity>();

            var stats = player.Stats;
            stats.IncreaseDefaultValue(stats.FullnessStat, healFullness);

            player.OnGetMeat();

            GameManager.Instance.GetExp(isElite);

            gameObject.SetActive(false);
        }
    }
}
