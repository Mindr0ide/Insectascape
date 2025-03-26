using System.Collections;
using UnityEngine;

public class PlayerPlatformOneWay : MonoBehaviour
{
    private GameObject currentOneWayPlatform;

    [SerializeField] private CapsuleCollider2D playerCollider;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentOneWayPlatform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
    }

    private void OnClisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatfrom"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OsionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatfrom"))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        CapsuleCollider2D platformCollider = currentOneWayPlatform.GetComponent<CapsuleCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
