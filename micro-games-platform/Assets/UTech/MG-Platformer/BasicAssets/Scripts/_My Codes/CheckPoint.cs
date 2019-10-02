using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    public class CheckPoint : MonoBehaviour
    {
        private bool notUsed = true; // this indicate if this checkpoint was already used

        //verify if the player collides with the checkpoint area
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                if(notUsed)
                {
                    //get the singleton instance of the gameobject than change the transform of the spawn point
                    GameController.instance.SpawnPoint = transform;
                }
            }
        }
    }
}
