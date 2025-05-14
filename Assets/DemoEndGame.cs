using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoEndGame : MonoBehaviour
{
    public Animator moleAnim;
    private BoxCollider2D boxCollider2D; // Use consistent casing
    public GameObject text;

    void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
        text.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Optional: only trigger for player
            StartCoroutine(EndSequence());
    }

    private System.Collections.IEnumerator EndSequence()
    {
        freeze();
        moleAnim.Play("MolePoppup 0");
        yield return new WaitForSeconds(1);
        text.SetActive(true);
        yield return new WaitForSeconds(6);
        EndGame();
    }

    private void freeze()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.Freeze();
        }
    }

    public void Freeze()
    {
        // Disable movement logic here
        enabled = false;
    }

    public void Unfreeze()
    {
        enabled = true;
    }

    private void EndGame()
    {
        unfreeze();
        SceneManager.LoadScene("Menu");
    }
}
