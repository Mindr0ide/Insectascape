using UnityEngine;

public class FireHurtBox : MonoBehaviour
{
    [Header("Attaque")]
    public float damage = 1f;
    public float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;
    public float knockbackForce = 5f;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>(); // Initialize the BoxCollider2D
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Health playerHealth = collision.transform.GetComponent<Health>();
            Rigidbody2D playerRb = collision.transform.GetComponent<Rigidbody2D>();

            if (playerHealth != null && Time.time >= lastAttackTime + attackCooldown)
            {
                playerHealth.TakeDamage(damage);

                if (playerRb != null)
                {
                    // Use Transform.position instead of rb.position
                    Vector2 rawDirection = (collision.transform.position - transform.position).normalized;
                    Vector2 knockbackDirection = new Vector2(rawDirection.x, 0.5f).normalized;
                    playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }

                lastAttackTime = Time.time;
            }
        }
    }
}
