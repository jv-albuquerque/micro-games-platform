using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;
using Pathfinding;

namespace Platformer.Mechanics
{
    /// <summary>
    /// AnimationController integrates physics and animation. It is generally used for simple enemy animation.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class AnimationController : KinematicObject
    {
        /// <summary>
        /// Max horizontal speed.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Max jump velocity
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        /// <summary>
        /// Used to indicated desired direction of travel.
        /// </summary>
        public Vector2 move;

        /// <summary>
        /// Set to true to initiate a jump.
        /// </summary>
        public bool jump;

        /// <summary>
        /// Set to true to set the current jump velocity to zero.
        /// </summary>
        public bool stopJump;

        /// <summary>
        /// Set to true if the enemy died
        /// </summary>
        [HideInInspector] public bool isDead = false;

        // ------  Fly properties ----- //

        /// <summary>
        /// If the enemy can fly
        /// </summary>
        [SerializeField] private bool canFly = false;

        /// <summary>
        /// the player transform
        /// </summary>
        [SerializeField] private Transform playerTransform = null;

        /// <summary>
        /// How close the enemy needs to be to the waypoint to o to the next one
        /// </summary>
        [SerializeField] private float pickNextWaypointDist = 3f;

        /// <summary>
        /// The path that is found in A*
        /// </summary>
        private Path path;

        /// <summary>
        /// The current waypoint that the enemy is now
        /// </summary>
        private int currentWaypoint = 0;

        /// <summary>
        /// If the enemy reched the end of the path
        /// </summary>
        private bool rechedEndPath = false;

        //objects in the enemy
        private Seeker seeker;
        private Rigidbody2D rb;

        // ------ END Fly properties ----- //

        SpriteRenderer spriteRenderer;
        Animator animator;
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();


        protected virtual void Awake()
        {
            if(canFly)
            {
                seeker = GetComponent<Seeker>();
                rb = GetComponent<Rigidbody2D>();

                InvokeRepeating("UpdatePath", 0, .5f);
            }
            
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void ComputeVelocity()
        {
            if (canFly)
                Fly();
            else
                Slime();
        }

        private void Fly()
        {
            if (path == null)
                return;

            //if the enemy reachs the final waypoint
            if(currentWaypoint >= path.vectorPath.Count)
            {
                rechedEndPath = true;
                return;
            }
            else
            {
                rechedEndPath = false;
            }            

            transform.position = Vector2.MoveTowards(rb.position, (Vector2)path.vectorPath[currentWaypoint], maxSpeed * Time.deltaTime);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
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
                path = p;
                currentWaypoint = 0;
            }
        }

        private void Slime()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }
    }
}