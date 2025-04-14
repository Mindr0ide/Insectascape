using UnityEngine;

public class MobBehavior : MonoBehaviour
{
    [Header("Déplacement")]
    public float moveSpeed = 2f;
    public float changeDirectionInterval = 2f;

    [Header("Détection du joueur")]
    public float detectionRadius = 5f;
    public Transform player;

    [Header("Détection du sol")]
    public float groundCheckDistance = 2f;
    public LayerMask groundLayer;
    public float groundYOffset = 0.5f; // Ajuste selon la taille du sprite

    private Rigidbody2D rb;
    private Vector2 randomDirection;
    private float directionTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PickNewDirection();
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 moveDirection;

        // Détermine la direction selon la proximité du joueur
        if (distanceToPlayer <= detectionRadius)
        {
            moveDirection = new Vector2(player.position.x - transform.position.x, 0f).normalized;
        }
        else
        {
            directionTimer -= Time.fixedDeltaTime;
            if (directionTimer <= 0f)
            {
                PickNewDirection();
            }
            moveDirection = randomDirection;
        }

        // Position horizontale ciblée
        Vector2 nextPosition = rb.position + new Vector2(moveDirection.x, 0f) * moveSpeed * Time.fixedDeltaTime;

        // Raycast vers le bas depuis cette position future
        RaycastHit2D hit = Physics2D.Raycast(nextPosition, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider != null)
        {
            // Ajuster Y pour coller à la surface détectée
            nextPosition.y = hit.point.y + groundYOffset;
        }

        rb.MovePosition(nextPosition);
    }

    void PickNewDirection()
    {
        float directionX = Random.value < 0.5f ? -1f : 1f;
        randomDirection = new Vector2(directionX, 0f);
        directionTimer = changeDirectionInterval;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
