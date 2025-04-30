using UnityEngine;

public class MobBehavior : MonoBehaviour
{
    [Header("Déplacement")]
    public float moveSpeed = 2f;
    public float changeDirectionInterval = 2f;

    [Header("Attaque")]
    public float damage = 1f;
    public float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;
    public float knockbackForce = 5f;

    [Header("Détection du joueur")]
    public float detectionRadius = 5f;
    public Transform player;

    [Header("Détection du sol")]
    public float groundCheckDistance = 2f;
    public float edgeCheckDistance = 0.4f;
    public LayerMask groundLayer;
    public float groundYOffset = 0.15f;

    [Header("Détection mur")]
    public float wallCheckDistance = 0.3f;
    public LayerMask wallLayer;

    [Header("Vie du mob")]
    public float maxHealth = 2f;
    private float currentHealth;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsule;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 randomDirection;
    private float directionTimer;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        PickNewDirection();

        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (isDead) return; // Ne plus bouger si mort

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 moveDirection;

        if (distanceToPlayer <= detectionRadius)
        {
            float horizontalDistance = player.position.x - transform.position.x;
            if (Mathf.Abs(horizontalDistance) > 0.2f)
            {
                moveDirection = new Vector2(horizontalDistance, 0f).normalized;
            }
            else
            {
                moveDirection = Vector2.zero;
            }
        }
        else
        {
            directionTimer -= Time.fixedDeltaTime;
            if (directionTimer <= 0f)
            {
                PickNewDirection();
            }

            if (IsFacingWall() || IsFacingEdge())
            {
                randomDirection.x *= -1f;
            }

            moveDirection = randomDirection;
        }

        if (moveDirection.x != 0f && spriteRenderer != null)
        {
            spriteRenderer.flipX = moveDirection.x > 0f;
        }

        if (animator != null)
        {
            float speed = Mathf.Abs(moveDirection.x);
            animator.SetFloat("Speed", speed);
            animator.speed = (speed > 0.01f) ? 1f : 0f;
        }

        Vector2 nextPosition = rb.position + new Vector2(moveDirection.x, 0f) * moveSpeed * Time.fixedDeltaTime;

        RaycastHit2D hit = Physics2D.Raycast(nextPosition, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider != null)
        {
            float targetY = hit.point.y + groundYOffset;
            nextPosition.y = Mathf.Lerp(rb.position.y, targetY, 10f * Time.fixedDeltaTime);
        }

        rb.MovePosition(nextPosition);
    }

    void PickNewDirection()
    {
        float directionX = Random.value < 0.5f ? -1f : 1f;
        randomDirection = new Vector2(directionX, 0f);
        directionTimer = changeDirectionInterval;
    }

    bool IsFacingWall()
    {
        float direction = Mathf.Sign(randomDirection.x);
        Vector2 origin = rb.position + new Vector2(direction * capsule.bounds.extents.x, -capsule.bounds.extents.y / 2f);
        RaycastHit2D wallHit = Physics2D.Raycast(origin, Vector2.right * direction, wallCheckDistance, wallLayer);
        return wallHit.collider != null;
    }

    bool IsFacingEdge()
    {
        float direction = Mathf.Sign(randomDirection.x);
        Vector2 origin = rb.position + new Vector2(direction * capsule.bounds.extents.x, -capsule.bounds.extents.y);
        RaycastHit2D groundHit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        return groundHit.collider == null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return; // Ne plus rien faire si mort

        if (collision.transform.CompareTag("Player"))
        {
            Health playerHealth = collision.transform.GetComponent<Health>();
            Rigidbody2D playerRb = collision.transform.GetComponent<Rigidbody2D>();

            if (playerHealth != null && Time.time >= lastAttackTime + attackCooldown)
            {
                playerHealth.TakeDamage(damage);

                if (playerRb != null)
                {
                    Vector2 rawDirection = (playerRb.position - rb.position).normalized;
                    Vector2 knockbackDirection = new Vector2(rawDirection.x, 0.5f).normalized;
                    playerRb.AddForce(Vector2.up * knockbackForce, ForceMode2D.Impulse);
                }

                lastAttackTime = Time.time;
            }
        }
    }

    // ✅ Nouvelle fonction pour prendre des dégâts
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);

        if (currentHealth > 0f)
        {
            animator.SetTrigger("hurt");
        }
        else
        {
            Die();
        }
    }

    // ✅ Fonction de mort
    void Die()
    {
        isDead = true;
        animator.SetTrigger("die");

        // Désactiver le collider et le mouvement
        rb.linearVelocity = Vector2.zero;
        capsule.enabled = false;

        // Détruire l'objet après 1.5 secondes (temps de l'animation)
        Destroy(gameObject, 1.5f);
    }
}
