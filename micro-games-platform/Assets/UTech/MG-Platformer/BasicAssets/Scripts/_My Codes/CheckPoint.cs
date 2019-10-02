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

        [SerializeField] private GameController gamecontroller;
        

        private void Start()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                if(notUsed)
                {
                    GameController.instance.SpawnPoint = transform;
                }
            }
        }
    }
}
