using System;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform currentCheckpoint;
    [SerializeField] private Health playerHealth;
    [SerializeField]private Vector2 initialSpawnPoint; // Fallback spawn

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            currentCheckpoint = other.transform;
            other.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void Respawn()
    {
        playerHealth.Respawn(); //Restore player health and reset animation
        transform.position = currentCheckpoint != null ? currentCheckpoint.position : initialSpawnPoint;
        transform.position = currentCheckpoint.position;
    }
}
