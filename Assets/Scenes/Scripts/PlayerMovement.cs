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
        [SerializeField] private float coyoteTime = 0.2f; //How much time the player can hang in the air before jumping
        private float coyoteCounter; //How much time passed since the player ran off the edge

        [Header("Dash Settings")] 
        [SerializeField] private float dashForce = 36f;

        [SerializeField] private float dashDuration = 0.3f;
        [SerializeField] private float dashCooldown = 1.5f;

        [Header("Multiple Jumps")]
        [SerializeField] private int extraJumps = 1;

        private int jumpCounter;

        private Rigidbody2D rb;
        private bool isGrounded;
        private float moveInput;
        private Animator anim;

        private bool isDashing = false;
        private bool canDash = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            // Disable controls while dashing
            if (isDashing) return;
            moveInput = Input.GetAxis("Horizontal");

            // Flip the player to the left if he looks left and vice-versa
            if (moveInput > 0f) transform.localScale = Vector3.one;
            else if (moveInput < 0f) transform.localScale = new Vector3(-1, 1, 1);

            Move();
            
            // Handle coyote time
            if (isGrounded)
            {
                coyoteCounter = coyoteTime;
                jumpCounter = extraJumps; // Reset extra jumps when grounded
            }
            else
                coyoteCounter -= Time.deltaTime;
            

            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && (coyoteCounter > 0f || jumpCounter > 0))
            {
                Jump();
                if (!isGrounded) jumpCounter--;
            }
            // Dash
            if (Input.GetKeyDown(KeyCode.LeftAlt) && canDash)
                StartCoroutine(Dash());
            // apply gravity modifiers
            if (rb.linearVelocity.y < 0)
                rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            else if (rb.linearVelocity.y > 0)
                rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
            

            //Set animator parameters
            anim.SetBool("run", moveInput != 0);
            anim.SetBool("grounded", isGrounded);
        }

        private void Move()
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, (float)(rb.linearVelocity.y));
        }

        private void Jump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("jump");
            isGrounded = false;
            coyoteCounter = 0;
        }



        private IEnumerator Dash()
        {
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
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Ground")
                isGrounded = true;
        }
    }
}