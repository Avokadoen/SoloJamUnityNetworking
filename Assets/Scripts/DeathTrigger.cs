using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerInterface>();
        if (player == null)
        {
            return;
        }

        player.TakeDmg(Mathf.Infinity);
    }

}
 
