using System;
using System.Collections;
using UnityEngine;

namespace Scenes.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 14f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;

        [Header("Coyote Time")] 
        [SerializeField] private float coyoteTime = 0.2f;
        private float coyoteCounter;

        [Header("Dash Settings")] 
        [SerializeField] private float dashForce = 36f;
        [SerializeField] private float dashDuration = 0.3f;
        [SerializeField] private float dashCooldown = 1.5f;

        [Header("Multiple Jumps")]
        [SerializeField] private int extraJumps = 1;
        private int jumpCounter;

        [Header("Melee Attack")]
        [SerializeField] private float meleeRange = 1f;
        [SerializeField] private Vector2 meleeBoxSize = new Vector2(1f, 1f);
        [SerializeField] private Vector2 meleeOffset = new Vector2(1f, 0f);
        [SerializeField] private int meleeDamage = 1;
        [SerializeField] private float meleeCooldown = 0.5f;
        [SerializeField] private LayerMask enemyLayer;
        private bool canMeleeAttack = true;
        
        [Header("Audio")]
        [SerializeField] private AudioClip meleeSound;
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip hurtSound;
        // [SerializeField] private AudioClip landSound;
        [SerializeField] private AudioClip dashSound;
        private AudioSource audioSource;

        private Rigidbody2D rb;
        private Animator anim;
        private bool isGrounded;
        private float moveInput;
        private bool isDashing = false;
        private bool canDash = true;
        private bool canMove = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            //print(isDashing);
            if (isDashing) return;
            moveInput = (canMove) ? Input.GetAxis("Horizontal") : 0f;

            // Flip
            if (moveInput > 0f) transform.localScale = Vector3.one;
            else if (moveInput < 0f) transform.localScale = new Vector3(-1, 1, 1);
            
            Move();
            HandleJumpAndCoyote();
            HandleDash();
            HandleAttack();
            HandleGravity();
            //HandleAnim();

            anim.SetBool("run", moveInput != 0);
            anim.SetBool("grounded", isGrounded);
        }

        private void Move() => rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        private void HandleJumpAndCoyote()
        {
            if (isGrounded)
            {
                coyoteCounter = coyoteTime;
                jumpCounter = extraJumps;
            }
            else coyoteCounter -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space) && (coyoteCounter > 0f || jumpCounter > 0) && canMove)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                anim.SetTrigger("jump");
                if (jumpSound != null)
                    audioSource.PlayOneShot(jumpSound);
                isGrounded = false;
                coyoteCounter = 0;
                if (!isGrounded) jumpCounter--;
            }
        }

        private void HandleDash()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && canMove)
                StartCoroutine(Dash());
        }

//        private void HandleAnim()
//        {
//            if (anim.GetBool("hurt"))
//            {
                //anim.SetTrigger("hurt");
//            }
//            else if (!isGrounded)
//            {
//                anim.SetBool("up", rb.linearVelocity.y > 0.1f);
//                anim.SetBool("down", rb.linearVelocity.y < -0.1f);
//            }
//            else
//            {
//                anim.SetBool("up", false);
//                anim.SetBool("down", false);
//            }
//        }

        private IEnumerator Dash()
        {
            //anim.SetBool("dash", true);
            anim.SetTrigger("dash");
            if (dashSound != null)
                audioSource.PlayOneShot(dashSound);
            canDash = false;
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(transform.localScale.x * dashForce, 0f);

            yield return new WaitForSeconds(dashDuration);
            rb.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
            //anim.SetBool("dash", false);
        }

        private void HandleGravity()
        {
            if (rb.linearVelocity.y <= 0)
                rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            else if (rb.linearVelocity.y > 0)
                rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
        }

        private void HandleAttack()
        {
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && canMeleeAttack)
                MeleeAttack();
        }

        private void MeleeAttack()
        {
            canMeleeAttack = false;
            anim.SetTrigger("attack");
            if (meleeSound != null)
                audioSource.PlayOneShot(meleeSound);


            // Determine box center based on facing
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Vector2 boxCenter = (Vector2)transform.position + direction * meleeOffset.x + Vector2.up * meleeOffset.y;

            // Detect enemies
            Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, meleeBoxSize, 0f, enemyLayer);
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent<MobBehavior>(out MobBehavior mob))
                {
                    //print("CA MARCHE LESGOOO");
                    mob.TakeDamage(meleeDamage);
                }
                else if (hit.TryGetComponent<BouncePad>(out BouncePad pad))
                {
                    //print("CA MARCHE LESGOOO 2");
                    pad.ActivateBounce();
                }
            }

            // Reset cooldown
            Invoke(nameof(ResetMeleeAttack), meleeCooldown);
        }

        private void ResetMeleeAttack() => canMeleeAttack = true;

        private void OnDrawGizmosSelected()
        {
            // Draw attack box
            Gizmos.color = Color.red;
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Vector2 boxCenter = (Vector2)transform.position + direction * meleeOffset.x + Vector2.up * meleeOffset.y;
            Gizmos.DrawWireCube(boxCenter, meleeBoxSize);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
                isGrounded = true;
        }

        public void freeze()
        {
            canMove = false;
        }

        public void unfreeze()
        {
            canMove = true;
        }
    }
}