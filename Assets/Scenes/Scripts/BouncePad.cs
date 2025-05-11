using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float bounce = 15f;
    private bool isBouncing = false;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isBouncing)
        {
            isBouncing = true;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            //Invoke(nameof(ResetBounce), 0.5f);
        }
    }
    //private void ResetBounce()
    //{
    //    isBouncing = false;
    //}
    public void ActivateBounce()
    {
        isBouncing = true;
    }
}
