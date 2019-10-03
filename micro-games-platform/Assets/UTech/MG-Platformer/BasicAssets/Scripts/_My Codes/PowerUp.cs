using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class PowerUp : MonoBehaviour
    {
        enum PowerUps
        {
            Star = 1,
            DoubleJump = 2,
            ZeroGravity = 3
        }

        [SerializeField] private PowerUps powerUpType = PowerUps.Star;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                switch (powerUpType)
                {
                    case PowerUps.Star:
                        player.StarPowerUp();
                        break;
                    case PowerUps.DoubleJump:
                        player.JumpPowerUp();
                        break;
                    case PowerUps.ZeroGravity:
                        player.GravityPowerUp();
                        break;
                    default:
                        break;
                }

                Destroy(gameObject);
            }
        }
    }
}
