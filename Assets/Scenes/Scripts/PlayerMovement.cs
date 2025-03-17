using System;
using UnityEngine;

namespace Scenes.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 7.5f;

        private Rigidbody2D rb;
        private bool isGrounded;
        private float moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            moveInput = Input.GetAxis("Horizontal");
            Move();
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                Jump();
        }
        
        private void Move()
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        private void Jump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Ground") 
                isGrounded = true;
        }
    }
}