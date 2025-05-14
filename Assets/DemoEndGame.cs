using UnityEngine;
using UnityEngine.SceneManagement;
using Scenes.Scripts;

public class DemoEndGame : MonoBehaviour
{
    public Animator moleAnim;
    private BoxCollider2D boxCollider2D; // Use consistent casing
    public GameObject text;



// freeze player does not work now i dont have time to fix
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
        //Freeze();
        moleAnim.Play("MolePoppup 0");
        yield return new WaitForSeconds(1);
        text.SetActive(true);
        yield return new WaitForSeconds(6);
        EndGame();
    }

    // private void freeze()
    // {
    //     var player = GameObject.FindGameObjectWithTag("Player");
    //     if (player != null)
    //     {
    //         var movement = player.GetComponent<PlayerMovement>();
    //         if (movement != null)
    //             movement = false();
    //     }
    // }

    // public void Freeze()
    // {
    //     // Disable movement logic here
        
    // }

    // public void Unfreeze()
    // {
    //     enabled = true;
    // }

    private void EndGame()
    {
        //Unfreeze();
        SceneManager.LoadScene("Menu");
    }
}
