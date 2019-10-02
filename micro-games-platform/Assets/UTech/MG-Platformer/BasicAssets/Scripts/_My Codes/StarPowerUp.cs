using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class StarPowerUp : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                //call star powerup
                player.StarPowerUP();

                Destroy(gameObject);
            }
        }
    }
}
