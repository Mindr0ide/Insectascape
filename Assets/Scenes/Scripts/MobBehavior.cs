using UnityEngine;

public class MobBehavior : MonoBehaviour
{
    [Header("Déplacement")]
    public float moveSpeed = 2f;
    public float changeDirectionInterval = 2f;
    
    [Header("Attaque")]
    public float damage = 1f;
    public float attackCooldown = 1f; // en secondes
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

    private Rigidbody2D rb;
    private CapsuleCollider2D capsule;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 randomDirection;
    private float directionTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        PickNewDirection();
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 moveDirection;

        if (distanceToPlayer <= detectionRadius)
        {
            float horizontalDistance = player.position.x - transform.position.x;

            // ✅ DEADZONE horizontale
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
                Debug.Log("🔁 Changement de direction");
                randomDirection.x *= -1f;
            }

            moveDirection = randomDirection;
        }

        // Flip sprite
        if (moveDirection.x != 0f && spriteRenderer != null)
        {
            spriteRenderer.flipX = moveDirection.x > 0f;
        }

        // ✅ Animation : set "Speed" et fige si arrêt
        if (animator != null)
        {
            float speed = Mathf.Abs(moveDirection.x);
            animator.SetFloat("Speed", speed);
            animator.speed = (speed > 0.01f) ? 1f : 0f; // Figer l’anim si immobile
        }

        // Calcul de la position
        Vector2 nextPosition = rb.position + new Vector2(moveDirection.x, 0f) * moveSpeed * Time.fixedDeltaTime;

        // Suivi du sol
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

        if (wallHit.collider != null)
        {
            Debug.Log("✅ Mur détecté !");
        }

        return wallHit.collider != null;
    }

    bool IsFacingEdge()
    {
        float direction = Mathf.Sign(randomDirection.x);
        Vector2 origin = rb.position + new Vector2(direction * capsule.bounds.extents.x, -capsule.bounds.extents.y);
        RaycastHit2D groundHit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);

        if (groundHit.collider == null)
        {
            Debug.Log("⚠️ Vide détecté !");
        }

        return groundHit.collider == null;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || capsule == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        float direction = Mathf.Sign(randomDirection.x != 0 ? randomDirection.x : 1f);

        // Raycast mur
        Vector2 wallOrigin = rb.position + new Vector2(direction * capsule.bounds.extents.x, -capsule.bounds.extents.y / 2f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(wallOrigin, wallOrigin + Vector2.right * direction * wallCheckDistance);

        // Raycast vide
        Vector2 edgeOrigin = rb.position + new Vector2(direction * capsule.bounds.extents.x, -capsule.bounds.extents.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(edgeOrigin, edgeOrigin + Vector2.down * groundCheckDistance);

        // Raycast sol
        Vector2 groundOrigin = rb.position + new Vector2(direction * moveSpeed * Time.fixedDeltaTime, 0f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(groundOrigin, groundOrigin + Vector2.down * groundCheckDistance);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Health playerHealth = collision.transform.GetComponent<Health>();
            Rigidbody2D playerRb = collision.transform.GetComponent<Rigidbody2D>();

            if (playerHealth != null)
            {
                // Vérifie le cooldown
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    // Inflige les dégâts
                    playerHealth.TakeDamage(damage);

                    // Applique un petit knockback
                    if (playerRb != null)
                    {
                        Vector2 rawDirection = (playerRb.position - rb.position).normalized;
                        Vector2 knockbackDirection = new Vector2(rawDirection.x, 0.5f).normalized;

                        playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                        // Stun le joueur pendant 0.3 secondes
                        // playerRb.GetComponent<Scenes.Scripts.PlayerMovement>()?.Stun(0.3f);
                    }

                    // Redémarre le cooldown
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                Debug.LogWarning("Le joueur n'a pas de composant Health !");
            }
        }
    }


}
