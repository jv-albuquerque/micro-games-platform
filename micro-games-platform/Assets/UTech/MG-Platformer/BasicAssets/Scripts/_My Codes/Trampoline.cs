using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class Trampoline : MonoBehaviour
    {
        //when the player collides in the top of the trampoline it will make the player jump higher
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TrampolineJump();
            }
        }
    }
}
