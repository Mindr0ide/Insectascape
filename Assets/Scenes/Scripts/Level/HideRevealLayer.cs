using UnityEngine;

public class HideRevealLayer : MonoBehaviour
{
    public GameObject layerToRelease;
    private PolygonCollider2D polygonCollider2D;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        polygonCollider2D.isTrigger = true; // Ensure it's a trigger
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            layerToRelease.SetActive(false);
        }
    } 
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            layerToRelease.SetActive(true);
        }
    } 
}
