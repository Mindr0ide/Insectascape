using System;
using UnityEngine;

namespace Scenes.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 3.5f;
        [SerializeField] private float gravity = 1.0f;

        private Rigidbody2D rb;
        private bool isGrounded;
        private float moveInput;
        private Animator anim;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            moveInput = Input.GetAxis("Horizontal");
            
            // Flip the player to the left if he looks left and vice-versa
            if (moveInput > 0f) 
                transform.localScale = Vector3.one;
            else if (moveInput < 0f)
                transform.localScale = new Vector3(-1, 1, 1);
            
            Move();
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                Jump();
            
            //Set animator parameters
            anim.SetBool("run", moveInput != 0);
            anim.SetBool("grounded", isGrounded);
        }
        
        private void Move()
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, (float)(rb.linearVelocity.y - gravity));
        }

        private void Jump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("jump");
            isGrounded = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Ground") 
                isGrounded = true;
        }
    }
}