using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class EventsFoxLevel : MonoBehaviour
{
    // reference to player camera
    public Camera playerCamera;
    // reference to sound that should be played
    public Animator foxAnimator;
    public Animator treeCrashAnimator;
    public Vector2 target = new Vector2(0,200);
    private PolygonCollider2D polygonCollider2D;

    private void Awake()
        {
            polygonCollider2D = GetComponent<PolygonCollider2D>();
            polygonCollider2D.isTrigger = true; // Ensure it's a trigger
        }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        // Firey tree branch crashes
        treeCrashAnimator.Play("Crash");
        // Shake camera

        //Play sound fire once

    }
    
}
