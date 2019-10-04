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
        private float pickNextWaypointDist = 0; //the distance the enemy needs to be of the waypoint to go to the next one
        private int currentWaypoint = 0; // the waypoint the enemy is
        private Path flyPath; // the A* mesh
        private Seeker seeker; // the class that find the best path in the mesh
        private Rigidbody2D rb; // the enemy's rigidbody 2D
        private Vector2 idleTarget; // where the fly wants to go when isnt seeing the player
        // when the player dies and the fly is in the checkpoint place the game bugs, so i'll use this variebles to sove this
        // when the player dies, the fly will go back to the original place, than will chage the player again
        private Vector2 originalPlace;
        private Cooldown cd_comeBack;
        private Cooldown cd_seekAgain;

        private bool isAlive = true;

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
                originalPlace = rb.position;
                cd_comeBack = new Cooldown(3.5f);
                cd_seekAgain = new Cooldown(1f);
            }
        }

        private void Start()
        {
            if (canFly)
                UpdatePath();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;

                if(canFly)
                    cd_comeBack.Start();
            }
        }

        void Update()
        {
            if (isAlive)
            {
                if (canFly)
                    Fly();
                else
                    Slime();
            }
        }

        private void Fly()
        {
            if (cd_seekAgain.IsFinished)
            {
                UpdatePath();
                cd_seekAgain.Start();
            }

            if (flyPath == null ||
                Vector2.Distance(rb.position, playerTransform.position) >= 20 /* if the enemy is too far away, do nothig*/ ||
                currentWaypoint >= flyPath.vectorPath.Count /*if the enemy reachs the final waypoint*/)
                return;            

            // the enemy only will go after the player if it can "see" him
            if (Vector2.Distance(rb.position, playerTransform.position) <= 6)
            {
                transform.position = Vector2.MoveTowards(rb.position, (Vector2)flyPath.vectorPath[currentWaypoint], control.maxSpeed * Time.deltaTime);

                float distance = Vector2.Distance(rb.position, flyPath.vectorPath[currentWaypoint]);
                if (distance <= pickNextWaypointDist)
                    currentWaypoint++;
            }
            // Else, the enemy will fly like a real fly
            else
            {
                float minMax = 1f;
                idleTarget = (new Vector2(Random.Range(-minMax, minMax), Random.Range(-minMax, minMax))) + rb.position;

                transform.position = Vector2.MoveTowards(rb.position, idleTarget, control.maxSpeed * Time.deltaTime);
            }

        }

        private void UpdatePath()
        {
            if (!cd_comeBack.IsFinished)
                seeker.StartPath(rb.position, originalPlace, OnPathComplete);
            else
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

        public void Kill()
        {
            isAlive = false;
        }

    }
}