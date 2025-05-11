using System.Collections;
using UnityEngine;

public class PlayerPlatformOneWay : MonoBehaviour
{
    private GameObject currentOneWayPlatform;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private float raycastDistance = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Si aucune collision enregistrée, on tente un raycast vers le bas
            if (currentOneWayPlatform == null)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, platformLayer);
                if (hit.collider != null)
                {
                    currentOneWayPlatform = hit.collider.gameObject;
                }
            }

            if (currentOneWayPlatform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        Collider2D platformCollider = currentOneWayPlatform.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
        yield return new WaitForSeconds(0.3f); // un peu plus long si stuck
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}