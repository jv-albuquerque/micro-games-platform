using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using Pathfinding;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        public PatrolPath path;
        public AudioClip ouch;

        // case the enemy is a fly one
        [SerializeField] private bool canFly = false; //verify if the enemy can fly
        private Transform playerTransform; //the Transform of the playr gameobject
        private float pickNextWaypointDist = 1f; //the distance the enemy needs to be of the waypoint to go to the next one
        private int currentWaypoint = 0; // the waypoint the enemy is
        private Path flyPath; // the A* mesh
        private Seeker seeker; // the class that find the best path in the mesh
        private Rigidbody2D rb; // the enemy's rigidbody 2D

        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;

        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (canFly)
            {
                seeker = GetComponent<Seeker>();
                rb = GetComponent<Rigidbody2D>();
                playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

                InvokeRepeating("UpdatePath", 0, .5f);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }
        }

        void Update()
        {
            if (canFly)
                Fly();
            else
                Slime();
        }

        private void Fly()
        {
            if (flyPath == null || control.isDead)
                return;

            //if the enemy reachs the final waypoint
            if (currentWaypoint >= flyPath.vectorPath.Count)
                return;

            transform.position = Vector2.MoveTowards(rb.position, (Vector2)flyPath.vectorPath[currentWaypoint], control.maxSpeed * Time.deltaTime);

            float distance = Vector2.Distance(rb.position, flyPath.vectorPath[currentWaypoint]);
            if (distance <= pickNextWaypointDist)
                currentWaypoint++;

        }

        private void UpdatePath()
        {
            seeker.StartPath(rb.position, playerTransform.position, OnPathComplete);
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                flyPath = p;
                currentWaypoint = 0;
            }
        }

        private void Slime()
        {
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

    }
}