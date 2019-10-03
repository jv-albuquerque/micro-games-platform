using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.UI;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        /// <summary>
        /// Verify if the jump was made by a trampoline
        /// </summary>
        private bool trampolineJump = false;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        // EVERYTHING ABOVE HERE IS ABOUT POWER UPS
        [Header("Power Up Things")]
        [SerializeField] private PowerUpProperties starPowerUP = null;
        [SerializeField] private PowerUpProperties jumpPowerUP = null;
        private bool doubleJumped = false;
        [SerializeField] private PowerUpProperties gravityPowerUP = null;
        private float originalGravity;
        private float originalJumpTakeOffSpeed = 7;


        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            originalGravity = gravityModifier;

            //get the "Set Text" of the gameobjects
            starPowerUP.textCount = starPowerUP.ui_count.GetComponent<SetText>();
            jumpPowerUP.textCount = jumpPowerUP.ui_count.GetComponent<SetText>();
            gravityPowerUP.textCount = gravityPowerUP.ui_count.GetComponent<SetText>();

            //iniciate the Cooldowns
            starPowerUP.cd = new Cooldown(starPowerUP.time);
            jumpPowerUP.cd = new Cooldown(jumpPowerUP.time);
            gravityPowerUP.cd = new Cooldown(gravityPowerUP.time);

            starPowerUP.Stop();
            jumpPowerUP.Stop();
            gravityPowerUP.Stop();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (Input.GetButtonDown("Jump"))
                {
                    if(jumpState == JumpState.Grounded)
                    {
                        jumpState = JumpState.PrepareToJump;
                    }
                    //Double jump
                    else if (jumpPowerUP.active && !doubleJumped)
                    {
                        velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                        jumpState = JumpState.PrepareToJump;
                        doubleJumped = true;
                    }

                }
                else if (Input.GetButtonUp("Jump") && !trampolineJump)
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();

            PowerUpsUpdates();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    trampolineJump = false; // mark as not jumping in a trampoline
                    doubleJumped = false; // mark possible to double jump
                    break;
            }
        }

        protected override void ComputeVelocity()
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

        //function that calls the trampoline Jumps, a higher jump
        public void TrampolineJump()
        {
            velocity.y = jumpTakeOffSpeed * model.jumpModifier*1.5f;
            trampolineJump = true;
            doubleJumped = false; // mark possible to double jump afeter jump in a trampoline

            //this area is made this way because if i call JumpState.PrepareToJump
            //the jump will jump high one time and than will jump in a normal hight
            jumpState = JumpState.Jumping;
            jump = true;
            stopJump = false;
        }

        private void PowerUpsUpdates()
        {
            if (starPowerUP.active)
            {
                if (starPowerUP.cd.IsFinished)
                {
                    starPowerUP.Stop();
                }
                else
                {
                    starPowerUP.UpdateText();
                }
            }

            if (jumpPowerUP.active)
            {
                if (jumpPowerUP.cd.IsFinished)
                {
                    jumpPowerUP.Stop();
                }
                else
                {
                    jumpPowerUP.UpdateText();
                }
            }

            if (gravityPowerUP.active)
            {
                if (gravityPowerUP.cd.IsFinished)
                {
                    gravityPowerUP.Stop();
                    gravityModifier = originalGravity;
                    jumpTakeOffSpeed = originalJumpTakeOffSpeed;
                }
                else
                {
                    gravityPowerUP.UpdateText();
                }
            }
        }

        public void JumpPowerUp()
        {
            jumpPowerUP.Start();
        }

        public void StarPowerUp()
        {
            starPowerUP.Start();
        }

        public void GravityPowerUp()
        {
            gravityPowerUP.Start();
            gravityModifier = originalGravity * 0.4f;
            jumpTakeOffSpeed = originalJumpTakeOffSpeed * 1.1f;
        }

        public bool isStarPowerUp { get { return starPowerUP.active; } }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}